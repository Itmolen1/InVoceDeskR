using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InvoiceDiskLast.Models;
using System.Net.Http;
using System.IO;
using Rotativa.Options;
using System.Text;
using System.Diagnostics;

namespace InvoiceDiskLast.Controllers
{

     [SessionExpireAttribute]
    public class MVCQutationController : Controller
    {
        // GET: MVCQutation
        public ActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public JsonResult IndexQutation()
        {
            List<MVCQutationModel> quationList = new List<MVCQutationModel>();
            try
            {
                #region
                int recordsTotal = 0;
                var draw = Request.Form.GetValues("draw").FirstOrDefault();
                var start = Request.Form.GetValues("start").FirstOrDefault();
                var length = Request.Form.GetValues("length").FirstOrDefault();
                var sortColumn = Request.Form.GetValues("columns[" +
                Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                string search = Request.Form.GetValues("search[value]")[0];
                int skip = start != null ? Convert.ToInt32(start) : 0;

                int companyId = Convert.ToInt32(Session["CompayID"]);

                GlobalVeriables.WebApiClient.DefaultRequestHeaders.Clear();
                GlobalVeriables.WebApiClient.DefaultRequestHeaders.Add("CompayID", companyId.ToString());

                HttpResponseMessage respose = GlobalVeriables.WebApiClient.GetAsync("APIQutation").Result;
                quationList = respose.Content.ReadAsAsync<List<MVCQutationModel>>().Result;
               
                List<MVCQutationModel> quationList1 = new List<MVCQutationModel>();
                if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
                {
                 if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
                {

                quationList = quationList.Where(p => p.QutationID.ToString().Contains(search)
               || p.Qutation_ID != null && p.Qutation_ID.ToLower().Contains(search.ToLower())
               || p.QutationDate != null && p.QutationDate.ToString().ToLower().Contains(search.ToLower())
               || p.Status != null && p.Status.ToString().ToLower().Contains(search.ToLower())              
               || p.RefNumber != null && p.RefNumber.ToString().ToLower().Contains(search.ToLower())            

              ).ToList();

                }
                  

                }

                recordsTotal = recordsTotal = quationList.Count();
                var data = quationList.Skip(skip).Take(pageSize).ToList();
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data }, JsonRequestBehavior.AllowGet);
                #endregion
            }
            catch (Exception ex)
            {
                return Json(new { draw = 0, recordsFiltered = 0, recordsTotal = 0, data = 0 }, JsonRequestBehavior.AllowGet);

                //return  Json(ex.ToString(), JsonRequestBehavior.AllowGet);

            }
        }

        int Contectid = 0;
        int CompanyID = 0;
        //MVCQutation/AddOrEdit

        [HttpGet]
        public ActionResult AddOrEdit(int id = 0)
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

