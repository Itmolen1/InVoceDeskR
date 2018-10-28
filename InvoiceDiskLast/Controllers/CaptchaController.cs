
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CaptchaMvc.HtmlHelpers;
using CaptchaMvc.Attributes;
using InvoiceDiskLast.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace InvoiceDiskLast.Controllers
{


    public class CaptchaController : Controller
    {

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(UserModels user)
        {

            if (this.IsCaptchaValid("Captcha is not valid"))
            {               

                UserInfo userInfo = new UserInfo();
                userInfo.username = user.Username;
                userInfo.password = user.Password;
                userInfo.grant_type = "password";
               // var jsonInput = new JavaScriptSerializer().Serialize(userInfo);

                BearerToken token;
                using (var httpClient = new HttpClient())
                {
                    var tokenRequest =
                        new List<KeyValuePair<string, string>>
                            {
                        new KeyValuePair<string, string>("grant_type", "password"),
                        new KeyValuePair<string, string>("username", userInfo.username),
                        new KeyValuePair<string, string>("password", userInfo.password)
                            };

                    HttpContent encodedRequest = new FormUrlEncodedContent(tokenRequest);

                    HttpResponseMessage response = httpClient.PostAsync("http://localhost:63861/Token", encodedRequest).Result;
                    token = response.Content.ReadAsAsync<BearerToken>().Result;

                    // Store token in ASP.NET Session State for later use
                    Session["ApiAccessToken"] = token.AccessToken;
                }

                // HttpClient client = new HttpClient();
                // request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
                // HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "ApiConatacts");

                //var client = new HttpClient
                //{
                //    BaseAddress = new Uri(GlobalVeriables.WebApiClient.GetAsync("apicontact").Result);
                //};
                //client.DefaultRequestHeaders.Add("access_token", "YWtoaWw6YWtoaWw=");

                //GlobalVeriables.WebApiClient.DefaultRequestHeaders.Accept.Authorization = "Authorization", "Bearer " + token.AccessToken);



                HttpResponseMessage respons = GlobalVeriables.WebApiClient.GetAsync("apicontact").Result;
                respons.Headers.Add("Authorization", "Bearer " + token.AccessToken);

               
               
                //respons.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);

            }

            ViewBag.ErrMessage = "Error: captcha is not valid upto now.";


            return View(user);
            // Code for validating the CAPTCHA 


        }

        public string ThankYouPage()
        {
            return "Valid";

        }
    }
    public class UserInfo
    {
        public string username { get; set; }
        public string password { get; set; }
        public string grant_type { get; set; }
    }


    public class BearerToken
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        [JsonProperty("expires_in")]
        public string ExpiresIn { get; set; }

        [JsonProperty("userName")]
        public string UserName { get; set; }

        [JsonProperty(".issued")]
        public string Issued { get; set; }

        [JsonProperty(".expires")]
        public string Expires { get; set; }
    }

}
