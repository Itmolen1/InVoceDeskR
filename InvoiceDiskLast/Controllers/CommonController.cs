using InvoiceDiskLast.MISC;
using InvoiceDiskLast.Models;
using Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InvoiceDiskLast.Controllers
{
    [RouteNotFoundAttribute]
    public class CommonController : Controller
    {
        private Ilog _iLog;
        // GET: Common
        public ActionResult Index()
        {
            return View();
        }
        public CommonController()
        {
            _iLog = Log.GetInstance;
        }


        [NonAction]
        public static double CalculateVat(float vat, float Total)
        {
            Double Result = 0;
            try
            {
                 Result = Convert.ToDouble((Total / 100) * vat);

                return Result;
            }
            catch(Exception ex)
            {
                return Result;
            }
        }

        
        public ActionResult DeleteFile(string FileName)
        {
            try
            {
                if (CreatDirectoryClass.DeleteFileFromPDF(FileName))
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


        [HttpPost]
        public ActionResult UploadFileToPDF()
        {
            try
            {
                HttpFileCollectionBase files = Request.Files;
                for (int i = 0; i < files.Count; i++)
                {
                    HttpPostedFileBase file = files[i];

                    string FileName = CreatDirectoryClass.UploadToPDFCommon(file);

                    return Json(FileName, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {

                throw;
            }

            return Json("Success", JsonRequestBehavior.AllowGet);
        }
    }
}