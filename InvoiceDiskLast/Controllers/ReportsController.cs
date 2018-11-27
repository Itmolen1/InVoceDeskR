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
    //[SessionExpireAttribute]
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

                string companyName = ReportName + "-" + FromDate.ToString("yyyy-MM-dd") + "-" + Todate.ToString("yyyy-MM-dd");
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
                    _searchModel._TransactionList = null;
                    return View(_searchModel);
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

            return View(_model);
        }



        int Contectid, CompanyID = 0;




        public ActionResult Journal()
        {

            if (Session["CompayID"] != null)
            {
                CompanyId = Convert.ToInt32(Session["CompayID"]);

                HttpResponseMessage responseCompany = GlobalVeriables.WebApiClient.GetAsync("APIComapny/" + CompanyId.ToString()).Result;
                _company = responseCompany.Content.ReadAsAsync<MVCCompanyInfoModel>().Result;
                TempData["Compantinfo"] = _company;
                TempData.Keep();
            }


            _SearchModel.FromDate = DateTime.Now;
            _SearchModel.Todate = DateTime.Now.AddDays(-20);
            long FromDate = DateTime.Now.Ticks;
            DateTime dt = DateTime.Now.AddDays(-20);
            long TDate = DateTime.Now.AddDays(-20).Ticks;

            try
            {

                HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("GetJournal/" + FromDate + "/" + TDate).Result;
                _SearchModel._TransactionList = response.Content.ReadAsAsync<List<TransactionModel>>().Result;
                _SearchModel.Todate = DateTime.Now.AddDays(-20);
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

                string PdfName = "journal" + "-" + _company.CompanyName + ".pdf";

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



        public ActionResult ReportByEmail()
        {
            try
            {
                SearchModel _SearcModel = new SearchModel();


            }
            catch (Exception)
            {

                throw;
            }

            return View();
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
                        listAc.Add(new Control_Head_Account_tran_ViewModel {ControleAccountTitile = item.ControlAccountTable.ControleAccountTitile, HeadAccountId = item.HeadAccountId, HeadAccountTitle = item.HeadAccountTitle });
                        foreach (var items in LAT.Where(x => x.FK_HeadAccountId == item.HeadAccountId))
                        {
                            AC = items.AccountTitle;
                            listAc.Add(new Control_Head_Account_tran_ViewModel {ControleAccountTitile = itmen.ControleAccountTitile, AccountId = items.AccountId, AccountTitle = items.AccountTitle });

                           //listAc2 = listAc.Where()

                            foreach (var itemt in LTT.Where(x => x.FK_AccountID == items.AccountId))
                            {

                                int id = (int)items.AccountId;

                                drr = drr+Convert.ToDouble(itemt.Dr);
                                Crr = Crr+ Convert.ToDouble(itemt.Cr);
                                Actitle = itemt.AccountTable.AccountTitle;
                               
                            }

                           
                            if(listAc2.Any(c => c.HeadAccountTitle == HA)) 
                            {
                                HA = "";
                            }


                            listAc2.Add(new Control_Head_Account_tran_ViewModel { ControleAccountTitile = Ca,HeadAccountTitle = HA, AccountTitle = AC, AmountDebit = drr, AmountCredit = Crr });

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