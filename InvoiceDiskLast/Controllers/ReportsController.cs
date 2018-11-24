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
        MVCCompanyInfoModel _company = new MVCCompanyInfoModel();
        SearchModel _SearchModel = new SearchModel();
        int CompanyId = 0;

        // GET: Reports
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Journal()
        {

            if (Session["CompayID"] != null)
            {
                CompanyId = Convert.ToInt32(Session["CompayID"]);

                HttpResponseMessage responseCompany = GlobalVeriables.WebApiClient.GetAsync("APIComapny/" + CompanyId.ToString()).Result;
                _company = responseCompany.Content.ReadAsAsync<MVCCompanyInfoModel>().Result;
                TempData["Compantinfo"] = _company;
                TempData.Keep();
            }


            _SearchModel.FromDate = DateTime.Now;
            _SearchModel.Todate = DateTime.Now.AddDays(-20);
            long FromDate = DateTime.Now.Ticks;
            DateTime dt = DateTime.Now.AddDays(-20);
            long TDate = DateTime.Now.AddDays(-20).Ticks;

            try
            {

                HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("GetJournal/" + FromDate + "/" + TDate).Result;
                _SearchModel._TransactionList = response.Content.ReadAsAsync<List<TransactionModel>>().Result;
                _SearchModel.Todate = DateTime.Now.AddDays(-20);
                return View(_SearchModel);
            }
            catch (Exception)
            {

                throw;
            }


            return View();
        }




        [HttpPost]
        public ActionResult Journal(SearchModel _searchModel)
        {
            SearchModel _model = new SearchModel();
            try
            {

                long FromDate = _searchModel.FromDate.Ticks;
                long TDate = _searchModel.FromDate.Ticks;

                HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("GetJournal/" + FromDate + "/" + TDate).Result;
                _model._TransactionList = response.Content.ReadAsAsync<List<TransactionModel>>().Result;

                if (TempData["Compantinfo"] == null)
                {
                    CompanyId = Convert.ToInt32(Session["CompayID"]);

                    HttpResponseMessage responseCompany = GlobalVeriables.WebApiClient.GetAsync("APIComapny/" + CompanyId.ToString()).Result;
                    _company = responseCompany.Content.ReadAsAsync<MVCCompanyInfoModel>().Result;
                    TempData["Compantinfo"] = _company;
                    TempData.Keep();
                }

            }
            catch (Exception)
            {

            }

            return View(_model);
        }



    }
}