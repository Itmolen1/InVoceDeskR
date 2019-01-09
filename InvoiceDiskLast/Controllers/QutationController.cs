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
    public class QutationController : Controller
    {
        // GET: Qutation
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult IndexQutation()
        {
            List<QutationIndexViewModel> quationList = new List<QutationIndexViewModel>();
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
                quationList = respose.Content.ReadAsAsync<List<QutationIndexViewModel>>().Result;

                List<QutationIndexViewModel> quationList1 = new List<QutationIndexViewModel>();
                if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
                {
                    if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
                    {

                        quationList = quationList.Where(p => p.QutationID.ToString().Contains(search)
                       || p.QutationDate != null && p.QutationDate.ToString().ToLower().Contains(search.ToLower())
                       || p.DueDate != null && p.DueDate.ToString().ToLower().Contains(search.ToLower())
                       || p.CustomerName != null && p.CustomerName.ToString().ToLower().Contains(search.ToLower())
                       || p.UserName != null && p.UserName.ToString().ToLower().Contains(search.ToLower())
                       || p.Vat != null && p.Vat.ToString().ToLower().Contains(search.ToLower())
                       || p.TotalAmount != null && p.TotalAmount.ToString().ToLower().Contains(search.ToLower())
                       || p.Status != null && p.Status.ToString().ToLower().Contains(search.ToLower())

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
            }
        }

        public ActionResult Create(int id)
        {

            MVCQutationViewModel quutionviewModel = new MVCQutationViewModel();

            CompanyID = Convert.ToInt32(Session["CompayID"]);


            try
            {
                HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("ApiConatacts/" + id.ToString()).Result;
                MVCContactModel contectmodel = response.Content.ReadAsAsync<MVCContactModel>().Result;

                HttpResponseMessage responseCompany = GlobalVeriables.WebApiClient.GetAsync("APIComapny/" + CompanyID.ToString()).Result;
                MVCCompanyInfoModel companyModel = responseCompany.Content.ReadAsAsync<MVCCompanyInfoModel>().Result;

                DateTime qutatioDate = new DateTime();
                qutatioDate = DateTime.Now;
                int PaymentDuration;
                if (contectmodel.PaymentTerm.ToString() !=  "" && contectmodel.PaymentTerm.ToString() != null)
                {
                     PaymentDuration = Convert.ToInt32(contectmodel.PaymentTerm);
                }
                else
                {
                    PaymentDuration = 15;
                }
                PaymentDuration = Convert.ToInt32(contectmodel.PaymentTerm);

                CommonModel commonModel = new CommonModel();
                commonModel.Name = "Quotation";
                
                ViewBag.Contentdata = contectmodel;
                ViewBag.Companydata = companyModel;
                commonModel.FromDate = qutatioDate;
                commonModel.DueDate = qutatioDate.AddDays(+PaymentDuration);

                MVCQutationModel q = new MVCQutationModel();
                HttpResponseMessage response1 = GlobalVeriables.WebApiClient.GetAsync("GetQuationCount/").Result;
                q = response1.Content.ReadAsAsync<MVCQutationModel>().Result;
                commonModel.Number_Id = q.Qutation_ID;

                ViewBag.commonModel = commonModel;


                return View(quutionviewModel);
            }
            catch (Exception)
            {
                return null;
            }

        }

        int Contactid = 0, CompanyID = 0;
        public ActionResult ViewQuation(int? quautionId)
        {
            int ID = Convert.ToInt32(quautionId);
            ViewBag.FILE = CreatDirectoryClass.GetFileDirectiory((int)quautionId, "Quotation");
            try
            {
                HttpResponseMessage responseQutation = GlobalVeriables.WebApiClient.GetAsync("APIQutation/" + quautionId.ToString()).Result;
                MVCQutationModel QutationModel = responseQutation.Content.ReadAsAsync<MVCQutationModel>().Result;

                HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("ApiConatacts/" + QutationModel.ContactId.ToString()).Result;
                MVCContactModel contectmodel = response.Content.ReadAsAsync<MVCContactModel>().Result;

                HttpResponseMessage responseCompany = GlobalVeriables.WebApiClient.GetAsync("APIComapny/" + QutationModel.CompanyId.ToString()).Result;
                MVCCompanyInfoModel companyModel = responseCompany.Content.ReadAsAsync<MVCCompanyInfoModel>().Result;



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
        public ActionResult UploadFile(HttpPostedFileBase fileUpload)
        {
            // DO Stuff
            return View();
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

        public ActionResult Print(int? QutationID)
        {

            try
            {
                HttpResponseMessage responseQutation = GlobalVeriables.WebApiClient.GetAsync("APIQutation/" + QutationID.ToString()).Result;
                MVCQutationModel QutationModel = responseQutation.Content.ReadAsAsync<MVCQutationModel>().Result;

                HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("ApiConatacts/" + QutationModel.ContactId.ToString()).Result;
                MVCContactModel contectmodel = response.Content.ReadAsAsync<MVCContactModel>().Result;

                HttpResponseMessage responseCompany = GlobalVeriables.WebApiClient.GetAsync("APIComapny/" + QutationModel.CompanyId.ToString()).Result;
                MVCCompanyInfoModel companyModel = responseCompany.Content.ReadAsAsync<MVCCompanyInfoModel>().Result;



                HttpResponseMessage responseQutationDetailsList = GlobalVeriables.WebApiClient.GetAsync("APIQutationDetails/" + QutationID.ToString()).Result;
                List<MVCQutationViewModel> QutationModelDetailsList = responseQutationDetailsList.Content.ReadAsAsync<List<MVCQutationViewModel>>().Result;

                DateTime qutationDueDate = Convert.ToDateTime(QutationModel.DueDate); //mm/dd/yyyy
                DateTime qutationDate = Convert.ToDateTime(QutationModel.QutationDate);//mm/dd/yyyy
                TimeSpan ts = qutationDueDate.Subtract(qutationDate);
                string diffDate = ts.Days.ToString();

                ViewBag.Contentdata = contectmodel;
                ViewBag.Companydata = companyModel;
                ViewBag.QutationDat = QutationModel;
                ViewBag.QutationDatailsList = QutationModelDetailsList;

                string PdfName = QutationID + "-" + companyModel.CompanyName + ".pdf";

                return new Rotativa.PartialViewAsPdf("~/Views/Qutation/PrintQutationPartialView.cshtml")
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

        public ActionResult DeleteFile(string FileName)
        {
            try
            {
                if (CreatDirectoryClass.DeleteFileFromPDF(FileName))
                {

                    return Json("Success", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("Fail", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                return Json("Fail", JsonRequestBehavior.AllowGet);
                throw;
            }


        }

        //public bool UploadFile(int? Id, string FileName, HttpPostedFileWrapper[] file)
        //{
        //    try
        //    {
        //        var allowedExtensions = new string[] { ".doc", ".docx", ".pdf", ".jpg", ".png", ".JPEG", ".JFIF", ".PNG", ".txt" };
        //        string FilePath = CreatDirectoryClass.CreateDirecotyFolder(Id, FileName);

        //        string fap = Server.MapPath(FilePath);

        //        for (int i = 0; i < file.Count(); i++)
        //        {
        //            HttpPostedFileBase f = file[i];
        //            FileInfo fi = new FileInfo(f.FileName);
        //            string ext = fi.Extension;
        //            if (allowedExtensions.Contains(ext))
        //            {
        //                string dateTime = DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();

        //                string FileName1 = f.FileName.Replace(ext, "");

        //                string FileNameSetting = FileName1 + dateTime + ext;

        //                f.SaveAs(fap + FileNameSetting);

        //            }
        //        }
        //    }

        //    catch (Exception)
        //    {

        //        throw;
        //    }

        //    return true;
        //}

        [HttpPost]
        public ActionResult DeleteFile(int Id, string FileName)
        {
            try
            {


                if (CreatDirectoryClass.Delete(Id, FileName, "Quotation"))
                {

                    return Json("Success", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("Fail", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                return Json("Fail", JsonRequestBehavior.AllowGet);
                throw;
            }


        }

      
        [HttpPost]
        public ActionResult SaveEmail(MVCQutationViewModel MVCQutationViewModel)
        {

            MVCQutationModel mvcQutationModel = new MVCQutationModel();
            try
            {

                mvcQutationModel.Qutation_ID = MVCQutationViewModel.Qutation_ID;
                mvcQutationModel.CompanyId = MVCQutationViewModel.CompanyId;
                mvcQutationModel.UserId = Convert.ToInt32(Session["LoginUserID"]);
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

                HttpResponseMessage response = GlobalVeriables.WebApiClient.PostAsJsonAsync("APIQutation", mvcQutationModel).Result;

                QutationTable qtd = response.Content.ReadAsAsync<QutationTable>().Result;

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    if (MVCQutationViewModel.QutationDetailslist1 != null)
                    {
                        foreach (QutationDetailsTable QDTList in MVCQutationViewModel.QutationDetailslist1)
                        {
                            QutationDetailsTable QtDetails = new QutationDetailsTable();
                            QtDetails.ItemId = Convert.ToInt32(QDTList.ItemId);
                            QtDetails.QutationID = qtd.QutationID;
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
                        if (MVCQutationViewModel.file23[0] != null)
                        {
                            CreatDirectoryClass.UploadFileToDirectoryCommon(qtd.QutationID, "Quatation", MVCQutationViewModel.file23, "Quotation");
                        }

                        return new JsonResult { Data = new { Status = "Success", path = "", QutationId = qtd.QutationID } };
                    }

                    if (MVCQutationViewModel.file23[0] != null)
                    {
                        CreatDirectoryClass.UploadFileToDirectoryCommon(qtd.QutationID, "Quatation", MVCQutationViewModel.file23, "Quotation");
                    }

                    return new JsonResult { Data = new { Status = "Success", QutationId = qtd.QutationID } };
                }
            }
            catch (Exception ex)
            {
                return new JsonResult { Data = new { Status = "Fail", Message = ex.Message.ToString() } };
            }

            return new JsonResult { Data = new { Status = "Success", path = "", QutationId = MVCQutationViewModel.QutationID } };

        }

        [HttpPost]
        public ActionResult SaveDraft(MVCQutationViewModel MVCQutationViewModel)
        {
            QutationTable qutationTable;
            MVCQutationModel mvcQutationModel = new MVCQutationModel();
            try
            {
                mvcQutationModel.Qutation_ID = MVCQutationViewModel.Qutation_ID;
                mvcQutationModel.CompanyId = MVCQutationViewModel.CompanyId;
                mvcQutationModel.UserId = Convert.ToInt32(Session["LoginUserID"]);
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
                HttpResponseMessage response = GlobalVeriables.WebApiClient.PostAsJsonAsync("APIQutation/", mvcQutationModel).Result;
                qutationTable = response.Content.ReadAsAsync<QutationTable>().Result;

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    if (MVCQutationViewModel.QutationDetailslist1 != null)
                    {

                        foreach (QutationDetailsTable QDTList in MVCQutationViewModel.QutationDetailslist1)
                        {
                            QutationDetailsTable QtDetails = new QutationDetailsTable();
                            QtDetails.ItemId = Convert.ToInt32(QDTList.ItemId);
                            QtDetails.QutationID = qutationTable.QutationID;
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
                        if (MVCQutationViewModel.file23[0] != null)
                        {
                            CreatDirectoryClass.UploadFileToDirectoryCommon(qutationTable.QutationID, "Quatation", MVCQutationViewModel.file23, "Quotation");
                        }
                    }

                    if (MVCQutationViewModel.file23[0] != null)
                    {
                        CreatDirectoryClass.UploadFileToDirectoryCommon(qutationTable.QutationID, "Quatation", MVCQutationViewModel.file23, "Quotation");
                    }
                }
            }
            catch (Exception ex)
            {

                return new JsonResult { Data = new { Status = "Fail", Message = ex.Message.ToString() } };
            }

            return new JsonResult { Data = new { Status = "Success", path = "", QutationId = qutationTable.QutationID } };
        }
        
        [HttpGet]
        public ActionResult EditQutation(int QutationId)
        {
            ViewBag.FILE = CreatDirectoryClass.GetFileDirectiory(QutationId, "Quotation");

            MVCQutationViewModel quutionviewModel2 = new MVCQutationViewModel();
            try
            {
                HttpResponseMessage responseQutation = GlobalVeriables.WebApiClient.GetAsync("APIQutation/" + QutationId.ToString()).Result;
                MVCQutationModel QutationModel = responseQutation.Content.ReadAsAsync<MVCQutationModel>().Result;

                HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("ApiConatacts/" + QutationModel.ContactId.ToString()).Result;
                MVCContactModel contectmodel = response.Content.ReadAsAsync<MVCContactModel>().Result;

                HttpResponseMessage responseCompany = GlobalVeriables.WebApiClient.GetAsync("APIComapny/" + QutationModel.CompanyId.ToString()).Result;
                MVCCompanyInfoModel companyModel = responseCompany.Content.ReadAsAsync<MVCCompanyInfoModel>().Result;


                quutionviewModel2.DueDate = QutationModel.DueDate;
                quutionviewModel2.QutationDate = QutationModel.QutationDate;
                quutionviewModel2.CompanyId = QutationModel.CompanyId;
                quutionviewModel2.QutationID = QutationModel.QutationID;
                quutionviewModel2.Qutation_ID = QutationModel.Qutation_ID;
                quutionviewModel2.CompanyId = QutationModel.CompanyId;
                quutionviewModel2.ConatctId = QutationModel.ContactId;
                HttpResponseMessage responseQutationDetailsList = GlobalVeriables.WebApiClient.GetAsync("APIQutationDetails/" + QutationId.ToString()).Result;
                List<MVCQutationViewModel> QutationModelDetailsList = responseQutationDetailsList.Content.ReadAsAsync<List<MVCQutationViewModel>>().Result;

                HttpResponseMessage GoodResponse = GlobalVeriables.WebApiClient.GetAsync("APIProduct/" + QutationModel.CompanyId + "/Good").Result;
                List<MVCProductModel> GoodModel = GoodResponse.Content.ReadAsAsync<List<MVCProductModel>>().Result;
                ViewBag.Good = GoodModel;
                HttpResponseMessage Services = GlobalVeriables.WebApiClient.GetAsync("APIProduct/" + QutationModel.CompanyId + "/Services").Result;
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
                return View(quutionviewModel2);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult EditQutation(MVCQutationViewModel MVCQutationViewModel)
        {
            MVCQutationModel mvcQutationModel = new MVCQutationModel();
            try
            {
                mvcQutationModel.Qutation_ID = MVCQutationViewModel.Qutation_ID;
                mvcQutationModel.Qutation_ID = MVCQutationViewModel.Qutation_ID;
                mvcQutationModel.CompanyId = MVCQutationViewModel.CompanyId;
                mvcQutationModel.UserId = Convert.ToInt32(Session["LoginUserID"]);
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

                if (mvcQutationModel.TotalVat6 != null && mvcQutationModel.TotalVat6 != 0)
                {
                    double vat61 = Math.Round((double)mvcQutationModel.TotalVat6, 2, MidpointRounding.AwayFromZero);
                    mvcQutationModel.TotalVat6 = vat61;
                }
                mvcQutationModel.TotalVat21 = MVCQutationViewModel.TotalVat21;
                if (mvcQutationModel.TotalVat21 != null && mvcQutationModel.TotalVat21 != 0)
                {
                    double vat21 = Math.Round((double)mvcQutationModel.TotalVat21, 2, MidpointRounding.AwayFromZero);
                    mvcQutationModel.TotalVat21 = vat21;
                }

                mvcQutationModel.Qutation_ID = MVCQutationViewModel.Qutation_ID;
                HttpResponseMessage response = GlobalVeriables.WebApiClient.PutAsJsonAsync("APIQutation/" + mvcQutationModel.QutationID, mvcQutationModel).Result;
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    if (MVCQutationViewModel.QutationDetailslist1 != null)
                    {
                        foreach (QutationDetailsTable QDTList in MVCQutationViewModel.QutationDetailslist1)
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
                    }

                    //return new JsonResult { Data = new { Status = "Success", QutationId = MVCQutationViewModel.QutationID } };


                }

            }
            catch (Exception ex)
            {

                return new JsonResult { Data = new { Status = "Fail", Message = ex.Message.ToString() } };
            }

            return new JsonResult { Data = new { Status = "Success", QutationId = MVCQutationViewModel.QutationID } };


        }
        
        [HttpPost]
        public ActionResult UploadFileToPDF()
        {
            try
            {
                HttpFileCollectionBase files = Request.Files;
                for (int i = 0; i < files.Count; i++)
                {
                    HttpPostedFileBase file = files[i];

                    string FileName = CreatDirectoryClass.UploadToPDFCommon(file);

                    return Json(FileName, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {

                throw;
            }

            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        
        [HttpPost]
        public ActionResult UploadFiles(MVCQutationViewModel MVCQutationViewModel)
        {
            try
            {

                string FileName = "";
                HttpFileCollectionBase files = Request.Files;
                for (int i = 0; i < files.Count; i++)
                {
                    HttpPostedFileBase file = files[i];


                    FileName = CreatDirectoryClass.UploadFileToDirectoryCommon(MVCQutationViewModel.QutationID, "Quatation", MVCQutationViewModel.file23, "Quotation");
                }

                return new JsonResult { Data = new { FilePath = FileName, FileName = FileName } };
            }
            catch (Exception)
            {
                throw;
            }
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

                HttpResponseMessage responseQutation = GlobalVeriables.WebApiClient.GetAsync("APIQutation/" + quttationId.ToString()).Result;
                MVCQutationModel QutationModel = responseQutation.Content.ReadAsAsync<MVCQutationModel>().Result;


                TempData["CompanyId"] = QutationModel.CompanyId;
                TempData["ConatctId"] = QutationModel.ContactId;


                HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("ApiConatacts/" + QutationModel.ContactId.ToString()).Result;
                MVCContactModel contectmodel = response.Content.ReadAsAsync<MVCContactModel>().Result;



                HttpResponseMessage responseCompany = GlobalVeriables.WebApiClient.GetAsync("APIComapny/" + QutationModel.CompanyId.ToString()).Result;
                MVCCompanyInfoModel companyModel = responseCompany.Content.ReadAsAsync<MVCCompanyInfoModel>().Result;


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
                    catch (System.IO.IOException)
                    {

                    }
                }


                var pdfResult = new Rotativa.PartialViewAsPdf("~/Views/Qutation/PrintQutationPartialView.cshtml")
                {

                    PageSize = Rotativa.Options.Size.A4,
                    MinimumFontSize = 16,
                    PageMargins = new Rotativa.Options.Margins(10, 12, 20, 3),
                    PageHeight = 40,

                    SaveOnServerPath = path, // Save your place

                    //  CustomSwitches = "--footer-center \"" + "Wilt u zo vriendelijk zijn om het verschuldigde bedrag binnen " + diffDate + " dagen over te maken naar IBAN:  " + companyModel.IBANNumber + " ten name van IT Molen o.v.v.bovenstaande factuurnummer.  (Op al onze diensten en producten zijn onze algemene voorwaarden van toepassing.Deze kunt u downloaden van onze website.)" + "  Printed date: " +
                    // DateTime.Now.Date.ToString("MM/dd/yyyy") + "  Page: [page]/[toPage]\"" +
                    //" --footer-line --footer-font-size \"10\" --footer-spacing 6 --footer-font-name \"calibri light\"",

                };

                pdfResult.BuildPdf(this.ControllerContext);
            }
            catch (Exception)
            {

                throw;
            }

            return pdfname;
        }

        public ActionResult InvoicebyEmail(int? QutationId)
        {
            var List = CreatDirectoryClass.GetFileDirectiory((int)QutationId, "Quotation");

            EmailModel email = new EmailModel();

            List<Selected> _list = new List<Selected>();

            foreach (var Item in List)
            {
                _list.Add(new Selected { IsSelected = true, FileName = Item.DirectoryPath, Directory = Item.FileFolderPathe + "/" + Item.DirectoryPath });
            }

            email.SelectList = _list;

            try
            {

                email.Attachment = PrintView((int)QutationId);

                HttpContext.Items["FilePath"] = email.Attachment;

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

                int ClientId = Convert.ToInt32(TempData["ConatctId"]);
                int ConmpanyId = Convert.ToInt32(TempData["CompanyId"]);

                HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("ApiConatacts/" + ClientId.ToString()).Result;
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
                string result = strToProcess.Replace(".", " \r");

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

        [DeleteFileClass]

        [HttpPost]
        public ActionResult InvoicebyEmail(EmailModel email, string[] Files, FormCollection formCollection)
        {
            var root = Server.MapPath("/PDF/");

            List<AttakmentList> _attackmentList = new List<AttakmentList>();
            var allowedExtensions = new string[] { "doc", "docx", "pdf", ".jpg", "png", "JPEG", "JFIF", "PNG" };

            if (email.SelectList != null)
            {

                foreach (var item in email.SelectList)
                {

                    if (item.IsSelected)
                    {

                        if (item.Directory.EndsWith("doc") || item.Directory.EndsWith("pdf") || item.Directory.EndsWith("docx") || item.Directory.EndsWith("jpg") || item.Directory.EndsWith("png") || item.Directory.EndsWith("txt"))
                        {
                            if (System.IO.File.Exists(Server.MapPath(item.Directory)))
                            {
                                _attackmentList.Add(new AttakmentList { Attckment = Server.MapPath(item.Directory) });
                            }

                            var filwe = Server.MapPath("/PDF/" + item.FileName);

                            if (System.IO.File.Exists(filwe))
                            {
                                _attackmentList.Add(new AttakmentList { Attckment = filwe });
                            }
                        }
                    }
                }
            }


            if (Request.Form["FileName"] != null)
            {
                var fileName2 = Request.Form["FileName"];
                string[] valueArray = fileName2.Split(',');

                foreach (var item in valueArray)
                {
                    if (item.EndsWith("doc") || item.EndsWith("pdf") || item.EndsWith("docx") || item.EndsWith("jpg") || item.EndsWith("png") || item.EndsWith("txt"))
                    {
                        var filwe = Server.MapPath("/PDF/" + item);
                        if (System.IO.File.Exists(filwe))
                        {
                            _attackmentList.Add(new AttakmentList { Attckment = filwe });
                        }
                    }
                }
            }


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

                    var pdfname = String.Format("{0}.pdf", p);
                    var path = Path.Combine(root, pdfname);
                    email.Attachment = path;
                    _attackmentList.Add(new AttakmentList { Attckment = email.Attachment });
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
                    var pdfname = String.Format("{0}.pdf", email.Attachment);
                    var path = Path.Combine(root, pdfname);
                    email.Attachment = path;
                    emailModel.From = email.From;
                    emailModel.ToEmail = email.ToEmail;
                    emailModel.Attachment = email.Attachment;
                    emailModel.EmailBody = email.EmailText;
                    _attackmentList.Add(new AttakmentList { Attckment = emailModel.Attachment });
                    bool result = EmailController.email(emailModel, _attackmentList);
                    TempData["EmailMessge"] = "Email Send successfully";
                }

                HttpResponseMessage res = GlobalVeriables.WebApiClient.GetAsync("APIQutation/" + email.invoiceId.ToString()).Result;
                MVCQutationModel ob = res.Content.ReadAsAsync<MVCQutationModel>().Result;



                var folderPath = Server.MapPath("/PDF/");
                EmailController.clearFolder(folderPath);
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
            email.Attachment = fileName;

            return View(email);
        }




        [HttpPost]
        [DeleteFileClass]
        public FileResult DownloadFile(string FilePath1)
        {

            string filepath = "";
            string FileName = FilePath1;
            try
            {
                filepath = System.IO.Path.Combine(Server.MapPath("/PDF/"), FilePath1);
                HttpContext.Items["FilePath"] = filepath;

            }
            catch (Exception)
            {
            }

            return File(filepath, MimeMapping.GetMimeMapping(filepath), FileName);
        }

        [HttpPost]
        public ActionResult SaveEmailPrint(MVCQutationViewModel MVCQutationViewModel)
        {

            QutationTable qutationTable;
            MVCQutationModel mvcQutationModel = new MVCQutationModel();

            try
            {
                mvcQutationModel.Qutation_ID = MVCQutationViewModel.Qutation_ID;
                mvcQutationModel.CompanyId = MVCQutationViewModel.CompanyId;
                mvcQutationModel.UserId = Convert.ToInt32(Session["LoginUserID"]);
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
                qutationTable = response.Content.ReadAsAsync<QutationTable>().Result;

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    if (MVCQutationViewModel.QutationDetailslist1 != null)
                    {

                        foreach (QutationDetailsTable QDTList in MVCQutationViewModel.QutationDetailslist1)
                        {
                            QutationDetailsTable QtDetails = new QutationDetailsTable();
                            QtDetails.ItemId = Convert.ToInt32(QDTList.ItemId);
                            QtDetails.QutationID = qutationTable.QutationID;
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



                    }

                    if (MVCQutationViewModel.file23[0] != null)
                    {
                        CreatDirectoryClass.UploadFileToDirectoryCommon(qutationTable.QutationID, "Quatation", MVCQutationViewModel.file23, "Quotation");
                    }

                    string path1 = PrintView((int)qutationTable.QutationID);
                    var root = Server.MapPath("/PDF/");
                    var pdfname = String.Format("{0}", path1);
                    var path = Path.Combine(root, pdfname);
                    path = Path.GetFullPath(path);

                    return new JsonResult { Data = new { Status = "Success", path = pdfname, QutationId = qutationTable.QutationID } };
                }
            }
            catch (Exception ex)
            {
                return new JsonResult { Data = new { Status = "Fail", Message = ex.Message.ToString() } };
            }
            return new JsonResult { Data = new { Status = "Success", path = "", QutationId = qutationTable.QutationID } };
        }

        [HttpPost]
        public ActionResult SaveEmailEdit(MVCQutationViewModel MVCQutationViewModel)
        {
            MVCQutationModel mvcQutationModel = new MVCQutationModel();
            try
            {


                mvcQutationModel.Qutation_ID = MVCQutationViewModel.Qutation_ID;

                mvcQutationModel.Qutation_ID = MVCQutationViewModel.Qutation_ID;
                mvcQutationModel.CompanyId = MVCQutationViewModel.CompanyId;
                mvcQutationModel.UserId = Convert.ToInt32(Session["LoginUserID"]);
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

                if (mvcQutationModel.TotalVat6 != null && mvcQutationModel.TotalVat6 != 0)
                {
                    double vat61 = Math.Round((double)mvcQutationModel.TotalVat6, 2, MidpointRounding.AwayFromZero);
                    mvcQutationModel.TotalVat6 = vat61;
                }

                mvcQutationModel.TotalVat21 = MVCQutationViewModel.TotalVat21;

                if (mvcQutationModel.TotalVat21 != null && mvcQutationModel.TotalVat21 != 0)
                {
                    double vat21 = Math.Round((double)mvcQutationModel.TotalVat21, 2, MidpointRounding.AwayFromZero);
                    mvcQutationModel.TotalVat21 = vat21;
                }

                mvcQutationModel.Qutation_ID = MVCQutationViewModel.Qutation_ID;

                HttpResponseMessage response = GlobalVeriables.WebApiClient.PutAsJsonAsync("APIQutation/" + mvcQutationModel.QutationID, mvcQutationModel).Result;

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    if (MVCQutationViewModel.QutationDetailslist1 != null)
                    {


                        foreach (QutationDetailsTable QDTList in MVCQutationViewModel.QutationDetailslist1)
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
                }
            }
            catch (Exception ex)
            {

                return new JsonResult { Data = new { Status = "Fail", Message = ex.Message.ToString() } };
            }

            return new JsonResult { Data = new { Status = "Success", QutationId = MVCQutationViewModel.QutationID } };
        }
        int companyId = 0;

        [HttpPost]
        public ActionResult SaveEmailPrintEdit(MVCQutationViewModel MVCQutationViewModel)
        {
            MVCQutationModel mvcQutationModel = new MVCQutationModel();

            try
            {

                mvcQutationModel.Qutation_ID = MVCQutationViewModel.Qutation_ID;
                mvcQutationModel.CompanyId = MVCQutationViewModel.CompanyId;
                mvcQutationModel.UserId = Convert.ToInt32(Session["LoginUserID"]);
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

                HttpResponseMessage response = GlobalVeriables.WebApiClient.PutAsJsonAsync("APIQutation/" + mvcQutationModel.QutationID, mvcQutationModel).Result;

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    if (MVCQutationViewModel.QutationDetailslist1 != null)
                    {
                        foreach (QutationDetailsTable QDTList in MVCQutationViewModel.QutationDetailslist1)
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
                    }


                }
                string FilePath = PrintView((int)MVCQutationViewModel.QutationID);
                var root = Server.MapPath("/PDF/");
                var pdfname = String.Format("{0}", FilePath);
                var path = Path.Combine(root, pdfname);
                path = Path.GetFullPath(path);
                return new JsonResult { Data = new { Status = "Success", path = FilePath, QutationId = MVCQutationViewModel.QutationID } };

            }
            catch (Exception ex)
            {
                return new JsonResult { Data = new { Status = "Fail", Message = ex.Message.ToString() } };
            }

        }


    }
}