                        HttpResponseMessage responsep = GlobalVeriables.WebApiClient.GetAsync("APIProduct").Result;
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
                        quutionviewModel.ConatctId = (int)ob.ContactId;
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
        public ActionResult AddOrEdit(MVCQutationViewModel MVCQutationViewModel)
        {
            MVCQutationModel mvcQutationModel = new MVCQutationModel();
            try
            {
                int companyId = 0;

                if (MVCQutationViewModel.QutationID != null)
                {
                    mvcQutationModel.Qutation_ID = MVCQutationViewModel.Qutation_ID;

                    if (Session["CompayID"] != null)
                    {
                        companyId = Convert.ToInt32(Session["CompayID"]);
                    }


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


                    if (mvcQutationModel.TotalVat6 != null)
                    {
                        double vat61 = Math.Round((double)mvcQutationModel.TotalVat6, 2, MidpointRounding.AwayFromZero);
                        mvcQutationModel.TotalVat6 = vat61;
                    }

                    mvcQutationModel.TotalVat21 = MVCQutationViewModel.TotalVat21;

                    if (mvcQutationModel.TotalVat21 != null)
                    {
                      double  vat21= Math.Round((double)mvcQutationModel.TotalVat21, 2, MidpointRounding.AwayFromZero);
                       mvcQutationModel.TotalVat21 =vat21;
                    }

                    mvcQutationModel.Qutation_ID = MVCQutationViewModel.Qutation_ID;
                    mvcQutationModel.Status = MVCQutationViewModel.Status;
                    HttpResponseMessage response = GlobalVeriables.WebApiClient.PutAsJsonAsync("APIQutation/" + mvcQutationModel.QutationID, mvcQutationModel).Result;
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        foreach (QutationDetailsTable QDTList in MVCQutationViewModel.QutationDetailslist)
                        {
                            QutationDetailsTable QtDetails = new QutationDetailsTable();
                            QtDetails.ItemId = Convert.ToInt32(QDTList.ItemId);
                            QtDetails.QutationID = QDTList.QutationID;
                            QtDetails.Description = QDTList.Description;
                            QtDetails.QutationDetailId = QDTList.QutationDetailId;
                            QtDetails.Quantity = QDTList.Quantity;
                            QtDetails.Rate = Convert.ToDouble(QDTList.Rate);
                            QtDetails.Total = Convert.ToDouble(QDTList.Total);
                            QtDetails.Vat = Convert.ToDouble(QDTList.Vat);
                              if (QtDetails.QutationDetailId == 0 || QtDetails.QutationDetailId == null)
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
            }
            catch (Exception ex)
            {

                return new JsonResult { Data = new { Status = "Fail", Message = ex.Message.ToString() } };
            }


            return new JsonResult { Data = new { Status = "Success", QutationId = MVCQutationViewModel.QutationID } };

        }



        public static Boolean IsFileLocked(FileInfo file)
        {
            FileStream stream = null;

            try
            {
                //Don't change FileAccess to ReadWrite, 
                //because if a file is in readOnly, it fails.
                stream = file.Open
                (
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.None
                );
            }
            catch (IOException ex)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
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
                email.From = "samarbudhni@gmail.com";
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
                            bool result = EmailController.email(emailModel);
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
                    bool result = EmailController.email(emailModel);

                    TempData["EmailMessge"] = "Email Send successfully";

                }
                //return RedirectToAction("ViewQuation", "MVCQutation", new { @id = id });
                return RedirectToAction("ViewQuation", new { quautionId = email.invoiceId });


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


            return View(email);
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

                HttpResponseMessage responseQutationDetailsList = GlobalVeriables.WebApiClient.GetAsync("APIQutationDetails/" + QutationID.ToString()).Result;
                List<MVCQutationViewModel> QutationModelDetailsList = responseQutationDetailsList.Content.ReadAsAsync<List<MVCQutationViewModel>>().Result;

                ViewBag.Contentdata = contectmodel;
                ViewBag.Companydata = companyModel;
                ViewBag.QutationDat = QutationModel;
                ViewBag.QutationDatailsList = QutationModelDetailsList;

                string PdfName = QutationID + "-" + companyModel.CompanyName + ".pdf";

                return new Rotativa.PartialViewAsPdf("~/Views/MVCQutation/Viewpp.cshtml") { FileName = PdfName };
            }
            catch (Exception)
            {

                throw;
            }
        }

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

                var pdfResult = new Rotativa.PartialViewAsPdf("~/Views/MVCQutation/Viewpp.cshtml")
                {
                    SaveOnServerPath = path, // Save your place
                    PageWidth = 200,
                    PageHeight = 350,
                };
                // This section allows you to save without downloading 
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
                    //    HttpResponseMessage deleteQuaution = GlobalVeriables.WebApiClient.DeleteAsync("APIQutationDetail/" + QutationId).Result;



