using InvoiceDiskLast.MISC;
using Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InvoiceDiskLast.Controllers
{
    [RouteNotFoundAttribute]
    public class ExceptionController : Controller
    {
        private Ilog _iLog;
        public ExceptionController()
        {
            _iLog = Log.GetInstance;
        }
        // GET: Exception
        public ActionResult Index()
        {
            return View();
        }
    }
}