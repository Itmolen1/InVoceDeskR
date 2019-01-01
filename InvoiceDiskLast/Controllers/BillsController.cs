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
    public class BillsController : Controller
    {
        // GET: Bills
        public ActionResult Index()
        {
            return View();
        }


       

        [HttpGet]
        public ActionResult Create(int id)
        {
            MvcPurchaseViewModel purchaseviewModel = new MvcPurchaseViewModel();
            try
            {
                //int  Contectid = Convert.ToInt32(Session["ClientId"]);
                int   CompanyID = Convert.ToInt32(Session["CompayID"]);
              
                HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("ApiConatacts/" + id.ToString()).Result;
                MVCContactModel contectmodel = response.Content.ReadAsAsync<MVCContactModel>().Result;

                HttpResponseMessage responseCompany = GlobalVeriables.WebApiClient.GetAsync("APIComapny/" + CompanyID.ToString()).Result;
                MVCCompanyInfoModel companyModel = responseCompany.Content.ReadAsAsync<MVCCompanyInfoModel>().Result;

                ViewBag.Contentdata = contectmodel;
                ViewBag.Companydata = companyModel;
                DateTime InvoiceDate = new DateTime();
                InvoiceDate = DateTime.Now;
                purchaseviewModel.PurchaseDate = InvoiceDate;
                purchaseviewModel.PurchaseDueDate = InvoiceDate.AddDays(+15);

                MvcPurchaseModel q = new MvcPurchaseModel();
                HttpResponseMessage response1 = GlobalVeriables.WebApiClient.GetAsync("GenrateInvoice/").Result;
                q = response1.Content.ReadAsAsync<MvcPurchaseModel>().Result;
                purchaseviewModel.PurchaseDate = InvoiceDate;
                purchaseviewModel.PurchaseDueDate = InvoiceDate.AddDays(+15);
                purchaseviewModel.Purchase_ID = q.PurchaseID;

                return View(purchaseviewModel);
            }
            catch (Exception ex)
            {
                return null;
            }


        }

        [HttpPost]
        public ActionResult save1(MvcPurchaseViewModel purchaseViewModel)
        {
            PurchaseOrderTable purchasemodel = new PurchaseOrderTable();
           
            try
            {

                purchasemodel.VenderId = purchaseViewModel.VenderId;
                purchasemodel.CompanyId = purchaseViewModel.CompanyId;
                purchasemodel.UserId = 1;
                purchasemodel.PurchaseID = purchaseViewModel.PurchaseId.ToString();
                purchasemodel.PurchaseOrderID = (Convert.ToInt32(purchaseViewModel.PurchaseOrderID != null ? purchaseViewModel.PurchaseOrderID : 0));
                purchasemodel.PurchaseRefNumber = purchaseViewModel.PurchaseRefNumber;
                purchasemodel.PurchaseDate = (DateTime)purchaseViewModel.PurchaseDate;
                purchasemodel.PurchaseDueDate = purchaseViewModel.PurchaseDueDate;
                purchasemodel.PurchaseSubTotal = purchaseViewModel.PurchaseSubTotal;
                purchasemodel.PurchaseDiscountAmount = purchaseViewModel.PurchaseDiscountAmount;
                purchasemodel.PurchaseTotoalAmount = purchaseViewModel.PurchaseTotoalAmount;
                purchasemodel.PurchaseVenderNote = purchaseViewModel.PurchaseVenderNote;
                purchasemodel.Vat6 = purchaseViewModel.Vat6;

               
                purchasemodel.Vat21 = purchaseViewModel.Vat21;
                purchasemodel.Status = "accepted";
                purchasemodel.Type = StatusEnum.Goods.ToString();


                if (purchaseViewModel.PurchaseOrderID == 0 || purchaseViewModel.PurchaseOrderID == null)
                {

                    HttpResponseMessage response = GlobalVeriables.WebApiClient.PostAsJsonAsync("APIPurchase", purchasemodel).Result;
                    purchasemodel = response.Content.ReadAsAsync<PurchaseOrderTable>().Result;

                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        purchaseViewModel.PurchaseOrderID = purchasemodel.PurchaseOrderID;

                        foreach (var item in purchaseViewModel.PurchaseDetailslist)
                        {
                            PurchaseOrderDetailsTable purchadeDetail = new PurchaseOrderDetailsTable();
                            purchadeDetail.PurchaseOrderDetailsId = item.PurchaseOrderDetailsId;
                            purchadeDetail.PurchaseItemId = item.PurchaseItemId;
                            purchadeDetail.PurchaseDescription = item.PurchaseDescription;
                            purchadeDetail.PurchaseQuantity = item.PurchaseQuantity;
                            purchadeDetail.PurchaseItemRate = item.PurchaseItemRate;
                            purchadeDetail.PurchaseTotal = item.PurchaseTotal;
                            purchadeDetail.Type = item.Type;
                            purchadeDetail.RowSubTotal = item.RowSubTotal;
                            purchadeDetail.PurchaseVatPercentage = item.PurchaseVatPercentage;
                            purchadeDetail.PurchaseId = purchaseViewModel.PurchaseOrderID;
                            purchadeDetail.ServiceDate = item.ServiceDate;
                            purchadeDetail.PurchaseId = purchasemodel.PurchaseOrderID;
                            HttpResponseMessage responsses = GlobalVeriables.WebApiClient.PostAsJsonAsync("APIPurchaseDetail", purchadeDetail).Result;
                        }

                        return new JsonResult { Data = new { Status = "Success", purchaseId = purchasemodel.PurchaseOrderID } };
                    }
                    else
                    {
                        return new JsonResult { Data = new { Status = "Fail", Message = "Error while adding purchase" } };
                    }
                }
            }
            catch (Exception ex)
            {

                return new JsonResult { Data = new { Status = "Fail", Message = ex.Message.ToString() } };
            }

            return new JsonResult { Data = new { Status = "Success", purchaseId = purchasemodel.PurchaseOrderID } };
        }


        public ActionResult ViewBill(int? id)
        {
            try
            {

                HttpResponseMessage res = GlobalVeriables.WebApiClient.GetAsync("APIPurchase/" + id.ToString()).Result;
                MvcPurchaseModel ob = res.Content.ReadAsAsync<MvcPurchaseModel>().Result;

                int? Contectid = ob.VenderId;
                int? CompanyID = ob.CompanyId;

                HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("ApiConatacts/" + Contectid.ToString()).Result;
                MVCContactModel contectmodel = response.Content.ReadAsAsync<MVCContactModel>().Result;

                HttpResponseMessage responseCompany = GlobalVeriables.WebApiClient.GetAsync("APIComapny/" + CompanyID.ToString()).Result;
                MVCCompanyInfoModel companyModel = responseCompany.Content.ReadAsAsync<MVCCompanyInfoModel>().Result;
                
                HttpResponseMessage responseQutationDetailsList = GlobalVeriables.WebApiClient.GetAsync("APIPurchaseDetail/" + id.ToString()).Result;
                List<MvcPurchaseViewModel> PuchaseModelDetailsList = responseQutationDetailsList.Content.ReadAsAsync<List<MvcPurchaseViewModel>>().Result;


                ViewBag.Contentdata = contectmodel;
                ViewBag.Companydata = companyModel;
                ViewBag.Purchase = ob;
                ViewBag.PurchaseDatailsList = PuchaseModelDetailsList;
            }
            catch (Exception ex)
            {
            }
            return View();
        }

        public ActionResult Print(int id)
        {
            try
            {
                HttpResponseMessage res = GlobalVeriables.WebApiClient.GetAsync("APIPurchase/" + id.ToString()).Result;
                MvcPurchaseModel ob = res.Content.ReadAsAsync<MvcPurchaseModel>().Result;

                int? Contectid = ob.VenderId;
                int? CompanyID = ob.CompanyId;

                HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("ApiConatacts/" + Contectid.ToString()).Result;
                MVCContactModel contectmodel = response.Content.ReadAsAsync<MVCContactModel>().Result;

                HttpResponseMessage responseCompany = GlobalVeriables.WebApiClient.GetAsync("APIComapny/" + CompanyID.ToString()).Result;
                MVCCompanyInfoModel companyModel = responseCompany.Content.ReadAsAsync<MVCCompanyInfoModel>().Result;

                HttpResponseMessage responseQutationDetailsList = GlobalVeriables.WebApiClient.GetAsync("APIPurchaseDetail/" + id.ToString()).Result;
                List<MvcPurchaseViewModel> PuchaseModelDetailsList = responseQutationDetailsList.Content.ReadAsAsync<List<MvcPurchaseViewModel>>().Result;

                DateTime PurchaseDueDate = Convert.ToDateTime(ob.PurchaseDueDate); //mm/dd/yyyy
                DateTime PurchaseDate = Convert.ToDateTime(ob.PurchaseDate);//mm/dd/yyyy
                TimeSpan ts = PurchaseDueDate.Subtract(PurchaseDate);
                string diffDate = ts.Days.ToString();

                ViewBag.Contentdata = contectmodel;
                ViewBag.Companydata = companyModel;
                ViewBag.Purchase = ob;
                ViewBag.PurchaseDatailsList = PuchaseModelDetailsList;

                string PdfName = id + "-" + companyModel.CompanyName + ".pdf";

                string cutomswitches = "";
                cutomswitches = "--footer-center \"" + "Wilt u zo vriendelijk zijn om het verschuldigde bedrag binnen " + diffDate + " dagen over te maken naar IBAN: \n " + companyModel.IBANNumber + " ten name van IT Molen o.v.v.bovenstaande factuurnummer. \n (Op al onze diensten en producten zijn onze algemene voorwaarden van toepassing.Deze kunt u downloaden van onze website.)" + " \n Printed date: " +
                   DateTime.Now.Date.ToString("MM/dd/yyyy") + "  Page: [page]/[toPage]\"" +
                  " --footer-line --footer-font-size \"10\" --footer-spacing 6 --footer-font-name \"calibri light\"";

                return new Rotativa.PartialViewAsPdf("~/Views/Bills/Viewapp.cshtml")
                {
                    PageSize = Rotativa.Options.Size.A4,
                    MinimumFontSize = 16,

                    FileName = PdfName,

                    PageHeight = 40,
                    CustomSwitches = cutomswitches,
                    PageMargins = new Rotativa.Options.Margins(10, 12, 20, 3)
                };
            }
            catch (Exception ex)
            {
                return null;

            }
        }

        [NonAction]
        public List<VatModel> GetVatList()
        {
            List<VatModel> model = new List<VatModel>();
            model.Add(new VatModel() { Vat1 = 0, Name = "0" });
            model.Add(new VatModel() { Vat1 = 6, Name = "6" });
            model.Add(new VatModel() { Vat1 = 21, Name = "21" });
            return model;
        }

        public ActionResult Edit(int Id)
        {
            MvcPurchaseViewModel purchaseviewModel = new MvcPurchaseViewModel();

            try
            {
                ViewBag.VatDrop = GetVatList();

                HttpResponseMessage res = GlobalVeriables.WebApiClient.GetAsync("APIPurchase/" + Id.ToString()).Result;
                MvcPurchaseModel ob = res.Content.ReadAsAsync<MvcPurchaseModel>().Result;

                int? contactid = ob.VenderId;
                int? CompanyId = ob.CompanyId;

                HttpResponseMessage responsep = GlobalVeriables.WebApiClient.GetAsync("APIProduct/" + CompanyId + "/All").Result;
                List<MVCProductModel> productModel = responsep.Content.ReadAsAsync<List<MVCProductModel>>().Result;
                ViewBag.Product = productModel;

                HttpResponseMessage GoodResponse = GlobalVeriables.WebApiClient.GetAsync("APIProduct/" + CompanyId + "/Good").Result;
                List<MVCProductModel> GoodModel = GoodResponse.Content.ReadAsAsync<List<MVCProductModel>>().Result;
                ViewBag.Good = GoodModel;

                HttpResponseMessage Services = GlobalVeriables.WebApiClient.GetAsync("APIProduct/" + CompanyId + "/Services").Result;
                List<MVCProductModel> ServiceModel = Services.Content.ReadAsAsync<List<MVCProductModel>>().Result;
                ViewBag.Service = ServiceModel;

                HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("ApiConatacts/" + contactid.ToString()).Result;
                MVCContactModel contectmodel = response.Content.ReadAsAsync<MVCContactModel>().Result;

                HttpResponseMessage responseCompany = GlobalVeriables.WebApiClient.GetAsync("APIComapny/" + CompanyId.ToString()).Result;
                MVCCompanyInfoModel companyModel = responseCompany.Content.ReadAsAsync<MVCCompanyInfoModel>().Result;

              
                purchaseviewModel.PurchaseOrderID = ob.PurchaseOrderID;
                purchaseviewModel.Purchase_ID = ob.PurchaseID;
                purchaseviewModel.PurchaseDate = Convert.ToDateTime(ob.PurchaseDate);
                purchaseviewModel.PurchaseDueDate = (DateTime)ob.PurchaseDueDate;
                purchaseviewModel.PurchaseRefNumber = ob.PurchaseRefNumber;
                purchaseviewModel.PurchaseSubTotal = ob.PurchaseSubTotal;
                purchaseviewModel.PurchaseDiscountPercenteage = ob.PurchaseDiscountPercenteage;
                purchaseviewModel.PurchaseDiscountAmount = ob.PurchaseDiscountAmount;
                purchaseviewModel.PurchaseVatPercentage = ob.PurchaseVatPercentage;
                purchaseviewModel.PurchaseTotoalAmount = ob.PurchaseTotoalAmount;
                purchaseviewModel.PurchaseVenderNote = ob.PurchaseVenderNote;
                purchaseviewModel.Status = ob.Status;
                purchaseviewModel.Vat21 = (int)ob.Vat21;
                purchaseviewModel.Vat6 = (int)ob.Vat6;
                purchaseviewModel.CompanyId = ob.CompanyId;
                purchaseviewModel.UserId = ob.UserId;

                HttpResponseMessage responseQutationDetailsList = GlobalVeriables.WebApiClient.GetAsync("APIPurchaseDetail/" + Id.ToString()).Result;
                List<MvcPurchaseViewModel> QutationModelDetailsList = responseQutationDetailsList.Content.ReadAsAsync<List<MvcPurchaseViewModel>>().Result;
                ViewBag.Contentdata = contectmodel;
                ViewBag.Companydata = companyModel;
                ViewBag.Purchase = ob;
                ViewBag.PurchaseDatailsList = QutationModelDetailsList;

                return View(purchaseviewModel);
            }
            catch (Exception)
            {

                throw;
            }

        }

        [HttpPost]
        public ActionResult Edit(MvcPurchaseViewModel purchaseViewModel)
        {


            PurchaseOrderTable purchasemodel = new PurchaseOrderTable();
            try
            {
               
                purchasemodel.CompanyId = purchasemodel.CompanyId;
                purchasemodel.VenderId = purchaseViewModel.VenderId;
                purchasemodel.UserId = 1;
                purchasemodel.PurchaseID = purchaseViewModel.PurchaseId.ToString();

                purchasemodel.PurchaseOrderID = (Convert.ToInt32(purchaseViewModel.PurchaseOrderID != null ? purchaseViewModel.PurchaseOrderID : 0));
                purchasemodel.PurchaseRefNumber = purchaseViewModel.PurchaseRefNumber;

                purchasemodel.PurchaseDate = (DateTime)purchaseViewModel.PurchaseDate;
                purchasemodel.PurchaseDueDate = purchaseViewModel.PurchaseDueDate;
                purchasemodel.PurchaseSubTotal = purchaseViewModel.PurchaseSubTotal;
                purchasemodel.PurchaseDiscountAmount = purchaseViewModel.PurchaseDiscountAmount;
                purchasemodel.PurchaseTotoalAmount = purchaseViewModel.PurchaseTotoalAmount;
                purchasemodel.PurchaseVenderNote = purchaseViewModel.PurchaseVenderNote;
                purchasemodel.Vat6 = purchaseViewModel.Vat6;                
                purchasemodel.Vat21 = purchaseViewModel.Vat21;
                purchasemodel.Status = "accepted";
                purchasemodel.Type = StatusEnum.Goods.ToString();

                HttpResponseMessage response = GlobalVeriables.WebApiClient.PutAsJsonAsync("APIPurchase/" + purchasemodel.PurchaseOrderID, purchasemodel).Result;

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    foreach (var item in purchaseViewModel.PurchaseDetailslist)
                    {
                        PurchaseOrderDetailsTable purchadeDetail = new PurchaseOrderDetailsTable();
                        purchadeDetail.PurchaseOrderDetailsId = item.PurchaseOrderDetailsId;
                        purchadeDetail.PurchaseItemId = item.PurchaseItemId;
                        purchadeDetail.PurchaseDescription = item.PurchaseDescription;
                        purchadeDetail.PurchaseQuantity = item.PurchaseQuantity;
                        purchadeDetail.PurchaseItemRate = item.PurchaseItemRate;
                        purchadeDetail.Type = item.Type;
                        purchadeDetail.RowSubTotal = item.RowSubTotal;
                        purchadeDetail.PurchaseDescription = item.PurchaseDescription;                       
                        purchadeDetail.ServiceDate = item.ServiceDate;                       
                        purchadeDetail.PurchaseTotal = item.PurchaseTotal;
                        purchadeDetail.PurchaseVatPercentage = item.PurchaseVatPercentage;
                        purchadeDetail.PurchaseId = purchaseViewModel.PurchaseOrderID;

                        if (purchadeDetail.PurchaseOrderDetailsId == 0)
                        {
                            HttpResponseMessage responsses = GlobalVeriables.WebApiClient.PostAsJsonAsync("APIPurchaseDetail", purchadeDetail).Result;
                        }
                        else
                        {
                            HttpResponseMessage responsses = GlobalVeriables.WebApiClient.PutAsJsonAsync("APIPurchaseDetail/" + purchadeDetail.PurchaseOrderDetailsId, purchadeDetail).Result;
                        }
                    }

                    return new JsonResult { Data = new { Status = "Success", purchaseId = purchasemodel.PurchaseOrderID } };

                }
            }
            catch (Exception ex)
            {
                return new JsonResult { Data = new { Status = "Fail", Message = ex.Message.ToString() } };
            }
            return View();
        }

        public string PrintView(int id)
        {
           
            string pdfname;
            try
            {
                HttpResponseMessage res = GlobalVeriables.WebApiClient.GetAsync("APIPurchase/" + id.ToString()).Result;
                MvcPurchaseModel ob = res.Content.ReadAsAsync<MvcPurchaseModel>().Result;

                int? Contectid = ob.VenderId;
                int? CompanyID = ob.CompanyId;               

                HttpResponseMessage responsep = GlobalVeriables.WebApiClient.GetAsync("APIProduct/" + CompanyID + "/All").Result;
                List<MVCProductModel> productModel = responsep.Content.ReadAsAsync<List<MVCProductModel>>().Result;
                ViewBag.Product = productModel;

                HttpResponseMessage GoodResponse = GlobalVeriables.WebApiClient.GetAsync("APIProduct/" + CompanyID + "/Good").Result;
                List<MVCProductModel> GoodModel = GoodResponse.Content.ReadAsAsync<List<MVCProductModel>>().Result;
                ViewBag.Good = GoodModel;

                HttpResponseMessage Services = GlobalVeriables.WebApiClient.GetAsync("APIProduct/" + CompanyID + "/Services").Result;
                List<MVCProductModel> ServiceModel = Services.Content.ReadAsAsync<List<MVCProductModel>>().Result;
                ViewBag.Service = ServiceModel;

                HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("ApiConatacts/" + Contectid.ToString()).Result;
                MVCContactModel contectmodel = response.Content.ReadAsAsync<MVCContactModel>().Result;

                HttpResponseMessage responseCompany = GlobalVeriables.WebApiClient.GetAsync("APIComapny/" + CompanyID.ToString()).Result;
                MVCCompanyInfoModel companyModel = responseCompany.Content.ReadAsAsync<MVCCompanyInfoModel>().Result;

                HttpResponseMessage responseQutationDetailsList = GlobalVeriables.WebApiClient.GetAsync("APIPurchaseDetail/" + id.ToString()).Result;
                List<MvcPurchaseViewModel> QutationModelDetailsList = responseQutationDetailsList.Content.ReadAsAsync<List<MvcPurchaseViewModel>>().Result;
                ViewBag.Contentdata = contectmodel;
                ViewBag.Companydata = companyModel;
                ViewBag.Purchase = ob;
                ViewBag.PurchaseDatailsList = QutationModelDetailsList;

                DateTime PurchaseDueDate = Convert.ToDateTime(ob.PurchaseDueDate); //mm/dd/yyyy
                DateTime PurchaseDate = Convert.ToDateTime(ob.PurchaseDate);//mm/dd/yyyy
                TimeSpan ts = PurchaseDueDate.Subtract(PurchaseDate);
                string diffDate = ts.Days.ToString();

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

                var pdfResult = new Rotativa.PartialViewAsPdf("~/Views/Bills/PrintPartialView.cshtml")
                {

                    PageSize = Rotativa.Options.Size.A4,
                    MinimumFontSize = 16,
                    PageMargins = new Rotativa.Options.Margins(10, 12, 20, 3),
                    PageHeight = 40,
                    SaveOnServerPath = path,

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

        public ActionResult EmailBill(int Id)
        {

            EmailModel email = new EmailModel();
            try
            {
                email.Attachment = PrintView(Id);
                HttpContext.Items["FilePath"] = email.Attachment;

                if (Session["CompayID"] == null)
                {
                    return RedirectToAction("Index", "Login");
                }

                var CompanyName = Session["CompanyName"];
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


                if (CompanyName == null)
                {
                    CompanyName = "Nocompany";
                }
                int id = 0;
                if (Session["ClientID"] != null)
                {
                    id = Convert.ToInt32(Session["ClientID"]);
                }


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
                string result = strToProcess.Replace(".", " \r\n");

                email.EmailText = result;


                email.invoiceId = Id;
                email.From = "infouurtjefactuur@gmail.com";

            }
            catch (Exception)
            {

                throw;
            }

            return View(email);
        }


        [HttpPost]
        public ActionResult EmailBill(EmailModel email)
        {
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

                    var root = Server.MapPath("/PDF/");
                    var pdfname = String.Format("{0}.pdf", p);
                    var path = Path.Combine(root, pdfname);
                    email.Attachment = path;
                    string[] EmailArray = email.ToEmail.Split(',');
                    _attackmentList.Add(new AttakmentList { Attckment = email.Attachment });

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

                    _attackmentList.Add(new AttakmentList { Attckment = path });

                    emailModel.From = email.From;
                    emailModel.ToEmail = email.ToEmail;
                    emailModel.Attachment = email.Attachment;
                    emailModel.EmailBody = email.EmailText;
                    bool result = EmailController.email(emailModel, _attackmentList);
                    TempData["EmailMessge"] = "Email Send successfully";
                }

                var folderPath = Server.MapPath("/PDF/");

                EmailController.clearFolder(folderPath);

                return RedirectToAction("ViewBill", new { id = email.invoiceId });
            }
            catch (Exception ex)
            {
                email.Attachment = fileName;
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
    }
}