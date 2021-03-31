using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Options;
using RestSharp;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using EASY.Website.Utility;

namespace EASY.Website.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("[controller]")]
    //[NonController]  //comment out for frontend / or api
    public class AppController : ControllerBase //for Web API ONLY
    {
        private readonly IOptions<MySettingsModel> appSettings; 
        public AppController(IOptions<MySettingsModel> app)  
        {  
            appSettings = app;  
        } 
        [HttpGet("version")]
        [AllowAnonymous]
        public IActionResult version([FromQuery(Name = "id")] string id)
        {
            try{
                return Ok(new {version=1});
            }
            catch(Exception)
            {
                return BadRequest(new {error = "Request Terminated!" });
            }
        }
        [HttpGet("dbtest")]
        [AllowAnonymous]
        public IActionResult dbtest([FromQuery(Name = "id")] string id)
        {
            try{
                SqlParameter[] param = {
                    //new SqlParameter("@id",id),
                };
                var lst = SqlHelper.ExecuteStatementDataTable(appSettings.Value.DbConnection, 
                @"select * from Product", param);
                return Ok(lst);
            }
            catch(Exception)
            {
                return BadRequest(new {error = "Request Terminated!" });
            }
        }

        [HttpGet("product/list")]
        [AllowAnonymous]
        public IActionResult listproduct()
        {
            try{
                SqlParameter[] param = {
                };
                var lst = SqlHelper.ExecuteStatementDataTable(appSettings.Value.DbConnection, 
                @"select * from Product", param);
                return Ok(lst);
            }
            catch(Exception)
            {
                return BadRequest(new {error = "Request Terminated!" });
            }
        }
        [HttpGet("product/item")]
        [AllowAnonymous]
        public IActionResult itemproduct([FromQuery(Name = "id")] string id)
        {
            try{
                SqlParameter[] param = {
                    new SqlParameter("@id",id),
                };
                var lst = SqlHelper.ExecuteStatementDataTable(appSettings.Value.DbConnection, 
                @"select * from Product where ProductID=@id", param);
                return Ok(lst);
            }
            catch(Exception)
            {
                return BadRequest(new {error = "Request Terminated!" });
            }
        }
        [HttpGet("product/item/{id}")]
        [AllowAnonymous]
        public IActionResult itemproductid(string id)
        {
            try{
                SqlParameter[] param = {
                    new SqlParameter("@id",id),
                };
                var lst = SqlHelper.ExecuteStatementDataTable(appSettings.Value.DbConnection, 
                @"select * from Product where ProductID=@id", param);
                return Ok(lst);
            }
            catch(Exception)
            {
                return BadRequest(new {error = "Request Terminated!" });
            }
        }
        [HttpPost("user/add")]
        [AllowAnonymous]
        public ActionResult userAdd([FromBody] Dictionary<string, object> obj)
        {
            SqlParameter[] param = {
                    new SqlParameter("@inputcode",obj["inputcode"].ToString()),
                    new SqlParameter("@inputmnum",obj["inputmnum"].ToString()),
                    new SqlParameter("@inputName",obj["inputName"].ToString()),
                    new SqlParameter("@inputLicense",obj["inputLicense"].ToString()),
                    new SqlParameter("@inputPlate",obj["inputPlate"].ToString())
                };

                var lst2 = SqlHelper.ExecuteStatementReturnString(appSettings.Value.DbConnection, 
                @"insert into Customer(name,LicensePlate,DriversLicenseNumber,Code,isActive) values (@inputName, @inputPlate, @inputLicense,'CODE',1);SELECT SCOPE_IDENTITY()", param);

                SqlParameter[] param2 = {
                    new SqlParameter("@id",lst2)
                };
                var lst = SqlHelper.ExecuteStatementDataTable(appSettings.Value.DbConnection, 
                @"insert into [User](CustomerID,IsActive,FailedLoginCount,IsAdmin) values (@id,0,0,0)", param2);
            return Ok(lst2);
        }
        [HttpPost("card/add")]
        [AllowAnonymous]
        public ActionResult cardAdd([FromBody] Dictionary<string, object> obj)
        {
            int totalLength = obj["inputCCNum"].ToString().Length;
            string ccnum = "**** " + obj["inputCCNum"].ToString().Substring(totalLength-5,4);
            SqlParameter[] param = {
                    new SqlParameter("@customer",obj["customer"].ToString()),
                    new SqlParameter("@RedactedReference",ccnum),
                    new SqlParameter("@ExpiryYear",obj["inputyear"].ToString()),
                    new SqlParameter("@ExpiryMonth",obj["inputmonth"].ToString()),
                    new SqlParameter("@PaymentToken","TOKEN")
                };

                var lst2 = SqlHelper.ExecuteStatementReturnString(appSettings.Value.DbConnection, 
                @"insert into PaymentCard(CustomerID,RedactedReference,ExpiryYear,ExpiryMonth,PaymentToken) values (@customer, @RedactedReference, @ExpiryYear,@ExpiryMonth,@PaymentToken);SELECT SCOPE_IDENTITY()", param);
                return Ok(lst2);
        }
    
    }
}