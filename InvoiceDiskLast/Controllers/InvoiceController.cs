using CrystalDecisions.CrystalReports.Engine;
using InvoiceDiskLast.MISC;
using InvoiceDiskLast.Models;
using Logger;
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
    //[RouteNotFoundAttribute]

    public class InvoiceController : Controller
    {
        private Ilog _iLog;
        public InvoiceController()
        {
            _iLog = Log.GetInstance;
        }
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

        //[HttpPost]
        //public ActionResult AddCustomer(MVCQutationViewModel model)
        //{
        //    string fileName;

        //    for (int i = 0; i < Request.Files.Count; i++)
        //    {
        //        HttpPostedFileBase file = Request.Files[i]; 
        //        int fileSize = file.ContentLength;
        //        fileName = file.FileName;
        //        string mimeType = file.ContentType;
        //        System.IO.Stream fileContent = file.InputStream;              
        //        file.SaveAs(Server.MapPath("~/") + fileName); //File will be saved in application root
        //    }
        //    return null;
        //}

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

                DateTime InvoiceDateDate = new DateTime();
                InvoiceDateDate = DateTime.Now.Date;

                int paymentTerm = 0;
                if (contectmodel.PaymentTerm.ToString() != "")
                {
                    paymentTerm = Convert.ToInt32(contectmodel.PaymentTerm);
                }
                else
                {
                    paymentTerm = 15;
                }
                CommonModel commonModel = new CommonModel();
                commonModel.Name = "Invoice";

                ViewBag.Contentdata = contectmodel;
                ViewBag.Companydata = companyModel;
                commonModel.FromDate = InvoiceDateDate;

                commonModel.DueDate = InvoiceDateDate.AddDays(+paymentTerm);

                MVCInvoiceModel InvoiceCount = new MVCInvoiceModel();
                HttpResponseMessage response1 = GlobalVeriables.WebApiClient.GetAsync("GetInvoiceCount/").Result;
                InvoiceCount = response1.Content.ReadAsAsync<MVCInvoiceModel>().Result;
                invoioceViewModel.Invoice_ID = InvoiceCount.Invoice_ID;

                commonModel.Number_Id = InvoiceCount.Invoice_ID;

                ViewBag.commonModel = commonModel;
                invoioceViewModel.ContactId = id;
                invoioceViewModel.CompanyId = CompanyID;
                invoioceViewModel.InvoiceDescription = "Graag betalen voor de uiterste betaaldatum onder vermelding van het factuurnummer";
                return View(invoioceViewModel);
            }
            catch (Exception ex)
            {
                return null;
            }

        }


        public bool AddTransaction(InvoiceViewModel invoiceViewModel)
        {

            bool result = false;
            try
            {
                TransactionModel model = new TransactionModel();
                model.CompanyId = (int)invoiceViewModel.CompanyId;
                model.AccountTitle = "Accounts receivable";
                int AccountId = CommonController.GetAcccountId(model);
                if (AccountId > 0)
                {
                    AccountTransictionTable _TransactionTable = new AccountTransictionTable();
                    _TransactionTable.FK_AccountID = AccountId;
                    _TransactionTable.Dr = invoiceViewModel.SubTotal;
                    _TransactionTable.Cr = 0;
                    _TransactionTable.TransictionNumber = Guid.NewGuid().ToString();
                    _TransactionTable.TransictionRefrenceId = invoiceViewModel.InvoiceID.ToString();
                    _TransactionTable.CreationTime = DateTime.Now.TimeOfDay;
                    _TransactionTable.AddedBy = Convert.ToInt32(Session["LoginUserID"]);
                    _TransactionTable.FK_CompanyId = invoiceViewModel.CompanyId;
                    _TransactionTable.TransictionType = "Dr";
                    _TransactionTable.FKPaymentTerm = null;
                    _TransactionTable.TransictionDate = DateTime.Now;
                    _TransactionTable.Description = "invoice Creating Time Transaction";

                    if (TransactionClass.PerformTransaction(_TransactionTable))
                    {
                        result = true;
                        model.AccountTitle = "Input VAT";
                        int AccountId1 = CommonController.GetAcccountId(model);
                        _TransactionTable.FK_AccountID = AccountId1;
                        _TransactionTable.Dr = invoiceViewModel.TotalVat21 + invoiceViewModel.TotalVat6;

                        if (TransactionClass.PerformTransaction(_TransactionTable))
                        {
                            result = true;
                            model.AccountTitle = "Cash on hand";
                            int AccountId12 = CommonController.GetAcccountId(model);
                            _TransactionTable.FK_AccountID = AccountId12;
                            _TransactionTable.Cr = invoiceViewModel.TotalAmount;
                            _TransactionTable.Dr = 0;
                            _TransactionTable.TransictionType = "Cr";
                            if (TransactionClass.PerformTransaction(_TransactionTable))
                            {
                                result = true;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            return result;

        }


        public bool Transaction(InvoiceViewModel invoiceViewModel, string TransType)
        {
            bool result = false;

            try
            {
                if (TransType == "Add")
                {
                    if (AddTransaction(invoiceViewModel))
                    {
                        return result = true;
                    }
                    else
                    {
                        return result = false;
                    }
                }

                else
                {
                    if (CommonController.DeleteFromTransactionTableByRefrenceId((int)invoiceViewModel.InvoiceID))
                    {
                        if (AddTransaction(invoiceViewModel))
                        {
                            return result = true;
                        }
                        else
                        {
                            return result = false;
                        }

                    }

                }

            }
            catch (Exception)
            {
                result = false;
                throw;
            }
            return result;

        }

        [HttpPost]
        public ActionResult SaveDraft(InvoiceViewModel invoiceViewModel, HttpPostedFileBase filess)
        {
            HttpResponseMessage responsses = new HttpResponseMessage();
            InvoiceTable InvoiceTable;
            MVCInvoiceModel mvcInvoiceModel = new MVCInvoiceModel();
            try
            {
                mvcInvoiceModel.Invoice_ID = invoiceViewModel.Invoice_ID;
                mvcInvoiceModel.CompanyId = invoiceViewModel.CompanyId;
                mvcInvoiceModel.UserId = Convert.ToInt32(Session["LoginUserID"]);
                mvcInvoiceModel.ContactId = invoiceViewModel.ContactId;
                mvcInvoiceModel.InvoiceID = 0;
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
                mvcInvoiceModel.InvoiceDescription = invoiceViewModel.InvoiceDescription;

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
                HttpResponseMessage response = GlobalVeriables.WebApiClient.PostAsJsonAsync("PostInvoice", mvcInvoiceModel).Result;
                InvoiceTable = response.Content.ReadAsAsync<InvoiceTable>().Result;

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    invoiceViewModel.InvoiceID = InvoiceTable.InvoiceID;

                    if (invoiceViewModel.InvoiceDetailsTable != null)
                    {
                        foreach (InvoiceDetailsTable InvoiceDetailsList in invoiceViewModel.InvoiceDetailsTable)
                        {
                            InvoiceDetailsTable InvoiceDetails = new InvoiceDetailsTable();
                            InvoiceDetails.ItemId = Convert.ToInt32(InvoiceDetailsList.ItemId);
                            InvoiceDetails.InvoiceId = InvoiceTable.InvoiceID;
                            InvoiceDetails.Description = InvoiceDetailsList.Description;
                            InvoiceDetails.Quantity = InvoiceDetailsList.Quantity;
                            InvoiceDetails.Rate = Convert.ToDouble(InvoiceDetailsList.Rate);
                            InvoiceDetails.Total = Convert.ToDouble(InvoiceDetailsList.Total);
                            InvoiceDetails.ServiceDate = InvoiceDetailsList.ServiceDate;
                            InvoiceDetails.RowSubTotal = InvoiceDetailsList.RowSubTotal;
                            InvoiceDetails.Vat = Convert.ToDouble(InvoiceDetailsList.Vat);
                            InvoiceDetails.Type = InvoiceDetailsList.Type;

                            if (InvoiceDetails.InvoiceDetailId == 0)
                            {
                                responsses = GlobalVeriables.WebApiClient.PostAsJsonAsync("PostinvoiceDetails", InvoiceDetails).Result;
                            }
                            else
                            {
                                responsses = GlobalVeriables.WebApiClient.PutAsJsonAsync("PostinvoiceDetails/" + InvoiceDetails.InvoiceDetailId, InvoiceDetails).Result;
                            }

                        }
                        if (responsses.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            if (Transaction(invoiceViewModel, "Add"))
                            {
                                //return new JsonResult { Data = new { Status = "Success", id = InvoiceTable.InvoiceID } };
                            }
                            else
                            {
                                return new JsonResult { Data = new { Status = "Fail", id = InvoiceTable.InvoiceID } };
                            }

                        }
                        if (invoiceViewModel.file23[0] != null)
                        {
                            CreatDirectoryClass.UploadFileToDirectoryCommon(InvoiceTable.InvoiceID, "Invoice", invoiceViewModel.file23, "Invoice");
                        }
                    }

                    
                }
            }
            catch (Exception ex)
            {

                return new JsonResult { Data = new { Status = "Fail", Message = ex.Message.ToString() } };
            }

            return new JsonResult { Data = new { Status = "Success", path = "", id = InvoiceTable.InvoiceID } };
        }

        [HttpPost]
        public ActionResult SavePrint(InvoiceViewModel invoiceViewModel, HttpPostedFileBase filess)
        {
            HttpResponseMessage responsses = new HttpResponseMessage();
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
                mvcInvoiceModel.InvoiceDescription = invoiceViewModel.InvoiceDescription;

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

                HttpResponseMessage response = GlobalVeriables.WebApiClient.PostAsJsonAsync("PostInvoice", mvcInvoiceModel).Result;
                InvoiceTable = response.Content.ReadAsAsync<InvoiceTable>().Result;

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    invoiceViewModel.InvoiceID = InvoiceTable.InvoiceID;

                    if (invoiceViewModel.InvoiceDetailsTable != null)
                    {
                        foreach (InvoiceDetailsTable InvoiceDetailsList in invoiceViewModel.InvoiceDetailsTable)
                        {
                            InvoiceDetailsTable InvoiceDetails = new InvoiceDetailsTable();
                            InvoiceDetails.ItemId = Convert.ToInt32(InvoiceDetailsList.ItemId);
                            InvoiceDetails.InvoiceId = InvoiceTable.InvoiceID;
                            InvoiceDetails.Description = InvoiceDetailsList.Description;
                            InvoiceDetails.Quantity = InvoiceDetailsList.Quantity;
                            InvoiceDetails.Rate = Convert.ToDouble(InvoiceDetailsList.Rate);
                            InvoiceDetails.Total = Convert.ToDouble(InvoiceDetailsList.Total);
                            InvoiceDetails.ServiceDate = InvoiceDetailsList.ServiceDate;
                            InvoiceDetails.RowSubTotal = InvoiceDetailsList.RowSubTotal;
                            InvoiceDetails.Vat = Convert.ToDouble(InvoiceDetailsList.Vat);
                            InvoiceDetails.Type = InvoiceDetailsList.Type;

                            if (InvoiceDetails.InvoiceDetailId == 0)
                            {
                                responsses = GlobalVeriables.WebApiClient.PostAsJsonAsync("PostinvoiceDetails", InvoiceDetails).Result;
                            }
                            else
                            {
                                responsses = GlobalVeriables.WebApiClient.PutAsJsonAsync("PostinvoiceDetails/" + InvoiceDetails.InvoiceDetailId, InvoiceDetails).Result;
                            }

                            if (responsses.StatusCode == System.Net.HttpStatusCode.OK)
                            {
                                if (Transaction(invoiceViewModel, "Add"))
                                {
                                }
                                else
                                {
                                    return new JsonResult { Data = new { Status = "Fail", path = "", id = InvoiceTable.InvoiceID } };
                                }
                            }
                        }

                        if (invoiceViewModel.file23[0] != null)
                        {
                            CreatDirectoryClass.UploadFileToDirectoryCommon(InvoiceTable.InvoiceID, "Invoice", invoiceViewModel.file23, "Invoice");
                        }
                    }
                }
                else
                {
                    return new JsonResult { Data = new { Status = "Fail", path = "", id = InvoiceTable.InvoiceID } };
                }
            }
            catch (Exception ex)
            {

                return new JsonResult { Data = new { Status = "Fail", Message = ex.Message.ToString() } };
            }

            string path1 = PrintView((int)InvoiceTable.InvoiceID);
            var root = Server.MapPath("/PDF/");
            var pdfname = String.Format("{0}", path1);
            var path = Path.Combine(root, pdfname);
            path = Path.GetFullPath(path);

            return new JsonResult { Data = new { Status = "Success", path = path1, id = InvoiceTable.InvoiceID } };
        }

        [HttpPost]
        public ActionResult SaveEmail(InvoiceViewModel invoiceViewModel)
        {
            HttpResponseMessage responsses = new HttpResponseMessage();
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
                mvcInvoiceModel.InvoiceID = invoiceViewModel.InvoiceID;
                HttpResponseMessage response = GlobalVeriables.WebApiClient.PostAsJsonAsync("PostInvoice", mvcInvoiceModel).Result;
                InvoiceTable = response.Content.ReadAsAsync<InvoiceTable>().Result;
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    invoiceViewModel.InvoiceID = InvoiceTable.InvoiceID;
                    if (invoiceViewModel.InvoiceDetailsTable != null)
                    {

                        foreach (InvoiceDetailsTable InvoiceDetailsList in invoiceViewModel.InvoiceDetailsTable)
                        {
                            InvoiceDetailsTable InvoiceDetails = new InvoiceDetailsTable();
                            InvoiceDetails.ItemId = Convert.ToInt32(InvoiceDetailsList.ItemId);
                            InvoiceDetails.InvoiceId = InvoiceTable.InvoiceID;
                            InvoiceDetails.Description = InvoiceDetailsList.Description;
                            InvoiceDetails.Quantity = InvoiceDetailsList.Quantity;
                            InvoiceDetails.Rate = Convert.ToDouble(InvoiceDetailsList.Rate);
                            InvoiceDetails.Total = Convert.ToDouble(InvoiceDetailsList.Total);
                            InvoiceDetails.ServiceDate = InvoiceDetailsList.ServiceDate;
                            InvoiceDetails.RowSubTotal = InvoiceDetailsList.RowSubTotal;
                            InvoiceDetails.Vat = Convert.ToDouble(InvoiceDetailsList.Vat);
                            InvoiceDetails.Type = InvoiceDetailsList.Type;

                            if (InvoiceDetails.InvoiceDetailId == 0)
                            {
                                responsses = GlobalVeriables.WebApiClient.PostAsJsonAsync("PostinvoiceDetails", InvoiceDetails).Result;
                            }
                            else
                            {
                                responsses = GlobalVeriables.WebApiClient.PutAsJsonAsync("PostinvoiceDetails/" + InvoiceDetails.InvoiceDetailId, InvoiceDetails).Result;
                            }
                        }

                        if (responsses.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            if (Transaction(invoiceViewModel, "Add"))
                            {
                                //return new JsonResult { Data = new { Status = "Success", id = InvoiceTable.InvoiceID } };
                            }
                            else
                            {
                                return new JsonResult { Data = new { Status = "Fail", id = InvoiceTable.InvoiceID } };
                            }
                        }
                    }
                }
                if (invoiceViewModel.file23[0] != null)
                {
                    CreatDirectoryClass.UploadFileToDirectoryCommon(InvoiceTable.InvoiceID, "Invoice", invoiceViewModel.file23, "Invoice");
                }
            }
            catch (Exception ex)
            {

                return new JsonResult { Data = new { Status = "Fail", Message = ex.Message.ToString() } };
            }
            return new JsonResult { Data = new { Status = "Success", path = "", id = InvoiceTable.InvoiceID } };
        }


        [HttpPost]
        public ActionResult Edit(InvoiceViewModel invoiceViewModel)
        {
            HttpResponseMessage responsses = new HttpResponseMessage();
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
                mvcInvoiceModel.InvoiceDescription = invoiceViewModel.InvoiceDescription;
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

                HttpResponseMessage response = GlobalVeriables.WebApiClient.PutAsJsonAsync("UpdateInvoice/" + mvcInvoiceModel.InvoiceID, mvcInvoiceModel).Result;
                InvoiceTable = response.Content.ReadAsAsync<InvoiceTable>().Result;
                InvoiceTable.InvoiceID = Convert.ToInt32(invoiceViewModel.InvoiceID);
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
                            InvoiceDetails.Quantity = InvoiceDetailsList.Quantity;
                            InvoiceDetails.Rate = Convert.ToDouble(InvoiceDetailsList.Rate);
                            InvoiceDetails.Total = Convert.ToDouble(InvoiceDetailsList.Total);
                            InvoiceDetails.ServiceDate = InvoiceDetailsList.ServiceDate;
                            InvoiceDetails.RowSubTotal = InvoiceDetailsList.RowSubTotal;
                            InvoiceDetails.Vat = Convert.ToDouble(InvoiceDetailsList.Vat);
                            InvoiceDetails.Type = InvoiceDetailsList.Type;
                            InvoiceDetails.InvoiceDetailId = InvoiceDetailsList.InvoiceDetailId;
                            if (InvoiceDetails.InvoiceDetailId != 0 && InvoiceDetails.InvoiceDetailId.ToString() != "")
                            {
                                responsses = GlobalVeriables.WebApiClient.PutAsJsonAsync("UpdateInvoiceDetails/" + InvoiceDetails.InvoiceDetailId, InvoiceDetails).Result;
                            }
                            else
                            {
                                responsses = GlobalVeriables.WebApiClient.PostAsJsonAsync("PostinvoiceDetails", InvoiceDetails).Result;
                            }
                        }

                        if (responsses.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            if (Transaction(invoiceViewModel, "remove"))
                            {
                                return new JsonResult { Data = new { Status = "Success", path = "", id = InvoiceTable.InvoiceID } };
                            }
                        }
                    }
                }
                else
                {
                    return new JsonResult { Data = new { Status = "Fail" } };
                }
            }
            catch (Exception ex)
            {

                return new JsonResult { Data = new { Status = "Fail", Message = ex.Message.ToString() } };
            }
            return new JsonResult { Data = new { Status = "Success", path = "", id = InvoiceTable.InvoiceID } };
        }



        [HttpPost]
        public ActionResult SaveEmailEdit(InvoiceViewModel invoiceViewModel)
        {
            HttpResponseMessage responsses = new HttpResponseMessage();
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
                mvcInvoiceModel.InvoiceDescription = invoiceViewModel.InvoiceDescription;
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
                HttpResponseMessage response = GlobalVeriables.WebApiClient.PutAsJsonAsync("UpdateInvoice/" + mvcInvoiceModel.InvoiceID, mvcInvoiceModel).Result;
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
                            InvoiceDetails.Type = InvoiceDetailsList.Type;
                            InvoiceDetails.Rate = Convert.ToDouble(InvoiceDetailsList.Rate);
                            InvoiceDetails.Total = Convert.ToDouble(InvoiceDetailsList.Total);
                            InvoiceDetails.ServiceDate = InvoiceDetailsList.ServiceDate;
                            InvoiceDetails.RowSubTotal = InvoiceDetailsList.RowSubTotal;
                            InvoiceDetails.Vat = Convert.ToDouble(InvoiceDetailsList.Vat);
                            InvoiceDetails.InvoiceDetailId = InvoiceDetailsList.InvoiceDetailId;

                            if (InvoiceDetails.InvoiceDetailId == 0)
                            {
                                responsses = GlobalVeriables.WebApiClient.PostAsJsonAsync("PostinvoiceDetails", InvoiceDetails).Result;

                                if (responsses.StatusCode != System.Net.HttpStatusCode.OK)
                                {
                                    return new JsonResult { Data = new { Status = "Fail", path = "", id = InvoiceTable.InvoiceID } };
                                }
                            }

                            else if (InvoiceDetails.InvoiceDetailId > 0)
                            {
                                responsses = GlobalVeriables.WebApiClient.PutAsJsonAsync("UpdateInvoiceDetails/" + InvoiceDetails.InvoiceDetailId, InvoiceDetails).Result;

                                if (responsses.StatusCode != System.Net.HttpStatusCode.OK)
                                {
                                    return new JsonResult { Data = new { Status = "Fail", path = "", id = InvoiceTable.InvoiceID } };
                                }
                            }
                        }


                        if (responsses.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            if (Transaction(invoiceViewModel, "remove"))
                            {
                                return new JsonResult { Data = new { Status = "Success", path = "", id = InvoiceTable.InvoiceID } };
                            }
                            else
                            {
                                return new JsonResult { Data = new { Status = "Fail", path = "", id = InvoiceTable.InvoiceID } };
                            }
                        }
                    }
                }
                else
                {
                    return new JsonResult { Data = new { Status = "Fail", path = "", id = InvoiceTable.InvoiceID } };
                }
            }
            catch (Exception ex)
            {

                return new JsonResult { Data = new { Status = "Fail", Message = ex.Message.ToString() } };
            }

            return new JsonResult { Data = new { Status = "Success", path = "", id = mvcInvoiceModel.InvoiceID } };
        }


        [HttpPost]
        public ActionResult SavePrintEdit(InvoiceViewModel invoiceViewModel)
        {
            HttpResponseMessage responsses = new HttpResponseMessage();
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
                mvcInvoiceModel.InvoiceDescription = invoiceViewModel.InvoiceDescription;

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

                mvcInvoiceModel.InvoiceID = invoiceViewModel.InvoiceID;
                HttpResponseMessage response = GlobalVeriables.WebApiClient.PutAsJsonAsync("UpdateInvoice/" + mvcInvoiceModel.InvoiceID, mvcInvoiceModel).Result; ;
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
                            InvoiceDetails.Quantity = InvoiceDetailsList.Quantity;
                            InvoiceDetails.Rate = Convert.ToDouble(InvoiceDetailsList.Rate);
                            InvoiceDetails.Total = Convert.ToDouble(InvoiceDetailsList.Total);
                            InvoiceDetails.ServiceDate = InvoiceDetailsList.ServiceDate;
                            InvoiceDetails.RowSubTotal = InvoiceDetailsList.RowSubTotal;
                            InvoiceDetails.Vat = Convert.ToDouble(InvoiceDetailsList.Vat);
                            InvoiceDetails.Type = InvoiceDetailsList.Type;
                            InvoiceDetails.InvoiceDetailId = InvoiceDetailsList.InvoiceDetailId;

                            if (InvoiceDetails.InvoiceDetailId == 0)
                            {
                                responsses = GlobalVeriables.WebApiClient.PostAsJsonAsync("PostinvoiceDetails", InvoiceDetails).Result;
                                if (responsses.StatusCode != System.Net.HttpStatusCode.OK)
                                {
                                    return new JsonResult { Data = new { Status = "Fail", path = "", id = InvoiceTable.InvoiceID } };
                                }
                            }
                            else if (InvoiceDetails.InvoiceDetailId > 0)
                            {
                                responsses = GlobalVeriables.WebApiClient.PutAsJsonAsync("UpdateInvoiceDetails/" + InvoiceDetails.InvoiceDetailId, InvoiceDetails).Result;

                                if (responsses.StatusCode != System.Net.HttpStatusCode.OK)
                                {
                                    return new JsonResult { Data = new { Status = "Fail", path = "", id = InvoiceTable.InvoiceID } };
                                }
                            }
                        }

                        if (responsses.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            if (Transaction(invoiceViewModel, "remove"))
                            {
                                //  return new JsonResult { Data = new { Status = "Success", path = "", id = InvoiceTable.InvoiceID } };
                            }
                            else
                            {
                                return new JsonResult { Data = new { Status = "Fail", path = "", id = InvoiceTable.InvoiceID } };
                            }
                        }
                    }
                }

                else
                {
                    return new JsonResult { Data = new { Status = "Fail", path = "", id = InvoiceTable.InvoiceID } };
                }


                if (invoiceViewModel.file23[0] != null)
                {
                    CreatDirectoryClass.UploadFileToDirectoryCommon(InvoiceTable.InvoiceID, "Invoice", invoiceViewModel.file23, "Invoice");
                }
            }
            catch (Exception ex)
            {

                return new JsonResult { Data = new { Status = "Fail", Message = ex.Message.ToString() } };
            }

            string path1 = PrintView((int)InvoiceTable.InvoiceID);
            var root = Server.MapPath("/PDF/");
            var pdfname = String.Format("{0}", path1);
            var path = Path.Combine(root, pdfname);
            path = Path.GetFullPath(path);

            return new JsonResult { Data = new { Status = "Success", path = path1, id = InvoiceTable.InvoiceID } };
        }



        [HttpGet]
        public ActionResult Edit(int id)
        {
            InvoiceViewModel invoiceViewModel = new InvoiceViewModel();
            try
            {
                ViewBag.FILE = CreatDirectoryClass.GetFileDirectiory(id, "Invoice");

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

                HttpResponseMessage responseInvoiceDetailsList = GlobalVeriables.WebApiClient.GetAsync("GetInvoiceDetails/" + id.ToString()).Result;
                List<InvoiceViewModel> InvoiceDetailsList = responseInvoiceDetailsList.Content.ReadAsAsync<List<InvoiceViewModel>>().Result;

                CommonModel commonModel = new CommonModel();
                commonModel.Name = "Invoice";

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

                commonModel.FromDate = Convert.ToDateTime(InvoiceModel.InvoiceDate);
                commonModel.DueDate = Convert.ToDateTime(InvoiceModel.InvoiceDueDate);

                commonModel.Number_Id = InvoiceModel.Invoice_ID;
                commonModel.ReferenceNumber = InvoiceModel.RefNumber;

                ViewBag.commonModel = commonModel;
                ViewBag.Contentdata = contectmodel;
                ViewBag.Companydata = companyModel;
                ViewBag.InvoiceData = InvoiceModel;
                ViewBag.InvoiceDetailsList = InvoiceDetailsList;
                ViewBag.definition = "Invoice";

                return View(invoiceViewModel);
            }
            catch (Exception ex)
            {
                return null;
            }

        }



        [HttpGet]
        public ActionResult Print(int id)
        {
            try
            {
                HttpResponseMessage responseInvoice = GlobalVeriables.WebApiClient.GetAsync("GetInvoiceById/" + id.ToString()).Result;
                MVCInvoiceModel invoiceModel = responseInvoice.Content.ReadAsAsync<MVCInvoiceModel>().Result;

                HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("ApiConatacts/" + invoiceModel.ContactId.ToString()).Result;
                MVCContactModel contectmodel = response.Content.ReadAsAsync<MVCContactModel>().Result;

                HttpResponseMessage responseCompany = GlobalVeriables.WebApiClient.GetAsync("APIComapny/" + invoiceModel.CompanyId.ToString()).Result;
                MVCCompanyInfoModel companyModel = responseCompany.Content.ReadAsAsync<MVCCompanyInfoModel>().Result;



                HttpResponseMessage responseInvoiceDetailsList = GlobalVeriables.WebApiClient.GetAsync("GetInvoiceDetails/" + id.ToString()).Result;
                List<InvoiceViewModel> InvoiceModelDetailsList = responseInvoiceDetailsList.Content.ReadAsAsync<List<InvoiceViewModel>>().Result;

                DateTime qutationDueDate = Convert.ToDateTime(invoiceModel.InvoiceDueDate); //mm/dd/yyyy
                DateTime qutationDate = Convert.ToDateTime(invoiceModel.InvoiceDate);//mm/dd/yyyy
                TimeSpan ts = qutationDueDate.Subtract(qutationDate);
                string diffDate = ts.Days.ToString();

                ViewBag.Contentdata = contectmodel;
                ViewBag.Companydata = companyModel;
                ViewBag.InvoiceData = invoiceModel;
                ViewBag.InvoiceDatailsList = InvoiceModelDetailsList;

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


                CommonModel commonModel = new CommonModel();
                commonModel.Name = "Invoice";
                commonModel.FromDate = Convert.ToDateTime(InvoceModel.InvoiceDate);
                commonModel.DueDate = Convert.ToDateTime(InvoceModel.InvoiceDueDate);
                commonModel.ReferenceNumber = InvoceModel.RefNumber;
                commonModel.Number_Id = InvoceModel.Invoice_ID;

                commonModel.SubTotal = InvoceModel.SubTotal.ToString();
                commonModel.Vat6 = InvoceModel.TotalVat6.ToString();
                commonModel.Vat21 = InvoceModel.TotalVat21.ToString();
                commonModel.grandTotal = InvoceModel.TotalAmount.ToString();

                commonModel.Note = InvoceModel.CustomerNote;
                ViewBag.commonModel = commonModel;
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

                HttpResponseMessage responseinvoice = GlobalVeriables.WebApiClient.GetAsync("GetInvoiceById/" + id.ToString()).Result;
                MVCInvoiceModel InvoiceModel = responseinvoice.Content.ReadAsAsync<MVCInvoiceModel>().Result;


                HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("ApiConatacts/" + InvoiceModel.ContactId.ToString()).Result;
                MVCContactModel contectmodel = response.Content.ReadAsAsync<MVCContactModel>().Result;

                HttpResponseMessage responseCompany = GlobalVeriables.WebApiClient.GetAsync("APIComapny/" + InvoiceModel.CompanyId.ToString()).Result;
                MVCCompanyInfoModel companyModel = responseCompany.Content.ReadAsAsync<MVCCompanyInfoModel>().Result;


                DateTime qutationDueDate = Convert.ToDateTime(InvoiceModel.InvoiceDueDate); //mm/dd/yyyy
                DateTime qutationDate = Convert.ToDateTime(InvoiceModel.InvoiceDate);//mm/dd/yyyy
                TimeSpan ts = qutationDueDate.Subtract(qutationDate);
                string diffDate = ts.Days.ToString();


                HttpResponseMessage responseInvoiceDetailsList = GlobalVeriables.WebApiClient.GetAsync("GetInvoiceDetails/" + id.ToString()).Result;
                List<InvoiceViewModel> InvoiceModelDetailsList = responseInvoiceDetailsList.Content.ReadAsAsync<List<InvoiceViewModel>>().Result;

                ViewBag.Contentdata = contectmodel;
                ViewBag.Companydata = companyModel;

                TempData["ContactInfo"] = contectmodel;
                TempData["CompanyInfo"] = companyModel;

                ViewBag.InvoiceData = InvoiceModel;
                ViewBag.InvoiceDatailsList = InvoiceModelDetailsList;
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

        [HttpGet]
        public ActionResult EmailInvoice(int? id)
        {
            var List = CreatDirectoryClass.GetFileDirectiory((int)id, "Invoice");

            EmailModel email = new EmailModel();

            List<Selected> _list = new List<Selected>();

            foreach (var Item in List)
            {
                _list.Add(new Selected { IsSelected = true, FileName = Item.DirectoryPath, Directory = Item.FileFolderPathe + "/" + Item.DirectoryPath });
            }

            email.SelectList = _list;

            try
            {
                email.Attachment = PrintView((int)id);
                MVCCompanyInfoModel companyModel = TempData["CompanyInfo"] as MVCCompanyInfoModel;
                MVCContactModel contectmodel = TempData["ContactInfo"] as MVCContactModel;


                HttpContext.Items["FilePath"] = email.Attachment;

                var CompanyName = companyModel.CompanyName;
                var companyEmail = companyModel.CompanyEmail;

                var contact = contectmodel.ContactName;

                email.EmailText = @"Geachte heer" + contectmodel.ContactName + "." +

                ".Hierbij ontvangt u onze offerte 10 zoals besproken,." +

                "." + "Graag horen we of u hiermee akkoord gaat." +

                "." + "De offerte vindt u als bijlage bij deze email." +

                "..Met vriendelijke groet." +

                contectmodel.ContactName + "." +

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
        public ActionResult EmailInvoice(EmailModel email, string[] Files, FormCollection formCollection)
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

                        if (item.Directory.EndsWith("doc") || item.Directory.EndsWith("pdf") || item.Directory.EndsWith("PDF") || item.Directory.EndsWith("docx") || item.Directory.EndsWith("jpg") || item.Directory.EndsWith("jpeg") || item.Directory.EndsWith("png") || item.Directory.EndsWith("PNG") || item.Directory.EndsWith("txt"))
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
        public ActionResult UploadFiles(InvoiceViewModel InvoiceViewModel)
        {
            try
            {
                string FileName = "";
                HttpFileCollectionBase files = Request.Files;
                for (int i = 0; i < files.Count; i++)
                {
                    HttpPostedFileBase file = files[i];
                    FileName = CreatDirectoryClass.UploadFileToDirectoryCommon(InvoiceViewModel.InvoiceID, "Invoice", InvoiceViewModel.file23, "Invoice");
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

                if (CreatDirectoryClass.Delete(Id, FileName, "Invoice"))
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
        public ActionResult DeleteInvoice(int InvoiceID, int InvoiceDetailsId, int vat, float total)
        {
            try
            {
                HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("GetInvoiceById/" + InvoiceID).Result;
                MVCInvoiceModel InvoiceModel = response.Content.ReadAsAsync<MVCInvoiceModel>().Result;

                double ResultVAT = CommonController.CalculateVat(vat, total);

                InvoiceModel.SubTotal = InvoiceModel.SubTotal - ResultVAT;
                InvoiceModel.TotalAmount = InvoiceModel.TotalAmount - total;
                InvoiceModel.TotalAmount = InvoiceModel.TotalAmount - ResultVAT;
                if (vat == 6)
                {
                    InvoiceModel.TotalVat6 = InvoiceModel.TotalVat6 - ResultVAT;
                }
                if (vat == 21)
                {
                    InvoiceModel.TotalVat21 = InvoiceModel.TotalVat21 - ResultVAT;
                }


                HttpResponseMessage ResponseUpdate = GlobalVeriables.WebApiClient.PutAsJsonAsync("UpdateInvoice/" + InvoiceModel.InvoiceID, InvoiceModel).Result;
                InvoiceTable InvoiceTable = ResponseUpdate.Content.ReadAsAsync<InvoiceTable>().Result;

                if (ResponseUpdate.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    HttpResponseMessage responseUpdateInvoiceDetails = GlobalVeriables.WebApiClient.DeleteAsync("DeleteInvoiceDetails/" + InvoiceDetailsId).Result;

                    if (responseUpdateInvoiceDetails.StatusCode == System.Net.HttpStatusCode.OK)
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
        public JsonResult GetInvoiceId(int Id)
        {
            try
            {
                HttpResponseMessage responseInvoice = GlobalVeriables.WebApiClient.GetAsync("GetId/" + Id).Result;
                MVCInvoiceModel InvoceModel = responseInvoice.Content.ReadAsAsync<MVCInvoiceModel>().Result;
                if (responseInvoice.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return new JsonResult { Data = new { Status = "Success", Id = InvoceModel.InvoiceID } };
                }
                else
                {
                    return new JsonResult { Data = new { Status = "Fail" } };
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }


        public ActionResult PrintReport(int ID)
        {

            try
            {

                DBEntities entity = new DBEntities();
                List<QuotationReportModel> quotationReportModels = entity.InvoiceTables.Where(x => x.InvoiceID == ID).Select(x => new QuotationReportModel
                {

                    QutationID = x.InvoiceID,
                    Qutation_ID = x.Invoice_ID,
                    RefNumber = x.RefNumber,
                    QutationDate = x.InvoiceDate.ToString(),
                    DueDate = x.InvoiceDueDate.ToString(),
                    SubTotal = x.SubTotal ?? 0,
                    TotalVat6 = x.TotalVat6 ?? 0,
                    TotalVat21 = x.TotalVat21 ?? 0,
                    DiscountAmount = x.DiscountAmount ?? 0,
                    TotalAmount = x.TotalAmount ?? 0,
                    CustomerNote = x.CustomerNote,
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

                InvoiceTable qt = entity.InvoiceTables.Where(x => x.InvoiceID == ID).FirstOrDefault();


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


                List<Contacts> Contact = entity.ContactsTables.Where(x => x.ContactsId == qt.ContactId).Select(c => new Contacts
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

                List<GoodsTable> goodsTable = entity.InvoiceDetailsTables.Where(x => x.InvoiceId == ID && x.Type == "Goods").Select(x => new GoodsTable
                {
                    ProductName = x.ProductTable.ProductName,
                    Quantity = x.Quantity ?? 0,
                    Rate = x.Rate ?? 0,
                    Total = x.Total ?? 0,
                    Vat = x.Vat ?? 0,
                    RowSubTotal = x.RowSubTotal ?? 0,

                }).ToList();

                DateTime dt = DateTime.Today;


                List<ServicesTables> servicesTabless = entity.InvoiceDetailsTables.Where(x => x.InvoiceId == ID && x.Type == "Service").Select(x => new ServicesTables
                {

                    Date = x.ServiceDate.ToString(),
                    ProductNames = x.ProductTable.ProductName,
                    Descriptions = x.Description,
                    Quantitys = x.Quantity ?? 0,
                    Rates = x.Rate ?? 0,
                    Totals = x.Total ?? 0,
                    Vats = x.Vat ?? 0,
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

                
                Report.Load(Server.MapPath("~/CrystalReport/InvoiceReport.rpt"));

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