using InvoiceDiskLast.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace InvoiceDiskLast.Controllers
{
    [SessionExpireAttribute]
    public class QutationServiceController : Controller
    {
        int Contectid, CompanyID = 0;
        List<AttakmentList> _attackmentList = new List<AttakmentList>();
        // GET: QutationService
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult AddOrEdit(int id)
        {
            MVCQutationViewModel quutionviewModel = new MVCQutationViewModel();
            try
            {
                var idd = Session["ClientID"];
                var cdd = Session["CompayID"];

                if (Session["ClientID"] != null && Session["CompayID"] != null)
                {
                    Contectid = Convert.ToInt32(Session["ClientID"]);
                    CompanyID = Convert.ToInt32(Session["CompayID"]);

                    HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("ApiConatacts/" + Contectid.ToString()).Result;
                    MVCContactModel contectmodel = response.Content.ReadAsAsync<MVCContactModel>().Result;


                    HttpResponseMessage responseCompany = GlobalVeriables.WebApiClient.GetAsync("APIComapny/" + CompanyID.ToString()).Result;
                    MVCCompanyInfoModel companyModel = responseCompany.Content.ReadAsAsync<MVCCompanyInfoModel>().Result;

                    DateTime qutatioDate = new DateTime();
                    qutatioDate = DateTime.Now;


                    ViewBag.Contentdata = contectmodel;
                    ViewBag.Companydata = companyModel;
                    quutionviewModel.QutationDate = qutatioDate;
                    quutionviewModel.DueDate = qutatioDate.AddDays(+15);

                    if (id == 0)
                    {
                        int id3 = 0;
                        MVCQutationModel q = new MVCQutationModel();
                        HttpResponseMessage response1 = GlobalVeriables.WebApiClient.GetAsync("GetQuationCount/").Result;
                        q = response1.Content.ReadAsAsync<MVCQutationModel>().Result;
                        quutionviewModel.Qutation_ID = q.Qutation_ID;
                        return View(quutionviewModel);
                    }
                    else
                    {
                        ViewBag.edit = "true";

                        List<VatModel> model = new List<VatModel>();
                        model.Add(new VatModel() { Vat1 = 0, Name = "0" });

                        model.Add(new VatModel() { Vat1 = 6, Name = "6" });
                        model.Add(new VatModel() { Vat1 = 21, Name = "21" });

                        ViewBag.VatDrop = model;

                        HttpResponseMessage responsep = GlobalVeriables.WebApiClient.GetAsync("APIProduct/" + CompanyID + "/Services").Result;
                        List<MVCProductModel> productModel = responsep.Content.ReadAsAsync<List<MVCProductModel>>().Result;
                        ViewBag.Product = productModel;

                        HttpResponseMessage res = GlobalVeriables.WebApiClient.GetAsync("APIQutation/" + id.ToString()).Result;
                        MVCQutationModel ob = res.Content.ReadAsAsync<MVCQutationModel>().Result;

                        quutionviewModel.QutationID = ob.QutationID;
                        quutionviewModel.Qutation_ID = ob.Qutation_ID;
                        quutionviewModel.QutationDate = ob.QutationDate;
                        quutionviewModel.RefNumber = ob.RefNumber;
                        quutionviewModel.DueDate = ob.DueDate;
                        quutionviewModel.CustomerNote = ob.CustomerNote;
                        quutionviewModel.SubTotal = ob.SubTotal;
                        quutionviewModel.TotalAmount = ob.TotalAmount;
                        quutionviewModel.TotalVat21 = (ob.TotalVat21 != null ? (float)(ob.TotalVat21) : (float)0.00);
                        quutionviewModel.TotalVat6 = (ob.TotalVat6 != null ? (float)(ob.TotalVat6) : (float)0.00);
                        quutionviewModel.ConatctId = (Contectid != null ? Contectid : 0);
                        HttpResponseMessage responseQutationDetailsList = GlobalVeriables.WebApiClient.GetAsync("APIQutationDetails/" + id.ToString()).Result;
                        List<MVCQutationDetailsModel> QutationModelDetailsList = responseQutationDetailsList.Content.ReadAsAsync<List<MVCQutationDetailsModel>>().Result;
                        ViewBag.Contentdata = contectmodel;
                        ViewBag.Companydata = companyModel;
                        ViewBag.QutationDatailsList = QutationModelDetailsList;
                        return View(quutionviewModel);
                    }
                }
                else
                {
                    return RedirectToAction("Index", "Login");
                }

            }
            catch (Exception ex)
            {

                ViewBag.Message = ex.ToString();
            }
            return View(quutionviewModel);

        }



        [HttpPost]
        public ActionResult save(MVCQutationViewModel MVCQutationViewModel)
        {
            var QutationOrderId = "";
            int intQutationorderId = 0;
            MVCQutationModel mvcQutationModel = new MVCQutationModel();
            try
            {
                if (Session["ClientId"] != null && Session["CompayID"] != null)
                {
                    Contectid = Convert.ToInt32(Session["ClientId"]);
                    CompanyID = Convert.ToInt32(Session["CompayID"]);
                }
                else
                {
                    return RedirectToAction("Index", "Login");
                }

                mvcQutationModel.Qutation_ID = MVCQutationViewModel.Qutation_ID;
                mvcQutationModel.CompanyId = CompanyID;
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
                mvcQutationModel.ContactId = Contectid;
                mvcQutationModel.Type = StatusEnum.Services.ToString();
                if (MVCQutationViewModel.QutationID == 0 || MVCQutationViewModel.QutationID == null)
                {

                    HttpResponseMessage response = GlobalVeriables.WebApiClient.PostAsJsonAsync("APIQutation", mvcQutationModel).Result;
                    IEnumerable<string> headerValues;
                    var userId = string.Empty;
                    if (response.Headers.TryGetValues("idd", out headerValues))
                    {
                        QutationOrderId = headerValues.FirstOrDefault();
                    }
                    intQutationorderId = Convert.ToInt32(QutationOrderId);


                    if (response.StatusCode == System.Net.HttpStatusCode.Created)
                    {
                        foreach (var item in MVCQutationViewModel.QutationDetailslist)
                        {
                            QutationDetailsTable QtDetails = new QutationDetailsTable();
                            QtDetails.ItemId = Convert.ToInt32(item.ItemId);
                            QtDetails.QutationID = item.QutationID;
                            QtDetails.Description = item.Description;
                            QtDetails.QutationDetailId = item.QutationDetailId;
                            QtDetails.Quantity = item.Quantity;
                            QtDetails.Rate = Convert.ToDouble(item.Rate);
                            QtDetails.Total = Convert.ToDouble(item.Total);
                            QtDetails.Vat = Convert.ToDouble(item.Vat);
                            if (QtDetails.QutationDetailId == 0 || QtDetails.QutationDetailId == null)
                            {
                                QtDetails.QutationID = intQutationorderId;

                                HttpResponseMessage responsses = GlobalVeriables.WebApiClient.PostAsJsonAsync("APIQutationDetails", QtDetails).Result;
                            }
                            else
                            {
                                HttpResponseMessage responsses = GlobalVeriables.WebApiClient.PutAsJsonAsync("APIQutationDetails/" + QtDetails.QutationDetailId, QtDetails).Result;
                            }
                        }
                    }
                }

                else
                {
                    if (MVCQutationViewModel.QutationID != 0)
                    {
                        MVCQutationViewModel.QutationID = MVCQutationViewModel.QutationID;
                        intQutationorderId = Convert.ToInt32(MVCQutationViewModel.QutationID);
                        HttpResponseMessage response2 = GlobalVeriables.WebApiClient.PutAsJsonAsync("APIQutation/" + MVCQutationViewModel.QutationID, MVCQutationViewModel).Result;
                        intQutationorderId = (int)MVCQutationViewModel.QutationID;
                        if (response2.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            foreach (var item in MVCQutationViewModel.QutationDetailslist)
                            {
                                QutationDetailsTable qutationDetail = new QutationDetailsTable();
                                qutationDetail.QutationDetailId = item.QutationDetailId;
                                qutationDetail.ItemId = item.ItemId;
                                qutationDetail.Description = item.Description;
                                qutationDetail.Quantity = item.Quantity;
                                qutationDetail.Rate = item.Rate;
                                qutationDetail.Total = item.Total;
                                qutationDetail.Vat = item.Vat;
                                qutationDetail.QutationID = MVCQutationViewModel.QutationID;
                                if (qutationDetail.QutationDetailId == 0 || qutationDetail.QutationDetailId == null)
                                {
                                    HttpResponseMessage responsses = GlobalVeriables.WebApiClient.PostAsJsonAsync("APIQutationDetails", qutationDetail).Result;
                                }
                                else
                                {
                                    HttpResponseMessage responsses = GlobalVeriables.WebApiClient.PutAsJsonAsync("APIQutationDetails/" + qutationDetail.QutationDetailId, qutationDetail).Result;
                                }
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {

                return new JsonResult { Data = new { Status = "Fail", Message = ex.Message.ToString() } };
            }
            return new JsonResult { Data = new { Status = "Success", Qutation = intQutationorderId } };
        }






        [HttpPost]
        public ActionResult saveEmail(MVCQutationViewModel MVCQutationViewModel)
        {
            var QutationOrderId = "";
            int intQutationorderId = 0;
            MVCQutationModel mvcQutationModel = new MVCQutationModel();
            try
            {
                if (Session["ClientId"] != null && Session["CompayID"] != null)
                {
                    Contectid = Convert.ToInt32(Session["ClientId"]);
                    CompanyID = Convert.ToInt32(Session["CompayID"]);
                }
                else
                {
                    return RedirectToAction("Index", "Login");
                }

                mvcQutationModel.Qutation_ID = MVCQutationViewModel.Qutation_ID;
                mvcQutationModel.CompanyId = CompanyID;
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
                mvcQutationModel.ContactId = Contectid;
                mvcQutationModel.Type = StatusEnum.Goods.ToString();
                mvcQutationModel.Status = "open";
                mvcQutationModel.Type = StatusEnum.Services.ToString();
                if (MVCQutationViewModel.QutationID == 0 || MVCQutationViewModel.QutationID == null)
                {

                    HttpResponseMessage response = GlobalVeriables.WebApiClient.PostAsJsonAsync("APIQutation", mvcQutationModel).Result;
                    IEnumerable<string> headerValues;
                    var userId = string.Empty;
                    if (response.Headers.TryGetValues("idd", out headerValues))
                    {
                        QutationOrderId = headerValues.FirstOrDefault();
                    }
                    intQutationorderId = Convert.ToInt32(QutationOrderId);


                    if (response.StatusCode == System.Net.HttpStatusCode.Created)
                    {
                        foreach (var item in MVCQutationViewModel.QutationDetailslist)
                        {
                            QutationDetailsTable QtDetails = new QutationDetailsTable();
                            QtDetails.ItemId = Convert.ToInt32(item.ItemId);
                            QtDetails.QutationID = item.QutationID;
                            QtDetails.Description = item.Description;
                            QtDetails.QutationDetailId = item.QutationDetailId;
                            QtDetails.Quantity = item.Quantity;
                            QtDetails.Rate = Convert.ToDouble(item.Rate);
                            QtDetails.Total = Convert.ToDouble(item.Total);
                            QtDetails.Vat = Convert.ToDouble(item.Vat);
                            if (QtDetails.QutationDetailId == 0 || QtDetails.QutationDetailId == null)
                            {
                                QtDetails.QutationID = intQutationorderId;

                                HttpResponseMessage responsses = GlobalVeriables.WebApiClient.PostAsJsonAsync("APIQutationDetails", QtDetails).Result;
                            }
                            else
                            {
                                HttpResponseMessage responsses = GlobalVeriables.WebApiClient.PutAsJsonAsync("APIQutationDetails/" + QtDetails.QutationDetailId, QtDetails).Result;
                            }
                        }
                    }
                }

                else
                {
                    if (MVCQutationViewModel.QutationID != 0)
                    {
                        MVCQutationViewModel.QutationID = MVCQutationViewModel.QutationID;
                        intQutationorderId = Convert.ToInt32(MVCQutationViewModel.QutationID);
                        HttpResponseMessage response2 = GlobalVeriables.WebApiClient.PutAsJsonAsync("APIQutation/" + MVCQutationViewModel.QutationID, MVCQutationViewModel).Result;
                        intQutationorderId = (int)MVCQutationViewModel.QutationID;
                        if (response2.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            foreach (var item in MVCQutationViewModel.QutationDetailslist)
                            {
                                QutationDetailsTable qutationDetail = new QutationDetailsTable();
                                qutationDetail.QutationDetailId = item.QutationDetailId;
                                qutationDetail.ItemId = item.ItemId;
                                qutationDetail.Description = item.Description;
                                qutationDetail.Quantity = item.Quantity;
                                qutationDetail.Rate = item.Rate;
                                qutationDetail.Total = item.Total;
                                qutationDetail.Vat = item.Vat;
                                qutationDetail.QutationID = MVCQutationViewModel.QutationID;
                                if (qutationDetail.QutationDetailId == 0 || qutationDetail.QutationDetailId == null)
                                {
                                    HttpResponseMessage responsses = GlobalVeriables.WebApiClient.PostAsJsonAsync("APIQutationDetails", qutationDetail).Result;
                                }
                                else
                                {
                                    HttpResponseMessage responsses = GlobalVeriables.WebApiClient.PutAsJsonAsync("APIQutationDetails/" + qutationDetail.QutationDetailId, qutationDetail).Result;
                                }
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {

                return new JsonResult { Data = new { Status = "Fail", Message = ex.Message.ToString() } };
            }
            return new JsonResult { Data = new { Status = "Success", QutationId = intQutationorderId } };
        }



        public ActionResult ViewServiceQutation(int? quautionId)
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

        public static Boolean IsFileLocked(FileInfo file)
        {
            FileStream stream = null;
            try
            {
                stream = file.Open
                (
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.None
                );
            }
            catch (IOException ex)
            {
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            //file is not locked
            return false;
        }


        public string PrintView(int quttationId)
        {
            string pdfname;
            try
            {
                int contactId = 0;

                if (Session["ClientID"] != null)
                {
                    contactId = Convert.ToInt32(Session["ClientID"]);
                }


                HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("ApiConatacts/" + contactId.ToString()).Result;
                MVCContactModel contectmodel = response.Content.ReadAsAsync<MVCContactModel>().Result;

                int companyId = 0;
                if (Session["CompayID"] != null)
                {
                    companyId = Convert.ToInt32(Session["CompayID"]);
                }

                HttpResponseMessage responseCompany = GlobalVeriables.WebApiClient.GetAsync("APIComapny/" + companyId.ToString()).Result;
                MVCCompanyInfoModel companyModel = responseCompany.Content.ReadAsAsync<MVCCompanyInfoModel>().Result;
                HttpResponseMessage responseQutation = GlobalVeriables.WebApiClient.GetAsync("APIQutation/" + quttationId.ToString()).Result;
                MVCQutationModel QutationModel = responseQutation.Content.ReadAsAsync<MVCQutationModel>().Result;

                DateTime qutationDueDate = Convert.ToDateTime(QutationModel.DueDate); //mm/dd/yyyy
                DateTime qutationDate = Convert.ToDateTime(QutationModel.QutationDate);//mm/dd/yyyy
                TimeSpan ts = qutationDueDate.Subtract(qutationDate);
                string diffDate = ts.Days.ToString();


                HttpResponseMessage responseQutationDetailsList = GlobalVeriables.WebApiClient.GetAsync("APIQutationDetails/" + quttationId.ToString()).Result;
                List<MVCQutationViewModel> QutationModelDetailsList = responseQutationDetailsList.Content.ReadAsAsync<List<MVCQutationViewModel>>().Result;

                ViewBag.Contentdata = contectmodel;
                ViewBag.Companydata = companyModel;
                ViewBag.QutationDat = QutationModel;
                ViewBag.QutationDatailsList = QutationModelDetailsList;
                string companyName = quttationId + "-" + companyModel.CompanyName;



                var root = Server.MapPath("/PDF/");
                pdfname = String.Format("{0}.pdf", companyName);
                var path = Path.Combine(root, pdfname);
                path = Path.GetFullPath(path);

                string subPath = "/PDF"; // your code goes here
                bool exists = System.IO.Directory.Exists(Server.MapPath(subPath));

                if (!exists)
                {
                    System.IO.Directory.CreateDirectory(Server.MapPath(subPath));
                }
                if (System.IO.File.Exists(path))
                {
                    try
                    {

                        if (System.IO.File.Exists(path))
                        {
                            FileInfo info = new FileInfo(path);

                            if (!IsFileLocked(info)) info.Delete();

                        }
                    }
                    catch (System.IO.IOException e)
                    {

                    }
                }


                var pdfResult = new Rotativa.PartialViewAsPdf("~/Views/QutationService/Viewpp.cshtml")
                {

                    PageSize = Rotativa.Options.Size.A4,
                    MinimumFontSize = 16,
                    PageMargins = new Rotativa.Options.Margins(10, 12, 20, 3),
                    PageHeight = 40,

                    SaveOnServerPath = path, // Save your place

                    CustomSwitches = "--footer-center \"" + "Wilt u zo vriendelijk zijn om het verschuldigde bedrag binnen " + diffDate + " dagen over te maken naar IBAN: \n " + companyModel.IBANNumber + " ten name van IT Molen o.v.v.bovenstaande factuurnummer. \n (Op al onze diensten en producten zijn onze algemene voorwaarden van toepassing.Deze kunt u downloaden van onze website.)" + " \n Printed date: " +
                   DateTime.Now.Date.ToString("MM/dd/yyyy") + "  Page: [page]/[toPage]\"" +
                  " --footer-line --footer-font-size \"10\" --footer-spacing 6 --footer-font-name \"calibri light\"",

                };
                pdfResult.BuildPdf(this.ControllerContext);
            }
            catch (Exception)
            {

                throw;
            }

            return pdfname;
        }
        [HttpPost]
        public ActionResult DeleteQuatation(int QutationId, int QutationDetailID, int vat, decimal total)
        {
            try
            {

                MVCQutationViewModel viewModel = new MVCQutationViewModel();
                viewModel.QutationDetailId = QutationDetailID;
                HttpResponseMessage responseQutation = GlobalVeriables.WebApiClient.GetAsync("APIQutation/" + QutationId.ToString()).Result;
                MVCQutationModel QutationModel = responseQutation.Content.ReadAsAsync<MVCQutationModel>().Result;

                GlobalVeriables.WebApiClient.DefaultRequestHeaders.Clear();

                GlobalVeriables.WebApiClient.DefaultRequestHeaders.Add("QTID", QutationId.ToString());
                //GlobalVeriables.WebApiClient.DefaultRequestHeaders.Add("QutationDetailID1", QutationDetailID1);



                HttpResponseMessage responseQutationDetailsList = GlobalVeriables.WebApiClient.GetAsync("APIQutationDetails/" + QutationId.ToString()).Result;
                List<MVCQutationDetailsModel> QutationModelDetailsList = responseQutationDetailsList.Content.ReadAsAsync<List<MVCQutationDetailsModel>>().Result;
                MVCQutationDetailsModel CMODEL = new MVCQutationDetailsModel();

                if (QutationModelDetailsList.Count() > 1)
                {
                    QutationModelDetailsList = QutationModelDetailsList.Where(C => C.QutationDetailId == QutationDetailID).ToList();

                    #region
                    if (QutationModelDetailsList.Count() != 0)
                    {
                        CMODEL.Vat = QutationModelDetailsList[0].Vat;
                        CMODEL.Total = QutationModelDetailsList[0].Total;
                        QutationModel.SubTotal = QutationModel.SubTotal - CMODEL.Total;
                        QutationModel.TotalAmount = QutationModel.TotalAmount - (CMODEL.Vat + CMODEL.Total);

                        QutationModel.QutationID = QutationModel.QutationID;



                        QutationModel.QutationDate = QutationModel.QutationDate;
                        QutationModel.CustomerNote = QutationModel.CustomerNote;
                        QutationModel.Qutation_ID = QutationModel.Qutation_ID;
                        QutationModel.DueDate = QutationModel.DueDate;
                        QutationModel.Status = QutationModel.Status;
                        QutationModel.CompanyId = QutationModel.CompanyId;
                        if (vat == 6)
                            QutationModel.TotalVat6 = QutationModel.TotalVat6 - 6;
                        else
                            QutationModel.TotalVat21 = QutationModel.TotalVat21 - 21;
                        HttpResponseMessage response = GlobalVeriables.WebApiClient.PutAsJsonAsync("APIQutation/" + QutationId, QutationModel).Result;
                        if (response.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            HttpResponseMessage deleteQuaution2 = GlobalVeriables.WebApiClient.DeleteAsync("APIQutationDetails/" + QutationDetailID).Result;
                            if (deleteQuaution2.StatusCode == System.Net.HttpStatusCode.OK)
                            {
                                GlobalVeriables.WebApiClient.DefaultRequestHeaders.Clear();
                                return new JsonResult { Data = new { Status = "Success" } };
                            }
                        }
                        else
                        {
                            return new JsonResult { Data = new { Status = "Fail" } };


                        }
                        #endregion
                    }
                }

                else
                {

                    HttpResponseMessage Qdresponse = GlobalVeriables.WebApiClient.DeleteAsync("APIQutationDetails/" + QutationDetailID).Result;

                    if (Qdresponse.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        QutationModel.SubTotal = 0.00;
                        QutationModel.TotalAmount = 0.00;
                        QutationModel.Qutation_ID = QutationModel.Qutation_ID;
                        QutationModel.QutationID = QutationModel.QutationID;
                        QutationModel.QutationDate = QutationModel.QutationDate;
                        QutationModel.CustomerNote = QutationModel.CustomerNote;
                        QutationModel.Qutation_ID = QutationModel.Qutation_ID;
                        QutationModel.DueDate = QutationModel.DueDate;
                        QutationModel.Status = QutationModel.Status;
                        QutationModel.CompanyId = QutationModel.CompanyId;

                        QutationModel.TotalVat6 = 0;

                        QutationModel.TotalVat21 = 0;

                        HttpResponseMessage response = GlobalVeriables.WebApiClient.PutAsJsonAsync("APIQutation/" + QutationId, QutationModel).Result;
                        if (response.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            GlobalVeriables.WebApiClient.DefaultRequestHeaders.Clear();
                            return new JsonResult { Data = new { Status = "Success" } };
                        }
                    }
                    else
                    {
                        return new JsonResult { Data = new { Status = "Fail" } };
                    }

                }



            }
            catch (Exception)
            {

                return new JsonResult { Data = new { Status = "Fail" } };
            }

            return new JsonResult { Data = new { Status = "Success" } };
        }



        [DeleteFileClass]
        [HttpPost]
        public FileResult DownloadFile(string FilePath1)
        {
            string filepath = "";
            string FileName = SetPdfName(FilePath1);
            try
            {
                filepath = System.IO.Path.Combine(Server.MapPath("/PDF/"), FilePath1);
                HttpContext.Items["FilePath"] = FilePath1;

            }
            catch (Exception)
            {
            }

            return File(filepath, MimeMapping.GetMimeMapping(filepath), FileName);
        }


        public string SetPdfName(string FilePath)
        {
            int CompanyId = 0;
            try
            {

                if (Session["CompayID"] != null)
                {
                    CompanyId = Convert.ToInt32(Session["CompayID"]);
                }

                HttpResponseMessage responseCompany = GlobalVeriables.WebApiClient.GetAsync("APIComapny/" + CompanyId.ToString()).Result;
                MVCCompanyInfoModel companyModel = responseCompany.Content.ReadAsAsync<MVCCompanyInfoModel>().Result;
                string[] arrya = FilePath.Split('-');
                string InvoiceNumber = arrya[1];
                string PdfName1 = InvoiceNumber + "-" + companyModel.CompanyName + ".pdf";
                return PdfName1;
            }
            catch (Exception)
            {

                throw;
            }

        }


        public ActionResult Print(int? QutationID)
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
                else
                {
                    return RedirectToAction("Index", "Login");
                }






                HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("ApiConatacts/" + Contectid.ToString()).Result;
                MVCContactModel contectmodel = response.Content.ReadAsAsync<MVCContactModel>().Result;

                HttpResponseMessage responseCompany = GlobalVeriables.WebApiClient.GetAsync("APIComapny/" + CompanyID.ToString()).Result;
                MVCCompanyInfoModel companyModel = responseCompany.Content.ReadAsAsync<MVCCompanyInfoModel>().Result;

                HttpResponseMessage responseQutation = GlobalVeriables.WebApiClient.GetAsync("APIQutation/" + QutationID.ToString()).Result;
                MVCQutationModel QutationModel = responseQutation.Content.ReadAsAsync<MVCQutationModel>().Result;

                GlobalVeriables.WebApiClient.DefaultRequestHeaders.Clear();

                DateTime qutationDueDate = Convert.ToDateTime(QutationModel.DueDate); //mm/dd/yyyy
                DateTime qutationDate = Convert.ToDateTime(QutationModel.QutationDate);//mm/dd/yyyy
                TimeSpan ts = qutationDueDate.Subtract(qutationDate);
                string diffDate = ts.Days.ToString();

                HttpResponseMessage responseQutationDetailsList = GlobalVeriables.WebApiClient.GetAsync("APIQutationDetails/" + QutationID.ToString()).Result;
                List<MVCQutationViewModel> QutationModelDetailsList = responseQutationDetailsList.Content.ReadAsAsync<List<MVCQutationViewModel>>().Result;

                ViewBag.Contentdata = contectmodel;
                ViewBag.Companydata = companyModel;
                ViewBag.QutationDat = QutationModel;
                ViewBag.QutationDatailsList = QutationModelDetailsList;

                string PdfName = QutationID + "-" + companyModel.CompanyName + ".pdf";

                //  string CustomSwitches = string.Format("--header-html \"{0} \" " +

                string cutomswitches = "";
                cutomswitches = "--footer-center \"" + "Wilt u zo vriendelijk zijn om het verschuldigde bedrag binnen " + diffDate + " dagen over te maken naar IBAN: \n " + companyModel.IBANNumber + " ten name van IT Molen o.v.v.bovenstaande factuurnummer. \n (Op al onze diensten en producten zijn onze algemene voorwaarden van toepassing.Deze kunt u downloaden van onze website.)" + " \n Printed date: " +
                   DateTime.Now.Date.ToString("MM/dd/yyyy") + "  Page: [page]/[toPage]\"" +
                  " --footer-line --footer-font-size \"10\" --footer-spacing 6 --footer-font-name \"calibri light\"";


                return new Rotativa.PartialViewAsPdf("~/Views/QutationService/Viewpp.cshtml")
                {
                    PageSize = Rotativa.Options.Size.A4,
                    MinimumFontSize = 16,

                    FileName = PdfName,

                    PageHeight = 40,
                    CustomSwitches = cutomswitches,
                    PageMargins = new Rotativa.Options.Margins(5, 0, 10, 0)
                };
            }
            catch (Exception ex)
            {
                return null;

            }
        }








         public ActionResult QuataionServicePrint(int? QutationID)
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
                else
                {
                    return RedirectToAction("Index", "Login");
                }






                HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("ApiConatacts/" + Contectid.ToString()).Result;
                MVCContactModel contectmodel = response.Content.ReadAsAsync<MVCContactModel>().Result;

                HttpResponseMessage responseCompany = GlobalVeriables.WebApiClient.GetAsync("APIComapny/" + CompanyID.ToString()).Result;
                MVCCompanyInfoModel companyModel = responseCompany.Content.ReadAsAsync<MVCCompanyInfoModel>().Result;

                HttpResponseMessage responseQutation = GlobalVeriables.WebApiClient.GetAsync("APIQutation/" + QutationID.ToString()).Result;
                MVCQutationModel QutationModel = responseQutation.Content.ReadAsAsync<MVCQutationModel>().Result;

                GlobalVeriables.WebApiClient.DefaultRequestHeaders.Clear();

                DateTime qutationDueDate = Convert.ToDateTime(QutationModel.DueDate); //mm/dd/yyyy
                DateTime qutationDate = Convert.ToDateTime(QutationModel.QutationDate);//mm/dd/yyyy
                TimeSpan ts = qutationDueDate.Subtract(qutationDate);
                string diffDate = ts.Days.ToString();

                HttpResponseMessage responseQutationDetailsList = GlobalVeriables.WebApiClient.GetAsync("APIQutationDetails/" + QutationID.ToString()).Result;
                List<MVCQutationViewModel> QutationModelDetailsList = responseQutationDetailsList.Content.ReadAsAsync<List<MVCQutationViewModel>>().Result;

                ViewBag.Contentdata = contectmodel;
                ViewBag.Companydata = companyModel;
                ViewBag.QutationDat = QutationModel;
                ViewBag.QutationDatailsList = QutationModelDetailsList;

                string PdfName = QutationID + "-" + companyModel.CompanyName + ".pdf";

                //  string CustomSwitches = string.Format("--header-html \"{0} \" " +

                string cutomswitches = "";
                cutomswitches = "--footer-center \"" + "Wilt u zo vriendelijk zijn om het verschuldigde bedrag binnen " + diffDate + " dagen over te maken naar IBAN: \n " + companyModel.IBANNumber + " ten name van IT Molen o.v.v.bovenstaande factuurnummer. \n (Op al onze diensten en producten zijn onze algemene voorwaarden van toepassing.Deze kunt u downloaden van onze website.)" + " \n Printed date: " +
                   DateTime.Now.Date.ToString("MM/dd/yyyy") + "  Page: [page]/[toPage]\"" +
                  " --footer-line --footer-font-size \"10\" --footer-spacing 6 --footer-font-name \"calibri light\"";


                return new Rotativa.PartialViewAsPdf("~/Views/QutationService/QutationInvoiceServicePrint.cshtml")
                {
                    PageSize = Rotativa.Options.Size.A4,
                    MinimumFontSize = 16,

                    FileName = PdfName,

                    PageHeight = 40,
                    CustomSwitches = cutomswitches,
                    PageMargins = new Rotativa.Options.Margins(5, 0, 10, 0)
                };
            }
            catch (Exception ex)
            {
                return null;

            }
        }

        public ActionResult InvoicebyEmail(int? QutationId)
        {



            EmailModel email = new EmailModel();
            try
            {

                email.Attachment = PrintView((int)QutationId);

                if (Session["CompayID"] == null)
                {
                    return RedirectToAction("Index", "Login");
                }

                var CompanyName = Session["CompanyName"];

                if (CompanyName == null)
                {
                    CompanyName = "Nocompany";
                }

                var contact = Session["CompanyContact"];
                var companyEmail = Session["CompanyEmail"];
                if (contact == null)
                {
                    contact = "Company Contact";
                }
                if (companyEmail == null)
                {
                    companyEmail = "Company Email";
                }


                int id = 0;
                if (Session["ClientID"] != null)
                {
                    id = Convert.ToInt32(Session["ClientID"]);
                }


                HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("ApiConatacts/" + id.ToString()).Result;
                MVCContactModel mvcContactModel = response.Content.ReadAsAsync<MVCContactModel>().Result;





                email.EmailText = @"Geachte heer" + mvcContactModel.ContactName + "." +


                ".Hierbij ontvangt u onze offerte 10 zoals besproken,." +

                "." + "Graag horen we of u hiermee akkoord gaat." +

                "." + "De offerte vindt u als bijlage bij deze email." +


                "..Met vriendelijke groet." +

                mvcContactModel.ContactName + "." +

                CompanyName.ToString() + "." +

                contact.ToString() + "." +

                companyEmail.ToString();

                string strToProcess = email.EmailText;
                string result = strToProcess.Replace(".", " \r\n");

                email.EmailText = result;


                email.invoiceId = (int)QutationId;
                email.From = "infouurtjefactuur@gmail.com";
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return View(email);
        }


        [HttpPost]
        public ActionResult InvoicebyEmail(EmailModel email)
        {
            TempData["EmailMessge"] = "";
            EmailModel emailModel = new EmailModel();
            var fileName = email.Attachment;

            if (Session["CompayID"] != null)
            {
                CompanyID = Convert.ToInt32(Session["CompayID"]);
            }

            try
            {
                if (email.Attachment.Contains(".pdf"))
                {
                    email.Attachment = email.Attachment.Replace(".pdf", "");
                }

                if (email.ToEmail.Contains(','))
                {
                    var p = email.Attachment.Split('.');
                    var root = Server.MapPath("/PDF/");
                    var pdfname = String.Format("{0}.pdf", p);
                    var path = Path.Combine(root, pdfname);
                    email.Attachment = path;
                    string[] EmailArray = email.ToEmail.Split(',');
                    if (EmailArray.Count() > 0)
                    {
                        foreach (var item in EmailArray)
                        {
                            emailModel.From = email.From;
                            emailModel.ToEmail = item;
                            emailModel.Attachment = email.Attachment;
                            emailModel.EmailBody = email.EmailText;
                            bool result = EmailController.email(emailModel, _attackmentList);
                        }
                    }
                }
                else
                {
                    var root = Server.MapPath("/PDF/");
                    var pdfname = String.Format("{0}.pdf", email.Attachment);
                    var path = Path.Combine(root, pdfname);
                    email.Attachment = path;
                    emailModel.From = email.From;
                    emailModel.ToEmail = email.ToEmail;
                    emailModel.Attachment = email.Attachment;
                    emailModel.EmailBody = email.EmailText;
                    bool result = EmailController.email(emailModel, _attackmentList);
                    TempData["EmailMessge"] = "Email Send successfully";
                }

                HttpResponseMessage res = GlobalVeriables.WebApiClient.GetAsync("APIQutation/" + email.invoiceId.ToString()).Result;
                MVCQutationModel ob = res.Content.ReadAsAsync<MVCQutationModel>().Result;

                if (PerformTransaction(ob, CompanyID))
                {
                    TempData["EmailMessge"] = "Email Send Succssfully";
                }
                else
                {
                    TempData["EmailMessge"] = "Your transaction is not perform with success";

                }


                return RedirectToAction("ViewServiceQutation", new { quautionId = email.invoiceId });
            }
            catch (Exception ex)
            {
                TempData["EmailMessge"] = ex.Message.ToString();
                TempData["Error"] = ex.Message.ToString();
            }

            if (TempData["Path"] == null)
            {
                TempData["Path"] = fileName;
            }

            TempData["Message"] = "Email Send Succssfully";
            email.Attachment = fileName;

            return View(email);
        }



        public bool PerformTransaction(MVCQutationModel purchaseViewModel, int CompanyId)
        {
            bool TransactionResult = false;

            try
            {
                string base64Guid = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                AccountTransictionTable accountTransictiontable = new AccountTransictionTable();
                accountTransictiontable.TransictionDate = DateTime.Now;
                accountTransictiontable.FK_AccountID = 4002;
                accountTransictiontable.Cr = purchaseViewModel.TotalAmount;
                accountTransictiontable.Dr = 0.00;
                accountTransictiontable.TransictionNumber = base64Guid;
                accountTransictiontable.TransictionRefrenceId = purchaseViewModel.QutationID.ToString();
                accountTransictiontable.TransictionType = "Qutation";
                accountTransictiontable.CreationTime = DateTime.Now.TimeOfDay;
                accountTransictiontable.AddedBy = 1;
                accountTransictiontable.FK_CompanyId = CompanyId;
                accountTransictiontable.FKPaymentTerm = 1;
                accountTransictiontable.Description = "Total + Qutation ,Qutation created at Qutation" + purchaseViewModel.TotalAmount.ToString() + "On Qutation genrattion";
                HttpResponseMessage responses = GlobalVeriables.WebApiClient.PostAsJsonAsync("APIAccountTransiction", accountTransictiontable).Result;
                if (responses.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string base64Guid1 = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                    TransactionResult = true;
                    accountTransictiontable.TransictionDate = DateTime.Now;
                    accountTransictiontable.FK_AccountID = 4003;
                    accountTransictiontable.Dr = purchaseViewModel.SubTotal;
                    accountTransictiontable.Cr = 0.00;
                    accountTransictiontable.TransictionNumber = base64Guid1;
                    accountTransictiontable.TransictionRefrenceId = purchaseViewModel.QutationID.ToString();
                    accountTransictiontable.TransictionType = "Qutation";
                    accountTransictiontable.CreationTime = DateTime.Now.TimeOfDay;
                    accountTransictiontable.AddedBy = 1;
                    accountTransictiontable.FK_CompanyId = CompanyId;
                    accountTransictiontable.FKPaymentTerm = 1;
                    accountTransictiontable.Description = "Total + Invoice ,Invoice created at Invoice" + purchaseViewModel.SubTotal.ToString() + "On invoice genrattion";
                    HttpResponseMessage responses2 = GlobalVeriables.WebApiClient.PostAsJsonAsync("APIAccountTransiction", accountTransictiontable).Result;

                    if (responses2.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        string base64Guid2 = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                        TransactionResult = true;
                        accountTransictiontable.TransictionDate = DateTime.Now;
                        accountTransictiontable.FK_AccountID = 3005;
                        accountTransictiontable.Dr = purchaseViewModel.SubTotal;
                        accountTransictiontable.Cr = 0.00;
                        accountTransictiontable.TransictionNumber = base64Guid2;
                        accountTransictiontable.TransictionRefrenceId = purchaseViewModel.QutationID.ToString();
                        accountTransictiontable.TransictionType = "Qutation";
                        accountTransictiontable.CreationTime = DateTime.Now.TimeOfDay;
                        accountTransictiontable.AddedBy = 1;
                        accountTransictiontable.FK_CompanyId = CompanyId;
                        accountTransictiontable.FKPaymentTerm = 1;
                        double TotalVat = Convert.ToDouble(purchaseViewModel.TotalVat21 + purchaseViewModel.TotalVat6);
                        accountTransictiontable.Dr = TotalVat;
                        accountTransictiontable.Description = "Total + Vat ,Invoice created at Invoice" + TotalVat + "On invoice genrattion";
                        HttpResponseMessage responses3 = GlobalVeriables.WebApiClient.PostAsJsonAsync("APIAccountTransiction", accountTransictiontable).Result;
                        if (responses3.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            return TransactionResult = true;
                        }

                    }
                }
            }


            catch (Exception)
            {

                return TransactionResult = false;
            }
            return TransactionResult;
        }


        [HttpPost]
        public ActionResult savePrintAndSentItToYouronsave(MVCQutationViewModel MVCQutationViewModel)
        {
            var QutationOrderId = "";
            int intQutationorderId = 0;
            MVCQutationModel mvcQutationModel = new MVCQutationModel();
            try
            {
                if (Session["ClientId"] != null && Session["CompayID"] != null)
                {
                    Contectid = Convert.ToInt32(Session["ClientId"]);
                    CompanyID = Convert.ToInt32(Session["CompayID"]);
                }
                else
                {
                    return RedirectToAction("Index", "Login");
                }

                mvcQutationModel.Qutation_ID = MVCQutationViewModel.Qutation_ID;
                mvcQutationModel.CompanyId = CompanyID;
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
                mvcQutationModel.ContactId = Contectid;
                mvcQutationModel.Status = "open";
                mvcQutationModel.Type = StatusEnum.Services.ToString();
                if (MVCQutationViewModel.QutationID == 0 || MVCQutationViewModel.QutationID == null)
                {

                    HttpResponseMessage response = GlobalVeriables.WebApiClient.PostAsJsonAsync("APIQutation", mvcQutationModel).Result;
                    IEnumerable<string> headerValues;
                    var userId = string.Empty;
                    if (response.Headers.TryGetValues("idd", out headerValues))
                    {
                        QutationOrderId = headerValues.FirstOrDefault();
                    }
                    intQutationorderId = Convert.ToInt32(QutationOrderId);


                    if (response.StatusCode == System.Net.HttpStatusCode.Created)
                    {
                        foreach (var item in MVCQutationViewModel.QutationDetailslist)
                        {
                            QutationDetailsTable QtDetails = new QutationDetailsTable();
                            QtDetails.ItemId = Convert.ToInt32(item.ItemId);
                            QtDetails.QutationID = item.QutationID;
                            QtDetails.Description = item.Description;
                            QtDetails.QutationDetailId = item.QutationDetailId;
                            QtDetails.Quantity = item.Quantity;
                            QtDetails.Rate = Convert.ToDouble(item.Rate);
                            QtDetails.Total = Convert.ToDouble(item.Total);
                            QtDetails.Vat = Convert.ToDouble(item.Vat);
                            if (QtDetails.QutationDetailId == 0 || QtDetails.QutationDetailId == null)
                            {
                                QtDetails.QutationID = intQutationorderId;

                                HttpResponseMessage responsses = GlobalVeriables.WebApiClient.PostAsJsonAsync("APIQutationDetails", QtDetails).Result;
                            }
                            else
                            {
                                HttpResponseMessage responsses = GlobalVeriables.WebApiClient.PutAsJsonAsync("APIQutationDetails/" + QtDetails.QutationDetailId, QtDetails).Result;
                            }
                        }
                    }
                }

                else
                {
                    if (MVCQutationViewModel.QutationID != 0)
                    {
                        MVCQutationViewModel.QutationID = MVCQutationViewModel.QutationID;
                        intQutationorderId = Convert.ToInt32(MVCQutationViewModel.QutationID);
                        HttpResponseMessage response2 = GlobalVeriables.WebApiClient.PutAsJsonAsync("APIQutation/" + MVCQutationViewModel.QutationID, MVCQutationViewModel).Result;
                        intQutationorderId = (int)MVCQutationViewModel.QutationID;
                        if (response2.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            foreach (var item in MVCQutationViewModel.QutationDetailslist)
                            {
                                QutationDetailsTable qutationDetail = new QutationDetailsTable();
                                qutationDetail.QutationDetailId = item.QutationDetailId;
                                qutationDetail.ItemId = item.ItemId;
                                qutationDetail.Description = item.Description;
                                qutationDetail.Quantity = item.Quantity;
                                qutationDetail.Rate = item.Rate;
                                qutationDetail.Total = item.Total;
                                qutationDetail.Vat = item.Vat;
                                qutationDetail.QutationID = MVCQutationViewModel.QutationID;
                                if (qutationDetail.QutationDetailId == 0 || qutationDetail.QutationDetailId == null)
                                {
                                    HttpResponseMessage responsses = GlobalVeriables.WebApiClient.PostAsJsonAsync("APIQutationDetails", qutationDetail).Result;
                                }
                                else
                                {
                                    HttpResponseMessage responsses = GlobalVeriables.WebApiClient.PutAsJsonAsync("APIQutationDetails/" + qutationDetail.QutationDetailId, qutationDetail).Result;
                                }
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {

                return new JsonResult { Data = new { Status = "Fail", Message = ex.Message.ToString() } };
            }

            //calling printing option
            string path1 = PrintView(intQutationorderId);
            var root = Server.MapPath("/PDF/");
            var pdfname = String.Format("{0}", path1);
            var path = Path.Combine(root, pdfname);
            path = Path.GetFullPath(path);
            //DownloadFile(path);
            return new JsonResult { Data = new { Status = "Success", path = path, QutationID = intQutationorderId } };
        }
    }
}


