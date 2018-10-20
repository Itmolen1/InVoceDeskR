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
            ViewBag.Title = "Home Page";

            return View();
        }
              


        public ActionResult IndexTest()
        {
            ViewBag.Title = "Home Page Rukhsar Test Repo Rukhsar";

            return View();
        }

    }
}
