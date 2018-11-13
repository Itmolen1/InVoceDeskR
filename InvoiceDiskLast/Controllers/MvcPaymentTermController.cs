using InvoiceDiskLast.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace InvoiceDiskLast.Controllers
{
    public class MvcPaymentTermController : Controller
    {
        // GET: MvcPaymentTerm
        public ActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public ActionResult GetPaymentTerm()
        {
           int companyId = Convert.ToInt32(Session["CompayID"]);
            HttpResponseMessage response1 = GlobalVeriables.WebApiClient.GetAsync("GetPaymentTerm/" + companyId).Result;
            
            List<PaymentTermModel> objpaymentTerm = response1.Content.ReadAsAsync<List<PaymentTermModel>>().Result;
            return Json(objpaymentTerm, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult PostPaymntTerm(PaymentTermModel paymentTermModel)
        {

          
            HttpResponseMessage response = GlobalVeriables.WebApiClient.PostAsJsonAsync("PostpaymentTerm",paymentTermModel).Result;

            if(response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return Json("OK",JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("Failed", JsonRequestBehavior.AllowGet);
            }
        }

    }
}