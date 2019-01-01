using InvoiceDiskLast.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace InvoiceDiskLast.Controllers
{
    public class PaymentTermController : Controller
    {
        // GET: PaymentTerm
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult GetPaymentTermDuration()
        {
            List<PaymentTermDuration> paymentTermDuration = new List<PaymentTermDuration>();
            try
            {
                HttpResponseMessage Response = GlobalVeriables.WebApiClient.GetAsync("GetPaymentTermDurations").Result;
                paymentTermDuration = Response.Content.ReadAsAsync<List<PaymentTermDuration>>().Result;

                return Json(paymentTermDuration, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return null;
            }
        }
    }
}