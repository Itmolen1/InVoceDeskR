using InvoiceDiskLast.MISC;
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
    public class InvoiceController : Controller
    {
        // GET: Invoice
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetInvoiceList()
        {
            List<InvoiceViewModel> invoiceViewModel = new List<InvoiceViewModel>();
            try
            {
                var draw = Request.Form.GetValues("draw").FirstOrDefault();
                var start = Request.Form.GetValues("start").FirstOrDefault();
                var length = Request.Form.GetValues("length").FirstOrDefault();
                var sortColumn = Request.Form.GetValues("columns[" +
                Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
                string search = Request.Form.GetValues("search[value]")[0];

                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;

                int CompanyId = Convert.ToInt32(Session["CompayID"]);              

                HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("GetInvoiceTable/" + CompanyId).Result;
                invoiceViewModel = response.Content.ReadAsAsync<List<InvoiceViewModel>>().Result;

                if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
                {
                    invoiceViewModel = invoiceViewModel.Where(p => p.InvoiceID.ToString().Contains(search)
                  || p.RefNumber != null && p.RefNumber.ToLower().Contains(search.ToLower())
                  || p.InvoiceDate != null && p.InvoiceDate.ToString().ToLower().Contains(search.ToLower())
                  || p.InvoiceDueDate != null && p.InvoiceDueDate.ToString().ToLower().Contains(search.ToLower())
                  || p.Status != null && p.Status.ToString().ToLower().Contains(search.ToLower())
                  || p.SubTotal != null && p.SubTotal.ToString().ToLower().Contains(search.ToLower())
                  || p.Status != null && p.Status.ToString().ToLower().Contains(search.ToLower())).ToList();
                }

                int recordsTotal = recordsTotal = invoiceViewModel.Count();
                var data = invoiceViewModel.Skip(skip).Take(pageSize).ToList();
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Response.Write(ex.ToString());
                Json(new { draw = 0, recordsFiltered = 0, recordsTotal = 0, data = 0 }, JsonRequestBehavior.AllowGet);
            }
            return Json(null, JsonRequestBehavior.AllowGet);

        }


        [HttpPost]
        public ActionResult AddCustomer(MVCQutationViewModel model)
        {
            string fileName;         

            for (int i = 0; i < Request.Files.Count; i++)
            {
                HttpPostedFileBase file = Request.Files[i]; //Uploaded file
                                                            //Use the following properties to get file's name, size and MIMEType
                int fileSize = file.ContentLength;
                 fileName = file.FileName;
                string mimeType = file.ContentType;
                System.IO.Stream fileContent = file.InputStream;
                //To save file, use SaveAs method
                file.SaveAs(Server.MapPath("~/") + fileName); //File will be saved in application root
            }
            return null;
        }

        public ActionResult Create(int id)
        {
                       
            InvoiceViewModel invoioceViewModel = new InvoiceViewModel();

            int CompanyID = Convert.ToInt32(Session["CompayID"]);
            try
            {
                HttpResponseMessage responseCompany = GlobalVeriables.WebApiClient.GetAsync("APIComapny/" + CompanyID.ToString()).Result;
                MVCCompanyInfoModel companyModel = responseCompany.Content.ReadAsAsync<MVCCompanyInfoModel>().Result;

                HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("ApiConatacts/" + id.ToString()).Result;
                MVCContactModel contectmodel = response.Content.ReadAsAsync<MVCContactModel>().Result;
                
                DateTime qutatioDate = new DateTime();
                qutatioDate = DateTime.Now.Date;

                int paymentTerm = 0;
                paymentTerm = Convert.ToInt32(contectmodel.PaymentTerm);

                ViewBag.Contentdata = contectmodel;
                ViewBag.Companydata = companyModel;
                invoioceViewModel.InvoiceDate = qutatioDate;
                invoioceViewModel.InvoiceDueDate = qutatioDate.AddDays(+paymentTerm);

                MVCInvoiceModel InvoiceCount = new MVCInvoiceModel();
                HttpResponseMessage response1 = GlobalVeriables.WebApiClient.GetAsync("GetInvoiceCount/").Result;
                InvoiceCount = response1.Content.ReadAsAsync<MVCInvoiceModel>().Result;
                invoioceViewModel.Invoice_ID = InvoiceCount.Invoice_ID;

                invoioceViewModel.ContactId = id;
                invoioceViewModel.CompanyId = CompanyID;
                return View(invoioceViewModel);
            }
            catch (Exception ex)
            {
                return null;
            }
           
        }

        [HttpPost]
        public ActionResult SaveDraft(InvoiceViewModel invoiceViewModel, HttpPostedFileBase filess)
        {
            InvoiceTable InvoiceTable;
            MVCInvoiceModel mvcInvoiceModel = new MVCInvoiceModel();
            try
            {
                mvcInvoiceModel.Invoice_ID = invoiceViewModel.Invoice_ID;
                mvcInvoiceModel.CompanyId = invoiceViewModel.CompanyId;
                mvcInvoiceModel.UserId = Convert.ToInt32(Session["LoginUserID"]);
                mvcInvoiceModel.ContactId = invoiceViewModel.ContactId;
                mvcInvoiceModel.InvoiceID = invoiceViewModel.InvoiceID;
                mvcInvoiceModel.RefNumber = invoiceViewModel.RefNumber;
                mvcInvoiceModel.InvoiceDate = invoiceViewModel.InvoiceDate;
                mvcInvoiceModel.InvoiceDueDate = invoiceViewModel.InvoiceDueDate;
                mvcInvoiceModel.SubTotal = invoiceViewModel.SubTotal;
                mvcInvoiceModel.DiscountAmount = invoiceViewModel.DiscountAmount;
                mvcInvoiceModel.TotalAmount = invoiceViewModel.TotalAmount;
                mvcInvoiceModel.CustomerNote = invoiceViewModel.CustomerNote;               
                mvcInvoiceModel.TotalVat21 = invoiceViewModel.TotalVat21;
                mvcInvoiceModel.TotalVat6 = invoiceViewModel.TotalVat6;
                mvcInvoiceModel.Type = StatusEnum.Goods.ToString();
                mvcInvoiceModel.Status = "accepted";

                if (mvcInvoiceModel.TotalVat6 != null)
                {
                    double vat61 = Math.Round((double)mvcInvoiceModel.TotalVat6, 2, MidpointRounding.AwayFromZero);
                    mvcInvoiceModel.TotalVat6 = vat61;
                }

                if (mvcInvoiceModel.TotalVat21 != null)
                {
                    double vat21 = Math.Round((double)mvcInvoiceModel.TotalVat21, 2, MidpointRounding.AwayFromZero);
                    mvcInvoiceModel.TotalVat21 = vat21;
                }

                mvcInvoiceModel.Invoice_ID = invoiceViewModel.Invoice_ID;
                HttpResponseMessage response = GlobalVeriables.WebApiClient.PostAsJsonAsync("PostInvoice", mvcInvoiceModel).Result;
                InvoiceTable = response.Content.ReadAsAsync<InvoiceTable>().Result;

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    if (invoiceViewModel.InvoiceDetailsTable != null)
                    {

                        foreach (InvoiceDetailsTable InvoiceDetailsList in invoiceViewModel.InvoiceDetailsTable)
                        {
                            InvoiceDetailsTable InvoiceDetails = new InvoiceDetailsTable();
                            InvoiceDetails.ItemId = Convert.ToInt32(InvoiceDetailsList.ItemId);
                            InvoiceDetails.InvoiceId = InvoiceTable.InvoiceID;
                            InvoiceDetails.Description = InvoiceDetailsList.Description;
                            //QtDetails.QutationDetailId = QDTList.QutationDetailId;
                            InvoiceDetails.Quantity = InvoiceDetailsList.Quantity;
                            InvoiceDetails.Rate = Convert.ToDouble(InvoiceDetailsList.Rate);
                            InvoiceDetails.Total = Convert.ToDouble(InvoiceDetailsList.Total);
                            InvoiceDetails.ServiceDate = InvoiceDetailsList.ServiceDate;
                            InvoiceDetails.RowSubTotal = InvoiceDetailsList.RowSubTotal;
                            InvoiceDetails.Vat = Convert.ToDouble(InvoiceDetailsList.Vat);
                            InvoiceDetails.Type = InvoiceDetailsList.Type;

                            if (InvoiceDetails.InvoiceDetailId == 0)
                            {
                                HttpResponseMessage responsses = GlobalVeriables.WebApiClient.PostAsJsonAsync("PostinvoiceDetails", InvoiceDetails).Result;
                            }
                            else
                            {
                                HttpResponseMessage responsses = GlobalVeriables.WebApiClient.PutAsJsonAsync("PostinvoiceDetails/" + InvoiceDetails.InvoiceDetailId, InvoiceDetails).Result;
                            }
                        }
                        if (invoiceViewModel.file23[0] != null)
                        {
                          
                            CreatDirectoryClass.UploadFileToDirectoryCommon(InvoiceTable.InvoiceID, "Invoice", invoiceViewModel.file23,"Invoice");
                          
                        }
                    }

                    if (invoiceViewModel.file23[0] != null)
                    {
                        UploadFiles.UploadFile(mvcInvoiceModel.InvoiceID, "Invoice", invoiceViewModel.file23,"Invoice");
                    }
                }
            }
            catch (Exception ex)
            {

                return new JsonResult { Data = new { Status = "Fail", Message = ex.Message.ToString() } };
            }

            return new JsonResult { Data = new { Status = "Success", path = "", id = InvoiceTable.InvoiceID } };
        }


        [HttpGet]
        public ActionResult Edit(int id)
        {
            InvoiceViewModel invoiceViewModel = new InvoiceViewModel();
            try
            {
                ViewBag.FILE = CreatDirectoryClass.GetFileDirectiory(id,"Invoice");

                HttpResponseMessage responseInvoice = GlobalVeriables.WebApiClient.GetAsync("GetInvoiceById/" + id.ToString()).Result;
                MVCInvoiceModel InvoiceModel = responseInvoice.Content.ReadAsAsync<MVCInvoiceModel>().Result;
                invoiceViewModel.InvoiceID = InvoiceModel.InvoiceID;
                invoiceViewModel.CompanyId = InvoiceModel.CompanyId;
                invoiceViewModel.ContactId = InvoiceModel.ContactId;
                invoiceViewModel.Invoice_ID = InvoiceModel.Invoice_ID;

                 HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("ApiConatacts/" + invoiceViewModel.ContactId.ToString()).Result;
                MVCContactModel contectmodel = response.Content.ReadAsAsync<MVCContactModel>().Result;

                HttpResponseMessage responseCompany = GlobalVeriables.WebApiClient.GetAsync("APIComapny/" + invoiceViewModel.CompanyId.ToString()).Result;
                MVCCompanyInfoModel companyModel = responseCompany.Content.ReadAsAsync<MVCCompanyInfoModel>().Result;


                invoiceViewModel.InvoiceDueDate = InvoiceModel.InvoiceDueDate;
                invoiceViewModel.InvoiceDate = InvoiceModel.InvoiceDate;

                HttpResponseMessage responseInvoiceDetailsList = GlobalVeriables.WebApiClient.GetAsync("GetInvoiceDetails/" + id.ToString()).Result;
                List<InvoiceViewModel> InvoiceDetailsList = responseInvoiceDetailsList.Content.ReadAsAsync<List<InvoiceViewModel>>().Result;


                HttpResponseMessage GoodResponse = GlobalVeriables.WebApiClient.GetAsync("APIProduct/" + invoiceViewModel.CompanyId + "/Good").Result;
                List<MVCProductModel> GoodModel = GoodResponse.Content.ReadAsAsync<List<MVCProductModel>>().Result;
                ViewBag.Good = GoodModel;

                HttpResponseMessage Services = GlobalVeriables.WebApiClient.GetAsync("APIProduct/" + invoiceViewModel.CompanyId + "/Services").Result;
                List<MVCProductModel> ServiceModel = Services.Content.ReadAsAsync<List<MVCProductModel>>().Result;
                ViewBag.Service = ServiceModel;


                List<VatModel> model = new List<VatModel>();
                model.Add(new VatModel() { Vat1 = 0, Name = "0" });

                model.Add(new VatModel() { Vat1 = 6, Name = "6" });
                model.Add(new VatModel() { Vat1 = 21, Name = "21" });

                ViewBag.VatDrop = model;

                ViewBag.Contentdata = contectmodel;
                ViewBag.Companydata = companyModel;
                ViewBag.InvoiceData = InvoiceModel;
                ViewBag.InvoiceDetailsList = InvoiceDetailsList;
                ViewBag.definition = "Invoice";

                return View(invoiceViewModel);
            }
            catch(Exception ex)
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
                ViewBag.FILE = CreatDirectoryClass.GetFileDirectiory((int)id, "Invoice");

                HttpResponseMessage responseInvoice = GlobalVeriables.WebApiClient.GetAsync("GetInvoiceById/" + id.ToString()).Result;
                MVCInvoiceModel InvoceModel = responseInvoice.Content.ReadAsAsync<MVCInvoiceModel>().Result;
                
                HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("ApiConatacts/" + InvoceModel.ContactId.ToString()).Result;
                MVCContactModel contectmodel = response.Content.ReadAsAsync<MVCContactModel>().Result;

                HttpResponseMessage responseCompany = GlobalVeriables.WebApiClient.GetAsync("APIComapny/" + InvoceModel.CompanyId.ToString()).Result;
                MVCCompanyInfoModel companyModel = responseCompany.Content.ReadAsAsync<MVCCompanyInfoModel>().Result;
                
                HttpResponseMessage responseInvoiceDetailsList = GlobalVeriables.WebApiClient.GetAsync("GetInvoiceDetails/" + id.ToString()).Result;
                List<InvoiceViewModel> InvoiceDetailsList = responseInvoiceDetailsList.Content.ReadAsAsync<List<InvoiceViewModel>>().Result;

                ViewBag.Contentdata = contectmodel;
                ViewBag.Companydata = companyModel;
                ViewBag.InvoiceData = InvoceModel;
                ViewBag.InvoiceDetailsList = InvoiceDetailsList;
                return View();
            }
            catch (Exception ex)
            {
                return null;
            }
          
        }


        public string PrintView(int id)
        {
            string pdfname;
            try
            {

                HttpResponseMessage responseQutation = GlobalVeriables.WebApiClient.GetAsync("APIQutation/" + id.ToString()).Result;
                MVCQutationModel QutationModel = responseQutation.Content.ReadAsAsync<MVCQutationModel>().Result;


                HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("ApiConatacts/" + QutationModel.ContactId.ToString()).Result;
                MVCContactModel contectmodel = response.Content.ReadAsAsync<MVCContactModel>().Result;

                int companyId = 0;
                if (Session["CompayID"] != null)
                {
                    companyId = Convert.ToInt32(Session["CompayID"]);
                }

                HttpResponseMessage responseCompany = GlobalVeriables.WebApiClient.GetAsync("APIComapny/" + companyId.ToString()).Result;
                MVCCompanyInfoModel companyModel = responseCompany.Content.ReadAsAsync<MVCCompanyInfoModel>().Result;
              

                DateTime qutationDueDate = Convert.ToDateTime(QutationModel.DueDate); //mm/dd/yyyy
                DateTime qutationDate = Convert.ToDateTime(QutationModel.QutationDate);//mm/dd/yyyy
                TimeSpan ts = qutationDueDate.Subtract(qutationDate);
                string diffDate = ts.Days.ToString();


                HttpResponseMessage responseQutationDetailsList = GlobalVeriables.WebApiClient.GetAsync("APIQutationDetails/" + id.ToString()).Result;
                List<MVCQutationViewModel> QutationModelDetailsList = responseQutationDetailsList.Content.ReadAsAsync<List<MVCQutationViewModel>>().Result;

                ViewBag.Contentdata = contectmodel;
                ViewBag.Companydata = companyModel;
                ViewBag.QutationDat = QutationModel;
                ViewBag.QutationDatailsList = QutationModelDetailsList;
                string companyName = id + "-" + companyModel.CompanyName;



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


                var pdfResult = new Rotativa.PartialViewAsPdf("~/Views/Invoice/Printinvoice.cshtml")
                {

                    PageSize = Rotativa.Options.Size.A4,
                    MinimumFontSize = 16,
                    PageMargins = new Rotativa.Options.Margins(10, 12, 20, 3),
                    PageHeight = 40,

                    SaveOnServerPath = path, // Save your place

                    CustomSwitches = "--footer-center \"" + "Wilt u zo vriendelijk zijn om het verschuldigde bedrag binnen " + diffDate + " dagen over te maken naar IBAN:  " + companyModel.IBANNumber + " ten name van IT Molen o.v.v.bovenstaande factuurnummer.  (Op al onze diensten en producten zijn onze algemene voorwaarden van toepassing.Deze kunt u downloaden van onze website.)" + "  Printed date: " +
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

        [HttpGet]
        public ActionResult EmailInvoice(int? id)
        {


            EmailModel email = new EmailModel();
            try
            {

                email.Attachment = PrintView((int)id);

                HttpContext.Items["FilePath"] = email.Attachment;
                
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

                //id = /*Convert.ToInt32(Session["ClientID"]);*/1;

                HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("ApiConatacts/" + 1.ToString()).Result;
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


                email.invoiceId = (int)id;
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
        public ActionResult EmailInvoice(EmailModel email)
        {
            
            var root = Server.MapPath("/PDF/");

            List<AttakmentList> _attackmentList = new List<AttakmentList>();
            var allowedExtensions = new string[] { "doc", "docx", "pdf", ".jpg", "png", "JPEG", "JFIF", "PNG" };

            if (Request.Form["FilePath"] != null)
            {
                var fileName2 = Request.Form["FilePath"];

                string[] valueArray = fileName2.Split(',');

                if (valueArray != null && valueArray.Count() > 0)
                {
                    _attackmentList = new List<AttakmentList>();
                    foreach (var itemm in valueArray)
                    {
                        if (itemm.EndsWith("doc") || itemm.EndsWith("docx") || itemm.EndsWith("jpg") || itemm.EndsWith("png") || itemm.EndsWith("txt"))
                        {
                            _attackmentList.Add(new AttakmentList { Attckment = itemm });
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
                return RedirectToAction("ViewInvoice", new { id = email.invoiceId });
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
        public ActionResult SaveEmail(MVCQutationViewModel MVCQutationViewModel)
        {
           
            MVCQutationModel mvcQutationModel = new MVCQutationModel();
            try
            {
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

                if (mvcQutationModel.TotalVat21 != null)
                {
                    double vat21 = Math.Round((double)mvcQutationModel.TotalVat21, 2, MidpointRounding.AwayFromZero);
                    mvcQutationModel.TotalVat21 = vat21;
                }

                mvcQutationModel.Qutation_ID = MVCQutationViewModel.Qutation_ID;

                HttpResponseMessage response = GlobalVeriables.WebApiClient.PostAsJsonAsync("APIQutation/" + mvcQutationModel.QutationID, mvcQutationModel).Result;
                MVCQutationModel mvcQutationModels = response.Content.ReadAsAsync<MVCQutationModel>().Result;
                if (response.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    foreach (QutationDetailsTable QDTList in MVCQutationViewModel.QutationDetailslist)
                    {
                        QutationDetailsTable QtDetails = new QutationDetailsTable();
                        QtDetails.ItemId = Convert.ToInt32(QDTList.ItemId);
                        QtDetails.QutationID = mvcQutationModels.QutationID;
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
                    
                    return new JsonResult { Data = new { Status = "Success", path = "", QutationId = mvcQutationModels.QutationID } };

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