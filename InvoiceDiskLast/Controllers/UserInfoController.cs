﻿
using InvoiceDiskLast.MISC;
using InvoiceDiskLast.Models;
using Logger;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace InvoiceDiskLast.Controllers
{
    [SessionExpireAttribute]
    [RouteNotFoundAttribute]
    public class UserInfoController : Controller
    {
        private Ilog _iLog;
        public UserInfoController()
        {
            _iLog = Log.GetInstance;
        }
        // GET: UserInfo
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetUserByCompayId()
        {
            try
            {
                int CompanyId = Convert.ToInt32(Session["CompayID"]);
                UserInfo user = new UserInfo();
                user.username = Session["username"].ToString();
                HttpResponseMessage response = GlobalVeriables.WebApiClient.PostAsJsonAsync("GetUserInfo",user).Result;
                UserModel model = response.Content.ReadAsAsync<UserModel>().Result;
                Session["userID"] = model.UserId;
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return Json(model, JsonRequestBehavior.AllowGet);
                }
                return Json("NotFount",JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(ex,JsonRequestBehavior.AllowGet);
            }
        } 
        
        [HttpPost]
        public ActionResult UpdateImage(UserModel model)
        {
            try
            {
                if (Request.Files.Count > 0)
                {
                    HttpFileCollectionBase files = Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        string fname;
                        //string path = AppDomain.CurrentDomain.BaseDirectory + "Uploads/";  
                        //string filename = Path.GetFileName(Request.Files[i].FileName);  

                        HttpPostedFileBase file = files[i];

                        // Checking for Internet Explorer  
                        if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                        {
                            string[] testfiles = file.FileName.Split(new char[] { '\\' });
                            fname = testfiles[testfiles.Length - 1];
                        }
                        else
                        {
                            string filename = Path.GetFileNameWithoutExtension(file.FileName);
                            string Extention = Path.GetExtension(file.FileName);

                            fname = file.FileName + DateTime.Now.ToString("yymmssfff") + Extention;
                            model.ImageUrl = fname;
                        }

                        // Get the complete folder path and store the file inside it.  
                        fname = Path.Combine(Server.MapPath("/images/"), fname);
                        file.SaveAs(fname);
                    }
                }

                HttpResponseMessage response = GlobalVeriables.WebApiClient.PutAsJsonAsync("UpdateUserImage", model).Result;
                UserModel UserModel = response.Content.ReadAsAsync<UserModel>().Result;

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return Json(UserModel, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(response.StatusCode, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        [HttpPost]    
        public ActionResult UpdateUserInfo(UserModel usermodel)
        {
            try
            {
                usermodel.CompanyId = Convert.ToInt32(Session["CompayID"]);
               
                HttpResponseMessage response = GlobalVeriables.WebApiClient.PostAsJsonAsync("PutUserInfo", usermodel).Result;
                UserModel Umodel = response.Content.ReadAsAsync<UserModel>().Result;

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return Json(Umodel, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("Failed", JsonRequestBehavior.AllowGet);
                }
            }
            catch(Exception ex)
            {
                return Json("Failed" + ex, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public async Task<string> UpdateUserPassword(ChangePasswordBindingModel model)
        {
            try
            {
                var jsonInput = new JavaScriptSerializer().Serialize(model);

                var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

                var response = await GlobalVeriables.WebApiClient.PostAsync("Account/ChangePassword", stringContent);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return "Ok";
                }
                else
                {
                    return "Not Found";
                }
                
            }
            catch (Exception ex)
            {
                return "Ex";
            }
           
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetAuthorityList()
        {
            try
            {
               // int companyId = Convert.ToInt32(Session["CompayID"]);
                HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("GetAuthorityList").Result;
               List<AuthorityModel> AuthorityModel = response.Content.ReadAsAsync<List<AuthorityModel>>().Result;
                
                return Json(AuthorityModel, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(ex.ToString(), JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public async Task<ActionResult> Register(NewUserModel model)
        {
            try
            {
                RegisterBindingModel NewModel = new RegisterBindingModel();

                NewModel.Email = model.email;
                NewModel.Password = model.password;
                NewModel.ConfirmPassword = model.confirmpassword;                

                var jsonInput = new JavaScriptSerializer().Serialize(NewModel);

                var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

                var response = await GlobalVeriables.WebApiClient.PostAsync("Account/register", stringContent);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    AspNetUser aspNetUser = new AspNetUser();
                    aspNetUser.UserName = model.email;
                    HttpResponseMessage responses = await GlobalVeriables.WebApiClient.PostAsJsonAsync("ConformNewUser", aspNetUser);

                    if (responses.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        CompanyUser companyUser = new CompanyUser();
                        companyUser.CompanyID = Convert.ToInt32(Session["CompayID"]);
                        companyUser.UserID = model.email;
                        companyUser.IsActive = true;
                        companyUser.AuthorityID = model.Authority;

                        HttpResponseMessage respons = await GlobalVeriables.WebApiClient.PostAsJsonAsync("PostCompanyUser", companyUser);

                        if(respons.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            UserTable userTable = new UserTable();
                            userTable.Username = model.email;
                            HttpResponseMessage responss = await GlobalVeriables.WebApiClient.PostAsJsonAsync("PostUserInfo", userTable);

                            if (responss.StatusCode == System.Net.HttpStatusCode.OK)
                            {
                                return Json(responss.StatusCode, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }
                   
                }
               
                return Json("OK", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("Fail",JsonRequestBehavior.AllowGet);
            }

        }
    }
}