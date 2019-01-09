using InvoiceDiskLast.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InvoiceDiskLast.Controllers
{
    public class TestController : Controller
    {
       
        public ActionResult Index()
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

    }
}