using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InvoiceDiskLast.Models;
using System.Net.Http;
using System.IO;
using Rotativa.Options;



namespace InvoiceDiskLast.Controllers
{
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

                HttpResponseMessage respose = GlobalVeriables.WebApiClient.GetAsync("APIQutation").Result;
                quationList = respose.Content.ReadAsAsync<List<MVCQutationModel>>().Result;
                List<MVCQutationModel> quationList1 = new List<MVCQutationModel>();
                if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
                {
                    // Apply search  on multiple field  
                    quationList = quationList.Where(p => p.QutationDate.ToString().Contains(search) ||
                    p.Qutation_ID.ToString().Contains(search) ||
                    p.RefNumber.ToString().Contains(search) ||
                    p.QutationDate.ToString().Contains(search) ||
                    p.QutationID.ToString().Contains(search)).ToList();

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
                if (Session["ClientID"] != null && Session["CompayID"] != null)
                {
                    Contectid = Convert.ToInt32(Session["ClientID"]);
                    CompanyID =  Convert.ToInt32(Session["CompayID"]);
                }
                else
                {
                    return RedirectToAction("Index","Login");
                }

               
                HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("ApiConatacts/" + Contectid.ToString()).Result;
                MVCContactModel contectmodel = response.Content.ReadAsAsync<MVCContactModel>().Result;

               

                HttpResponseMessage responseCompany = GlobalVeriables.WebApiClient.GetAsync("APIComapny/" + CompanyID.ToString()).Result;
                MVCCompanyInfoModel companyModel = responseCompany.Content.ReadAsAsync<MVCCompanyInfoModel>().Result;

                ViewBag.Contentdata = contectmodel;
                ViewBag.Companydata = companyModel;

                if (id == 0)
                {
                    return View(new MVCQutationViewModel());
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

                    HttpResponseMessage responseQutation2 = GlobalVeriables.WebApiClient.GetAsync("APIQutation/" + id.ToString()).Result;
                    MVCQutationModel ob = responseQutation2.Content.ReadAsAsync<MVCQutationModel>().Result;

                    quutionviewModel.QutationID = ob.QutationID;
                    quutionviewModel.QutationDate = ob.QutationDate;
                    quutionviewModel.RefNumber = ob.RefNumber;
                    quutionviewModel.DueDate = ob.DueDate;
                    quutionviewModel.CustomerNote = ob.CustomerNote;
                    quutionviewModel.SubTotal = ob.SubTotal;
                    quutionviewModel.TotalAmount = ob.TotalAmount;
                    quutionviewModel.TotalVat21 = (ob.TotalVat21 != null ? (float)(ob.TotalVat21) : (float)0.00);
                    quutionviewModel.TotalVat6 = (ob.TotalVat6 != null ? (float)(ob.TotalVat6) : (float)0.00);



                    HttpResponseMessage responseQutationDetailsList = GlobalVeriables.WebApiClient.GetAsync("APIQutationDetail/" + id.ToString()).Result;
                    List<MVCQutationDetailsModel> QutationModelDetailsList = responseQutationDetailsList.Content.ReadAsAsync<List<MVCQutationDetailsModel>>().Result;
                    ViewBag.Contentdata = contectmodel;
                    ViewBag.Companydata = companyModel;
                    ViewBag.QutationDatailsList = QutationModelDetailsList;




                    return View(quutionviewModel);
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
                if (MVCQutationViewModel.QutationID != null)
                {
                    mvcQutationModel.Qutation_ID = MVCQutationViewModel.Qutation_ID;
                    mvcQutationModel.CompanyId = 2;
                    mvcQutationModel.UserId = 1;
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
                            if (QtDetails.QutationDetailId == 0)
                            {
                                HttpResponseMessage responsses = GlobalVeriables.WebApiClient.PostAsJsonAsync("APIQutationDetail", QtDetails).Result;
                            }
                            else
                            {
                                HttpResponseMessage responsses = GlobalVeriables.WebApiClient.PutAsJsonAsync("APIQutationDetail/" + QtDetails.QutationDetailId, QtDetails).Result;
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




        public ActionResult InvoicebyEmail(int? QutationId)
        {
            EmailModel email = new EmailModel();
            //email.Attachment

            email.EmailText = @"Geachte heer samar gul,

            Hierbij ontvangt u onze offerte 10 zoals besproken.
            Graag horen we of u hiermee akkoord gaat.

            De offerte vindt u als bijlage bij deze email.

            Met vriendelijke groet,

         samar gul
         It Molen

         345465
         fuygk @yahoo.com";


            email.invoiceId = (int)QutationId;
            email.From = "samarbudhni@gmail.com";
            return View(email);
        }

        [HttpPost]
        public ActionResult InvoicebyEmail(EmailModel email)
        {
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

                    var root = Server.MapPath("~/PDF/");
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
                    var root = Server.MapPath("~/PDF/");
                    var pdfname = String.Format("{0}.pdf", email.Attachment);
                    var path = Path.Combine(root, pdfname);
                    email.Attachment = path;
                    emailModel.From = email.From;
                    emailModel.ToEmail = email.ToEmail;
                    emailModel.Attachment = email.Attachment;
                    emailModel.EmailBody = email.EmailText;
                    bool result = EmailController.email(emailModel);
                }

            }
            catch (Exception ex)
            {

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

         
            HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("ApiConatacts/" + Contectid.ToString()).Result;
            MVCContactModel contectmodel = response.Content.ReadAsAsync<MVCContactModel>().Result;     

            HttpResponseMessage responseCompany = GlobalVeriables.WebApiClient.GetAsync("APIComapny/" + CompanyID.ToString()).Result;
            MVCCompanyInfoModel companyModel = responseCompany.Content.ReadAsAsync<MVCCompanyInfoModel>().Result;

            HttpResponseMessage responseQutation = GlobalVeriables.WebApiClient.GetAsync("APIQutation/" + QutationID.ToString()).Result;
            MVCQutationModel QutationModel = responseQutation.Content.ReadAsAsync<MVCQutationModel>().Result;

            HttpResponseMessage responseQutationDetailsList = GlobalVeriables.WebApiClient.GetAsync("APIQutationDetail/" + QutationID.ToString()).Result;
            List<MVCQutationViewModel> QutationModelDetailsList = responseQutationDetailsList.Content.ReadAsAsync<List<MVCQutationViewModel>>().Result;

            ViewBag.Contentdata = contectmodel;
            ViewBag.Companydata = companyModel;
            ViewBag.QutationDat = QutationModel;
            ViewBag.QutationDatailsList = QutationModelDetailsList;

            string PdfName = QutationID + "-" + companyModel.CompanyName + ".pdf";

            return new Rotativa.PartialViewAsPdf("~/Views/MVCQutation/Viewpp.cshtml") { FileName = PdfName };


        }


        public ActionResult ViewQuation(int? quautionId)
        {
            try
            {
               
                HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("ApiConatacts/" + Contectid.ToString()).Result;
                MVCContactModel contectmodel = response.Content.ReadAsAsync<MVCContactModel>().Result;
             
                HttpResponseMessage responseCompany = GlobalVeriables.WebApiClient.GetAsync("APIComapny/" + CompanyID.ToString()).Result;
                MVCCompanyInfoModel companyModel = responseCompany.Content.ReadAsAsync<MVCCompanyInfoModel>().Result;

                HttpResponseMessage responseQutation = GlobalVeriables.WebApiClient.GetAsync("APIQutation/" + quautionId.ToString()).Result;
                MVCQutationModel QutationModel = responseQutation.Content.ReadAsAsync<MVCQutationModel>().Result;

                GlobalVeriables.WebApiClient.DefaultRequestHeaders.Clear();


                HttpResponseMessage responseQutationDetailsList = GlobalVeriables.WebApiClient.GetAsync("APIQutationDetail/" + quautionId.ToString()).Result;
                List<MVCQutationViewModel> QutationModelDetailsList = responseQutationDetailsList.Content.ReadAsAsync<List<MVCQutationViewModel>>().Result;

                ViewBag.Contentdata = contectmodel;
                ViewBag.Companydata = companyModel;
                ViewBag.QutationDat = QutationModel;
                ViewBag.QutationDatailsList = QutationModelDetailsList;
            }
            catch (Exception)
            {
            }

            return View();
        }



        public string PrintView(int quttationId)
        {

            Random rnd = new Random();
            int month = rnd.Next(1, 13); // creates a number between 1 and 12
            int dice = rnd.Next(1, 7);   // creates a number between 1 and 6
            int card = rnd.Next(52);     // creates a number between 0 and 51


         
            HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("ApiConatacts/" + Contectid.ToString()).Result;
            MVCContactModel contectmodel = response.Content.ReadAsAsync<MVCContactModel>().Result;

         

            HttpResponseMessage responseCompany = GlobalVeriables.WebApiClient.GetAsync("APIComapny/" + CompanyID.ToString()).Result;
            MVCCompanyInfoModel companyModel = responseCompany.Content.ReadAsAsync<MVCCompanyInfoModel>().Result;

            HttpResponseMessage responseQutation = GlobalVeriables.WebApiClient.GetAsync("APIQutation/" + quttationId.ToString()).Result;
            MVCQutationModel QutationModel = responseQutation.Content.ReadAsAsync<MVCQutationModel>().Result;

            HttpResponseMessage responseQutationDetailsList = GlobalVeriables.WebApiClient.GetAsync("APIQutationDetail/" + quttationId.ToString()).Result;
            List<MVCQutationViewModel> QutationModelDetailsList = responseQutationDetailsList.Content.ReadAsAsync<List<MVCQutationViewModel>>().Result;

            ViewBag.Contentdata = contectmodel;
            ViewBag.Companydata = companyModel;
            ViewBag.QutationDat = QutationModel;
            ViewBag.QutationDatailsList = QutationModelDetailsList;
            string companyName = quttationId + "-" + companyModel.CompanyName;
            var root = Server.MapPath("~/PDF/");
            var pdfname = String.Format("{0}.pdf", companyName);
            var path = Path.Combine(root, pdfname);
            path = Path.GetFullPath(path);

            string subPath = "~/PDF"; // your code goes here
            bool exists = System.IO.Directory.Exists(Server.MapPath(subPath));
            if (!exists)
            {
                System.IO.Directory.CreateDirectory(Server.MapPath(subPath));
            }
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }


            var pdfResult = new Rotativa.PartialViewAsPdf("~/Views/MVCQutation/Viewpp.cshtml")
            {
                SaveOnServerPath = path, // Save your place
                PageWidth = 200,
                PageHeight = 350,
            };

            // This section allows you to save without downloading 

            pdfResult.BuildPdf(this.ControllerContext);
            //EmailController.email(path);

            return pdfname;
        }





        [HttpPost]
        public ActionResult DeleteQuatation(int QutationId, int QutationDetailID, int vat, int total)
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
                HttpResponseMessage responseQutationDetailsList = GlobalVeriables.WebApiClient.GetAsync("APIQutationDetail/" + QutationId.ToString()).Result;
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
                        QutationModel.Qutation_ID = QutationModel.RefNumber;
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
                            HttpResponseMessage deleteQuaution2 = GlobalVeriables.WebApiClient.DeleteAsync("APIQutationDetail/" + QutationDetailID).Result;
                            if (deleteQuaution2.StatusCode == System.Net.HttpStatusCode.OK)
                            {
                                GlobalVeriables.WebApiClient.DefaultRequestHeaders.Clear();
                                return Json("Success", JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            return Json("Fail", JsonRequestBehavior.AllowGet);
                        }
                        #endregion
                    }
                }

                else
                {

                    HttpResponseMessage Qdresponse = GlobalVeriables.WebApiClient.DeleteAsync("APIQutationDetail/" + QutationDetailID).Result;

                    if (Qdresponse.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        QutationModel.SubTotal = 0.00;
                        QutationModel.TotalAmount = 0.00;
                        QutationModel.Qutation_ID = QutationModel.RefNumber;
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
                            return Json("Success", JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        return Json("Fail", JsonRequestBehavior.AllowGet);
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

                return Json("Fail", JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        public ActionResult Save(MVCQutationViewModel mvcQutationViewModel)
        {
            bool Status = false;
            var Qutationid = "";
            #region             
            try
            {
                MVCQutationModel mvcQutationModel = new MVCQutationModel();


                mvcQutationModel.Qutation_ID = mvcQutationViewModel.Qutation_ID;
                mvcQutationModel.CompanyId = 2;
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
                mvcQutationModel.Status = "Open";



                var response = GlobalVeriables.WebApiClient.PostAsJsonAsync("Qutation", mvcQutationModel).Result;

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
                    HttpResponseMessage responsses = GlobalVeriables.WebApiClient.PostAsJsonAsync("QutationDetail", QtDetails).Result;
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
            return new JsonResult { Data = new { Status = "Success", QutationId = Qutationid } };
        }

        public ActionResult GeneraPDF()
        {


            //int contectId = 3;
            //HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("ApiConatacts/" + contectId.ToString()).Result;
            //MVCContactsModel contectmodel = response.Content.ReadAsAsync<MVCContactsModel>().Result;

            //int companyId = 2;

            //HttpResponseMessage responseCompany1 = GlobalVeriables.WebApiClient.GetAsync("CompanyInformations/" + companyId.ToString()).Result;
            //MVCCompanyModel companyModel = responseCompany1.Content.ReadAsAsync<MVCCompanyModel>().Result;

            //int Qutationid = 2;
            //HttpResponseMessage responseQutation = GlobalVeriables.WebApiClient.GetAsync("APIQutation/" + Qutationid.ToString()).Result;
            //MVCQutationModel QutationModel = responseQutation.Content.ReadAsAsync<MVCQutationModel>().Result;

            //HttpResponseMessage responseQutationDetailsList = GlobalVeriables.WebApiClient.GetAsync("APIQutationDetail/" + Qutationid.ToString()).Result;
            //List<MVCQutationDetailsModel> QutationModelDetailsList = responseQutationDetailsList.Content.ReadAsAsync<List<MVCQutationDetailsModel>>().Result;
            //ViewBag.Contentdata = contectmodel;
            //ViewBag.Companydata = companyModel;
            //ViewBag.QutationDat = QutationModel;
            //ViewBag.QutationDatailsList = QutationModelDetailsList;

            //var root = Server.MapPath("~/PDF/");
            //var pdfname = String.Format("{0}.pdf", Guid.NewGuid().ToString());
            //var path = Path.Combine(root, pdfname);
            //string p = path.ToString();
            //path = Path.GetFullPath(path);

            //TempData["filePath"] = path;

            //return new Rotativa.ViewAsPdf("PrintView",new {p=path }) { FileName = "cv.pdf", SaveOnServerPath = path };

            return View();
        }




        [HttpPost]
        public ActionResult SaveEmail(MVCQutationViewModel MVCQutationViewModel)
        {
            MVCQutationModel mvcQutationModel = new MVCQutationModel();
            try
            {
                if (MVCQutationViewModel.QutationID != null)
                {
                    mvcQutationModel.Qutation_ID = MVCQutationViewModel.Qutation_ID;
                    mvcQutationModel.CompanyId = 2;
                    mvcQutationModel.UserId = 1;
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
                            if (QtDetails.QutationDetailId == 0)
                            {
                                HttpResponseMessage responsses = GlobalVeriables.WebApiClient.PostAsJsonAsync("APIQutationDetail", QtDetails).Result;
                            }
                            else
                            {
                                HttpResponseMessage responsses = GlobalVeriables.WebApiClient.PutAsJsonAsync("APIQutationDetail/" + QtDetails.QutationDetailId, QtDetails).Result;
                            }
                        }

                        string path = PrintView((int)MVCQutationViewModel.QutationID);

                        if (path != null)
                        {
                            TempData["path"] = path;
                        }

                        TempData["quaution"] = mvcQutationModel.QutationID.ToString();

                        return new JsonResult { Data = new { Status = "Success", path = path, QutationId = MVCQutationViewModel.QutationID } };

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
                if (MVCQutationViewModel.QutationID == null)
                {
                    mvcQutationModel.Qutation_ID = MVCQutationViewModel.Qutation_ID;
                    mvcQutationModel.CompanyId = 2;
                    mvcQutationModel.UserId = 1;
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
                            if (QtDetails.QutationDetailId == 0)
                            {
                                HttpResponseMessage responsses = GlobalVeriables.WebApiClient.PostAsJsonAsync("APIQutationDetail", QtDetails).Result;
                            }
                            else
                            {
                                HttpResponseMessage responsses = GlobalVeriables.WebApiClient.PutAsJsonAsync("APIQutationDetail/" + QtDetails.QutationDetailId, QtDetails).Result;
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
         
           
            HttpResponseMessage responseCompany = GlobalVeriables.WebApiClient.GetAsync("APIComapny/" + CompanyID.ToString()).Result;
            MVCCompanyInfoModel companyModel = responseCompany.Content.ReadAsAsync<MVCCompanyInfoModel>().Result;

            string[] arrya = FilePath.Split('-');
            string QuationId = arrya[0];

            string PdfName = QuationId + "-" + companyModel.CompanyName + ".pdf";
            return PdfName;
        }




        [HttpPost]
        public FileResult DownloadFile(string FilePath)
        {
            string filepath = "";
            string FileName = SetPdfName(FilePath);
            try
            {
                filepath = System.IO.Path.Combine(Server.MapPath("~/PDF/"), FilePath);
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
                if (MVCQutationViewModel.QutationID != null)
                {
                    mvcQutationModel.Qutation_ID = MVCQutationViewModel.Qutation_ID;
                    mvcQutationModel.CompanyId = 2;
                    mvcQutationModel.UserId = 1;
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

                            if (QtDetails.QutationDetailId == 0)
                            {
                                HttpResponseMessage responsses = GlobalVeriables.WebApiClient.PostAsJsonAsync("APIQutationDetail", QtDetails).Result;
                            }
                            else
                            {
                                HttpResponseMessage responsses = GlobalVeriables.WebApiClient.PutAsJsonAsync("APIQutationDetail/" + QtDetails.QutationDetailId, QtDetails).Result;
                            }
                        }

                        //calling printing option
                        string path1 = PrintView((int)MVCQutationViewModel.QutationID);
                        var root = Server.MapPath("~/PDF/");
                        var pdfname = String.Format("{0}", path1);
                        var path = Path.Combine(root, pdfname);
                        path = Path.GetFullPath(path);
                        DownloadFile(path);


                        return new JsonResult { Data = new { Status = "Success", path = pdfname, QutationId = MVCQutationViewModel.QutationID } };

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
            return new JsonResult { Data = new { Status = "Success", QutationId = MVCQutationViewModel.QutationID } };
        }





        [HttpPost]
        public ActionResult savePrintAndSentItToYouronsave(MVCQutationViewModel MVCQutationViewModel)
        {
            var Qutationid = "";
            int Qid = 0;
            MVCQutationModel mvcQutationModel = new MVCQutationModel();
            try
            {
                if (MVCQutationViewModel.QutationID == null)
                {
                    mvcQutationModel.Qutation_ID = MVCQutationViewModel.Qutation_ID;
                    mvcQutationModel.CompanyId = 2;
                    mvcQutationModel.UserId = 1;
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

                            if (QtDetails.QutationDetailId == 0)
                            {
                                HttpResponseMessage responsses = GlobalVeriables.WebApiClient.PostAsJsonAsync("APIQutationDetail", QtDetails).Result;
                            }
                            else
                            {
                                HttpResponseMessage responsses = GlobalVeriables.WebApiClient.PutAsJsonAsync("APIQutationDetail/" + QtDetails.QutationDetailId, QtDetails).Result;
                            }
                        }

                        //calling printing option
                        string path1 = PrintView(Qid);
                        var root = Server.MapPath("~/PDF/");
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

        public ActionResult p()
        {
          

          
            HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("ApiConatacts/" + Contectid.ToString()).Result;
            MVCContactModel contectmodel = response.Content.ReadAsAsync<MVCContactModel>().Result;

         
            HttpResponseMessage responseCompany = GlobalVeriables.WebApiClient.GetAsync("APIComapny/" + CompanyID.ToString()).Result;
            MVCCompanyInfoModel companyModel = responseCompany.Content.ReadAsAsync<MVCCompanyInfoModel>().Result;

            int quttationId = 2118;
           
            HttpResponseMessage responseQutation = GlobalVeriables.WebApiClient.GetAsync("APIQutation/" + quttationId.ToString()).Result;
            MVCQutationModel QutationModel = responseQutation.Content.ReadAsAsync<MVCQutationModel>().Result;

            HttpResponseMessage responseQutationDetailsList = GlobalVeriables.WebApiClient.GetAsync("APIQutationDetail/" + quttationId.ToString()).Result;
            List<MVCQutationDetailsModel> QutationModelDetailsList = responseQutationDetailsList.Content.ReadAsAsync<List<MVCQutationDetailsModel>>().Result;

            ViewBag.Contentdata = contectmodel;
            ViewBag.Companydata = companyModel;
            ViewBag.QutationDat = QutationModel;
            ViewBag.QutationDatailsList = QutationModelDetailsList;
            return View();
        }


        public class VatModel
        {
            public int Vat1 { get; set; }
            public string Name { get; set; }

        }
    }
}