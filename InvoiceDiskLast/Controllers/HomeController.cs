using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InvoiceDiskLast.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            string AlreadCheck = "";
         
            if (Session["CompayID"] == null )
            {
                if (AlreadCheck == "")
                {
                    AlreadCheck = "true";

                    return RedirectToAction("Index", "Login");
                }
                else
                {
                   // return RedirectToAction("Index", "Login");
                }  
            }
           
                return View();
            }
        
              

       
        public ActionResult IndexTestSamar()
        {
            ViewBag.Title = "Home Page Rukhsar Test Repo Rukhsar";

            return View();
        }

        public ActionResult IndexTestRukhsar()
        {
            ViewBag.Title = "Home Page Rukhsar Test Repo Rukhsar changes";

            return View();
        }
    }
}
