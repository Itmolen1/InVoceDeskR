﻿using InvoiceDiskLast.Models;
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
            catch (Exception ex)
            {
                return null;
            }

        }


        int Contectid = 0;

        public ActionResult ViewQuation(int? quautionId)
        {
            try
            {
                var idd = Session["ClientID"];
                var cdd = Session["CompayID"];


                if (Session["ClientID"] != null && Session["CompayID"] != null)
                {
                    Contectid = Convert.ToInt32(Session["ClientID"]);
                    CompanyID = Convert.ToInt32(Session["CompayID"]);
                }

                HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("ApiConatacts/" + Contectid.ToString()).Result;
                MVCContactModel contectmodel = response.Content.ReadAsAsync<MVCContactModel>().Result;

                HttpResponseMessage responseCompany = GlobalVeriables.WebApiClient.GetAsync("APIComapny/" + CompanyID.ToString()).Result;
                MVCCompanyInfoModel companyModel = responseCompany.Content.ReadAsAsync<MVCCompanyInfoModel>().Result;

                HttpResponseMessage responseQutation = GlobalVeriables.WebApiClient.GetAsync("APIQutation/" + quautionId.ToString()).Result;
                MVCQutationModel QutationModel = responseQutation.Content.ReadAsAsync<MVCQutationModel>().Result;

                GlobalVeriables.WebApiClient.DefaultRequestHeaders.Clear();


                HttpResponseMessage responseQutationDetailsList = GlobalVeriables.WebApiClient.GetAsync("APIQutationDetails/" + quautionId.ToString()).Result;
                List<MVCQutationViewModel> QutationModelDetailsList = responseQutationDetailsList.Content.ReadAsAsync<List<MVCQutationViewModel>>().Result;

                ViewBag.Contentdata = contectmodel;
                ViewBag.Companydata = companyModel;
                ViewBag.QutationDat = QutationModel;
                ViewBag.QutationDatailsList = QutationModelDetailsList;
            }
            catch (Exception ex)
            {
            }

            return View();
        }



        [HttpPost]
        public ActionResult SaveDraft(MVCQutationViewModel MVCQutationViewModel)
        {
            var Qutationid = "";
            int Qid = 0;

            int companyId = 0;
            MVCQutationModel mvcQutationModel = new MVCQutationModel();
            try
            {
                if (Session["CompayID"] != null)
                {
                    companyId = Convert.ToInt32(Session["CompayID"]);
                }



                mvcQutationModel.Qutation_ID = MVCQutationViewModel.Qutation_ID;
                mvcQutationModel.CompanyId = companyId;
                mvcQutationModel.UserId = 1;
                mvcQutationModel.ContactId = MVCQutationViewModel.ConatctId;

                mvcQutationModel.QutationID = MVCQutationViewModel.QutationID;
                mvcQutationModel.RefNumber = MVCQutationViewModel.RefNumber;
                mvcQutationModel.QutationDate = MVCQutationViewModel.QutationDate;
                mvcQutationModel.DueDate = MVCQutationViewModel.DueDate;
                mvcQutationModel.SubTotal = MVCQutationViewModel.SubTotal;
                mvcQutationModel.DiscountAmount = MVCQutationViewModel.DiscountAmount;
                mvcQutationModel.TotalAmount = MVCQutationViewModel.TotalAmount;
                mvcQutationModel.CustomerNote = MVCQutationViewModel.CustomerNote;
                mvcQutationModel.Qutation_ID = MVCQutationViewModel.Qutation_ID;
                mvcQutationModel.TotalVat6 = MVCQutationViewModel.TotalVat6;
                mvcQutationModel.TotalVat21 = MVCQutationViewModel.TotalVat21;
                mvcQutationModel.Type = StatusEnum.Goods.ToString();
                mvcQutationModel.Status = "open";
                if (mvcQutationModel.TotalVat6 != null)
                {
                    double vat61 = Math.Round((double)mvcQutationModel.TotalVat6, 2, MidpointRounding.AwayFromZero);
                    mvcQutationModel.TotalVat6 = vat61;
                }



                if (mvcQutationModel.TotalVat21 != null)
                {
                    double vat21 = Math.Round((double)mvcQutationModel.TotalVat21, 2, MidpointRounding.AwayFromZero);
                    mvcQutationModel.TotalVat21 = vat21;
                }

                mvcQutationModel.Qutation_ID = MVCQutationViewModel.Qutation_ID;

                HttpResponseMessage response = GlobalVeriables.WebApiClient.PostAsJsonAsync("APIQutation/" + mvcQutationModel.QutationID, mvcQutationModel).Result;

                IEnumerable<string> headerValues;
                var userId = string.Empty;

                if (response.Headers.TryGetValues("idd", out headerValues))
                {
                    Qutationid = headerValues.FirstOrDefault();
                }

                Qid = Convert.ToInt32(Qutationid);

                if (response.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    foreach (QutationDetailsTable QDTList in MVCQutationViewModel.QutationDetailslist)
                    {
                        QutationDetailsTable QtDetails = new QutationDetailsTable();
                        QtDetails.ItemId = Convert.ToInt32(QDTList.ItemId);
                        QtDetails.QutationID = Qid;
                        QtDetails.Description = QDTList.Description;
                        QtDetails.QutationDetailId = QDTList.QutationDetailId;
                        QtDetails.Quantity = QDTList.Quantity;
                        QtDetails.Rate = Convert.ToDouble(QDTList.Rate);
                        QtDetails.Total = Convert.ToDouble(QDTList.Total);
                        QtDetails.ServiceDate = QDTList.ServiceDate;
                        QtDetails.Vat = Convert.ToDouble(QDTList.Vat);
                        QtDetails.Type = QDTList.Type;
                        if (QtDetails.QutationDetailId == 0)
                        {
                            HttpResponseMessage responsses = GlobalVeriables.WebApiClient.PostAsJsonAsync("APIQutationDetails", QtDetails).Result;
                        }
                        else
                        {
                            HttpResponseMessage responsses = GlobalVeriables.WebApiClient.PutAsJsonAsync("APIQutationDetails/" + QtDetails.QutationDetailId, QtDetails).Result;
                        }
                    }



                    return new JsonResult { Data = new { Status = "Success", path = "", QutationId = Qid } };

                }
                else
                {
                    return Json("Fail", JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {

                return new JsonResult { Data = new { Status = "Fail", Message = ex.Message.ToString() } };
            }
          
        }

    }
}