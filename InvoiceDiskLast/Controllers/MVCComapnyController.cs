using InvoiceDiskLast.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace InvoiceDiskLast.Controllers
{
    [SessionExpireAttribute]
    public class MVCComapnyController : Controller
    {
        // GET: MVCComapny
        public ActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public ActionResult CreateCompany()
        {
            return View();
        }


        [HttpPost]
        public ActionResult AddOrEdit(MVCCompanyInfoModel compnayViewModel)
        {
            var se = Session["username"].ToString();
            if (Session["username"].ToString() != null)
            {
                compnayViewModel.UserName = Session["username"].ToString();

                if (Request.Files.Count > 0)
                {
                    #region
                    try
                    {
                        //  Get all files from Request object  
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
                                compnayViewModel.CompanyLogo = fname;
                            }


                            var path = Server.MapPath("/images/");



                            try
                            {
                                if (!System.IO.Directory.Exists(path))
                                {
                                    System.IO.Directory.CreateDirectory(path);
                                }

                                // Get the complete folder path and store the file inside it.  
                                fname = Path.Combine(Server.MapPath("/images/"), fname);
                                file.SaveAs(fname);
                            }
                            catch (Exception)
                            {

                                throw;
                            }
                        }

                        // Returns message that successfully uploaded  
                        if (compnayViewModel.CompanyID == null)
                        {

                            HttpResponseMessage response = GlobalVeriables.WebApiClient.PostAsJsonAsync("APIComapny", compnayViewModel).Result;
                            MVCCompanyInfoModel CompanyModel = response.Content.ReadAsAsync<MVCCompanyInfoModel>().Result;
                            Session["CompayID"] = CompanyModel.CompanyID;
                            return Json(response.StatusCode, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            HttpResponseMessage response = GlobalVeriables.WebApiClient.PutAsJsonAsync("APIComapny", compnayViewModel).Result;
                            MVCCompanyInfoModel CompanyModel = response.Content.ReadAsAsync<MVCCompanyInfoModel>().Result;
                            return Json(response.StatusCode, JsonRequestBehavior.AllowGet);
                        }

                    }
                    catch (Exception ex)
                    {
                        return Json("Please select logo: " + ex.Message);
                    }
                    #endregion

                }
                else
                {
                    return Json(se, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return RedirectToAction("Index", "Captcha");
            }

        }



        [HttpPost]
        public ActionResult UpdateCompany(MVCCompanyInfoModel compnayViewModel)
        {
            compnayViewModel.CompanyID = Convert.ToInt32(Session["CompayID"]);
            if (compnayViewModel.CompanyID > 0)
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
                                compnayViewModel.CompanyLogo = fname;
                            }

                            // Get the complete folder path and store the file inside it.  
                            fname = Path.Combine(Server.MapPath("/images/"), fname);
                            file.SaveAs(fname);
                        }
                    }

                    HttpResponseMessage response = GlobalVeriables.WebApiClient.PutAsJsonAsync("APIComapny/" + compnayViewModel.CompanyID, compnayViewModel).Result;
                    MVCCompanyInfoModel CompanyModel = response.Content.ReadAsAsync<MVCCompanyInfoModel>().Result;
                    return Json(response.StatusCode, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }

            return null;
        }

        [HttpGet]
        public ActionResult CompanyEdit(int id = 0)
        {

            id = Convert.ToInt32(Session["CompayID"]);

            HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("APIComapny/" + id.ToString()).Result;
            return Json(response.Content.ReadAsAsync<MVCCompanyInfoModel>().Result, JsonRequestBehavior.AllowGet);

        }


        [HttpGet]
        public ActionResult GetCompanyInfo(string ID)
        {
            MVCCompanyInfoModel cominfo = new MVCCompanyInfoModel();
            if (ID != null)
            {
                Session["CompayID"] = ID;

                HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("APIComapny/" + ID.ToString()).Result;
                cominfo = response.Content.ReadAsAsync<MVCCompanyInfoModel>().Result;
                Session["CompanyName"] = cominfo.CompanyName;
                Session["CompanyEmail"] = cominfo.CompanyEmail;
                Session["CompanyContact"] = cominfo.CompanyPhone;
                return Json(cominfo, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(cominfo);
            }

        }

    }
}