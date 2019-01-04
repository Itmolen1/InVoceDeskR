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
using InvoiceDiskLast.MISC;
using Logger;

namespace InvoiceDiskLast.Controllers
{
    [RouteNotFoundAttribute]
    [RouteNotFoundAttribute]
    public class CaptchaController : Controller
    {
        DBEntities db = new DBEntities();

        private Ilog _iLog;
        public CaptchaController()
        {
            _iLog = Log.GetInstance;
        }

        public ActionResult Index()
        {
            UserModels model = new UserModels();

            if (TempData["EmailNotExist"] != null)
            {
                ViewBag.ErrorMessage = TempData["EmailNotExist"].ToString();
            }

            try
            {
                 if (Request.Cookies["Login"] != null)
                {
                    model.Username = Request.Cookies["Login"].Values["UserName"];
                    model.Password = Request.Cookies["Login"].Values["Password"];
                    model.Rememberme = Convert.ToBoolean(Request.Cookies["Login"].Values["RememberMe"]);

                }
            }
            catch (Exception)
            {

                throw;
            }


            return View(model);


          
        }

        [SessionExpireAttribute]
        public ActionResult NewCompany()
        {
            return View();
        }

        //xsaxsax
        [HttpPost]
        public ActionResult Index(UserModels user)
         {

            HttpResponseMessage messageConformed = GlobalVeriables.WebApiClient.PostAsJsonAsync("AccountCheckStatus", user).Result;

            if (messageConformed.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                ViewBag.ErrorMessage = "E-mail not conformed until! Please Conform";
                return View(user);
            }
            else
            {
                string url = GlobalVeriables.WebApiClient.BaseAddress.ToString().Replace("/api", "");
                if (this.IsCaptchaValid("Captcha is not valid"))
                {

                    UserInfo userInfo = new UserInfo();
                    userInfo.username = user.Username;
                    userInfo.password = user.Password;
                    userInfo.grant_type = "password";

                    var jsonInput = new JavaScriptSerializer().Serialize(userInfo);

                    #region
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


                        //HttpResponseMessage response = httpClient.PostAsync(url+"/Token", encodedRequest).Result;

                        HttpResponseMessage response = httpClient.PostAsync(url + "Token", encodedRequest).Result;

                        token = response.Content.ReadAsAsync<BearerToken>().Result;

                        #region


                        if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                        {
                            ViewBag.ErrorMessage = "username or password incorrect!";
                            return View(user);
                        }
                        #endregion
                        // Store token in ASP.NET Session State for later use
                        Session["ApiAccessToken"] = token.AccessToken;
                    }

                    if (user.Rememberme)
                    {

                        HttpCookie cookie = new HttpCookie("Login");
                        cookie.Values.Add("UserName", user.Username);
                        cookie.Values.Add("Password", user.Password);
                        cookie.Values.Add("RememberMe", user.Rememberme.ToString());
                        cookie.Expires = DateTime.Now.AddDays(1);
                        Response.Cookies.Add(cookie);
                    }
                    else
                    {
                        HttpCookie cookie = new HttpCookie("Login");
                        cookie.Expires = DateTime.Now.AddDays(-1);
                        Response.Cookies.Add(cookie);
                    }

                    #endregion
                    if (Session["ApiAccessToken"] != null)
                    {
                        UserModel UserTbale = new UserModel();
                        //HttpResponseMessage respons = GlobalVeriables.WebApiClient.GetAsync("/api/GetCompanyID" + "test").Result;
                        string name = userInfo.username.ToString();
                        Session["username"] = name;
                        GlobalVeriables.WebApiClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Session["ApiAccessToken"].ToString());
                        HttpResponseMessage response = GlobalVeriables.WebApiClient.PostAsJsonAsync("CompanyExist", userInfo).Result;
                        if (response.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            UserTbale = response.Content.ReadAsAsync<UserModel>().Result;
                        }
                        int compnyID = Convert.ToInt32(UserTbale.CompanyId);
                        Session["LoginUserID"] = Convert.ToInt32(UserTbale.UserId);
                        #region
                        if (response.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            MVCCompanyInfoModel cominfo = new MVCCompanyInfoModel();

                            Session["CompayID"] = compnyID;

                            HttpResponseMessage responses = GlobalVeriables.WebApiClient.GetAsync("APIComapny/" + compnyID.ToString()).Result;
                            cominfo = responses.Content.ReadAsAsync<MVCCompanyInfoModel>().Result;
                            Session["CompanyName"] = cominfo.CompanyName;
                            Session["CompanyEmail"] = cominfo.CompanyEmail;
                            Session["CompanyContact"] = cominfo.CompanyPhone;
                            Session["username"].ToString();

                            HttpResponseMessage responseUser = GlobalVeriables.WebApiClient.PostAsJsonAsync("GetUserInfo", userInfo).Result;
                            UserModel usermodel = responseUser.Content.ReadAsAsync<UserModel>().Result;

                           
                            return RedirectToAction("Index", "Home");

                        }
                        else
                        {
                            return RedirectToAction("NewCompany");
                        }
                        #endregion
                    }
                }

                ViewBag.ErrorMessage = "captcha is not valid upto now.";

                return View(user);
            }
        }


        // public string Test(int id)
        // {

        //     HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("students/" + id).Result;
        //     return response.Content.ToString();
        // }


        //public string Test2(string name)
        //{
        //    name = "it1@gmail.com";
        //    HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("CompanyExist/" + name).Result;
        //    return response.Content.ToString();
        //}

        public string Test1(string Email)
        {

            HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("EmailExist/" + Email).Result;
            return response.Content.ToString();
        }
        [SessionExpireAttribute]
        public string ThankYouPage()
        {
            return "Valid";

        }

        //for forgot pasword

        [HttpPost]
        public ActionResult CheckEmail(UserModels UserEmail)
        {

            LoginController login = new LoginController();
            bool result = db.AspNetUsers.Count(e => e.UserName == UserEmail.Username) > 0;

            if (result == true)
            {
                login.VerifyEmail(UserEmail.Username, 1);

                //return Json("Success", JsonRequestBehavior.AllowGet);

                return RedirectToAction("ResitPasswordSuccess", "Login");
            }
            TempData["EmailNotExist"] = "Email Not Exist!";
            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public ActionResult RestPassword(string Code)
        {

            if (Code != null)
            {

                RestPasswordModel model = new RestPasswordModel();
                model.Codes = Code;
                return View(model);

            }
            else
            {
                TempData["EmailNotExist"] = "Not a vlaid user!";
                return RedirectToAction(nameof(Index));
            }
        }



        [HttpPost]
        public ActionResult RestPassword(RestPasswordModel model)
        {
            if (model.NewPassword != model.ConfirmPassword)
            {
                return View(model);
            }

            RestSetPasswordBindingModel md = new RestSetPasswordBindingModel();
            md.NewPassword = model.NewPassword;
            md.ConfirmPassword = model.ConfirmPassword;
            md.Email = model.Email;
            md.Code = model.Codes;

            HttpResponseMessage response = GlobalVeriables.WebApiClient.PostAsJsonAsync("Account/RestSetPassword", md).Result;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View(model);
            }
        }


        //for new register 
        [HttpPost]
        public JsonResult CheckUsername(string username)
        {
            try
            {
                return Json(!db.AspNetUsers.Any(x => x.UserName == username), JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                ex.ToString();
                return Json(ex, JsonRequestBehavior.AllowGet);
            }
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
