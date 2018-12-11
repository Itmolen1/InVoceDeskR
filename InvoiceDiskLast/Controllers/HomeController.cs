﻿using InvoiceDiskLast.MISC;
using InvoiceDiskLast.Models;
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
     
        public ActionResult Index()
        {
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

        public ActionResult Test()
        {
            return View();
        }
    }
}
