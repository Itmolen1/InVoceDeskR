using InvoiceDiskLast.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InvoiceDiskLast.Controllers
{
    [SessionExpireAttribute]
    public class PurchaseDetailController : Controller
    {
        // GET: PurchaseDetail
        public ActionResult Index()
        {
            return View();
        }
    }
}