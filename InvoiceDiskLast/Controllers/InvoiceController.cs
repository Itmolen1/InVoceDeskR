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
    public class InvoiceController : Controller
    {
        // GET: Invoice
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Create()
        {

            int Contactid, CompanyID;
            MVCQutationViewModel quutionviewModel = new MVCQutationViewModel();
            //Contactid = Convert.ToInt32(Session["ClientID"]);
            Contactid = 1;
            //CompanyID = Convert.ToInt32(Session["CompayID"]);
            CompanyID = 1;

            try
            {
                HttpResponseMessage responseCompany = GlobalVeriables.WebApiClient.GetAsync("APIComapny/" + CompanyID.ToString()).Result;
                MVCCompanyInfoModel companyModel = responseCompany.Content.ReadAsAsync<MVCCompanyInfoModel>().Result;

                HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("ApiConatacts/" + Contactid.ToString()).Result;
                MVCContactModel contectmodel = response.Content.ReadAsAsync<MVCContactModel>().Result;
                
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

                return View();
            }
            catch (Exception ex)
            {
                return null;
            }
           
        }

        [HttpPost]
        public ActionResult SaveDraft(MVCQutationViewModel MVCQutationViewModel)
        {
           
           
            MVCQutationModel mvcQutationModel = new MVCQutationModel();
            try
            {


                int ? companyId = 1;

                mvcQutationModel.Qutation_ID = MVCQutationViewModel.Qutation_ID;
                mvcQutationModel.CompanyId = companyId;
                mvcQutationModel.UserId = 1;
                mvcQutationModel.ContactId = 1;

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
                mvcQutationModel.Status = "accepted";
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

                mvcQutationModel = response.Content.ReadAsAsync<MVCQutationModel>().Result;

             

                if (response.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    foreach (QutationDetailsTable QDTList in MVCQutationViewModel.QutationDetailslist)
                    {
                        QutationDetailsTable QtDetails = new QutationDetailsTable();
                        QtDetails.ItemId = Convert.ToInt32(QDTList.ItemId);
                        QtDetails.QutationID = mvcQutationModel.QutationID; 
                        QtDetails.Description = QDTList.Description;
                        QtDetails.QutationDetailId = QDTList.QutationDetailId;
                        QtDetails.Quantity = QDTList.Quantity;
                        QtDetails.Rate = Convert.ToDouble(QDTList.Rate);
                        QtDetails.Total = Convert.ToDouble(QDTList.Total);
                        QtDetails.ServiceDate = QDTList.ServiceDate;
                        QtDetails.RowSubTotal = QDTList.RowSubTotal;

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

                    return new JsonResult { Data = new { Status = "Success", path = "", QutationId = mvcQutationModel.QutationID
                }


            };
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

        [HttpGet]
        public ActionResult Edit(int Id)
        {
            MVCQutationViewModel quotionviewModel = new MVCQutationViewModel();
            try
            {
                HttpResponseMessage responseQutation = GlobalVeriables.WebApiClient.GetAsync("APIQutation/" + Id.ToString()).Result;
                MVCQutationModel QutationModel = responseQutation.Content.ReadAsAsync<MVCQutationModel>().Result;
                quotionviewModel.QutationID = QutationModel.QutationID;
                quotionviewModel.CompanyId = QutationModel.CompanyId;
               // quotionviewModel.ConatctId = QutationModel.ContactId;


                int? Contectid = QutationModel.ContactId;
             
                int? CompanyID = QutationModel.CompanyId;

                HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("ApiConatacts/" + Contectid.ToString()).Result;
                MVCContactModel contectmodel = response.Content.ReadAsAsync<MVCContactModel>().Result;

                HttpResponseMessage responseCompany = GlobalVeriables.WebApiClient.GetAsync("APIComapny/" + CompanyID.ToString()).Result;
                MVCCompanyInfoModel companyModel = responseCompany.Content.ReadAsAsync<MVCCompanyInfoModel>().Result;


                quotionviewModel.DueDate = QutationModel.DueDate;
                quotionviewModel.QutationDate = QutationModel.QutationDate;

                HttpResponseMessage responseQutationDetailsList = GlobalVeriables.WebApiClient.GetAsync("APIQutationDetails/" + Id.ToString()).Result;
                List<MVCQutationViewModel> QutationModelDetailsList = responseQutationDetailsList.Content.ReadAsAsync<List<MVCQutationViewModel>>().Result;


                HttpResponseMessage GoodResponse = GlobalVeriables.WebApiClient.GetAsync("APIProduct/" + CompanyID + "/Good").Result;
                List<MVCProductModel> GoodModel = GoodResponse.Content.ReadAsAsync<List<MVCProductModel>>().Result;
                ViewBag.Good = GoodModel;


                HttpResponseMessage Services = GlobalVeriables.WebApiClient.GetAsync("APIProduct/" + CompanyID + "/Services").Result;
                List<MVCProductModel> ServiceModel = Services.Content.ReadAsAsync<List<MVCProductModel>>().Result;
                ViewBag.Service = ServiceModel;


                List<VatModel> model = new List<VatModel>();
                model.Add(new VatModel() { Vat1 = 0, Name = "0" });

                model.Add(new VatModel() { Vat1 = 6, Name = "6" });
                model.Add(new VatModel() { Vat1 = 21, Name = "21" });

                ViewBag.VatDrop = model;

                ViewBag.Contentdata = contectmodel;
                ViewBag.Companydata = companyModel;
                ViewBag.QutationDat = QutationModel;
                ViewBag.QutationDatailsList = QutationModelDetailsList;

                return View(quotionviewModel);
            }
            catch(Exception EX)
            {
                return null;
            }
           
        }

        [HttpPost]
        public ActionResult Edit(MVCQutationViewModel MVCQutationViewModel)
        {
            MVCQutationModel mvcQutationModel = new MVCQutationModel();
            try
            {

                mvcQutationModel.Qutation_ID = MVCQutationViewModel.Qutation_ID;


                mvcQutationModel.Qutation_ID = MVCQutationViewModel.Qutation_ID;
                mvcQutationModel.CompanyId = MVCQutationViewModel.CompanyId;
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
                mvcQutationModel.Status = "accepted";

                if (mvcQutationModel.TotalVat6 != null)
                {
                    double vat61 = Math.Round((double)mvcQutationModel.TotalVat6, 2, MidpointRounding.AwayFromZero);
                    mvcQutationModel.TotalVat6 = vat61;
                }

                mvcQutationModel.TotalVat21 = MVCQutationViewModel.TotalVat21;

                if (mvcQutationModel.TotalVat21 != null)
                {
                    double vat21 = Math.Round((double)mvcQutationModel.TotalVat21, 2, MidpointRounding.AwayFromZero);
                    mvcQutationModel.TotalVat21 = vat21;
                }

                mvcQutationModel.Qutation_ID = MVCQutationViewModel.Qutation_ID;

                HttpResponseMessage response = GlobalVeriables.WebApiClient.PutAsJsonAsync("APIQutation/" + mvcQutationModel.QutationID, mvcQutationModel).Result;
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    foreach (QutationDetailsTable QDTList in MVCQutationViewModel.QutationDetailslist)
                    {
                        QutationDetailsTable QtDetails = new QutationDetailsTable();
                        QtDetails.ItemId = Convert.ToInt32(QDTList.ItemId);
                        QtDetails.QutationID = MVCQutationViewModel.QutationID;
                        QtDetails.Description = QDTList.Description;
                        QtDetails.QutationDetailId = QDTList.QutationDetailId;
                        QtDetails.Quantity = QDTList.Quantity;
                        QtDetails.Rate = Convert.ToDouble(QDTList.Rate);
                        QtDetails.Total = Convert.ToDouble(QDTList.Total);
                        QtDetails.ServiceDate = QDTList.ServiceDate;
                        QtDetails.RowSubTotal = QDTList.RowSubTotal;
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

                    return new JsonResult { Data = new { Status = "Success", QutationId = MVCQutationViewModel.QutationID } };

                }
                else
                {
                    return new JsonResult { Data = new { Status = "Fail", QutationId = MVCQutationViewModel.QutationID } };
                }
            }
            catch (Exception ex)
            {

                return new JsonResult { Data = new { Status = "Fail", Message = ex.Message.ToString() } };
            }
        }


        [HttpGet]
        public ActionResult Print(int id)
        {
           try
            {
                HttpResponseMessage responseQutation = GlobalVeriables.WebApiClient.GetAsync("APIQutation/" + id.ToString()).Result;
                MVCQutationModel QutationModel = responseQutation.Content.ReadAsAsync<MVCQutationModel>().Result;

                HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("ApiConatacts/" + QutationModel.ContactId.ToString()).Result;
                MVCContactModel contectmodel = response.Content.ReadAsAsync<MVCContactModel>().Result;

                HttpResponseMessage responseCompany = GlobalVeriables.WebApiClient.GetAsync("APIComapny/" + QutationModel.CompanyId.ToString()).Result;
                MVCCompanyInfoModel companyModel = responseCompany.Content.ReadAsAsync<MVCCompanyInfoModel>().Result;

             

                HttpResponseMessage responseQutationDetailsList = GlobalVeriables.WebApiClient.GetAsync("APIQutationDetails/" + id.ToString()).Result;
                List<MVCQutationViewModel> QutationModelDetailsList = responseQutationDetailsList.Content.ReadAsAsync<List<MVCQutationViewModel>>().Result;

                DateTime qutationDueDate = Convert.ToDateTime(QutationModel.DueDate); //mm/dd/yyyy
                DateTime qutationDate = Convert.ToDateTime(QutationModel.QutationDate);//mm/dd/yyyy
                TimeSpan ts = qutationDueDate.Subtract(qutationDate);
                string diffDate = ts.Days.ToString();

                ViewBag.Contentdata = contectmodel;
                ViewBag.Companydata = companyModel;
                ViewBag.QutationDat = QutationModel;
                ViewBag.QutationDatailsList = QutationModelDetailsList;

                string PdfName = id + "-" + companyModel.CompanyName + ".pdf";

                return new Rotativa.PartialViewAsPdf("~/Views/Invoice/PrintInvoice.cshtml")
                {
                    FileName = PdfName,

                    PageSize = Rotativa.Options.Size.A4,
                    MinimumFontSize = 16,
                    PageMargins = new Rotativa.Options.Margins(5, 0, 10, 0),

                    PageHeight = 40,
                    CustomSwitches = "--footer-center \"" + "Wilt u zo vriendelijk zijn om het verschuldigde bedrag binnen " + diffDate + " dagen over te maken naar IBAN:\n  " + companyModel.IBANNumber + " ten name van IT Molen o.v.v.bovenstaande factuurnummer. \n (Op al onze diensten en producten zijn onze algemene voorwaarden van toepassing.Deze kunt u downloaden van onze website.)\n" + "  Printed date: " +
                    DateTime.Now.Date.ToString("MM/dd/yyyy") + "  Page: [page]/[toPage]\"" +
                   " --footer-line --footer-font-size \"10\" --footer-spacing 6 --footer-font-name \"calibri light\"",

                };
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpGet]
        public ActionResult ViewInvoice(int id)
        {
            try
            {

                HttpResponseMessage responseQutation = GlobalVeriables.WebApiClient.GetAsync("APIQutation/" + id.ToString()).Result;
                MVCQutationModel QutationModel = responseQutation.Content.ReadAsAsync<MVCQutationModel>().Result;

                //int? Contactid = QutationModel.ContactId;
                int? Contactid = 1;

                int? CompanyID = QutationModel.CompanyId;
                HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("ApiConatacts/" + Contactid.ToString()).Result;
                MVCContactModel contectmodel = response.Content.ReadAsAsync<MVCContactModel>().Result;

                HttpResponseMessage responseCompany = GlobalVeriables.WebApiClient.GetAsync("APIComapny/" + CompanyID.ToString()).Result;
                MVCCompanyInfoModel companyModel = responseCompany.Content.ReadAsAsync<MVCCompanyInfoModel>().Result;
                
                HttpResponseMessage responseQutationDetailsList = GlobalVeriables.WebApiClient.GetAsync("APIQutationDetails/" + id.ToString()).Result;
                List<MVCQutationViewModel> QutationModelDetailsList = responseQutationDetailsList.Content.ReadAsAsync<List<MVCQutationViewModel>>().Result;

                ViewBag.Contentdata = contectmodel;
                ViewBag.Companydata = companyModel;
                ViewBag.QutationDat = QutationModel;
                ViewBag.QutationDatailsList = QutationModelDetailsList;
                return View();
            }
            catch (Exception ex)
            {
                return null;
            }
          
        }


       
    }
}