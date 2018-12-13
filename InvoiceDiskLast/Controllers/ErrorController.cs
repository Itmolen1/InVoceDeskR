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
    public class ErrorController : Controller
    {
        private Ilog _iLog;

        public ErrorController()
        {
            _iLog = Log.GetInstance;
        }
        // GET: Error
        public ViewResult Index()
        {
            return View("Error");
            
        }

        public ViewResult NotFound()
        {
            Response.StatusCode = 404; //you may want to set this to 200
            return View("NotFound");
        }

        public ViewResult ServerError()
        {
            return View("ServerErrorView");
        }
    }
}