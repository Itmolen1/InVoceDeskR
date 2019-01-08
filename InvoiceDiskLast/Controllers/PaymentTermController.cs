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
                int id = Convert.ToInt32(Session["CompayID"]);
                HttpResponseMessage Response = GlobalVeriables.WebApiClient.GetAsync("GetPaymentTermDurations/"+ id).Result;
                paymentTermDuration = Response.Content.ReadAsAsync<List<PaymentTermDuration>>().Result;

                return Json(paymentTermDuration, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        [HttpPost]
        public ActionResult Save(PaymentTermDuration paymentTermDuration)
        {
            try
            {
                paymentTermDuration.Status = true;
                paymentTermDuration.CompanyId = Convert.ToInt32(Session["CompayID"]);
                HttpResponseMessage response = GlobalVeriables.WebApiClient.PostAsJsonAsync("PaymentTermDurations", paymentTermDuration).Result;
                paymentTermDuration = response.Content.ReadAsAsync<PaymentTermDuration>().Result;
                if(response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return Json("Success",JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("Failed", JsonRequestBehavior.AllowGet);
                }
            }
            catch(Exception ex)
            {
                return Json("Failed", JsonRequestBehavior.AllowGet);
            }
        }
    }
}