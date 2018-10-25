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
        public ActionResult AddOrEdit(CompanyViewModel CompnayViewModel)
        {
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
                            CompnayViewModel.CompanyLogo = fname;
                        }

                        // Get the complete folder path and store the file inside it.  
                        fname = Path.Combine(Server.MapPath("~/images/"), fname);
                        file.SaveAs(fname);
                    }
                    // Returns message that successfully uploaded  
                    if (CompnayViewModel.CompanyID == 0)
                    {
                       
                        HttpResponseMessage response = GlobalVeriables.WebApiClient.PostAsJsonAsync("APIComapny", CompnayViewModel).Result;
                        MVCCompanyInfoModel CompanyModel = response.Content.ReadAsAsync<MVCCompanyInfoModel>().Result;
                        Session["CompayID"] = CompanyModel.CompanyID;
                        return Json(response.StatusCode, JsonRequestBehavior.AllowGet);
                    }
                   
                }
                catch (Exception ex)
                {
                    return Json("Error occurred. Error details: " + ex.Message);
                }
                #endregion
            }
            else
            {
                return Json(false,JsonRequestBehavior.AllowGet);
            }
            return null;
            
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
                Session["CompanyContact"]=cominfo.CompanyPhone;
                return Json(cominfo, JsonRequestBehavior.AllowGet);            
            }
            else
            {
                return Json(cominfo);
            }
           
        }
       
    }
}