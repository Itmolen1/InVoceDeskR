using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InvoiceDiskLast.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }



        public ActionResult RegisterNew()
        {
            var id = Session["id"];
            return View();
        }

        [HttpPost]
        public ActionResult Logut()
        {
            Session.Clear();
            Session.Abandon();

            //return RedirectToAction("Index","Login");
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
    }
}