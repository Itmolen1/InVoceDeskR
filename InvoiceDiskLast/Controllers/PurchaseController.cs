using CrystalDecisions.CrystalReports.Engine;
using InvoiceDiskLast.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace InvoiceDiskLast.Controllers
{

    [SessionExpireAttribute]
    public class PurchaseController : Controller
    {


        MVCCompanyInfoModel companyInfo = new MVCCompanyInfoModel();
        MVCContactModel mvcContactModel = new MVCContactModel();

        // GET: Purchase
        public ActionResult Index()
        {

            return View();
        }

        [HttpPost]
        public JsonResult GetPurchaseList()
        {
            IEnumerable<MvcPurchaseModel> PurchaseList;
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

                GlobalVeriables.WebApiClient.DefaultRequestHeaders.Clear();
                int CompanyId = Convert.ToInt32(Session["CompayID"]);
                GlobalVeriables.WebApiClient.DefaultRequestHeaders.Add("CompayID", CompanyId.ToString());

                int companyId = Convert.ToInt32(Session["CompayID"]);

                HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("GetPurchaseInvoiceList/" + StatusEnum.Goods + "/" + companyId).Result;
                PurchaseList = response.Content.ReadAsAsync<IEnumerable<MvcPurchaseModel>>().Result;


                if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
                {
                    PurchaseList = PurchaseList.Where(p => p.PurchaseOrderID.ToString().Contains(search)
                  || p.PurchaseRefNumber != null && p.PurchaseRefNumber.ToLower().Contains(search.ToLower())
                  || p.PurchaseDate != null && p.PurchaseDate.ToString().ToLower().Contains(search.ToLower())
                  || p.PurchaseDueDate != null && p.PurchaseDueDate.ToString().ToLower().Contains(search.ToLower())
                  || p.Status != null && p.Status.ToString().ToLower().Contains(search.ToLower())
                  || p.PurchaseTotoalAmount != null && p.PurchaseTotoalAmount.ToString().ToLower().Contains(search.ToLower())
                  || p.Status != null && p.Status.ToString().ToLower().Contains(search.ToLower())).ToList();
                }

                int recordsTotal = recordsTotal = PurchaseList.Count();
                var data = PurchaseList.Skip(skip).Take(pageSize).ToList();
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Response.Write(ex.ToString());
                return Json(new { draw = 0, recordsFiltered = 0, recordsTotal = 0, data = 0 }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { draw = 0, recordsFiltered = 0, recordsTotal = 0, data = 0 }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult GetVender()
        {
            var ProductList = new List<MVCContactModel>();
            try
            {
                int CompanyId = Convert.ToInt32(Session["CompayID"]);
                GlobalVeriables.WebApiClient.DefaultRequestHeaders.Clear();
                GlobalVeriables.WebApiClient.DefaultRequestHeaders.Add("CompayID", CompanyId.ToString());

                //GlobalVeriables.WebApiClient.DefaultRequestHeaders.Remove("CustomerStatus");

                // GlobalVeriables.WebApiClient.DefaultRequestHeaders.Add("CustomerStatus", "Vender");


                HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("ApiConatacts/" + CompanyId + "/Vender").Result;
                //HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("ApiConatacts").Result;
                ProductList = response.Content.ReadAsAsync<List<MVCContactModel>>().Result;

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return Json(ProductList, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception)
            {

                throw;
            }

            return Json(ProductList, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult savePrintAndSentItToYouronsave(MvcPurchaseViewModel purchaseViewModel)
        {
            var purchaseorderId = "";
            int intpurchaseorderId = 0;

            PurchaseOrderTable purchasemodel = new PurchaseOrderTable();
            try
            {

                purchasemodel.CompanyId = purchaseViewModel.CompanyId;
                purchasemodel.VenderId = purchaseViewModel.VenderId;
                purchasemodel.UserId = Convert.ToInt32(Session["LoginUserID"]);
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
                purchasemodel.Status = "open";
                purchasemodel.Type = StatusEnum.Goods.ToString();
                if (purchaseViewModel.PurchaseOrderID == 0)
                {

                    HttpResponseMessage response = GlobalVeriables.WebApiClient.PostAsJsonAsync("APIPurchase", purchasemodel).Result;
                    IEnumerable<string> headerValues;
                    var userId = string.Empty;
                    if (response.Headers.TryGetValues("idd", out headerValues))
                    {
                        purchaseorderId = headerValues.FirstOrDefault();
                    }
                    intpurchaseorderId = Convert.ToInt32(purchaseorderId);


                    if (response.StatusCode == System.Net.HttpStatusCode.Created)
                    {
                        foreach (var item in purchaseViewModel.PurchaseDetailslist)
                        {
                            PurchaseOrderDetailsTable purchadeDetail = new PurchaseOrderDetailsTable();
                            purchadeDetail.PurchaseOrderDetailsId = item.PurchaseOrderDetailsId;
                            purchadeDetail.PurchaseItemId = item.PurchaseItemId;
                            purchadeDetail.PurchaseDescription = item.PurchaseDescription;
                            purchadeDetail.PurchaseQuantity = item.PurchaseQuantity;
                            purchadeDetail.PurchaseItemRate = item.PurchaseItemRate;

                            purchadeDetail.PurchaseTotal = item.PurchaseTotal;
                            purchadeDetail.PurchaseVatPercentage = item.PurchaseVatPercentage;
                            purchadeDetail.PurchaseId = purchaseViewModel.PurchaseOrderID;
                            if (purchadeDetail.PurchaseOrderDetailsId == 0)
                            {
                                purchadeDetail.PurchaseId = intpurchaseorderId;

                                intpurchaseorderId = Convert.ToInt32(purchaseorderId);
                                HttpResponseMessage responsses = GlobalVeriables.WebApiClient.PostAsJsonAsync("APIPurchaseDetail", purchadeDetail).Result;
                            }
                            else
                            {
                                HttpResponseMessage responsses = GlobalVeriables.WebApiClient.PutAsJsonAsync("APIPurchaseDetail/" + purchadeDetail.PurchaseOrderDetailsId, purchadeDetail).Result;
                            }
                        }
                    }
                }

                else
                {
                    if (purchaseViewModel.PurchaseOrderID != 0)
                    {
                        purchasemodel.PurchaseID = purchaseViewModel.Purchase_ID;
                        intpurchaseorderId = Convert.ToInt32(purchaseViewModel.PurchaseOrderID);
                        HttpResponseMessage response2 = GlobalVeriables.WebApiClient.PutAsJsonAsync("APIPurchase/" + purchasemodel.PurchaseOrderID, purchasemodel).Result;
                        intpurchaseorderId = (int)purchaseViewModel.PurchaseOrderID;
                        if (response2.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            foreach (var item in purchaseViewModel.PurchaseDetailslist)
                            {
                                PurchaseOrderDetailsTable purchadeDetail = new PurchaseOrderDetailsTable();
                                purchadeDetail.PurchaseOrderDetailsId = item.PurchaseOrderDetailsId;
                                purchadeDetail.PurchaseItemId = item.PurchaseItemId;
                                purchadeDetail.PurchaseDescription = item.PurchaseDescription;
                                purchadeDetail.PurchaseQuantity = item.PurchaseQuantity;
                                purchadeDetail.PurchaseItemRate = item.PurchaseItemRate;
                                purchadeDetail.PurchaseTotal = item.PurchaseTotal;
                                purchadeDetail.PurchaseVatPercentage = item.PurchaseVatPercentage;
                                purchadeDetail.PurchaseId = purchaseViewModel.PurchaseOrderID;
                                if (purchadeDetail.PurchaseOrderDetailsId == 0)
                                {
                                    purchadeDetail.PurchaseId = intpurchaseorderId;
                                    HttpResponseMessage responsses = GlobalVeriables.WebApiClient.PostAsJsonAsync("APIPurchaseDetail", purchadeDetail).Result;
                                }
                                else
                                {
                                    HttpResponseMessage responsses = GlobalVeriables.WebApiClient.PutAsJsonAsync("APIPurchaseDetail/" + purchadeDetail.PurchaseOrderDetailsId, purchadeDetail).Result;
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
            string path1 = PrintView(intpurchaseorderId);
            var root = Server.MapPath("/PDF/");
            var pdfname = String.Format("{0}", path1);
            var path = Path.Combine(root, pdfname);
            path = Path.GetFullPath(path);
            //DownloadFile(path);
            return new JsonResult { Data = new { Status = "Success", path = path, PurchaseOrderId = intpurchaseorderId } };
        }

        public JsonResult AddOrEditPurchase(int purchaseId)
        {
            PurchaseOrderTable m = new Models.PurchaseOrderTable();
            HttpResponseMessage response1 = GlobalVeriables.WebApiClient.GetAsync("APIPurchase/" + purchaseId.ToString()).Result;
            m = response1.Content.ReadAsAsync<PurchaseOrderTable>().Result;
            Session["ClientId"] = m.VenderId;
            return Json("", JsonRequestBehavior.AllowGet);

        }

        int Contectid, CompanyID;


        [HttpPost]
        public ActionResult saveEmailEdit(MvcPurchaseViewModel purchaseViewModel)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            PurchaseOrderTable purchasemodel = new PurchaseOrderTable();
            try
            {
                PurchaseOrderTable p = db.PurchaseOrderTables.Find(purchaseViewModel.PurchaseOrderID);

                if (p.PurchaseTotoalAmount != purchaseViewModel.PurchaseTotoalAmount)
                {
                    purchasemodel.PurchaseID = purchaseViewModel.PurchaseId.ToString();
                    purchasemodel.UserId = Convert.ToInt32(Session["LoginUserID"]);

                    response = GlobalVeriables.WebApiClient.PutAsJsonAsync("APIPurchase/" + purchasemodel.PurchaseOrderID, purchaseViewModel).Result;
                    MvcPurchaseModel _PurchaseModel = response.Content.ReadAsAsync<MvcPurchaseModel>().Result;

                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        return new JsonResult { Data = new { Status = "Success", PurchaseOrderId = purchaseViewModel.PurchaseOrderID } };
                    }
                    else
                    {
                        return new JsonResult { Data = new { Status = "Fail", PurchaseOrderId = purchaseViewModel.PurchaseOrderID } };
                    }
                }
                else
                {
                    return new JsonResult { Data = new { Status = "Success", PurchaseOrderId = purchaseViewModel.PurchaseOrderID } };
                }
            }
            catch (Exception ex)
            {
                return new JsonResult { Data = new { Status = "Fail", Message = ex.Message.ToString() } };
                throw ex;
            }

        }

        [HttpPost]
        public ActionResult saveEmailPrint(MvcPurchaseViewModel purchaseViewModel)
        {
            PurchaseOrderTable purchasemodel = new PurchaseOrderTable();
            try
            {
                purchasemodel.CompanyId = purchaseViewModel.CompanyId;
                purchasemodel.UserId = Convert.ToInt32(Session["LoginUserID"]);
                purchasemodel.PurchaseID = purchaseViewModel.PurchaseId.ToString();
                purchasemodel.VenderId = purchaseViewModel.VenderId;
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
                purchasemodel.Status = "open";
                purchasemodel.Type = StatusEnum.Goods.ToString();

                HttpResponseMessage response = GlobalVeriables.WebApiClient.PostAsJsonAsync("APIPurchase", purchasemodel).Result;
                PurchaseOrderTable Purchasetable = response.Content.ReadAsAsync<PurchaseOrderTable>().Result;

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    purchaseViewModel.PurchaseOrderID = Purchasetable.PurchaseOrderID;

                    foreach (PurchaseOrderDetailsTable item in purchaseViewModel.PurchaseOrderList)
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
                        purchadeDetail.PurchaseId = purchaseViewModel.PurchaseOrderID;
                        HttpResponseMessage responsses = GlobalVeriables.WebApiClient.PostAsJsonAsync("APIPurchaseDetail", purchadeDetail).Result;
                    }

                }
            }
            catch (Exception ex)
            {
                return new JsonResult { Data = new { Status = "Fail", Message = ex.Message.ToString() } };
            }
            return new JsonResult { Data = new { Status = "Success", PurchaseOrderId = purchaseViewModel.PurchaseOrderID } };

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

                string QuationId = arrya[1];
                string PdfName = QuationId + "-" + companyModel.CompanyName + ".pdf";

                return PdfName;
            }
            catch (Exception)
            {

                throw;
            }

        }

        [HttpPost]
        public ActionResult DeleteFile(int Id, string FileName)
        {
            try
            {


                if (CreatDirectoryClass.Delete(Id, FileName, "Purchase"))
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

        [HttpGet]
        public ActionResult deleteFile(string FileName)
        {
            try
            {
                var root = Server.MapPath("/PDF/");
                var path = Path.Combine(root, FileName);
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [DeleteFileClass]
        [HttpPost]
        public FileResult DownloadFile(string FilePath1)
        {
            string filepath = "";
            string FileName = FilePath1;
            try
            {
                filepath = System.IO.Path.Combine(Server.MapPath("/PDF/"), FilePath1);
                HttpContext.Items["FilePath"] = FilePath1;

            }
            catch (Exception)
            {
            }

            return File(filepath, MimeMapping.GetMimeMapping(filepath), FilePath1);
        }

        [AllowAnonymous]
        public ActionResult Footer()
        {
            return View();
        }

        public ActionResult Print(int? purchaseOrderId)
        {
            try
            {
                HttpResponseMessage res = GlobalVeriables.WebApiClient.GetAsync("APIPurchase/" + purchaseOrderId.ToString()).Result;
                MvcPurchaseModel ob = res.Content.ReadAsAsync<MvcPurchaseModel>().Result;

                HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("ApiConatacts/" + ob.VenderId.ToString()).Result;
                MVCContactModel contectmodel = response.Content.ReadAsAsync<MVCContactModel>().Result;

                HttpResponseMessage responseCompany = GlobalVeriables.WebApiClient.GetAsync("APIComapny/" + ob.CompanyId.ToString()).Result;
                MVCCompanyInfoModel companyModel = responseCompany.Content.ReadAsAsync<MVCCompanyInfoModel>().Result;



                GlobalVeriables.WebApiClient.DefaultRequestHeaders.Clear();

                HttpResponseMessage responseQutationDetailsList = GlobalVeriables.WebApiClient.GetAsync("APIPurchaseDetail/" + purchaseOrderId.ToString()).Result;
                List<MvcPurchaseViewModel> PuchaseModelDetailsList = responseQutationDetailsList.Content.ReadAsAsync<List<MvcPurchaseViewModel>>().Result;

                DateTime PurchaseDueDate = Convert.ToDateTime(ob.PurchaseDueDate); //mm/dd/yyyy
                DateTime PurchaseDate = Convert.ToDateTime(ob.PurchaseDate);//mm/dd/yyyy
                TimeSpan ts = PurchaseDueDate.Subtract(PurchaseDate);
                string diffDate = ts.Days.ToString();

                ViewBag.Contentdata = contectmodel;
                ViewBag.Companydata = companyModel;
                ViewBag.Purchase = ob;
                ViewBag.PurchaseDatailsList = PuchaseModelDetailsList;

                string PdfName = purchaseOrderId + "-" + companyModel.CompanyName + ".pdf";


                string cutomswitches = "";
                cutomswitches = "--footer-center \"" + "Wilt u zo vriendelijk zijn om het verschuldigde bedrag binnen " + diffDate + " dagen over te maken naar IBAN: \n " + companyModel.IBANNumber + " ten name van IT Molen o.v.v.bovenstaande factuurnummer. \n (Op al onze diensten en producten zijn onze algemene voorwaarden van toepassing.Deze kunt u downloaden van onze website.)" + " \n Printed date: " +
                   DateTime.Now.Date.ToString("MM/dd/yyyy") + "  Page: [page]/[toPage]\"" +
                  " --footer-line --footer-font-size \"10\" --footer-spacing 6 --footer-font-name \"calibri light\"";


                return new Rotativa.PartialViewAsPdf("~/Views/Purchase/PrintPurchasePartialView.cshtml")
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

        public ActionResult InvoicebyEmail(int? purchaseOrderId)
        {

            EmailModel email = new EmailModel();
            var List = CreatDirectoryClass.GetFileDirectiory((int)purchaseOrderId, "Purchase");
            List<Selected> _list = new List<Selected>();

            foreach (var Item in List)
            {
                _list.Add(new Selected { IsSelected = true, FileName = Item.DirectoryPath, Directory = Item.FileFolderPathe + "/" + Item.DirectoryPath });
            }

             email.SelectList = _list;

            if (purchaseOrderId == 0 || purchaseOrderId == null)
            {
                return RedirectToAction("Index", "Captcha");
            }

            try
            {
                email.Attachment = PrintView((int)purchaseOrderId);
                HttpContext.Items["FilePath"] = email.Attachment;

                if (Session["CompayID"] == null)
                {
                    return RedirectToAction("Index", "Login");
                }

              

                if (TempData["CompanyId"] != null)
                {
                    companyInfo = TempData["CompanyId"] as MVCCompanyInfoModel;
                }

                if (TempData["VenderId"] != null)
                {
                    mvcContactModel = TempData["VenderId"] as MVCContactModel;
                }


                email.EmailText = @"Geachte heer" + mvcContactModel.ContactName + "." +
               ".Hierbij ontvangt u onze offerte 10 zoals besproken,." +
                "." + "Graag horen we of u hiermee akkoord gaat." +
                "." + "De offerte vindt u als bijlage bij deze email." +
                "..Met vriendelijke groet." +
                mvcContactModel.ContactName + "." +
                companyInfo.CompanyName.ToString() + "." +
                mvcContactModel.ContactName.ToString() + "." +
                companyInfo.CompanyEmail.ToString();
                string strToProcess = email.EmailText;
                string result = strToProcess.Replace(".", " \r\n");
                email.EmailText = result;
                email.invoiceId = (int)purchaseOrderId;
                email.From = "infouurtjefactuur@gmail.com";
            }
            catch (Exception)
            {

                throw;
            }

            return View(email);
        }

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

                        if (item.Directory.EndsWith("doc") || item.Directory.EndsWith("pdf") || item.Directory.EndsWith("PNG") || item.Directory.EndsWith("JPEG") || item.Directory.EndsWith("docx") || item.Directory.EndsWith("jpg") || item.Directory.EndsWith("png") || item.Directory.EndsWith("txt"))
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
                    if (item.EndsWith("doc") || item.EndsWith("pdf") || item.EndsWith("PNG") || item.EndsWith("JPEG") || item.EndsWith("docx") || item.EndsWith("jpg") || item.EndsWith("png") || item.EndsWith("txt"))
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

                var folderPath = Server.MapPath("/PDF/");
                EmailController.clearFolder(folderPath);

                return RedirectToAction("Viewinvoice1", new { purchaseOrderId = email.invoiceId });
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

        public bool PerformTransaction(MvcPurchaseModel purchaseViewModel, int CompanyId)
        {
            bool TransactionResult = false;
            try
            {
                string base64Guid = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                AccountTransictionTable accountTransictiontable = new AccountTransictionTable();
                accountTransictiontable.TransictionDate = DateTime.Now;
                accountTransictiontable.FK_AccountID = 4002;
                //with tax
                accountTransictiontable.Cr = purchaseViewModel.PurchaseTotoalAmount;
                accountTransictiontable.Dr = 0.00;
                accountTransictiontable.TransictionNumber = base64Guid;
                accountTransictiontable.TransictionRefrenceId = purchaseViewModel.PurchaseOrderID.ToString();
                accountTransictiontable.TransictionType = "Purchase";
                accountTransictiontable.CreationTime = DateTime.Now.TimeOfDay;
                accountTransictiontable.AddedBy = 1;
                accountTransictiontable.FK_CompanyId = CompanyId;
                accountTransictiontable.FKPaymentTerm = 1;
                accountTransictiontable.Description = "Total + Invoice ,Invoice created at Invoice" + purchaseViewModel.PurchaseTotoalAmount.ToString() + "On invoice genrattion";
                HttpResponseMessage responses = GlobalVeriables.WebApiClient.PostAsJsonAsync("APIAccountTransiction", accountTransictiontable).Result;
                if (responses.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string base64Guid1 = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                    TransactionResult = true;
                    accountTransictiontable.TransictionDate = DateTime.Now;
                    accountTransictiontable.FK_AccountID = 4003;
                    accountTransictiontable.Dr = purchaseViewModel.PurchaseSubTotal;
                    accountTransictiontable.Cr = 0.00;
                    accountTransictiontable.TransictionNumber = base64Guid1;
                    accountTransictiontable.TransictionRefrenceId = purchaseViewModel.PurchaseOrderID.ToString();
                    accountTransictiontable.TransictionType = "Purchase";
                    accountTransictiontable.CreationTime = DateTime.Now.TimeOfDay;
                    accountTransictiontable.AddedBy = 1;
                    accountTransictiontable.FK_CompanyId = CompanyId;
                    accountTransictiontable.FKPaymentTerm = 1;
                    accountTransictiontable.Description = "Total + Invoice ,Invoice created at Invoice" + purchaseViewModel.PurchaseSubTotal.ToString() + "On invoice genrattion";
                    HttpResponseMessage responses2 = GlobalVeriables.WebApiClient.PostAsJsonAsync("APIAccountTransiction", accountTransictiontable).Result;

                    if (responses2.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        string base64Guid2 = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                        TransactionResult = true;
                        accountTransictiontable.TransictionDate = DateTime.Now;
                        accountTransictiontable.FK_AccountID = 3005;
                        accountTransictiontable.Dr = purchaseViewModel.PurchaseSubTotal;
                        accountTransictiontable.Cr = 0.00;
                        accountTransictiontable.TransictionNumber = base64Guid2;
                        accountTransictiontable.TransictionRefrenceId = purchaseViewModel.PurchaseOrderID.ToString();
                        accountTransictiontable.TransictionType = "Purchase";
                        accountTransictiontable.CreationTime = DateTime.Now.TimeOfDay;
                        accountTransictiontable.AddedBy = 1;
                        accountTransictiontable.FK_CompanyId = CompanyId;
                        accountTransictiontable.FKPaymentTerm = 1;
                        double TotalVat = Convert.ToDouble(purchaseViewModel.Vat21 + purchaseViewModel.Vat6);
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

        public string PrintView(int purchaseOrderId)
        {
            string pdfname;
            try
            {
                HttpResponseMessage res = GlobalVeriables.WebApiClient.GetAsync("APIPurchase/" + purchaseOrderId.ToString()).Result;
                MvcPurchaseModel ob = res.Content.ReadAsAsync<MvcPurchaseModel>().Result;

                HttpResponseMessage responsep = GlobalVeriables.WebApiClient.GetAsync("APIProduct/" + ob.CompanyId + "/All").Result;
                List<MVCProductModel> productModel = responsep.Content.ReadAsAsync<List<MVCProductModel>>().Result;
                ViewBag.Product = productModel;

                HttpResponseMessage GoodResponse = GlobalVeriables.WebApiClient.GetAsync("APIProduct/" + ob.CompanyId + "/Good").Result;
                List<MVCProductModel> GoodModel = GoodResponse.Content.ReadAsAsync<List<MVCProductModel>>().Result;
                ViewBag.Good = GoodModel;

                HttpResponseMessage Services = GlobalVeriables.WebApiClient.GetAsync("APIProduct/" + ob.CompanyId + "/Services").Result;
                List<MVCProductModel> ServiceModel = Services.Content.ReadAsAsync<List<MVCProductModel>>().Result;
                ViewBag.Service = ServiceModel;

                HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("ApiConatacts/" + ob.VenderId.ToString()).Result;
                MVCContactModel contectmodel = response.Content.ReadAsAsync<MVCContactModel>().Result;

                HttpResponseMessage responseCompany = GlobalVeriables.WebApiClient.GetAsync("APIComapny/" + ob.CompanyId.ToString()).Result;
                MVCCompanyInfoModel companyModel = responseCompany.Content.ReadAsAsync<MVCCompanyInfoModel>().Result;

                TempData["CompanyId"] = companyModel;
                TempData["VenderId"] = contectmodel;

                HttpResponseMessage responseQutationDetailsList = GlobalVeriables.WebApiClient.GetAsync("APIPurchaseDetail/" + purchaseOrderId.ToString()).Result;
                List<MvcPurchaseViewModel> QutationModelDetailsList = responseQutationDetailsList.Content.ReadAsAsync<List<MvcPurchaseViewModel>>().Result;
                ViewBag.Contentdata = contectmodel;
                ViewBag.Companydata = companyModel;
                ViewBag.Purchase = ob;
                ViewBag.PurchaseDatailsList = QutationModelDetailsList;

                DateTime PurchaseDueDate = Convert.ToDateTime(ob.PurchaseDueDate); //mm/dd/yyyy
                DateTime PurchaseDate = Convert.ToDateTime(ob.PurchaseDate);//mm/dd/yyyy
                TimeSpan ts = PurchaseDueDate.Subtract(PurchaseDate);
                string diffDate = ts.Days.ToString();

                string companyName = purchaseOrderId + "-" + companyModel.CompanyName;
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

                var pdfResult = new Rotativa.PartialViewAsPdf("~/Views/Purchase/PrintPurchasePartialView.cshtml")
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

        [HttpPost]
        public ActionResult DeleteInvoice(int PurchaseOrderId, int purchaseOrderDetailId, int vat, decimal total)
        {
            try
            {

                MvcPurchaseModel mvcpurchaseModel = new MvcPurchaseModel();
                HttpResponseMessage res = GlobalVeriables.WebApiClient.GetAsync("APIPurchase/" + PurchaseOrderId.ToString()).Result;
                mvcpurchaseModel = res.Content.ReadAsAsync<MvcPurchaseModel>().Result;

                MVCPurchaseDetailsModel detailModel = new MVCPurchaseDetailsModel();

                GlobalVeriables.WebApiClient.DefaultRequestHeaders.Clear();

                GlobalVeriables.WebApiClient.DefaultRequestHeaders.Add("PDID", PurchaseOrderId.ToString());
                //GlobalVeriables.WebApiClient.DefaultRequestHeaders.Add("QutationDetailID1", QutationDetailID1);



                HttpResponseMessage responseQutationDetailsList = GlobalVeriables.WebApiClient.GetAsync("APIPurchaseDetail/" + purchaseOrderDetailId.ToString()).Result;
                List<MvcPurchaseViewModel> PurchaseDetailsList = responseQutationDetailsList.Content.ReadAsAsync<List<MvcPurchaseViewModel>>().Result;

                if (PurchaseDetailsList.Count() > 1)
                {
                    PurchaseDetailsList = PurchaseDetailsList.Where(C => C.PurchaseOrderDetailsId == purchaseOrderDetailId).ToList();

                    #region
                    if (PurchaseDetailsList.Count() != 0)
                    {
                        detailModel.PurchaseVatPercentage = PurchaseDetailsList[0].PurchaseVatPercentage;

                        detailModel.PurchaseTotal = PurchaseDetailsList[0].PurchaseTotal;

                        mvcpurchaseModel.PurchaseSubTotal = mvcpurchaseModel.PurchaseSubTotal - detailModel.PurchaseTotal;

                        mvcpurchaseModel.PurchaseTotal = mvcpurchaseModel.PurchaseTotal - detailModel.PurchaseTotal;

                        if (vat == 6)
                            mvcpurchaseModel.Vat6 = detailModel.PurchaseVatPercentage - 6;
                        else
                            mvcpurchaseModel.Vat21 = detailModel.PurchaseVatPercentage - 21;


                        HttpResponseMessage response = GlobalVeriables.WebApiClient.PutAsJsonAsync("APIPurchase/" + PurchaseOrderId, mvcpurchaseModel).Result;
                        if (response.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            HttpResponseMessage deleteQuaution2 = GlobalVeriables.WebApiClient.DeleteAsync("APIPurchaseDetail/" + purchaseOrderDetailId).Result;
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

                    HttpResponseMessage Qdresponse = GlobalVeriables.WebApiClient.DeleteAsync("APIPurchaseDetail/" + purchaseOrderDetailId).Result;

                    if (Qdresponse.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        mvcpurchaseModel.PurchaseSubTotal = 0.00;
                        mvcpurchaseModel.PurchaseTotoalAmount = 0.00;
                        mvcpurchaseModel.PurchaseID = mvcpurchaseModel.PurchaseID;
                        mvcpurchaseModel.PurchaseOrderID = mvcpurchaseModel.PurchaseOrderID;
                        mvcpurchaseModel.PurchaseDueDate = mvcpurchaseModel.PurchaseDueDate;
                        mvcpurchaseModel.PurchaseVenderNote = mvcpurchaseModel.PurchaseVenderNote;
                        mvcpurchaseModel.PurchaseDueDate = mvcpurchaseModel.PurchaseDueDate;
                        mvcpurchaseModel.Status = mvcpurchaseModel.Status;
                        mvcpurchaseModel.CompanyId = mvcpurchaseModel.CompanyId;
                        mvcpurchaseModel.Vat6 = 0;
                        mvcpurchaseModel.Vat21 = 0;

                        HttpResponseMessage response = GlobalVeriables.WebApiClient.PutAsJsonAsync("APIPurchase/" + PurchaseOrderId, mvcpurchaseModel).Result;
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

        public ActionResult Design()
        {
            return View();
        }

        public ActionResult PurchaseList()
        {
            return View();

        }

        [HttpPost]
        public ActionResult Trasactionpayment(List<TransactionModel> TransactionModel)
        {

            try
            {

                HttpResponseMessage res = GlobalVeriables.WebApiClient.GetAsync("APIPurchase/" + TransactionModel[0].PurchaseOrderID.ToString()).Result;
                MvcPurchaseModel ob = res.Content.ReadAsAsync<MvcPurchaseModel>().Result;
                if (res.StatusCode == System.Net.HttpStatusCode.OK)
                {

                    foreach (var item in TransactionModel)
                    {
                        string base64Guid1 = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                        AccountTransictionTable accountTransictiontable = new AccountTransictionTable();
                        accountTransictiontable.FK_CompanyId = Convert.ToInt32(Session["CompayID"]);
                        accountTransictiontable.Description = item.descrition;
                        accountTransictiontable.FK_AccountID = item.Id;
                        accountTransictiontable.Description = item.descrition;
                        accountTransictiontable.FKPaymentTerm = 1;
                        accountTransictiontable.TransictionRefrenceId = ob.PurchaseOrderID.ToString();
                        accountTransictiontable.Dr = item.AmountDebit;
                        accountTransictiontable.Cr = item.AmountCredit;
                        accountTransictiontable.TransictionNumber = base64Guid1;
                        accountTransictiontable.TransictionType = "Purchase";
                        accountTransictiontable.CreationTime = DateTime.Now.TimeOfDay;
                        accountTransictiontable.TransictionDate = item.TranDate;
                        TransactionClass.PerformTransaction(accountTransictiontable);
                    }
                }
            }
            catch (Exception)
            {
                return Json("Fail", JsonRequestBehavior.AllowGet);
            }

            return Json("Success", JsonRequestBehavior.AllowGet);

            return View();
        }

        public ActionResult InvoiceSerVice()
        {
            return View();
        }
        [HttpPost]
        public JsonResult GetPurchaseServiceList(string Type)
        {
            IEnumerable<MvcPurchaseModel> PurchaseList;
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

                GlobalVeriables.WebApiClient.DefaultRequestHeaders.Clear();
                int CompanyId = Convert.ToInt32(Session["CompayID"]);
                GlobalVeriables.WebApiClient.DefaultRequestHeaders.Add("CompayID", CompanyId.ToString());

                int companyId = Convert.ToInt32(Session["CompayID"]);

                HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("GetPurchaseServiceList/" + Type + "/" + companyId).Result;
                PurchaseList = response.Content.ReadAsAsync<IEnumerable<MvcPurchaseModel>>().Result;


                if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
                {
                    PurchaseList = PurchaseList.Where(p => p.PurchaseOrderID.ToString().Contains(search)
                  || p.PurchaseRefNumber != null && p.PurchaseRefNumber.ToLower().Contains(search.ToLower())
                  || p.PurchaseDate != null && p.PurchaseDate.ToString().ToLower().Contains(search.ToLower())
                  || p.PurchaseDueDate != null && p.PurchaseDueDate.ToString().ToLower().Contains(search.ToLower())
                  || p.Status != null && p.Status.ToString().ToLower().Contains(search.ToLower())
                  || p.PurchaseTotoalAmount != null && p.PurchaseTotoalAmount.ToString().ToLower().Contains(search.ToLower())
                  || p.Status != null && p.Status.ToString().ToLower().Contains(search.ToLower())).ToList();
                }



                int recordsTotal = recordsTotal = PurchaseList.Count();
                var data = PurchaseList.Skip(skip).Take(pageSize).ToList();
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Response.Write(ex.ToString());
                return Json(new { draw = 0, recordsFiltered = 0, recordsTotal = 0, data = 0 }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { draw = 0, recordsFiltered = 0, recordsTotal = 0, data = 0 }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult ViewAndPrintInvoiceService(int Id)
        {

            if (Id == 0)
            {
                return RedirectToAction("Index", "Login");
            }

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

                HttpResponseMessage res = GlobalVeriables.WebApiClient.GetAsync("APIPurchase/" + Id.ToString()).Result;
                MvcPurchaseModel ob = res.Content.ReadAsAsync<MvcPurchaseModel>().Result;
                GlobalVeriables.WebApiClient.DefaultRequestHeaders.Clear();

                HttpResponseMessage responseQutationDetailsList = GlobalVeriables.WebApiClient.GetAsync("APIPurchaseDetail/" + Id.ToString()).Result;
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

        public ActionResult PirnPurchaseInvoiceGood(int? purchaseOrderId)
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

                HttpResponseMessage res = GlobalVeriables.WebApiClient.GetAsync("APIPurchase/" + purchaseOrderId.ToString()).Result;
                MvcPurchaseModel ob = res.Content.ReadAsAsync<MvcPurchaseModel>().Result;

                GlobalVeriables.WebApiClient.DefaultRequestHeaders.Clear();

                HttpResponseMessage responseQutationDetailsList = GlobalVeriables.WebApiClient.GetAsync("APIPurchaseDetail/" + purchaseOrderId.ToString()).Result;
                List<MvcPurchaseViewModel> PuchaseModelDetailsList = responseQutationDetailsList.Content.ReadAsAsync<List<MvcPurchaseViewModel>>().Result;

                DateTime PurchaseDueDate = Convert.ToDateTime(ob.PurchaseDueDate); //mm/dd/yyyy
                DateTime PurchaseDate = Convert.ToDateTime(ob.PurchaseDate);//mm/dd/yyyy
                TimeSpan ts = PurchaseDueDate.Subtract(PurchaseDate);
                string diffDate = ts.Days.ToString();

                ViewBag.Contentdata = contectmodel;
                ViewBag.Companydata = companyModel;
                ViewBag.Purchase = ob;
                ViewBag.PurchaseDatailsList = PuchaseModelDetailsList;

                string PdfName = purchaseOrderId + "-" + companyModel.CompanyName + ".pdf";


                string cutomswitches = "";
                cutomswitches = "--footer-center \"" + "Wilt u zo vriendelijk zijn om het verschuldigde bedrag binnen " + diffDate + " dagen over te maken naar IBAN: \n " + companyModel.IBANNumber + " ten name van IT Molen o.v.v.bovenstaande factuurnummer. \n (Op al onze diensten en producten zijn onze algemene voorwaarden van toepassing.Deze kunt u downloaden van onze website.)" + " \n Printed date: " +
                   DateTime.Now.Date.ToString("MM/dd/yyyy") + "  Page: [page]/[toPage]\"" +
                  " --footer-line --footer-font-size \"10\" --footer-spacing 6 --footer-font-name \"calibri light\"";


                return new Rotativa.PartialViewAsPdf("~/Views/Purchase/PurchaseInvoiceGoodPrint.cshtml")
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

        public ActionResult InvoiceGoods()
        {
            return View();
        }

        public ActionResult ViewAndPrintInvoiceGood(int Id)
        {
            if (Id == 0)
            {
                return RedirectToAction("Index", "Login");
            }

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

                HttpResponseMessage res = GlobalVeriables.WebApiClient.GetAsync("APIPurchase/" + Id.ToString()).Result;
                MvcPurchaseModel ob = res.Content.ReadAsAsync<MvcPurchaseModel>().Result;

                GlobalVeriables.WebApiClient.DefaultRequestHeaders.Clear();

                HttpResponseMessage responseQutationDetailsList = GlobalVeriables.WebApiClient.GetAsync("APIPurchaseDetail/" + Id.ToString()).Result;
                List<MvcPurchaseViewModel> PuchaseModelDetailsList = responseQutationDetailsList.Content.ReadAsAsync<List<MvcPurchaseViewModel>>().Result;


                ViewBag.Contentdata = contectmodel;
                ViewBag.Companydata = companyModel;
                ViewBag.Purchase = ob;
                ViewBag.PurchaseDatailsList = PuchaseModelDetailsList;

                return View();
            }
            catch (Exception ex) { }

            return View();
        }

        public ActionResult Create(int id)
        {
            MvcPurchaseViewModel purchaseviewModel = new MvcPurchaseViewModel();
            try
            {
                int CompanyID = Convert.ToInt32(Session["CompayID"]);

                HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("ApiConatacts/" + id.ToString()).Result;
                MVCContactModel contectmodel = response.Content.ReadAsAsync<MVCContactModel>().Result;

                HttpResponseMessage responseCompany = GlobalVeriables.WebApiClient.GetAsync("APIComapny/" + CompanyID.ToString()).Result;
                MVCCompanyInfoModel companyModel = responseCompany.Content.ReadAsAsync<MVCCompanyInfoModel>().Result;

                ViewBag.Contentdata = contectmodel;
                ViewBag.Companydata = companyModel;
                DateTime InvoiceDate = new DateTime();
                InvoiceDate = DateTime.Now;
                purchaseviewModel.PurchaseDate = InvoiceDate;
                //purchaseviewModel.PurchaseDueDate = InvoiceDate.AddDays(+15);
                purchaseviewModel.CompanyId = companyModel.CompanyID;
                purchaseviewModel.VenderId = id;

                CommonModel commonModel = new CommonModel();
                commonModel.Name = "Purchase";

                commonModel.FromDate = InvoiceDate;

                MvcPurchaseModel q = new MvcPurchaseModel();
                HttpResponseMessage response1 = GlobalVeriables.WebApiClient.GetAsync("GenrateInvoice/").Result;
                q = response1.Content.ReadAsAsync<MvcPurchaseModel>().Result;
                purchaseviewModel.PurchaseDate = InvoiceDate;
                commonModel.DueDate = InvoiceDate.AddDays(+15);
                commonModel.Number_Id = q.PurchaseID;

                ViewBag.commonModel = commonModel;
                return View(purchaseviewModel);
            }
            catch (Exception ex)
            {
                return null;
            }


        }

        [HttpPost]
        public ActionResult UploadFiles()
        {
            try
            {
                string FileName = "";
                HttpFileCollectionBase files = Request.Files;
                for (int i = 0; i < files.Count; i++)
                {
                    HttpPostedFileBase file = files[i];
                    FileName = CreatDirectoryClass.UploadToPDFCommon(file);
                }

                return new JsonResult { Data = new { FilePath = FileName, FileName = FileName } };
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult UploadFiles12(MvcPurchaseViewModel _MvcPurchaseViewModel, HttpPostedFileWrapper[] file23)
        {
            try
            {

                string FileName = "";
                HttpFileCollectionBase files = Request.Files;
                for (int i = 0; i < files.Count; i++)
                {
                    HttpPostedFileBase file = files[i];


                    FileName = CreatDirectoryClass.UploadFileToDirectoryCommon(_MvcPurchaseViewModel.PurchaseOrderID, "Purchase", file23, "Purchase");

                    if (FileName == "FileNotallowed")
                    {
                        return new JsonResult { Data = new { FilePath = FileName, FileName = FileName } };
                    }
                }

                return new JsonResult { Data = new { FilePath = FileName, FileName = FileName, } };
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult Viewinvoice1(int? purchaseOrderId)
        {
            try
            {

                ViewBag.FILE = CreatDirectoryClass.GetFileDirectiory((int)purchaseOrderId, "Purchase");

                HttpResponseMessage res = GlobalVeriables.WebApiClient.GetAsync("APIPurchase/" + purchaseOrderId.ToString()).Result;
                MvcPurchaseModel ob = res.Content.ReadAsAsync<MvcPurchaseModel>().Result;
                ob.PurchaseOrderID = purchaseOrderId;
                HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("ApiConatacts/" + ob.VenderId.ToString()).Result;
                MVCContactModel contectmodel = response.Content.ReadAsAsync<MVCContactModel>().Result;

                HttpResponseMessage responseCompany = GlobalVeriables.WebApiClient.GetAsync("APIComapny/" + ob.CompanyId.ToString()).Result;
                MVCCompanyInfoModel companyModel = responseCompany.Content.ReadAsAsync<MVCCompanyInfoModel>().Result;

                GlobalVeriables.WebApiClient.DefaultRequestHeaders.Clear();

                HttpResponseMessage responseQutationDetailsList = GlobalVeriables.WebApiClient.GetAsync("APIPurchaseDetail/" + purchaseOrderId.ToString()).Result;
                List<MvcPurchaseViewModel> PuchaseModelDetailsList = responseQutationDetailsList.Content.ReadAsAsync<List<MvcPurchaseViewModel>>().Result;

                CommonModel commonModel = new CommonModel();
                commonModel.Name = "Purchase";
                commonModel.FromDate = Convert.ToDateTime(ob.PurchaseDate);
                commonModel.DueDate = Convert.ToDateTime(ob.PurchaseDueDate);
                commonModel.ReferenceNumber = ob.PurchaseRefNumber;
                commonModel.Number_Id = ob.PurchaseID;

                commonModel.SubTotal = ob.PurchaseSubTotal.ToString();
                commonModel.Vat6 = ob.Vat6.ToString();
                commonModel.Vat21 = ob.Vat21.ToString();
                commonModel.grandTotal = ob.PurchaseTotoalAmount.ToString();
                commonModel.Note = ob.PurchaseVenderNote;
                ViewBag.commonModel = commonModel;

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

        public List<VatModel> GetVatList()
        {
            List<VatModel> model = new List<VatModel>();
            model.Add(new VatModel() { Vat1 = 0, Name = "0" });
            model.Add(new VatModel() { Vat1 = 6, Name = "6" });
            model.Add(new VatModel() { Vat1 = 21, Name = "21" });
            return model;
        }


        [HttpGet]
        public ActionResult Edit(int Id = 0)
        {
            MvcPurchaseViewModel purchaseviewModel = new MvcPurchaseViewModel();

            ViewBag.FILE = CreatDirectoryClass.GetFileDirectiory(Id, "Purchase");

            try
            {
                ViewBag.VatDrop = GetVatList();

                HttpResponseMessage res = GlobalVeriables.WebApiClient.GetAsync("APIPurchase/" + Id.ToString()).Result;
                MvcPurchaseModel ob = res.Content.ReadAsAsync<MvcPurchaseModel>().Result;

                HttpResponseMessage responsep = GlobalVeriables.WebApiClient.GetAsync("APIProduct/" + ob.CompanyId + "/All").Result;
                List<MVCProductModel> productModel = responsep.Content.ReadAsAsync<List<MVCProductModel>>().Result;
                ViewBag.Product = productModel;

                HttpResponseMessage GoodResponse = GlobalVeriables.WebApiClient.GetAsync("APIProduct/" + ob.CompanyId + "/Good").Result;
                List<MVCProductModel> GoodModel = GoodResponse.Content.ReadAsAsync<List<MVCProductModel>>().Result;
                ViewBag.Good = GoodModel;

                HttpResponseMessage Services = GlobalVeriables.WebApiClient.GetAsync("APIProduct/" + ob.CompanyId + "/Services").Result;
                List<MVCProductModel> ServiceModel = Services.Content.ReadAsAsync<List<MVCProductModel>>().Result;
                ViewBag.Service = ServiceModel;

                HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("ApiConatacts/" + ob.VenderId.ToString()).Result;
                MVCContactModel contectmodel = response.Content.ReadAsAsync<MVCContactModel>().Result;

                HttpResponseMessage responseCompany = GlobalVeriables.WebApiClient.GetAsync("APIComapny/" + ob.CompanyId.ToString()).Result;
                MVCCompanyInfoModel companyModel = responseCompany.Content.ReadAsAsync<MVCCompanyInfoModel>().Result;

                purchaseviewModel.PurchaseOrderID = ob.PurchaseOrderID;

                purchaseviewModel.Purchase_ID = ob.PurchaseID;
                purchaseviewModel.VenderId = Convert.ToInt32(ob.VenderId);
                purchaseviewModel.CompanyId = Convert.ToInt32(ob.CompanyId);

                purchaseviewModel.PurchaseDate = Convert.ToDateTime(ob.PurchaseDate);
                purchaseviewModel.PurchaseDueDate = Convert.ToDateTime(ob.PurchaseDueDate);
                purchaseviewModel.PurchaseRefNumber = ob.PurchaseRefNumber;
                purchaseviewModel.PurchaseSubTotal = ob.PurchaseSubTotal;
                purchaseviewModel.PurchaseDiscountPercenteage = ob.PurchaseDiscountPercenteage;
                purchaseviewModel.PurchaseDiscountAmount = ob.PurchaseDiscountAmount;
                purchaseviewModel.PurchaseVatPercentage = ob.PurchaseVatPercentage;
                purchaseviewModel.PurchaseTotoalAmount = ob.PurchaseTotoalAmount;
                purchaseviewModel.PurchaseVenderNote = ob.PurchaseVenderNote;
                purchaseviewModel.Status = ob.Status;
                purchaseviewModel.Vat21 = Convert.ToInt32(ob.Vat21);
                purchaseviewModel.Vat6 = Convert.ToInt32(ob.Vat6);
                purchaseviewModel.CompanyId = ob.CompanyId;
                purchaseviewModel.UserId = ob.UserId;

                HttpResponseMessage responseQutationDetailsList = GlobalVeriables.WebApiClient.GetAsync("APIPurchaseDetail/" + Id.ToString()).Result;
                List<MvcPurchaseViewModel> QutationModelDetailsList = responseQutationDetailsList.Content.ReadAsAsync<List<MvcPurchaseViewModel>>().Result;

                CommonModel commonModel = new CommonModel();
                commonModel.Name = "Purchase";

                commonModel.FromDate = Convert.ToDateTime(ob.PurchaseDate);
                commonModel.DueDate = Convert.ToDateTime(ob.PurchaseDueDate);
                commonModel.Number_Id = ob.PurchaseID;
                commonModel.ReferenceNumber = ob.PurchaseRefNumber;

                ViewBag.commonModel = commonModel;
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

        DBEntities db = new DBEntities();

        [HttpPost]
        public ActionResult SaveToDraftEdit(MvcPurchaseViewModel purchaseViewModel)
        {
            PurchaseOrderTable purchasemodel = new PurchaseOrderTable();
            try
            {

                PurchaseOrderTable p = db.PurchaseOrderTables.Find(purchaseViewModel.PurchaseOrderID);

                if (p.PurchaseTotoalAmount != purchaseViewModel.PurchaseTotoalAmount)
                {
                    purchasemodel.PurchaseID = purchaseViewModel.PurchaseId.ToString();
                    purchasemodel.UserId = Convert.ToInt32(Session["LoginUserID"]);

                    HttpResponseMessage response = GlobalVeriables.WebApiClient.PutAsJsonAsync("APIPurchase/" + purchasemodel.PurchaseOrderID, purchaseViewModel).Result;
                    MvcPurchaseModel _PurchaseModel = response.Content.ReadAsAsync<MvcPurchaseModel>().Result;

                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        return new JsonResult { Data = new { Status = "Success", purchaseId = purchaseViewModel.PurchaseOrderID } };
                    }
                    else
                    {
                        return new JsonResult { Data = new { Status = "Fail", purchaseId = purchaseViewModel.PurchaseOrderID } };
                    }
                }
                else
                {
                    return new JsonResult { Data = new { Status = "Success", purchaseId = purchaseViewModel.PurchaseOrderID } };
                }


            }
            catch (Exception ex)
            {

            }

            return View();
        }

        public ActionResult PrintPurchase(int? purchaseOrderId)
        {

            try
            {
                HttpResponseMessage res = GlobalVeriables.WebApiClient.GetAsync("APIPurchase/" + purchaseOrderId.ToString()).Result;
                MvcPurchaseModel ob = res.Content.ReadAsAsync<MvcPurchaseModel>().Result;


                HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("ApiConatacts/" + ob.VenderId.ToString()).Result;
                MVCContactModel contectmodel = response.Content.ReadAsAsync<MVCContactModel>().Result;

                HttpResponseMessage responseCompany = GlobalVeriables.WebApiClient.GetAsync("APIComapny/" + ob.CompanyId.ToString()).Result;
                MVCCompanyInfoModel companyModel = responseCompany.Content.ReadAsAsync<MVCCompanyInfoModel>().Result;

                GlobalVeriables.WebApiClient.DefaultRequestHeaders.Clear();

                HttpResponseMessage responseQutationDetailsList = GlobalVeriables.WebApiClient.GetAsync("APIPurchaseDetail/" + purchaseOrderId.ToString()).Result;
                List<MvcPurchaseViewModel> PuchaseModelDetailsList = responseQutationDetailsList.Content.ReadAsAsync<List<MvcPurchaseViewModel>>().Result;

                DateTime PurchaseDueDate = Convert.ToDateTime(ob.PurchaseDueDate); //mm/dd/yyyy
                DateTime PurchaseDate = Convert.ToDateTime(ob.PurchaseDate);//mm/dd/yyyy
                TimeSpan ts = PurchaseDueDate.Subtract(PurchaseDate);
                string diffDate = ts.Days.ToString();

                ViewBag.Contentdata = contectmodel;
                ViewBag.Companydata = companyModel;
                ViewBag.Purchase = ob;

                ViewBag.PurchaseDatailsList = PuchaseModelDetailsList;
                string PdfName = purchaseOrderId + "-" + companyModel.CompanyName + ".pdf";

                string cutomswitches = "";
                cutomswitches = "--footer-center \"" + "Wilt u zo vriendelijk zijn om het verschuldigde bedrag binnen " + diffDate + " dagen over te maken naar IBAN: \n " + companyModel.IBANNumber + " ten name van IT Molen o.v.v.bovenstaande factuurnummer. \n (Op al onze diensten en producten zijn onze algemene voorwaarden van toepassing.Deze kunt u downloaden van onze website.)" + " \n Printed date: " +
                   DateTime.Now.Date.ToString("MM/dd/yyyy") + "  Page: [page]/[toPage]\"" +
                  " --footer-line --footer-font-size \"10\" --footer-spacing 6 --footer-font-name \"calibri light\"";

                return new Rotativa.PartialViewAsPdf("~/Views/Purchase/PrintPurchasePartialView.cshtml")
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

        [HttpPost]
        public ActionResult SaveDraft(MvcPurchaseViewModel purchaseViewModel, HttpPostedFileWrapper[] file23)
        {

            try
            {
                purchaseViewModel.CompanyId = purchaseViewModel.CompanyId;
                purchaseViewModel.UserId = Convert.ToInt32(Session["LoginUserID"]);
                HttpResponseMessage response = GlobalVeriables.WebApiClient.PostAsJsonAsync("APIPurchase", purchaseViewModel).Result;
                MvcPurchaseModel Purchasetable = response.Content.ReadAsAsync<MvcPurchaseModel>().Result;

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    purchaseViewModel.PurchaseOrderID = Purchasetable.PurchaseOrderID;
                }

                if (file23[0] != null)
                {
                    CreatDirectoryClass.UploadFileToDirectoryCommon(purchaseViewModel.PurchaseOrderID, "Purchase", file23, "Purchase");
                }
            }
            catch (Exception ex)
            {

                return new JsonResult { Data = new { Status = "Fail", Message = ex.Message.ToString() } };
            }

            return new JsonResult { Data = new { Status = "Success", purchaseId = purchaseViewModel.PurchaseOrderID } };
        }

        [HttpPost]
        public ActionResult SaveEmail(MvcPurchaseViewModel purchaseViewModel, HttpPostedFileWrapper[] file23)
        {

            try
            {
                purchaseViewModel.CompanyId = purchaseViewModel.CompanyId;
                purchaseViewModel.UserId = Convert.ToInt32(Session["LoginUserID"]);

                HttpResponseMessage response = GlobalVeriables.WebApiClient.PostAsJsonAsync("APIPurchase", purchaseViewModel).Result;
                MvcPurchaseModel Purchasetable = response.Content.ReadAsAsync<MvcPurchaseModel>().Result;

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    purchaseViewModel.PurchaseOrderID = Purchasetable.PurchaseOrderID;
                }

                if (file23[0] != null)
                {
                    CreatDirectoryClass.UploadFileToDirectoryCommon(purchaseViewModel.PurchaseOrderID, "Purchase", file23, "Purchase");
                }
            }

            catch (Exception ex)
            {
                return new JsonResult { Data = new { Status = "Fail", Message = ex.Message } };

            }
            return new JsonResult { Data = new { Status = "Success", purchaseId = purchaseViewModel.PurchaseOrderID } };
        }

        [HttpPost]
        public ActionResult SaveEmailPrint1(MvcPurchaseViewModel purchaseViewModel, HttpPostedFileWrapper[] file23)
        {

            try
            {
                purchaseViewModel.CompanyId = purchaseViewModel.CompanyId;
                purchaseViewModel.UserId = Convert.ToInt32(Session["LoginUserID"]);
                HttpResponseMessage response = GlobalVeriables.WebApiClient.PostAsJsonAsync("APIPurchase", purchaseViewModel).Result;
                MvcPurchaseModel Purchasetable = response.Content.ReadAsAsync<MvcPurchaseModel>().Result;

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    purchaseViewModel.PurchaseOrderID = Purchasetable.PurchaseOrderID;
                }

                if (file23[0] != null)
                {
                    CreatDirectoryClass.UploadFileToDirectoryCommon(purchaseViewModel.PurchaseOrderID, "Purchase", file23, "Purchase");
                }
            }
            catch (Exception ex)
            {

                return new JsonResult { Data = new { Status = "Fail", Message = ex.Message.ToString() } };
            }

            //calling printing option
            string FilePath = PrintView((int)purchaseViewModel.PurchaseOrderID);
            var root = Server.MapPath("/PDF/");
            var pdfname = String.Format("{0}", FilePath);
            var path = Path.Combine(root, pdfname);
            path = Path.GetFullPath(path);
            return new JsonResult { Data = new { Status = "Success", path = pdfname, PurchaseOrderId = purchaseViewModel.PurchaseOrderID } };
        }

        [HttpPost]
        public ActionResult PrintEmailEdit(MvcPurchaseViewModel purchaseViewModel)
        {
            PurchaseOrderTable purchasemodel = new PurchaseOrderTable();
            try
            {
                PurchaseOrderTable p = db.PurchaseOrderTables.Find(purchaseViewModel.PurchaseOrderID);
                if (p.PurchaseTotoalAmount != purchaseViewModel.PurchaseTotoalAmount)
                {
                    purchasemodel.PurchaseID = purchaseViewModel.PurchaseId.ToString();
                    purchasemodel.UserId = Convert.ToInt32(Session["LoginUserID"]);

                    HttpResponseMessage response = GlobalVeriables.WebApiClient.PutAsJsonAsync("APIPurchase/" + purchasemodel.PurchaseOrderID, purchaseViewModel).Result;
                    MvcPurchaseModel _PurchaseModel = response.Content.ReadAsAsync<MvcPurchaseModel>().Result;

                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        string path1 = PrintView((int)purchaseViewModel.PurchaseOrderID);
                        var root = Server.MapPath("/PDF/");
                        var pdfname = String.Format("{0}", path1);
                        var path = Path.Combine(root, pdfname);
                        path = Path.GetFullPath(path);
                        return new JsonResult { Data = new { Status = "Success", path = pdfname, PurchaseOrderId = purchaseViewModel.PurchaseOrderID } };
                    }
                    else
                    {
                        return new JsonResult { Data = new { Status = "Fail", PurchaseOrderId = purchaseViewModel.PurchaseOrderID } };
                    }
                }
                else
                {
                    string path1 = PrintView((int)purchaseViewModel.PurchaseOrderID);
                    var root = Server.MapPath("/PDF/");
                    var pdfname = String.Format("{0}", path1);
                    var path = Path.Combine(root, pdfname);
                    path = Path.GetFullPath(path);

                    return new JsonResult { Data = new { Status = "Success", path = pdfname, PurchaseOrderId = purchaseViewModel.PurchaseOrderID } };
                }
            }

            catch (Exception ex)
            {
                return new JsonResult { Data = new { Status = "Fail", Message = ex.Message.ToString() } };
            }
        }

        public ActionResult PrintReport(int ID)
        {

            try
            {

                DBEntities entity = new DBEntities();


                List<QuotationReportModel> quotationReportModels = entity.PurchaseOrderTables.Where(x => x.PurchaseOrderID == ID).Select(x => new QuotationReportModel
                {

                    QutationID = x.PurchaseOrderID,
                    Qutation_ID = x.PurchaseID,
                    RefNumber = x.PurchaseRefNumber,
                    QutationDate = x.PurchaseDate.ToString(),
                    DueDate = x.PurchaseDueDate.ToString(),
                    SubTotal = x.PurchaseSubTotal ?? 0,
                    TotalVat6 = x.Vat6 ?? 0,
                    TotalVat21 = x.Vat21 ?? 0,
                    DiscountAmount = x.PurchaseDiscountAmount ?? 0,
                    TotalAmount = x.PurchaseTotoalAmount ?? 0,
                    CustomerNote = x.PurchaseVenderNote,
                    Status = x.Status

                }).ToList();

                List<QuotationReportModel> quotationReportModel = new List<QuotationReportModel>();



                foreach (var x in quotationReportModels)
                {
                    QuotationReportModel qut = new QuotationReportModel();


                    DateTime QT = Convert.ToDateTime(x.QutationDate);
                    DateTime DQT = Convert.ToDateTime(x.DueDate);

                    qut.QutationID = x.QutationID;
                    qut.Qutation_ID = x.Qutation_ID;
                    qut.RefNumber = x.RefNumber;
                    qut.QutationDate = QT.ToShortDateString();
                    qut.DueDate = DQT.ToShortDateString();
                    qut.SubTotal = x.SubTotal;
                    qut.TotalVat6 = x.TotalVat6;
                    qut.TotalVat21 = x.TotalVat21;
                    qut.DiscountAmount = x.DiscountAmount;
                    qut.TotalAmount = x.TotalAmount;
                    qut.CustomerNote = x.CustomerNote;
                    qut.Status = x.Status;
                    quotationReportModel.Add(qut);
                }

                PurchaseOrderTable qt = entity.PurchaseOrderTables.Where(x => x.PurchaseOrderID == ID).FirstOrDefault();


                List<Comp> info = entity.ComapnyInfoes.Where(x => x.CompanyID == qt.CompanyId).Select(c => new Comp
                {
                    // Company Information   
                    CompanyID = c.CompanyID,
                    CompanyTRN = c.CompanyTRN,
                    CompanyName = c.CompanyName,
                    CompanyAddress = c.CompanyAddress,
                    CompanyPhone = c.CompanyPhone,
                    CompanyCell = c.CompanyCell,
                    CompanyEmail = c.CompanyEmail,
                    CompanyLogo = c.CompanyLogo,
                    CompanyCity = c.CompanyCity,
                    CompanyCountry = c.CompanyCountry,
                    StreetNumber = c.StreetNumber,
                    PostalCode = c.PostalCode,
                    IBANNumber = c.IBANNumber,
                    Website = c.Website,
                    BIC = c.BIC,
                    KVK = c.KVK,
                    BTW = c.BTW,
                    BankName = c.BankName,
                    UserName = c.UserName,

                }).ToList();

                string Name = info[0].CompanyLogo;

                //info.Remove(info.Single(X => X.CompanyLogo == info[0].CompanyLogo));

                info[0].CompanyLogo = Server.MapPath("~/images/" + Name);

                //  info.Add(new Comp { CompanyLogo = Server.MapPath("~/images/"+ Name) });


                List<Contacts> Contact = entity.ContactsTables.Where(x => x.ContactsId == qt.VenderId).Select(c => new Contacts
                {
                    ContactName = c.ContactName,
                    ContactAddress = c.ContactAddress,
                    ContactCity = c.City,
                    ContactLand = c.Land,
                    ContactPostalCode = c.PostalCode,
                    contactMobile = c.Mobile,
                    Contacttelephone = c.telephone,
                    ContactStreetNumber = c.StreetNumber,
                }).ToList();

                List<GoodsTable> goodsTable = entity.PurchaseOrderDetailsTables.Where(x => x.PurchaseId == ID && x.Type == "Goods").Select(x => new GoodsTable
                {
                    ProductName = x.ProductTable.ProductName,
                    Quantity = x.PurchaseQuantity ?? 0,
                    Rate = x.PurchaseItemRate ?? 0,
                    Total = x.PurchaseTotal ?? 0,
                    Vat = x.PurchaseVatPercentage ?? 0,
                    RowSubTotal = x.RowSubTotal ?? 0,

                }).ToList();

                DateTime dt = DateTime.Today;


                List<ServicesTables> servicesTabless = entity.PurchaseOrderDetailsTables.Where(x => x.PurchaseId == ID && x.Type == "Service").Select(x => new ServicesTables
                {

                    Date = x.ServiceDate.ToString(),
                    ProductNames = x.ProductTable.ProductName,
                    Descriptions = x.PurchaseDescription,
                    Quantitys = x.PurchaseQuantity ?? 0,
                    Rates = x.PurchaseItemRate ?? 0,
                    Totals = x.PurchaseTotal ?? 0,
                    Vats = x.PurchaseVatPercentage ?? 0,
                    RowSubTotals = x.RowSubTotal ?? 0,

                }).ToList();


                List<ServicesTables> servicesTables = new List<ServicesTables>();

                foreach (var x in servicesTabless)
                {
                    ServicesTables Serv = new ServicesTables();

                    DateTime dtt = Convert.ToDateTime(x.Date);

                    Serv.Date = dtt.ToShortDateString();
                    Serv.ProductNames = x.ProductNames;
                    Serv.Descriptions = x.Descriptions;
                    Serv.Quantitys = x.Quantitys;
                    Serv.Rates = x.Rates;
                    Serv.Totals = x.Totals;
                    Serv.Vats = x.Vats;
                    Serv.RowSubTotals = x.RowSubTotals;

                    servicesTables.Add(Serv);

                }


                ReportDocument Report = new ReportDocument();

                //ParameterField paramField = new ParameterField();
                //ParameterFields paramFields = new ParameterFields();
                //ParameterDiscreteValue paramDiscreteValue = new ParameterDiscreteValue();

                //paramField.Name = "ImageURL";
                //var s = Server.MapPath("~/images/" + info[0].CompanyLogo);
                //paramDiscreteValue.Value = s;
                //paramField.CurrentValues.Add(paramDiscreteValue);
                //paramFields.Add(paramField);

                //Report.ParameterFieldInfo = paramFields;
                Report.Load(Server.MapPath("~/CrystalReport/PurchaseReport.rpt"));


                Report.Database.Tables[0].SetDataSource(info);
                Report.Database.Tables[1].SetDataSource(Contact);
                Report.Database.Tables[2].SetDataSource(goodsTable);
                Report.Database.Tables[3].SetDataSource(servicesTables);
                Report.Database.Tables[4].SetDataSource(quotationReportModel);


                Stream stram = Report.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                stram.Seek(0, SeekOrigin.Begin);

                return new FileStreamResult(stram, "application/pdf");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
