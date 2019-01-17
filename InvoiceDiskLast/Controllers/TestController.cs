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
    public class TestController : Controller
    {
       
        public ActionResult Index()
        {
            HttpResponseMessage responseCompany = GlobalVeriables.WebApiClient.GetAsync("APIComapny/" + 53.ToString()).Result;
            MVCCompanyInfoModel companyModel = responseCompany.Content.ReadAsAsync<MVCCompanyInfoModel>().Result;

            HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("ApiConatacts/" + 64.ToString()).Result;
            MVCContactModel contectmodel = response.Content.ReadAsAsync<MVCContactModel>().Result;


            CommonModel commonModel = new CommonModel();
            commonModel.Name = "Invoice";
            commonModel.ReferenceNumber = "1221";
            commonModel.FromDate = System.DateTime.Now;
            commonModel.DueDate = System.DateTime.Now;
            commonModel.Number_Id = "1110001";

            ViewBag.Contentdata = contectmodel;
            ViewBag.Companydata = companyModel;
            ViewBag.commonModel = commonModel;

            return View();
        }


        public ActionResult ReportView()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DeleteFile(int Id, string Description,string FileName)
        {
            try
            {


                if (CreatDirectoryClass.Delete(Id, FileName, "Quotation"))
                {

                    return Json("Success", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("Fail", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                return Json("Fail", JsonRequestBehavior.AllowGet);
                throw;
            }


        }


        [HttpGet]
        public ActionResult deleteFile(string FileName)
        {
            try
            {
                var root = Server.MapPath("/PDF/");
                var path = Path.Combine(root, FileName);
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                throw;
            }
        }


        public ActionResult QuotationViewTest()
        {
            return View();
        }


        public  ActionResult TestCrystal()
        {
          

            DBEntities entities = new DBEntities();
            return View(from ComapnyInfo in entities.ComapnyInfoes.Take(10)
                        select ComapnyInfo);
        }

    }
}