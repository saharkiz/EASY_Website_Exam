using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using EASY.Website.Models;
using System.Text;
using System;


namespace EASY.Website.MiddleWare
{
    public class ProductService
    {

        public List<Product> Products { get; set; }
        private HttpClient client;

        public ProductService()
        {
            client = new HttpClient();
        }

        public List<Product> FetchAll(string apiurl)
        {
            string url = apiurl + "product/list";
            HttpRequestMessage m = new HttpRequestMessage(HttpMethod.Get, url);
            HttpResponseMessage resp = client.SendAsync(m).Result;
            client.Dispose();
            string j = resp.Content.ReadAsStringAsync().Result ;
            return JsonSerializer.Deserialize<List<Product>>(j);
        }
        public List<Product> FetchOne(string apiurl, int id)
        {
            string url = apiurl + "product/item/" + id;
            HttpRequestMessage m = new HttpRequestMessage(HttpMethod.Get, url);
            HttpResponseMessage resp = client.SendAsync(m).Result;
            client.Dispose();
            string j = resp.Content.ReadAsStringAsync().Result ;
            return JsonSerializer.Deserialize<List<Product>>(j);
        }
        public string SendUser(string apiurl, object myData)
        {
            string url = apiurl + "user/add";
            var payload = JsonSerializer.Serialize(myData);

            HttpContent c = new StringContent(payload, Encoding.UTF8, "application/json");

            Uri u = new Uri(url);
            HttpRequestMessage request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = u,
                Content = c
            };
            HttpResponseMessage result = client.SendAsync(request).Result;
            return result.Content.ReadAsStringAsync().Result.ToString();
        }
        public string SendCard(string apiurl, object myData)
        {
            string url = apiurl + "card/add";
            var payload = JsonSerializer.Serialize(myData);

            HttpContent c = new StringContent(payload, Encoding.UTF8, "application/json");

            Uri u = new Uri(url);
            HttpRequestMessage request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = u,
                Content = c
            };
            HttpResponseMessage result = client.SendAsync(request).Result;
            return result.Content.ReadAsStringAsync().Result.ToString();
        }
        

    }
}