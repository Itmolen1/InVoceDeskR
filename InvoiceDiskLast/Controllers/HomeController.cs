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

   [SessionExpireAttribute]
   [RouteNotFoundAttribute]
    public class HomeController : Controller
    {
        private Ilog _iLog;
     
        public ActionResult Index()
        {
            return View();
        }

        public HomeController()
        {
            _iLog = Log.GetInstance;
        }
                
        //{
        //    _iLog.LogException(filterContext.Exception.ToString());
        //    filterContext.ExceptionHandled = true;
        //    this.View("Error").ExecuteResult(this.ControllerContext);
        //}
        public ActionResult IndexTestSamar()
        {
            ViewBag.Title = "Home Page Rukhsar Test Repo Rukhsar";
            //int k = Convert.ToInt32("sadadasd");
            return View();
        }

        public ActionResult IndexTestRukhsar()
        {
            ViewBag.Title = "Home Page Rukhsar Test Repo Rukhsar changes";

            return View();
        }

        public ActionResult Test()
        {
            return View();
        }
    }
}
