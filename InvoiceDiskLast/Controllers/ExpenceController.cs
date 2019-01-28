using CrystalDecisions.CrystalReports.Engine;
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
    public class ExpenceController : Controller
    {
        DBEntities db = new DBEntities();

        // GET: Expence
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List()
        {
            return View();
        }


        [HttpPost]
        public ActionResult GetExpenseList()
        {
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

                if (search == "")
                {
                    search = "NoSearch";
                }
                HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("GetExpenseDetailList122/" + companyId + "/" + search + "/" + skip + "/" + pageSize).Result;
                List<ExpenseViewModel> ExpenseList = response.Content.ReadAsAsync<List<ExpenseViewModel>>().Result;

                return Json(new { draw = draw, recordsFiltered = ExpenseList[0].TotalRecord, recordsTotal = ExpenseList[0].TotalRecord, data = ExpenseList }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Response.Write(ex.ToString());
                return Json(new { draw = 0, recordsFiltered = 0, recordsTotal = 0, data = 0 }, JsonRequestBehavior.AllowGet);
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

        public string PrintView(int Id)
        {
            string pdfname;
            try
            {
                int CompanyId = 0;

                ExpenseViewModel experviewModel = new ExpenseViewModel();

                if (Session["CompayID"] != null)
                {
                    CompanyId = Convert.ToInt32(Session["CompayID"]);
                }

                HttpResponseMessage responseCompany = GlobalVeriables.WebApiClient.GetAsync("APIComapny/" + CompanyId.ToString()).Result;
                MVCCompanyInfoModel companyModel = responseCompany.Content.ReadAsAsync<MVCCompanyInfoModel>().Result;

                CommonModel commonModel = new CommonModel();
                commonModel.Name = "Expense";
                ViewBag.commonModel = commonModel;
                ViewBag.Companydata = companyModel;

                HttpResponseMessage Expense = GlobalVeriables.WebApiClient.GetAsync("GetExpenseById/" + Id).Result;
                experviewModel = Expense.Content.ReadAsAsync<ExpenseViewModel>().Result;

                HttpResponseMessage expenseDetail = GlobalVeriables.WebApiClient.GetAsync("GetExpenseDetailById/" + Id).Result;
                List<ExpenseViewModel> ExpenseDetailList = expenseDetail.Content.ReadAsAsync<List<ExpenseViewModel>>().Result;

                HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("GetExpense/" + CompanyId).Result;
                List<MVCAccountTableModel> AccountmodelObj = response.Content.ReadAsAsync<List<MVCAccountTableModel>>().Result;

                ViewBag.Accounts = AccountmodelObj;
                ViewBag.ExpenseDetail = ExpenseDetailList;

                ViewBag.AccountId = experviewModel.ACCOUNT_ID;
                ViewBag.Ref = experviewModel.REFERENCEno;
                ViewBag.Date = experviewModel.AddedDate;
                ViewBag.VenderId = experviewModel.VENDOR_ID;

                TempData["CompanyId"] = companyModel.CompanyID;
                TempData["ConatctId"] = experviewModel.VENDOR_ID;



                string companyName = Id + "-" + companyModel.CompanyName;

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


        public ActionResult ExpenseByEmail(int Id)
        {
            var List = CreatDirectoryClass.GetFileDirectiory(Id, "Expense");

            EmailModel email = new EmailModel();

            List<Selected> _list = new List<Selected>();

            foreach (var Item in List)
            {
                _list.Add(new Selected { IsSelected = true, FileName = Item.DirectoryPath, Directory = Item.FileFolderPathe + "/" + Item.DirectoryPath });
            }

            email.SelectList = _list;

            try
            {
                email.Attachment = ExportPdf((int)Id);
                HttpContext.Items["FilePath"] = email.Attachment;
                List<Comp> comlist = new List<Comp>();
                comlist = TempData["CompanyId"] as List<Comp>;
                email.EmailText = @".Hierbij ontvangt u onze offerte 10 zoals besproken,." +
                "." + "Graag horen we of u hiermee akkoord gaat." +
                "." + "De offerte vindt u als bijlage bij deze email." +
                "..Met vriendelijke groet." +
                 comlist[0].CompanyName.ToString() + "." +
                 comlist[0].CompanyEmail.ToString();
                string strToProcess = email.EmailText;
                string result = strToProcess.Replace(".", " \r");
                email.EmailText = result;
                email.invoiceId = (int)Id;
                email.From = "infouurtjefactuur@gmail.com";
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return View(email);

        }

        [HttpPost]
        public ActionResult ExpenseByEmail(EmailModel email, string[] Files, FormCollection formCollection)
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
                return RedirectToAction("ViewExpense", new { Id = email.invoiceId });
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



        public ActionResult Add()
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
                CommonModel commonModel = new CommonModel();
                commonModel.Name = "Expense";
                ViewBag.commonModel = commonModel;
                ViewBag.Companydata = companyModel;
            }
            catch (Exception)
            {
                throw;
            }
            return View();
        }



        [HttpGet]
        public ActionResult Edit(int Id = 0)
        {
            int CompanyId = 0;
            ExpenseViewModel experviewModel = new ExpenseViewModel();
            try
            {
                ViewBag.FILE = CreatDirectoryClass.GetFileDirectiory(Id, "Expense");

                if (Session["CompayID"] != null)
                {
                    CompanyId = Convert.ToInt32(Session["CompayID"]);
                }

                ViewBag.VatDrop = GetVatList();

                HttpResponseMessage responseCompany = GlobalVeriables.WebApiClient.GetAsync("APIComapny/" + CompanyId.ToString()).Result;
                MVCCompanyInfoModel companyModel = responseCompany.Content.ReadAsAsync<MVCCompanyInfoModel>().Result;
                CommonModel commonModel = new CommonModel();
                commonModel.Name = "Expense";
                ViewBag.commonModel = commonModel;
                ViewBag.Companydata = companyModel;

                HttpResponseMessage Expense = GlobalVeriables.WebApiClient.GetAsync("GetExpenseById/" + Id).Result;
                experviewModel = Expense.Content.ReadAsAsync<ExpenseViewModel>().Result;

                HttpResponseMessage expenseDetail = GlobalVeriables.WebApiClient.GetAsync("GetExpenseDetailById/" + Id).Result;
                List<ExpenseViewModel> ExpenseDetailList = expenseDetail.Content.ReadAsAsync<List<ExpenseViewModel>>().Result;

                HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("GetExpense/" + CompanyId).Result;
                List<MVCAccountTableModel> AccountmodelObj = response.Content.ReadAsAsync<List<MVCAccountTableModel>>().Result;

                ViewBag.Accounts = AccountmodelObj;
                ViewBag.ExpenseDetail = ExpenseDetailList;

                ViewBag.AccountId = experviewModel.ACCOUNT_ID;
                ViewBag.Ref = experviewModel.REFERENCEno;
                ViewBag.Date = experviewModel.AddedDate;
                ViewBag.VenderId = experviewModel.VENDOR_ID;
            }
            catch (Exception)
            {

                throw;
            }

            if (Session["CompayID"] != null)
            {
                CompanyId = Convert.ToInt32(Session["CompayID"]);
            }


            return View(experviewModel);
        }


        public List<VatModel> GetVatList()
        {
            List<VatModel> model = new List<VatModel>();
            model.Add(new VatModel() { Vat1 = 0, Name = "0" });
            model.Add(new VatModel() { Vat1 = 6, Name = "6" });
            model.Add(new VatModel() { Vat1 = 21, Name = "21" });
            return model;
        }



        [HttpPost]
        public ActionResult DeleteInvoice(int Id, int ExpenseDetailId, int vat, float total)
        {
            try
            {
                HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("GetExpenseById/" + Id).Result;
                ExpenseViewModel ExpenseMosel = response.Content.ReadAsAsync<ExpenseViewModel>().Result;
                double ResultVAT = CommonController.CalculateVat(vat, total);

                ExpenseMosel.GRAND_TOTAL = Convert.ToDouble(ExpenseMosel.GRAND_TOTAL) - Convert.ToDouble(ResultVAT);

                ExpenseMosel.GRAND_TOTAL = Convert.ToDouble(ExpenseMosel.GRAND_TOTAL) - Convert.ToDouble(total);

                ExpenseMosel.VAT_AMOUNT = ExpenseMosel.VAT_AMOUNT - Convert.ToDouble(vat);
                ExpenseMosel.SUBTOTAL = ExpenseMosel.SUBTOTAL - Convert.ToDouble(total);

                if (vat == 6)
                {
                    ExpenseMosel.Vat6 = (ExpenseMosel.Vat6) - Convert.ToDouble(ResultVAT);
                }

                if (vat == 21)
                {
                    ExpenseMosel.Vat21 = (ExpenseMosel.Vat21) - Convert.ToDouble(ResultVAT);
                }

                response = GlobalVeriables.WebApiClient.PutAsJsonAsync("PutExpense/" + ExpenseMosel.Id, ExpenseMosel).Result;
                EXPENSE exp = response.Content.ReadAsAsync<EXPENSE>().Result;

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    HttpResponseMessage deleteExpenseDetail = GlobalVeriables.WebApiClient.DeleteAsync("DeleteExpenseDetails/" + ExpenseDetailId).Result;

                    if (deleteExpenseDetail.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        return new JsonResult { Data = new { Status = "Success" } };
                    }
                }
                return new JsonResult { Data = new { Status = "Fail" } };
            }
            catch (Exception ex)
            {
                return new JsonResult { Data = new { Status = "Fail" } };
            }
        }



        [HttpPost]
        public ActionResult UploadFiles(ExpenseViewModel MVCQutationViewModel, HttpPostedFileWrapper[] file23)
        {
            try
            {
                string FileName = "";
                HttpFileCollectionBase files = Request.Files;
                for (int i = 0; i < files.Count; i++)
                {
                    HttpPostedFileBase file = files[i];
                    FileName = CreatDirectoryClass.UploadFileToDirectoryCommon(MVCQutationViewModel.Id, "Expense", file23, "Expense");
                }
                return new JsonResult { Data = new { FilePath = FileName, FileName = FileName } };
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
                if (CreatDirectoryClass.Delete(Id, FileName, "Expense"))
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
        public ActionResult Edit(ExpenseViewModel _ExpeseViewModel)
        {

            HttpResponseMessage response = new HttpResponseMessage();

            try
            {
                EXPENSE expense = new EXPENSE();
                expense.REFERENCEno = _ExpeseViewModel.REFERENCEno;
                expense.ACCOUNT_ID = _ExpeseViewModel.ACCOUNT_ID;
                expense.VENDOR_ID = _ExpeseViewModel.VENDOR_ID;
                expense.Id = _ExpeseViewModel.Id;
                expense.notes = _ExpeseViewModel.notes;
                expense.user_id = _ExpeseViewModel.user_id;
                expense.SUBTOTAL = _ExpeseViewModel.SUBTOTAL;
                expense.VAT_AMOUNT = _ExpeseViewModel.VAT_AMOUNT;
                expense.GRAND_TOTAL = _ExpeseViewModel.GRAND_TOTAL;
                expense.AddedDate = _ExpeseViewModel.AddedDate;
                expense.comapny_id = _ExpeseViewModel.comapny_id;
                expense.Vat6 = _ExpeseViewModel.Vat6;
                expense.Vat21 = _ExpeseViewModel.Vat21;

                response = GlobalVeriables.WebApiClient.PutAsJsonAsync("PutExpense/" + _ExpeseViewModel.Id, expense).Result;

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    if (_ExpeseViewModel.ExpensenDetailList != null)
                    {
                        foreach (ExpenseDetail item in _ExpeseViewModel.ExpensenDetailList)
                        {
                            ExpenseDetail expenseDetailModel = new ExpenseDetail();
                            expenseDetailModel.Id = item.Id;
                            expenseDetailModel.expense_id = _ExpeseViewModel.Id;
                            expenseDetailModel.EXPENSE_ACCOUNT_ID = item.EXPENSE_ACCOUNT_ID;
                            expenseDetailModel.DESCRIPTION = item.DESCRIPTION;
                            expenseDetailModel.AMOUNT = item.AMOUNT;
                            expenseDetailModel.TAX_PERCENT = item.TAX_PERCENT;
                            expenseDetailModel.TAX_AMOUNT = item.TAX_AMOUNT;
                            expenseDetailModel.SUBTOTAL = item.SUBTOTAL;
                            expenseDetailModel.user_id = _ExpeseViewModel.user_id;
                            expenseDetailModel.comapny_id = _ExpeseViewModel.comapny_id;

                            if (item.Id == 0)
                            {
                                response = GlobalVeriables.WebApiClient.PostAsJsonAsync("PostExpenseDetail", expenseDetailModel).Result;

                                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                                {
                                    return new JsonResult { Data = new { Status = "Fail" } };
                                }
                            }
                            else
                            {
                                response = GlobalVeriables.WebApiClient.PutAsJsonAsync("PutExpenseDetail/" + _ExpeseViewModel.Id, expenseDetailModel).Result;

                                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                                {
                                    return new JsonResult { Data = new { Status = "Fail" } };
                                }
                            }
                        }

                        if (response.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            return new JsonResult { Data = new { Status = "Success" } };
                        }
                    }
                }
                else
                {
                    return Json("Fail", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return Json("", JsonRequestBehavior.AllowGet);

        }



        public string ExportPdf(int Id)
        {
            string pdfname = "";

            DBEntities db2 = new DBEntities();
            try
            {
                List<ExpenseModel> _ExpenseList = new List<ExpenseModel>();
                List<ExpenseDetailModel> _ExpenseDetailList = new List<ExpenseDetailModel>();


                _ExpenseList = db2.EXPENSEs.Where(Ex => Ex.Id == Id).Select(c => new ExpenseModel
                {
                    Id = c.Id,
                    REFERENCEno = c.REFERENCEno,
                    ACCOUNT_ID = c.ACCOUNT_ID ?? 0,
                    VENDOR_ID = c.VENDOR_ID ?? 0,
                    notes = c.notes,
                    VenderAccount = c.ContactsTable.ContactName,
                    SUBTOTAL = c.SUBTOTAL ?? 0,
                    VAT_AMOUNT = c.VAT_AMOUNT ?? 0,
                    GRAND_TOTAL = c.GRAND_TOTAL ?? 0,
                    comapny_id = c.comapny_id ?? 0,
                    user_id = 1,
                    AccountName = "ferferfg",
                    AddedDate = c.AddedDate.ToString(),

                }).ToList();



                //TempData["ConatctId"] = QutationModel.ContactId;




                _ExpenseDetailList = db2.ExpenseDetails.Where(C => C.expense_id == Id).Select(C => new ExpenseDetailModel
                {
                    Id = C.Id,
                    EXPENSE_ACCOUNT_ID = C.EXPENSE_ACCOUNT_ID ?? 0,
                    DESCRIPTION = C.DESCRIPTION,
                    AMOUNT = C.AMOUNT ?? 0,
                    TAX_PERCENT = C.TAX_PERCENT ?? 0,
                    TAX_AMOUNT = C.TAX_AMOUNT ?? 0,
                    SUBTOTAL = C.SUBTOTAL ?? 0,
                    user_id = C.user_id ?? 0,
                    comapny_id = C.comapny_id ?? 0,
                    expense_id = C.expense_id ?? 0,

                    AccountTitle = C.AccountTable.AccountTitle,
                }).ToList();




                List<Comp> info = db2.ComapnyInfoes.Where(x => x.CompanyID == 54).Select(c => new Comp
                {
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


                TempData["CompanyId"] = info;

                string Name = info[0].CompanyLogo;
                info[0].CompanyLogo = Server.MapPath("~/images/" + Name);

                ReportDocument Report = new ReportDocument();

                Report.Load(Server.MapPath("~/CrystalReport/ExpenceReport.rpt"));

                Report.Database.Tables[0].SetDataSource(info);
                Report.Database.Tables[1].SetDataSource(_ExpenseList);
                Report.Database.Tables[2].SetDataSource(_ExpenseDetailList);
                string companyName = Id + "-" + info[0].CompanyName;

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
                            FileInfo inf = new FileInfo(path);

                            if (!IsFileLocked(inf)) inf.Delete();

                        }
                    }
                    catch (System.IO.IOException)
                    {

                    }
                }
                Report.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, path);
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return pdfname;
        }




        public ActionResult ViewExpense(int Id)
        {

            ViewBag.FILE = CreatDirectoryClass.GetFileDirectiory(Id, "Expense");

            int CompanyId = 0;
            ExpenseViewModel experviewModel = new ExpenseViewModel();

            try
            {

                ViewBag.VatDrop = GetVatList();

                if (Session["CompayID"] != null)
                {
                    CompanyId = Convert.ToInt32(Session["CompayID"]);
                }

                HttpResponseMessage responseCompany = GlobalVeriables.WebApiClient.GetAsync("APIComapny/" + CompanyId.ToString()).Result;
                MVCCompanyInfoModel companyModel = responseCompany.Content.ReadAsAsync<MVCCompanyInfoModel>().Result;

                CommonModel commonModel = new CommonModel();
                commonModel.Name = "Expense";
                ViewBag.commonModel = commonModel;
                ViewBag.Companydata = companyModel;

                HttpResponseMessage Expense = GlobalVeriables.WebApiClient.GetAsync("GetExpenseById/" + Id).Result;
                experviewModel = Expense.Content.ReadAsAsync<ExpenseViewModel>().Result;

                HttpResponseMessage expenseDetail = GlobalVeriables.WebApiClient.GetAsync("GetExpenseDetailById/" + Id).Result;
                List<ExpenseViewModel> ExpenseDetailList = expenseDetail.Content.ReadAsAsync<List<ExpenseViewModel>>().Result;

                HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("GetExpense/" + CompanyId).Result;
                List<MVCAccountTableModel> AccountmodelObj = response.Content.ReadAsAsync<List<MVCAccountTableModel>>().Result;


                ViewBag.Accounts = AccountmodelObj;
                ViewBag.ExpenseDetail = ExpenseDetailList;

                ViewBag.AccountId = experviewModel.ACCOUNT_ID;
                ViewBag.Ref = experviewModel.REFERENCEno;
                ViewBag.Date = experviewModel.AddedDate;
                ViewBag.VenderId = experviewModel.VENDOR_ID;

                return View(experviewModel);
            }
            catch (Exception)
            {

                throw;
            }
        }


        [HttpPost]
        public ActionResult AddExpence(ExpenseViewModel ExpenseViewModel, HttpPostedFileWrapper[] file23)
        {
            HttpResponseMessage response = new HttpResponseMessage();
         
            int ExpenseId = 0;
            EXPENSE expense = new EXPENSE();
            try
            {
                if (Session["LoginUserID"] != null)
                {
                    ExpenseViewModel.user_id = Convert.ToInt32(Session["LoginUserID"]);
                }

              
                response = GlobalVeriables.WebApiClient.PostAsJsonAsync("PostExpense", ExpenseViewModel).Result;
                ExpenseModel Purchasetable = response.Content.ReadAsAsync<ExpenseModel>().Result;
                ExpenseId = Purchasetable.Id;
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    if (file23[0] != null)
                    {
                        CreatDirectoryClass.UploadFileToDirectoryCommon(ExpenseViewModel.Id, "Expense", file23, "Expense");
                    }
                }
            }
            catch (Exception ex)
            {

                return new JsonResult { Data = new { Status = "Fail", Message = ex.Message.ToString() } };
            }
            return new JsonResult { Data = new { Status = "Success", Id = ExpenseId } };



        }

    }
}
