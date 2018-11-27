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
    public class ReportsController : Controller
    {
        MVCCompanyInfoModel _company = new MVCCompanyInfoModel();
        SearchModel _SearchModel = new SearchModel();
        int CompanyId = 0;

        // GET: Reports
        public ActionResult Index()
        {
            return View();
        }

        public string SaveOnPathe(string ReportName, DateTime FromDate, DateTime Todate)
        {

            TempData["FromDate"] = FromDate;
            TempData["ToDate"] = Todate;

            string pdfname = "";

            try
            {
                CompanyID = Convert.ToInt32(Session["CompayID"]);
                long FromDa = Convert.ToDateTime(FromDate).Ticks;
                long TDate = Convert.ToDateTime(Todate).Ticks;


                HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("GetJournal/" + FromDa + "/" + TDate).Result;
                ViewBag.JournalList = response.Content.ReadAsAsync<List<TransactionModel>>().Result;

                CompanyId = Convert.ToInt32(Session["CompayID"]);

                HttpResponseMessage responseCompany = GlobalVeriables.WebApiClient.GetAsync("APIComapny/" + CompanyId.ToString()).Result;
                _company = responseCompany.Content.ReadAsAsync<MVCCompanyInfoModel>().Result;
                TempData["Compantinfo"] = _company;
                TempData.Keep();

                string companyName = ReportName + "-" + FromDate + "-" + Todate + ".pdf";
                var root = Server.MapPath("/PDF/");
                var path = Path.Combine(root, companyName);
                pdfname = path;
                string subPath = "/PDF";
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
                            pdfname = path;

                            if (!IsFileLocked(info)) info.Delete();
                        }
                    }
                    catch (System.IO.IOException e)
                    {

                    }

                }

                var pdfResult = new Rotativa.PartialViewAsPdf("~/Views/Reports/JournalPartialView.cshtml")
                {
                    PageSize = Rotativa.Options.Size.A4,
                    MinimumFontSize = 16,
                    PageMargins = new Rotativa.Options.Margins(10, 12, 20, 3),
                    PageHeight = 40,
                    SaveOnServerPath = path,

                };
                pdfResult.BuildPdf(this.ControllerContext);
            }

            catch (Exception EX)
            {

                throw EX;
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
        public ActionResult Journal(SearchModel _searchModel)
        {
            SearchModel _model = new SearchModel();
            try
            {




                if (_searchModel.FromDate > _searchModel.Todate)
                {
                    ViewBag.massage = "From Date must be Less from To Date";

                    if (TempData["Compantinfo"] == null)
                    {
                        CompanyId = Convert.ToInt32(Session["CompayID"]);

                        HttpResponseMessage responseCompany = GlobalVeriables.WebApiClient.GetAsync("APIComapny/" + CompanyId.ToString()).Result;
                        _company = responseCompany.Content.ReadAsAsync<MVCCompanyInfoModel>().Result;
                        TempData["Compantinfo"] = _company;
                        TempData.Keep();
                    }


                    _model.FromDate = _searchModel.FromDate;
                    _model.Todate = _searchModel.Todate;

                    return View(_model);
                }
                else
                {

                    FormCollection form;
                    if (Request.Form["SendEmail"] != null)
                    {
                        string Path = SaveOnPathe("Journal", _searchModel.FromDate, _searchModel.Todate);

                        TempData["Pathe"] = Path;
                        TempData.Keep();

                        return RedirectToAction("ReportByEmail");

                    }

                    long FromDate = Convert.ToDateTime(_searchModel.FromDate).Ticks;
                    long TDate = Convert.ToDateTime(_searchModel.Todate).Ticks;

                    HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("GetJournal/" + FromDate + "/" + TDate).Result;
                    _model._TransactionList = response.Content.ReadAsAsync<List<TransactionModel>>().Result;

                    if (TempData["Compantinfo"] == null)
                    {
                        CompanyId = Convert.ToInt32(Session["CompayID"]);

                        HttpResponseMessage responseCompany = GlobalVeriables.WebApiClient.GetAsync("APIComapny/" + CompanyId.ToString()).Result;
                        _company = responseCompany.Content.ReadAsAsync<MVCCompanyInfoModel>().Result;
                        TempData["Compantinfo"] = _company;
                        TempData.Keep();
                    }
                }
            }
            catch (Exception EX)
            {
                throw EX;
            }

            _model.FromDate = _searchModel.FromDate;
            _model.Todate = _searchModel.Todate;


            return View(_model);
        }


        int Contectid, CompanyID = 0;


        public ActionResult Journal()
        {
            long lFromDate, lTDate = 0;

            if (Session["CompayID"] != null)
            {
                CompanyId = Convert.ToInt32(Session["CompayID"]);

                HttpResponseMessage responseCompany = GlobalVeriables.WebApiClient.GetAsync("APIComapny/" + CompanyId.ToString()).Result;
                _company = responseCompany.Content.ReadAsAsync<MVCCompanyInfoModel>().Result;
                TempData["Compantinfo"] = _company;
                TempData.Keep();
            }
            if (TempData["FromDate"] != null)
            {
                _SearchModel.FromDate = (DateTime)TempData["FromDate"];
                TempData["FromDate"] = null;
            }
            else
            {
                _SearchModel.FromDate = DateTime.Now;
            }
            if (TempData["ToDate"] != null)
            {
                _SearchModel.Todate = (DateTime)TempData["Todate"];
                TempData["ToDate"] = null;
            }
            else
            {
                _SearchModel.Todate = DateTime.Now;
            }

            lFromDate = _SearchModel.FromDate.Ticks;
            lTDate = _SearchModel.Todate.Ticks;
            try
            {
                HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("GetJournal/" + lFromDate + "/" + lTDate).Result;
                _SearchModel._TransactionList = response.Content.ReadAsAsync<List<TransactionModel>>().Result;

                return View(_SearchModel);
            }
            catch (Exception)
            {

                throw;
            }


            return View();
        }



        public ActionResult PrintJournal(DateTime? FromDate, DateTime? Todate)
        {
            try
            {
                var idd = Session["ClientID"];
                var cdd = Session["CompayID"];

                if (Session["CompayID"] != null)
                {
                    CompanyID = Convert.ToInt32(Session["CompayID"]);
                }
                else
                {
                    return RedirectToAction("Index", "Login");
                }
                long FromDa = Convert.ToDateTime(FromDate).Ticks;
                long TDate = Convert.ToDateTime(Todate).Ticks;

                HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("GetJournal/" + FromDa + "/" + TDate).Result;
                ViewBag.JournalList = response.Content.ReadAsAsync<List<TransactionModel>>().Result;

                if (TempData["Compantinfo"] == null)
                {
                    CompanyId = Convert.ToInt32(Session["CompayID"]);

                    HttpResponseMessage responseCompany = GlobalVeriables.WebApiClient.GetAsync("APIComapny/" + CompanyId.ToString()).Result;
                    _company = responseCompany.Content.ReadAsAsync<MVCCompanyInfoModel>().Result;
                    TempData["Compantinfo"] = _company;
                    TempData.Keep();
                }

                string PdfName = "journal" + "-" + FromDate + "-" + Todate + ".pdf";

                return new Rotativa.PartialViewAsPdf("~/Views/Reports/JournalPartialView.cshtml")
                {
                    PageSize = Rotativa.Options.Size.A4,
                    MinimumFontSize = 16,

                    FileName = PdfName,
                    PageHeight = 40,
                    PageMargins = new Rotativa.Options.Margins(10, 12, 20, 3)
                };
            }


            catch (Exception)
            {

                throw;
            }

            return View();
        }


        EmailModel EmailModel = new EmailModel();
        public ActionResult ReportByEmail()
        {

            try
            {

                if (TempData["Pathe"] != null)
                {
                    EmailModel.Attachment = TempData["Pathe"].ToString();
                }


                if (Session["CompayID"] == null)
                {
                    return RedirectToAction("Index", "Login");
                }

                var CompanyName = Session["CompanyName"];

                var contact = Session["CompanyContact"];

                var companyEmail = Session["CompanyEmail"];

                if (companyEmail == null)
                {
                    companyEmail = "Company Email";
                }

                if (CompanyName == null)
                {
                    CompanyName = "Nocompany";
                }
                EmailModel.EmailText = @"Geachte heer" + "Boss" + "." +
                ".Hierbij ontvangt u onze offerte 10 zoals besproken,." +
                "." + "Graag horen we of u hiermee akkoord gaat." +
                "." + "De offerte vindt u als bijlage bij deze email." +
                "..Met vriendelijke groet." +
                "Conatct Name" + "." +
                CompanyName.ToString() + "." +
                companyEmail.ToString();
                string strToProcess = EmailModel.EmailText;
                string result = strToProcess.Replace(".", " \r\n");
                EmailModel.EmailText = result;
                EmailModel.From = "infouurtjefactuur@gmail.com";

            }
            catch (Exception)
            {

            }
            return View(EmailModel);
        }





        [HttpPost]
        public ActionResult ReportByEmail(EmailModel email)
        {

            var idd = Session["ClientID"];
            var cdd = Session["CompayID"];
            if (Session["ClientID"] != null && Session["CompayID"] != null)
            {
                Contectid = Convert.ToInt32(Session["ClientID"]);
                CompanyID = Convert.ToInt32(Session["CompayID"]);
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

                    return RedirectToAction("Journal");
                }
            }
            catch (Exception ex)
            {

                TempData["Message"] = "Email Send Succssfully";
                email.Attachment = fileName;
            }
            return View(email);

        }


        public ActionResult BalanceSheet()
        {
            return View();
        }



        public class modelcheck
        {
            public List<ControlAccountTable> CAT { get; set; }
            public List<HeadAccountTable> HAT { get; set; }
            public List<AccountTable> AAT { get; set; }
            public List<AccountTransictionTable> ATT { get; set; }
        }

        [HttpGet]
        public ActionResult BalanceSheetbyDate()
        {

            #region
            List<Control_Head_Account_tran_ViewModel> listAc = new List<Control_Head_Account_tran_ViewModel>();
            List<Control_Head_Account_tran_ViewModel> listAc2 = new List<Control_Head_Account_tran_ViewModel>();


            List<ControlAccountTable> LCAT = new List<ControlAccountTable>();
            List<HeadAccountTable> LHT = new List<HeadAccountTable>();
            List<AccountTable> LAT = new List<AccountTable>();
            List<AccountTransictionTable> LTT = new List<AccountTransictionTable>();

            List<object> acc = new List<object>();

            #endregion
            try
            {

                DBEntities db = new DBEntities();

                LCAT = db.ControlAccountTables.ToList();

                LHT = db.HeadAccountTables.ToList();
                LAT = db.AccountTables.ToList();
                LTT = db.AccountTransictionTables.ToList();

                double drr = 0.00;
                double Crr = 0.00;
                string Actitle = "";
                string Ca = "";
                string HA = "";
                string AC = "";
                foreach (var itmen in LCAT)
                {
                    listAc.Add(new Control_Head_Account_tran_ViewModel { ControlAccountId = itmen.ControlAccountId, ControleAccountTitile = itmen.ControleAccountTitile });


                    Ca = itmen.ControleAccountTitile;


                    foreach (var item in LHT.Where(x => x.FK_ControlAccountID == itmen.ControlAccountId))
                    {
                        HA = item.HeadAccountTitle;
                        listAc.Add(new Control_Head_Account_tran_ViewModel { ControleAccountTitile = item.ControlAccountTable.ControleAccountTitile, HeadAccountId = item.HeadAccountId, HeadAccountTitle = item.HeadAccountTitle });
                        foreach (var items in LAT.Where(x => x.FK_HeadAccountId == item.HeadAccountId))
                        {
                            AC = items.AccountTitle;
                            listAc.Add(new Control_Head_Account_tran_ViewModel { ControleAccountTitile = itmen.ControleAccountTitile, AccountId = items.AccountId, AccountTitle = items.AccountTitle });

                            //listAc2 = listAc.Where()

                            foreach (var itemt in LTT.Where(x => x.FK_AccountID == items.AccountId))
                            {

                                int id = (int)items.AccountId;

                                drr = drr + Convert.ToDouble(itemt.Dr);
                                Crr = Crr + Convert.ToDouble(itemt.Cr);
                                Actitle = itemt.AccountTable.AccountTitle;

                            }


                            if (listAc2.Any(c => c.HeadAccountTitle == HA))
                            {
                                HA = "";
                            }


                            listAc2.Add(new Control_Head_Account_tran_ViewModel { ControleAccountTitile = Ca, HeadAccountTitle = HA, AccountTitle = AC, AmountDebit = drr, AmountCredit = Crr });

                        }
                    }
                }
                List<Control_Head_Account_tran_ViewModel> listAcz = new List<Control_Head_Account_tran_ViewModel>();
                //listAcz = listAc.Where(c => c.ControleAccountTitile != null && c.HeadAccountTitle != null && c.AccountTitle != null).ToList();
                //return View(listAc);
                return Json(listAc2, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ex.ToString();

            }

            return Json(listAc2, JsonRequestBehavior.AllowGet);
        }
    }
}