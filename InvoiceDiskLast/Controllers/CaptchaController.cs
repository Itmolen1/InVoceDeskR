
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
        DBEntities db = new DBEntities();
        
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult NewCompany()
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
               var jsonInput = new JavaScriptSerializer().Serialize(userInfo);

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

                    //HttpResponseMessage response = httpClient.PostAsync("http://uurtjefactuur.nl/Token", encodedRequest).Result;

                    HttpResponseMessage response = httpClient.PostAsync("http://localhost:63861/Token", encodedRequest).Result;

                    token = response.Content.ReadAsAsync<BearerToken>().Result;

                    if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    {
                        ViewBag.ErrorMessage = "username or password incorrect!";
                        return View(user);
                    }
                    // Store token in ASP.NET Session State for later use
                    Session["ApiAccessToken"] = token.AccessToken;
                }


                if (Session["ApiAccessToken"] != null)
                {

                    //HttpResponseMessage respons = GlobalVeriables.WebApiClient.GetAsync("/api/GetCompanyID" + "test").Result;
                    string name = userInfo.username.ToString();
                    Session["username"] = name;
                    GlobalVeriables.WebApiClient.DefaultRequestHeaders.Add("name", name);
                    HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("ApiCompanyStatus/" + "ss").Result;
                    var apiresut = response.Content.ReadAsAsync<object>().Result;

                    int compnyID = Convert.ToInt32(apiresut);
                    if (compnyID > 0)
                    {
                        MVCCompanyInfoModel cominfo = new MVCCompanyInfoModel();
                       
                            Session["CompayID"] = compnyID;

                            HttpResponseMessage responses = GlobalVeriables.WebApiClient.GetAsync("APIComapny/" + compnyID.ToString()).Result;
                            cominfo = responses.Content.ReadAsAsync<MVCCompanyInfoModel>().Result;
                            Session["CompanyName"] = cominfo.CompanyName;
                            Session["CompanyEmail"] = cominfo.CompanyEmail;
                            Session["CompanyContact"] = cominfo.CompanyPhone;

                        return RedirectToAction("Index","Home");
                       
                    }
                    else
                    {
                        return RedirectToAction("NewCompany");
                    }

                }
            }

           ViewBag.ErrorMessage = "captcha is not valid upto now.";

            return View(user);          
        }


       // public string Test(int id)
       // {
            
       //     HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("students/" + id).Result;
       //     return response.Content.ToString();
       // }


       // public string Test2(string name,int id=0)
       //{

       //     HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("students/" + name+ "/"+id).Result;
       //     return response.Content.ToString();
       // }

       // public string Test1(string name)
       // {

       //     HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("students/" + name).Result;
       //     return response.Content.ToString();
       // }

        public string ThankYouPage()
        {
            return "Valid";

        }
                

        [HttpPost]
        public JsonResult CheckUsername(string username)
        {

            //HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("GetUserbyEmail/" + username).Result;

            //if (response.StatusCode == System.Net.HttpStatusCode.OK)
            //{

            //    bool isValid = false;
            //    return Json(isValid);

            //}
            //else
            //{
            //    bool isValid = true;
            //    return Json(isValid);
            //}

        
            return Json(!db.AspNetUsers.Any(x => x.UserName == username),JsonRequestBehavior.AllowGet);
       

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
