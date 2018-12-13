using InvoiceDiskLast.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace InvoiceDiskLast.Controllers
{
    public class QutationController : Controller
    {
        // GET: Qutation
        public ActionResult Index()
        {
            return View();
        }
        int Contactid, CompanyID;
        public QutationController()
        {
            //Contactid = Convert.ToInt32(Session["ClientID"]);
            //CompanyID = Convert.ToInt32(Session["CompayID"]);

            Contactid = 33;
            CompanyID = 1;
        }
          
        public ActionResult Create()
        {
            MVCQutationViewModel quutionviewModel = new MVCQutationViewModel();

           
            try
            {
                HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("ApiConatacts/" + Contactid.ToString()).Result;
                MVCContactModel contectmodel = response.Content.ReadAsAsync<MVCContactModel>().Result;

                HttpResponseMessage responseCompany = GlobalVeriables.WebApiClient.GetAsync("APIComapny/" + CompanyID.ToString()).Result;
                MVCCompanyInfoModel companyModel = responseCompany.Content.ReadAsAsync<MVCCompanyInfoModel>().Result;

                DateTime qutatioDate = new DateTime();
                qutatioDate = DateTime.Now;


                ViewBag.Contentdata = contectmodel;
                ViewBag.Companydata = companyModel;
                quutionviewModel.QutationDate = qutatioDate;
                quutionviewModel.DueDate = qutatioDate.AddDays(+15);

                
                MVCQutationModel q = new MVCQutationModel();
                HttpResponseMessage response1 = GlobalVeriables.WebApiClient.GetAsync("GetQuationCount/").Result;
                q = response1.Content.ReadAsAsync<MVCQutationModel>().Result;
                quutionviewModel.Qutation_ID = q.Qutation_ID;

                return View(quutionviewModel);
            }
            catch(Exception ex)
            {
                return null;
            }
           
        }
    }
}