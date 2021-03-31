using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Options;

using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Net.Mail;

using System.Security.Cryptography;
using System.Collections;
using RestSharp;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using EASY.Website.Utility;
using EASY.Website.Models;
using System.Diagnostics;
using Newtonsoft.Json;
using EASY.Website.MiddleWare;
using EASY.Website.OTP;

namespace EASY.Website.Controllers
{
    [ApiController]
    [Route("[controller]")]
    //[NonController]
    public class HomeController : Controller
    {

        private readonly IOptions<MySettingsModel> appSettings; 
        private readonly ILogger<HomeController> _logger;
        private readonly IMemoryCache _cache;

        public HomeController(IOptions<MySettingsModel> app, ILogger<HomeController> logger,IMemoryCache memoryCache)  
        {  
            appSettings = app;  
            _logger = logger;
            _cache = memoryCache; 
        } 
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        [HttpGet]
        [HttpGet("Verify")]
        [AllowAnonymous]
        public IActionResult Index()
        {
                /* //Direct DB
                SqlParameter[] param = {
                };
                var lst = SqlHelper.ExecuteStatementDataTable(appSettings.Value.DbConnection, 
                @"select * from Product", param);
                ViewBag.products = lst;
                */

                //Thru HttpClient OLD School style
                ProductService ps = new ProductService();
                var data = ps.FetchAll(appSettings.Value.APIURL);

                ViewBag.products = data;

            return View("Index");
        }
        [HttpGet("Privacy")]
        [AllowAnonymous]
        public IActionResult Privacy()
        {
            return View("Privacy");
        }
        [HttpGet("View/{id}")]
        [AllowAnonymous]
        public IActionResult Viewitem(string id)
        {
            var lst = new List<Product>();
            string CacheEntry = "productlist." + id;
            if (!_cache.TryGetValue(CacheEntry, out lst))
            {
                try{
                        int num = Convert.ToInt32(id);

                        ProductService ps = new ProductService();
                        lst = ps.FetchOne(appSettings.Value.APIURL, num);
                        //Cache for performance
                        var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(3));
                        _cache.Set(CacheEntry, lst, cacheEntryOptions);
                        ViewBag.Name = lst[0].Name;
                        ViewBag.BasePrice = lst[0].BasePrice;
                        ViewBag.HtmlDescription = lst[0].HtmlDescription;
                        ViewBag.id = num; //keeping a reference for POST

                        return View("Viewitem");
                }
                catch(Exception)
                {
                    return Redirect("/Home?error=error");
                }
            }
            else{
                ViewBag.Name = lst[0].Name;
                ViewBag.BasePrice = lst[0].BasePrice;
                ViewBag.HtmlDescription = lst[0].HtmlDescription;
                ViewBag.id = id; //keeping a reference for POST

                return View("Viewitem");
            }
        }
        [HttpPost("Verify")]
        [AllowAnonymous]
        public async Task<IActionResult>  Verify([FromForm]FormModel model)
        {
            try{
                var lst = new List<Product>();
                string CacheEntry = "productlist." + model.id;
                if (!_cache.TryGetValue(CacheEntry, out lst))
                {
                    return Redirect("/Home/View/" + model.id);
                }
                else{
                    ViewBag.Name = lst[0].Name;
                    ViewBag.BasePrice = lst[0].BasePrice;
                    ViewBag.HtmlDescription = lst[0].HtmlDescription;
                    ViewBag.id = model.id; //keeping a reference for POST

                    ViewBag.code = model.inputcode;
                    ViewBag.mobile = model.inputmnum;

                    ProductService ps = new ProductService();
                    object myData = new
                    {
                        inputcode = model.inputcode,
                        inputName = model.inputName,
                        inputmnum =model.inputmnum,
                        inputLicense = model.inputLicense,
                        inputPlate = model.inputPlate
                    };
                    var t = ps.SendUser(appSettings.Value.APIURL,myData);
                    ViewBag.customer = t;
                    string mymobile = "+" + model.inputcode + model.inputmnum;
                    ViewBag.mobile = mymobile;
                    await requestOTP(mymobile);
                    return View("Verify");
                }
            }
            catch(Exception)
            {
                return Redirect("/Home?error=error");
            }
        }
        [HttpPost("Payment")]
        [AllowAnonymous]
        public async Task<IActionResult>  Payment([FromForm]FormModel model)
        {
            string otp = model.inputmnum1 + model.inputmnum2 + model.inputmnum3 + model.inputmnum4 + model.inputmnum5+ model.inputmnum6; // can be done in class
            string otpsuccess = await validateOTP(model.mobile, otp);
            //return Ok(otpsuccess);
            if(otpsuccess.Trim() == "pending")
            {
                Redirect("/Home?error=OTP INVALID");
            }
            else if (otpsuccess.Trim() == "approved")
            {
                try{

                    int num = Convert.ToInt32(model.id);
                    #region My preference as its faster
                        var client = new RestClient(appSettings.Value.APIURL + "product/item/" + num);
                        client.Timeout = -1;
                        var request = new RestRequest(Method.GET);
                        IRestResponse response = client.Execute(request);
                        if (response.Content.Length > 2)
                        {
                            dynamic data = JsonConvert.DeserializeObject(response.Content);
                            ViewBag.Name = data[0].Name;
                            ViewBag.BasePrice = data[0].BasePrice;
                            ViewBag.HtmlDescription = data[0].HtmlDescription;
                            ViewBag.id = model.id; //keeping a reference for POST
                            ViewBag.customer = model.customer;
                        }
                        else
                        {
                            return Redirect("/Home?error=Purchase Not Found");
                        }
                    #endregion

                    return View("Payment");
                }
                catch(Exception)
                {
                    return Redirect("/Home?error=error");
                }
            }
            else
            {
                return Redirect("/Home?error=OTP Error");
            }
            return Redirect("/Home?error=OTP Error");
        }
        [HttpPost("Review")]
        [AllowAnonymous]
        public IActionResult Review([FromForm]FormModel model)
        {
            try{

                var lst = new List<Product>();
                string CacheEntry = "productlist." + model.id;
                if (!_cache.TryGetValue(CacheEntry, out lst))
                {
                    return Redirect("/Home/View/" + model.id);
                }
                else{
                    ViewBag.Name = lst[0].Name;
                    ViewBag.BasePrice = lst[0].BasePrice;
                    ViewBag.HtmlDescription = lst[0].HtmlDescription;
                    ViewBag.id = model.id; //keeping a reference for POST
                    ViewBag.customer = model.customer;
                }

                
                        ProductService ps = new ProductService();
                        object myData = new
                        {
                            customer = model.customer,
                            inputCCNum = model.inputCCNum,
                            inputyear =model.inputyear,
                            inputmonth = model.inputmonth
                        };
                        var t = ps.SendCard(appSettings.Value.APIURL,myData);
                return View("Review?t=" + t);
            }
            catch(Exception)
            {
                return Redirect("/Home?error=error");
            }
        }
    
    