                    //if (deleteQuaution.StatusCode == System.Net.HttpStatusCode.OK)
                    //{
                    //  return Json("Success", JsonRequestBehavior.AllowGet);
                    // }

                }



            }
            catch (Exception)
            {

                return new JsonResult { Data = new { Status = "Fail" } };
            }

            return new JsonResult { Data = new { Status = "Success" } };
        }

        [HttpPost]
        public ActionResult Save(MVCQutationViewModel mvcQutationViewModel)
        {
            bool Status = false;
            var Qutationid = "";
            #region             
            try
            {
                int companyId = 0;

                if (Session["CompayID"] != null)
                {
                    companyId = Convert.ToInt32(Session["CompayID"]);
                }


                MVCQutationModel mvcQutationModel = new MVCQutationModel();
                mvcQutationModel.Qutation_ID = mvcQutationViewModel.Qutation_ID;
                mvcQutationModel.CompanyId = companyId;
                mvcQutationModel.ContactId = mvcQutationViewModel.ConatctId;
                mvcQutationModel.UserId = 1;
                mvcQutationModel.RefNumber = mvcQutationViewModel.RefNumber;
                mvcQutationModel.QutationDate = Convert.ToDateTime(mvcQutationViewModel.QutationDate);
                mvcQutationModel.DueDate = Convert.ToDateTime(mvcQutationViewModel.DueDate);
                mvcQutationModel.SubTotal = Convert.ToDouble(mvcQutationViewModel.SubTotal);
                mvcQutationModel.DiscountAmount = Convert.ToDouble(mvcQutationViewModel.DiscountAmount);
                mvcQutationModel.TotalAmount = Convert.ToDouble(mvcQutationViewModel.Total);
                mvcQutationModel.CustomerNote = mvcQutationViewModel.CustomerNote;
                mvcQutationModel.TotalVat21 = mvcQutationViewModel.TotalVat21;
                mvcQutationModel.TotalVat6 = mvcQutationViewModel.TotalVat6;



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


                mvcQutationModel.Status = "Open";



                var response = GlobalVeriables.WebApiClient.PostAsJsonAsync("APIQutation", mvcQutationModel).Result;

                IEnumerable<string> headerValues;
                var userId = string.Empty;


                if (response.Headers.TryGetValues("idd", out headerValues))
                {
                    Qutationid = headerValues.FirstOrDefault();
                }

                List<QutationDetailsTable> QutationDetailsList = mvcQutationViewModel.QutationDetailslist;


                foreach (QutationDetailsTable QDTList in QutationDetailsList)
                {
                    QutationDetailsTable QtDetails = new QutationDetailsTable();
                    QtDetails.ItemId = Convert.ToInt32(QDTList.ItemId);
                    QtDetails.QutationID = Convert.ToInt32(Qutationid);
                    QtDetails.Description = QDTList.Description;
                    QtDetails.Quantity = QDTList.Quantity;
                    QtDetails.Rate = Convert.ToDouble(QDTList.Rate);
                    QtDetails.Total = Convert.ToDouble(QDTList.Total);
                    QtDetails.Vat = Convert.ToDouble(QDTList.Vat);
                    HttpResponseMessage responsses = GlobalVeriables.WebApiClient.PostAsJsonAsync("APIQutationDetails", QtDetails).Result;
                }

                List<MVCVatDetailsModel> mvcVatDetailsList = new List<MVCVatDetailsModel>();


                Status = true;

                return new JsonResult { Data = new { Status = "Success", QutationId = Qutationid } };

            }
            catch (Exception ex)
            {
                return new JsonResult { Data = new { Status = "Fail", QutationId = Qutationid, message = ex.Message.ToString() } };
            }


            #endregion

        }


        [HttpPost]
        public ActionResult SaveEmail(MVCQutationViewModel MVCQutationViewModel)
        {
            MVCQutationModel mvcQutationModel = new MVCQutationModel();
            try
            {
                int compnayId = 0;

                if (Session["CompayID"] != null)
                {
                    compnayId = Convert.ToInt32(Session["CompayID"]);
                }

                if (MVCQutationViewModel.QutationID != null)
                {
                    mvcQutationModel.Qutation_ID = MVCQutationViewModel.Qutation_ID;
                    mvcQutationModel.CompanyId = compnayId;
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
                    mvcQutationModel.Status = "Open";

                    HttpResponseMessage response = GlobalVeriables.WebApiClient.PutAsJsonAsync("APIQutation/" + mvcQutationModel.QutationID, mvcQutationModel).Result;
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        foreach (QutationDetailsTable QDTList in MVCQutationViewModel.QutationDetailslist)
                        {
                            QutationDetailsTable QtDetails = new QutationDetailsTable();
                            QtDetails.ItemId = Convert.ToInt32(QDTList.ItemId);
                            QtDetails.QutationID = QDTList.QutationID;
                            QtDetails.Description = QDTList.Description;
                            QtDetails.QutationDetailId = QDTList.QutationDetailId;
                            QtDetails.Quantity = QDTList.Quantity;
                            QtDetails.Rate = Convert.ToDouble(QDTList.Rate);
                            QtDetails.Total = Convert.ToDouble(QDTList.Total);
                            QtDetails.Vat = Convert.ToDouble(QDTList.Vat);
                            if (QtDetails.QutationDetailId == 0 || QtDetails.QutationDetailId == null)
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

                        //return Json("Fail", JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {

                return new JsonResult { Data = new { Status = "Fail", Message = ex.Message.ToString() } };
            }
            return new JsonResult { Data = new { Status = "Success", QutationId = MVCQutationViewModel.QutationID } };
        }

        [HttpPost]
        public ActionResult SaveEmailAdd(MVCQutationViewModel MVCQutationViewModel)
        {
            var Qutationid = "";
            int Qid = 0;
            MVCQutationModel mvcQutationModel = new MVCQutationModel();
            try
            {
                int companyId = 0;

                if (Session["CompayID"] != null)
                {
                    companyId = Convert.ToInt32(Session["CompayID"]);
                }


                if (MVCQutationViewModel.QutationID == null)
                {
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
                    mvcQutationModel.Status = "Open";
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
                            QtDetails.Vat = Convert.ToDouble(QDTList.Vat);
                            if (QtDetails.QutationDetailId == 0 || QtDetails.QutationDetailId == null)
                            {
                                HttpResponseMessage responsses = GlobalVeriables.WebApiClient.PostAsJsonAsync("APIQutationDetails", QtDetails).Result;
                            }
                            else
                            {
                                HttpResponseMessage responsses = GlobalVeriables.WebApiClient.PutAsJsonAsync("APIQutationDetail/s" + QtDetails.QutationDetailId, QtDetails).Result;
                            }
                        }

                        string path = PrintView(Qid);

                        if (path != null)
                        {
                            TempData["path"] = path;
                        }

                        TempData["quaution"] = Qid;

                        return new JsonResult { Data = new { Status = "Success", path = path, QutationId = Qid } };

                    }
                    else
                    {
                        return Json("Fail", JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                return new JsonResult { Data = new { Status = "Fail", Message = ex.Message.ToString() } };
            }
            return new JsonResult { Data = new { Status = "Success", QutationId = Qid } };
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
                string QuationId = arrya[0];
                string PdfName = QuationId + "-" + companyModel.CompanyName + ".pdf";
                return PdfName;
            }
            catch (Exception)
            {

                throw;
            }

        }

        [HttpPost]
        [DeleteFileClass]
        public FileResult DownloadFile(string FilePath)
        {

            string filepath = "";
            string FileName = SetPdfName(FilePath);
            try
            {
                filepath = System.IO.Path.Combine(Server.MapPath("/PDF/"), FilePath);
                HttpContext.Items["FilePath"] = filepath;

            }
            catch (Exception)
            {
            }

            return File(filepath, MimeMapping.GetMimeMapping(filepath), FileName);
        }

        [HttpPost]
        public ActionResult savePrintAndSentItToYourSenf(MVCQutationViewModel MVCQutationViewModel)
        {
            MVCQutationModel mvcQutationModel = new MVCQutationModel();
            try
            {
                int companyId = 0;
                if (Session["CompayID"] != null)
                {
                    companyId = Convert.ToInt32(Session["CompayID"]);
                }

                if (MVCQutationViewModel.QutationID != null)
                {
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
                    mvcQutationModel.Status = "Open";
                    HttpResponseMessage response = GlobalVeriables.WebApiClient.PutAsJsonAsync("APIQutation/" + mvcQutationModel.QutationID, mvcQutationModel).Result;
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        foreach (QutationDetailsTable QDTList in MVCQutationViewModel.QutationDetailslist)
                        {
                            QutationDetailsTable QtDetails = new QutationDetailsTable();
                            QtDetails.ItemId = Convert.ToInt32(QDTList.ItemId);
                            QtDetails.QutationID = QDTList.QutationID;
                            QtDetails.Description = QDTList.Description;
                            QtDetails.QutationDetailId = QDTList.QutationDetailId;
                            QtDetails.Quantity = QDTList.Quantity;
                            QtDetails.Rate = Convert.ToDouble(QDTList.Rate);
                            QtDetails.Total = Convert.ToDouble(QDTList.Total);
                            QtDetails.Vat = Convert.ToDouble(QDTList.Vat);

                            if (QtDetails.QutationDetailId == 0 || QtDetails.QutationDetailId==null)
                            {
                                HttpResponseMessage responsses = GlobalVeriables.WebApiClient.PostAsJsonAsync("APIQutationDetails", QtDetails).Result;
                            }
                            else
                            {
                                HttpResponseMessage responsses = GlobalVeriables.WebApiClient.PutAsJsonAsync("APIQutationDetails/" + QtDetails.QutationDetailId, QtDetails).Result;
                            }
                        }

                        //calling printing option
                        string path1 = PrintView((int)MVCQutationViewModel.QutationID);
                        var root = Server.MapPath("/PDF/");
                        var pdfname = String.Format("{0}", path1);
                        var path = Path.Combine(root, pdfname);
                        path = Path.GetFullPath(path);
                        //DownloadFile(path);

                        return new JsonResult { Data = new { Status = "Success", path = pdfname, QutationId = MVCQutationViewModel.QutationID } };
                    }
                    else
                    {
                        return new JsonResult { Data = new { Status = "Fail", QutationId = MVCQutationViewModel.QutationID } };
                    }
                }
            }

            catch (Exception ex)
            {

                return new JsonResult { Data = new { Status = "Fail", Message = ex.Message.ToString() } };
            }
            return new JsonResult { Data = new { Status = "Success", QutationId = MVCQutationViewModel.QutationID } };
        }


        [HttpPost]
        public ActionResult savePrintAndSentItToYouronsave(MVCQutationViewModel MVCQutationViewModel)
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


                if (MVCQutationViewModel.QutationID == null)
                {
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
                    mvcQutationModel.Status = "Open";
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
                            QtDetails.Vat = Convert.ToDouble(QDTList.Vat);

                            if (QtDetails.QutationDetailId == 0 || QtDetails.QutationDetailId == null)
                            {
                                HttpResponseMessage responsses = GlobalVeriables.WebApiClient.PostAsJsonAsync("APIQutationDetails", QtDetails).Result;
                            }
                            else
                            {
                                HttpResponseMessage responsses = GlobalVeriables.WebApiClient.PutAsJsonAsync("APIQutationDetails/" + QtDetails.QutationDetailId, QtDetails).Result;
                            }
                        }

                        //calling printing option
                        string path1 = PrintView(Qid);
                        var root = Server.MapPath("/PDF/");
                        var pdfname = String.Format("{0}", path1);
                        var path = Path.Combine(root, pdfname);
                        path = Path.GetFullPath(path);
                        DownloadFile(path);


                        return new JsonResult { Data = new { Status = "Success", path = pdfname, QutationId = Qid } };

                    }
                    else
                    {
                        return Json("Fail", JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {

                return new JsonResult { Data = new { Status = "Fail", Message = ex.Message.ToString() } };
            }
            return new JsonResult { Data = new { Status = "Success", QutationId = Qid } };
        }




        [HttpPost]
        public JsonResult ProductPricebyId(int ProductId)
        {
            

            HttpResponseMessage responsep = GlobalVeriables.WebApiClient.GetAsync("APIProduct/" + ProductId.ToString()).Result;
            MVCProductModel productModel = responsep.Content.ReadAsAsync<MVCProductModel>().Result;
            float price = (float)productModel.SalePrice;
            return Json(price, JsonRequestBehavior.AllowGet);
        }




        public class VatModel
        {
            public int Vat1 { get; set; }
            public string Name { get; set; }

        }
    }
}