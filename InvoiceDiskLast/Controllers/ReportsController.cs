using InvoiceDiskLast.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace InvoiceDiskLast.Controllers
{
    [SessionExpireAttribute]
    public class ReportsController : Controller
    {

        List<TransactionModel> _TransactionModel = new List<TransactionModel>();

        // GET: Reports
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Journal()
        {         
            try
            {
                HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("GetJournal/").Result;
                _TransactionModel = response.Content.ReadAsAsync<List<TransactionModel>>().Result;
                return View(_TransactionModel);
            }
            catch (Exception)
            {

                throw;
            }


            return View();
        }



    }
}