        [HttpPost("Done/{id}")]
        [AllowAnonymous]
        public IActionResult Done([FromForm]FormModel model)
        {
            try{

                int num = Convert.ToInt32(model.id);
                #region My preference as its faster
                    var client = new RestClient(appSettings.Value.APIURL + "product/item/" + num);
                    client.Timeout = -1;
                    var request = new RestRequest(Method.GET);
                    IRestResponse response = client.Execute(request);
                    if (response.Content.Length > 2)
                    {
                        dynamic data = JsonConvert.DeserializeObject(response.Content);
                        ViewBag.Name = data[0].Name;
                        ViewBag.BasePrice = data[0].BasePrice;
                        ViewBag.HtmlDescription = data[0].HtmlDescription;
                    }
                    else
                    {
                        return Redirect("/Home?error=Purchase Not Found");
                    }
                #endregion

                ViewBag.subscription = "";
                ViewBag.startDate = "";
                ViewBag.status = "ACTIVE";
                return View("Done");
            }
            catch(Exception)
            {
                return Redirect("/Home?error=error");
            }
        }
        
        
        private async Task<bool> requestOTP(string mobile)
        {
            string tokencall = await Task.Run(() => {
                return TwilioOTP.sendOTP(mobile);
            });
            
            dynamic json = SimpleJson.DeserializeObject(tokencall);
            return true;
        }
        private async Task<string> validateOTP(string mobile, string otp)
        {
            string tokencall = await Task.Run(() => {
                return TwilioOTP.validateOTP(mobile, otp);
            });
            
            dynamic json = SimpleJson.DeserializeObject(tokencall);
            return json.status.ToString();
        }
    }
    
}