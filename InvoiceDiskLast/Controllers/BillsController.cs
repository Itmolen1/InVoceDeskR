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

        [HttpPost]
        public JsonResult GetBillOrderList()
        {
            List<BillDetailViewModel> BillList = new List<BillDetailViewModel>();
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

                HttpResponseMessage respose = GlobalVeriables.WebApiClient.GetAsync("GetbillDetails/" + companyId).Result;
                BillList = respose.Content.ReadAsAsync<List<BillDetailViewModel>>().Result;
                if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
                {
                    if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
                    {
                        BillList = BillList.Where(p => p.BilID.ToString().Contains(search)
                       || p.BillDate != null && p.BillDate.ToString().ToLower().Contains(search.ToLower())
                       || p.BillDueDate != null && p.BillDueDate.ToString().ToLower().Contains(search.ToLower())
                       || p.VenderName != null && p.VenderName.ToString().ToLower().Contains(search.ToLower())
                       || p.UserName != null && p.UserName.ToString().ToLower().Contains(search.ToLower())
                       || p.Vat != null && p.Vat.ToString().ToLower().Contains(search.ToLower())
                       || p.TotalAmount != null && p.TotalAmount.ToString().ToLower().Contains(search.ToLower())
                       || p.Status != null && p.Status.ToString().ToLower().Contains(search.ToLower())
                      ).ToList();

                    }
                }

                recordsTotal = recordsTotal = BillList.Count();
                var data = BillList.Skip(skip).Take(pageSize).ToList();
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data }, JsonRequestBehavior.AllowGet);
                #endregion
            }
            catch (Exception ex)
            {
                return Json(new { draw = 0, recordsFiltered = 0, recordsTotal = 0, data = 0 }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpGet]
        public ActionResult Create(int id)
        {
            BillDetailViewModel _BillDetailView = new BillDetailViewModel();
            try
            {
                int CompanyID = Convert.ToInt32(Session["CompayID"]);
                HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("ApiConatacts/" + id.ToString()).Result;
                MVCContactModel contectmodel = response.Content.ReadAsAsync<MVCContactModel>().Result;
                HttpResponseMessage responseCompany = GlobalVeriables.WebApiClient.GetAsync("APIComapny/" + CompanyID.ToString()).Result;
                MVCCompanyInfoModel companyModel = responseCompany.Content.ReadAsAsync<MVCCompanyInfoModel>().Result;
                CommonModel commonModel = new CommonModel();
                commonModel.Name = "Bill";

                ViewBag.Contentdata = contectmodel;
                ViewBag.Companydata = companyModel;
                DateTime InvoiceDate = new DateTime();
                InvoiceDate = DateTime.Now;
                commonModel.FromDate = InvoiceDate;
                commonModel.DueDate = InvoiceDate.AddDays(+15);

                MvcBillModel q = new MvcBillModel();
                HttpResponseMessage response1 = GlobalVeriables.WebApiClient.GetAsync("GenrateBilNumber/").Result;
                q = response1.Content.ReadAsAsync<MvcBillModel>().Result;
                _BillDetailView.BillDate = InvoiceDate;
                _BillDetailView.VenderId = id;
                _BillDetailView.BillDueDate = InvoiceDate.AddDays(+15);
                commonModel.Number_Id = q.Bill_ID;
                ViewBag.commonModel = commonModel;
                return View(_BillDetailView);
            }
            catch (Exception ex)
            {

            }

            return View();
        }

        [HttpPost]
        public ActionResult EditSaveDraft(BillDetailViewModel _billDetailViewModel)
        {
            HttpResponseMessage responsses = new HttpResponseMessage();

            MvcBillModel mvcbillMoel = new MvcBillModel();
            try
            {
                mvcbillMoel.Bill_ID = _billDetailViewModel.Bill_ID;
                mvcbillMoel.BilID = _billDetailViewModel.BilID;
                mvcbillMoel.CompanyId = _billDetailViewModel.CompanyId;
                mvcbillMoel.UserId = Convert.ToInt32(Session["LoginUserID"]);
                mvcbillMoel.VenderId = _billDetailViewModel.VenderId;
                mvcbillMoel.RefNumber = _billDetailViewModel.RefNumber;
                mvcbillMoel.BillDate = _billDetailViewModel.BillDate;
                mvcbillMoel.BillDueDate = _billDetailViewModel.BillDueDate;
                mvcbillMoel.SubTotal = _billDetailViewModel.SubTotal;
                mvcbillMoel.DiscountAmount = _billDetailViewModel.DiscountAmount;
                mvcbillMoel.TotalAmount = _billDetailViewModel.TotalAmount;
                mvcbillMoel.CustomerNote = _billDetailViewModel.CustomerNote;
                mvcbillMoel.TotalVat6 = _billDetailViewModel.TotalVat6;
                mvcbillMoel.TotalVat21 = _billDetailViewModel.TotalVat21;
                mvcbillMoel.Type = StatusEnum.Goods.ToString();
                mvcbillMoel.Status = "accepted";

                if (mvcbillMoel.TotalVat6 != null && mvcbillMoel.TotalVat6 != 0)
                {
                    double vat61 = Math.Round((double)mvcbillMoel.TotalVat6, 2, MidpointRounding.AwayFromZero);
                    mvcbillMoel.TotalVat6 = vat61;
                }
                mvcbillMoel.TotalVat21 = _billDetailViewModel.TotalVat21;
                if (mvcbillMoel.TotalVat21 != null && mvcbillMoel.TotalVat21 != 0)
                {
                    double vat21 = Math.Round((double)mvcbillMoel.TotalVat21, 2, MidpointRounding.AwayFromZero);
                    mvcbillMoel.TotalVat21 = vat21;
                }

                mvcbillMoel.BilID = _billDetailViewModel.BilID;
                HttpResponseMessage response = GlobalVeriables.WebApiClient.PutAsJsonAsync("APIBill/" + mvcbillMoel.BilID, mvcbillMoel).Result;

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    if (_billDetailViewModel.BillDetail != null)
                    {
                        foreach (BillDetailTable bdetail in _billDetailViewModel.BillDetail)
                        {
                            BillDetailTable billdetailTable = new BillDetailTable();
                            billdetailTable.ItemId = Convert.ToInt32(bdetail.ItemId);
                            billdetailTable.BillID = mvcbillMoel.BilID;
                            billdetailTable.Description = bdetail.Description;
                            billdetailTable.BillDetailId = bdetail.BillDetailId;
                            billdetailTable.Quantity = bdetail.Quantity;
                            billdetailTable.Rate = Convert.ToDouble(bdetail.Rate);
                            billdetailTable.Total = Convert.ToDouble(bdetail.Total);
                            billdetailTable.ServiceDate = bdetail.ServiceDate;
                            billdetailTable.RowSubTotal = bdetail.RowSubTotal;
                            billdetailTable.Vat = Convert.ToDouble(bdetail.Vat);
                            billdetailTable.Type = bdetail.Type;
                            if (billdetailTable.BillDetailId == 0)
                            {
                                responsses = GlobalVeriables.WebApiClient.PostAsJsonAsync("AddBillDetail", billdetailTable).Result;
                            }
                            else
                            {
                                responsses = GlobalVeriables.WebApiClient.PutAsJsonAsync("APIBillDetail/" + billdetailTable.BillDetailId, billdetailTable).Result;
                            }
                        }


                        if (responsses.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            if (Transaction(_billDetailViewModel, "Remove"))
                            {
                            }
                            else
                            {
                                return new JsonResult { Data = new { Status = "Fail", path = "", id = _billDetailViewModel.BilID } };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return new JsonResult { Data = new { Status = "Fail", Message = ex.Message.ToString() } };
            }
            return new JsonResult { Data = new { Status = "Success", BillId = mvcbillMoel.BilID } };
        }

        [HttpPost]
        public ActionResult EditEmailPrint(BillDetailViewModel _billDetailViewModel)
        {
            HttpResponseMessage responsses = new HttpResponseMessage();
            MvcBillModel mvcbillMoel = new MvcBillModel();
            try
            {
                mvcbillMoel.Bill_ID = _billDetailViewModel.Bill_ID;
                mvcbillMoel.BilID = _billDetailViewModel.BilID;
                mvcbillMoel.CompanyId = _billDetailViewModel.CompanyId;
                mvcbillMoel.UserId = Convert.ToInt32(Session["LoginUserID"]);
                mvcbillMoel.VenderId = _billDetailViewModel.VenderId;
                mvcbillMoel.RefNumber = _billDetailViewModel.RefNumber;
                mvcbillMoel.BillDate = _billDetailViewModel.BillDate;
                mvcbillMoel.BillDueDate = _billDetailViewModel.BillDueDate;
                mvcbillMoel.SubTotal = _billDetailViewModel.SubTotal;
                mvcbillMoel.DiscountAmount = _billDetailViewModel.DiscountAmount;
                mvcbillMoel.TotalAmount = _billDetailViewModel.TotalAmount;
                mvcbillMoel.CustomerNote = _billDetailViewModel.CustomerNote;
                mvcbillMoel.TotalVat6 = _billDetailViewModel.TotalVat6;
                mvcbillMoel.TotalVat21 = _billDetailViewModel.TotalVat21;
                mvcbillMoel.Type = StatusEnum.Goods.ToString();
                mvcbillMoel.Status = "accepted";

                if (mvcbillMoel.TotalVat6 != null && mvcbillMoel.TotalVat6 != 0)
                {
                    double vat61 = Math.Round((double)mvcbillMoel.TotalVat6, 2, MidpointRounding.AwayFromZero);
                    mvcbillMoel.TotalVat6 = vat61;
                }
                mvcbillMoel.TotalVat21 = _billDetailViewModel.TotalVat21;
                if (mvcbillMoel.TotalVat21 != null && mvcbillMoel.TotalVat21 != 0)
                {
                    double vat21 = Math.Round((double)mvcbillMoel.TotalVat21, 2, MidpointRounding.AwayFromZero);
                    mvcbillMoel.TotalVat21 = vat21;
                }

                mvcbillMoel.BilID = _billDetailViewModel.BilID;
                HttpResponseMessage response = GlobalVeriables.WebApiClient.PutAsJsonAsync("APIBill/" + mvcbillMoel.BilID, mvcbillMoel).Result;

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    if (_billDetailViewModel.BillDetail != null)
                    {
                        foreach (BillDetailTable bdetail in _billDetailViewModel.BillDetail)
                        {
                            BillDetailTable billdetailTable = new BillDetailTable();
                            billdetailTable.ItemId = Convert.ToInt32(bdetail.ItemId);
                            billdetailTable.BillID = mvcbillMoel.BilID;
                            billdetailTable.Description = bdetail.Description;
                            billdetailTable.BillDetailId = bdetail.BillDetailId;
                            billdetailTable.Quantity = bdetail.Quantity;
                            billdetailTable.Rate = Convert.ToDouble(bdetail.Rate);
                            billdetailTable.Total = Convert.ToDouble(bdetail.Total);
                            billdetailTable.ServiceDate = bdetail.ServiceDate;
                            billdetailTable.RowSubTotal = bdetail.RowSubTotal;
                            billdetailTable.Vat = Convert.ToDouble(bdetail.Vat);
                            billdetailTable.Type = bdetail.Type;

                            if (billdetailTable.BillDetailId == 0)
                            {
                                responsses = GlobalVeriables.WebApiClient.PostAsJsonAsync("AddBillDetail", billdetailTable).Result;
                            }
                            else
                            {
                                responsses = GlobalVeriables.WebApiClient.PutAsJsonAsync("APIBillDetail/" + billdetailTable.BillDetailId, billdetailTable).Result;
                            }
                        }
                        if (responsses.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            if (Transaction(_billDetailViewModel, "Remove"))
                            {
                            }
                            else
                            {
                                return new JsonResult { Data = new { Status = "Fail", path = "", id = _billDetailViewModel.BilID } };
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {

                return new JsonResult { Data = new { Status = "Fail", Message = ex.Message.ToString() } };
            }

            string path1 = PrintView((int)_billDetailViewModel.BilID);
            var root = Server.MapPath("/PDF/");
            var pdfname = String.Format("{0}", path1);
            var path = Path.Combine(root, pdfname);
            path = Path.GetFullPath(path);
            return new JsonResult { Data = new { Status = "Success", path = path1, id = _billDetailViewModel.BilID } };
        }


        [HttpPost]
        public ActionResult EditEmail(BillDetailViewModel _billDetailViewModel)
        {
            HttpResponseMessage responsses = new HttpResponseMessage();

            MvcBillModel mvcbillMoel = new MvcBillModel();
            try
            {
                mvcbillMoel.Bill_ID = _billDetailViewModel.Bill_ID;
                mvcbillMoel.BilID = _billDetailViewModel.BilID;
                mvcbillMoel.CompanyId = _billDetailViewModel.CompanyId;
                mvcbillMoel.UserId = Convert.ToInt32(Session["LoginUserID"]);
                mvcbillMoel.VenderId = _billDetailViewModel.VenderId;
                mvcbillMoel.RefNumber = _billDetailViewModel.RefNumber;
                mvcbillMoel.BillDate = _billDetailViewModel.BillDate;
                mvcbillMoel.BillDueDate = _billDetailViewModel.BillDueDate;
                mvcbillMoel.SubTotal = _billDetailViewModel.SubTotal;
                mvcbillMoel.DiscountAmount = _billDetailViewModel.DiscountAmount;
                mvcbillMoel.TotalAmount = _billDetailViewModel.TotalAmount;
                mvcbillMoel.CustomerNote = _billDetailViewModel.CustomerNote;
                mvcbillMoel.TotalVat6 = _billDetailViewModel.TotalVat6;
                mvcbillMoel.TotalVat21 = _billDetailViewModel.TotalVat21;
                mvcbillMoel.Type = StatusEnum.Goods.ToString();
                mvcbillMoel.Status = "accepted";
                if (mvcbillMoel.TotalVat6 != null && mvcbillMoel.TotalVat6 != 0)
                {
                    double vat61 = Math.Round((double)mvcbillMoel.TotalVat6, 2, MidpointRounding.AwayFromZero);
                    mvcbillMoel.TotalVat6 = vat61;
                }
                mvcbillMoel.TotalVat21 = _billDetailViewModel.TotalVat21;
                if (mvcbillMoel.TotalVat21 != null && mvcbillMoel.TotalVat21 != 0)
                {
                    double vat21 = Math.Round((double)mvcbillMoel.TotalVat21, 2, MidpointRounding.AwayFromZero);
                    mvcbillMoel.TotalVat21 = vat21;
                }

                mvcbillMoel.BilID = _billDetailViewModel.BilID;
                HttpResponseMessage response = GlobalVeriables.WebApiClient.PutAsJsonAsync("APIBill/" + mvcbillMoel.BilID, mvcbillMoel).Result;

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    if (_billDetailViewModel.BillDetail != null)
                    {
                        foreach (BillDetailTable bdetail in _billDetailViewModel.BillDetail)
                        {
                            BillDetailTable billdetailTable = new BillDetailTable();

                            billdetailTable.ItemId = Convert.ToInt32(bdetail.ItemId);
                            billdetailTable.BillID = mvcbillMoel.BilID;
                            billdetailTable.Description = bdetail.Description;
                            billdetailTable.BillDetailId = bdetail.BillDetailId;
                            billdetailTable.Quantity = bdetail.Quantity;
                            billdetailTable.Rate = Convert.ToDouble(bdetail.Rate);
                            billdetailTable.Total = Convert.ToDouble(bdetail.Total);
                            billdetailTable.ServiceDate = bdetail.ServiceDate;
                            billdetailTable.RowSubTotal = bdetail.RowSubTotal;
                            billdetailTable.Vat = Convert.ToDouble(bdetail.Vat);
                            billdetailTable.Type = bdetail.Type;
                            if (billdetailTable.BillDetailId == 0)
                            {
                                responsses = GlobalVeriables.WebApiClient.PostAsJsonAsync("AddBillDetail", billdetailTable).Result;
                            }
                            else
                            {
                                responsses = GlobalVeriables.WebApiClient.PutAsJsonAsync("APIBillDetail/" + billdetailTable.BillDetailId, billdetailTable).Result;
                            }
                        }

                        if (responsses.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            if (Transaction(_billDetailViewModel, "Remove"))
                            {
                            }
                            else
                            {
                                return new JsonResult { Data = new { Status = "Fail", path = "", id = _billDetailViewModel.BilID } };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return new JsonResult { Data = new { Status = "Fail", Message = ex.Message.ToString() } };
            }
            return new JsonResult { Data = new { Status = "Success", BillId = mvcbillMoel.BilID } };
        }


        [HttpPost]
        public ActionResult DeleteInvoice(int billid, int BillDetailId, int vat, float total)
        {
            try
            {
                HttpResponseMessage res = GlobalVeriables.WebApiClient.GetAsync("GetbillDetail/" + billid).Result;
                MvcBillModel InvoiceModel = res.Content.ReadAsAsync<MvcBillModel>().Result;
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

                HttpResponseMessage ResponseUpdate = GlobalVeriables.WebApiClient.PutAsJsonAsync("APIBill/" + InvoiceModel.BilID, InvoiceModel).Result;
                BillTable InvoiceTable = ResponseUpdate.Content.ReadAsAsync<BillTable>().Result;
                if (ResponseUpdate.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    HttpResponseMessage responseUpdateInvoiceDetails = GlobalVeriables.WebApiClient.DeleteAsync("DeleteDetails/" + BillDetailId).Result;

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
        public ActionResult SaveEmailPrint(BillDetailViewModel billDetailViewModel)
        {
            HttpResponseMessage responsses = new HttpResponseMessage();
            BillTable billtable = new BillTable();
            BillDetailViewModel billviewModel = new BillDetailViewModel();
            try
            {
                billtable.CompanyId = billDetailViewModel.CompanyId;
                billtable.UserId = Convert.ToInt32(Session["LoginUserID"]);
                billtable.Bill_ID = billDetailViewModel.Bill_ID.ToString();
                billtable.VenderId = billDetailViewModel.VenderId;
                billtable.RefNumber = billDetailViewModel.RefNumber;
                billtable.BillDate = billDetailViewModel.BillDate;
                billtable.BillDueDate = billDetailViewModel.BillDueDate;
                billtable.SubTotal = billDetailViewModel.SubTotal;
                billtable.DiscountAmount = billDetailViewModel.DiscountAmount;
                billtable.TotalAmount = billDetailViewModel.TotalAmount;
                billtable.CustomerNote = billDetailViewModel.CustomerNote;
                billtable.TotalVat6 = billDetailViewModel.TotalVat6;
                billtable.TotalVat21 = billDetailViewModel.TotalVat21;
                billtable.Status = "accepted";
                billtable.Type = StatusEnum.Goods.ToString();
                // bill Api
                HttpResponseMessage response = GlobalVeriables.WebApiClient.PostAsJsonAsync("AddBill", billtable).Result;
                BillTable billviewmodel = response.Content.ReadAsAsync<BillTable>().Result;

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    billviewModel.BilID = billviewmodel.BilID;
                    billDetailViewModel.BilID = billviewmodel.BilID;

                    if (billDetailViewModel.BillDetail != null)
                    {
                        foreach (BillDetailTable item in billDetailViewModel.BillDetail)
                        {
                            BillDetailTable billdetailtable = new BillDetailTable();
                            billdetailtable.BillID = billviewmodel.BilID;
                            billdetailtable.ItemId = item.ItemId;
                            billdetailtable.Description = item.Description;
                            billdetailtable.Quantity = item.Quantity;
                            billdetailtable.Rate = item.Rate;
                            billdetailtable.Total = item.Total;
                            billdetailtable.Type = item.Type;
                            billdetailtable.RowSubTotal = item.RowSubTotal;
                            billdetailtable.Vat = item.Vat;
                            billdetailtable.ServiceDate = item.ServiceDate;
                            // APIBill   
                            responsses = GlobalVeriables.WebApiClient.PostAsJsonAsync("AddBillDetail", billdetailtable).Result;
                            if (responsses.StatusCode != System.Net.HttpStatusCode.OK)
                            {
                                return new JsonResult { Data = new { Status = "Fail" } };
                            }
                        }

                        if (responsses.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            if (Transaction(billDetailViewModel, "Add"))
                            {
                            }
                            else
                            {
                                return new JsonResult { Data = new { Status = "Fail", path = "", id = billDetailViewModel.BilID } };
                            }
                        }

                    }
                }

                if (billDetailViewModel.file23[0] != null)
                {
                    CreatDirectoryClass.UploadFileToDirectoryCommon(billDetailViewModel.BilID, "Bill", billDetailViewModel.file23, "Bill");
                }
            }
            catch (Exception ex)
            {

                return new JsonResult { Data = new { Status = "Fail", Message = ex.Message.ToString() } };
            }

            string path1 = PrintView((int)billviewModel.BilID);
            var root = Server.MapPath("/PDF/");
            var pdfname = String.Format("{0}", path1);
            var path = Path.Combine(root, pdfname);
            path = Path.GetFullPath(path);
            return new JsonResult { Data = new { Status = "Success", path = path1, id = billviewModel.BilID } };
        }

        #region Bill by samar



        public ActionResult ViewBilDetail(int? BillId)
        {
            try
            {
                ViewBag.FILE = CreatDirectoryClass.GetFileDirectiory((int)BillId, "Bill");


                HttpResponseMessage res = GlobalVeriables.WebApiClient.GetAsync("GetbillDetail/" + BillId).Result;
                MvcBillModel ob = res.Content.ReadAsAsync<MvcBillModel>().Result;
                ob.BilID = BillId;
                HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("ApiConatacts/" + ob.VenderId.ToString()).Result;
                MVCContactModel contectmodel = response.Content.ReadAsAsync<MVCContactModel>().Result;

                HttpResponseMessage responseCompany = GlobalVeriables.WebApiClient.GetAsync("APIComapny/" + ob.CompanyId.ToString()).Result;
                MVCCompanyInfoModel companyModel = responseCompany.Content.ReadAsAsync<MVCCompanyInfoModel>().Result;

                GlobalVeriables.WebApiClient.DefaultRequestHeaders.Clear();
                HttpResponseMessage billdetailresponse = GlobalVeriables.WebApiClient.GetAsync("GetBillDetailTablebyId/" + BillId.ToString()).Result;
                List<BillDetailViewModel> _billDetailList = billdetailresponse.Content.ReadAsAsync<List<BillDetailViewModel>>().Result;

                CommonModel commonModel = new CommonModel();
                commonModel.Name = "Bill";
                commonModel.FromDate = Convert.ToDateTime(ob.BillDate);
                commonModel.DueDate = Convert.ToDateTime(ob.BillDueDate);
                commonModel.ReferenceNumber = ob.RefNumber;
                commonModel.Number_Id = ob.Bill_ID;

                commonModel.SubTotal = ob.SubTotal.ToString();
                commonModel.Vat6 = ob.TotalVat6.ToString();
                commonModel.Vat21 = ob.TotalVat21.ToString();
                commonModel.grandTotal = ob.TotalAmount.ToString();
                commonModel.Note = ob.CustomerNote;                
                ViewBag.commonModel = commonModel;

                ViewBag.Contentdata = contectmodel;
                ViewBag.Companydata = companyModel;
                ViewBag.BilllData = ob;
                ViewBag.BillDetail = _billDetailList;
            }
            catch (Exception ex)
            {
            }
            return View();
        }




        [HttpPost]
        public ActionResult DeleteFile(int Id, string FileName)
        {
            try
            {


                if (CreatDirectoryClass.Delete(Id, FileName, "Bill"))
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
        public ActionResult UploadFiles(BillDetailViewModel billdetailviewModel)
        {
            try
            {

                string FileName = "";
                HttpFileCollectionBase files = Request.Files;
                for (int i = 0; i < files.Count; i++)
                {
                    HttpPostedFileBase file = files[i];

                    FileName = CreatDirectoryClass.UploadFileToDirectoryCommon(billdetailviewModel.BilID, "Bill", billdetailviewModel.file23, "Bill");
                }

                return new JsonResult { Data = new { FilePath = FileName, FileName = FileName } };
            }
            catch (Exception)
            {
                throw;
            }
        }


        public ActionResult Edit(int Id)
        {
            BillDetailViewModel _BilldetailViewModel = new BillDetailViewModel();

            try
            {
                ViewBag.FILE = CreatDirectoryClass.GetFileDirectiory(Id, "Bill");


                ViewBag.VatDrop = GetVatList();

                HttpResponseMessage res = GlobalVeriables.WebApiClient.GetAsync("GetbillDetail/" + Id.ToString()).Result;
                MvcBillModel ob = res.Content.ReadAsAsync<MvcBillModel>().Result;

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

                _BilldetailViewModel.BilID = ob.BilID;
                _BilldetailViewModel.Bill_ID = ob.Bill_ID;
                _BilldetailViewModel.BillDate = ob.BillDate;
                _BilldetailViewModel.BillDueDate = ob.BillDueDate;
                _BilldetailViewModel.RefNumber = ob.RefNumber;
                _BilldetailViewModel.SubTotal = ob.SubTotal;
                _BilldetailViewModel.TotalAmount = ob.TotalAmount;
                _BilldetailViewModel.CustomerNote = ob.CustomerNote;
                _BilldetailViewModel.Status = ob.Status;
                _BilldetailViewModel.TotalVat21 = ob.TotalVat21;
                _BilldetailViewModel.TotalVat6 = ob.TotalVat6;
                _BilldetailViewModel.CompanyId = ob.CompanyId;
                _BilldetailViewModel.UserId = ob.UserId;
                HttpResponseMessage billdetailresponse = GlobalVeriables.WebApiClient.GetAsync("GetBillDetailTablebyId/" + Id.ToString()).Result;
                List<BillDetailViewModel> _billDetailList = billdetailresponse.Content.ReadAsAsync<List<BillDetailViewModel>>().Result;
                CommonModel commonModel = new CommonModel();
                commonModel.Name = "Bill";
                commonModel.FromDate = ob.BillDate;
                commonModel.DueDate = ob.BillDueDate;
                commonModel.Number_Id = ob.Bill_ID;
                commonModel.ReferenceNumber = ob.RefNumber;
                ViewBag.commonModel = commonModel;
                ViewBag.Contentdata = contectmodel;
                ViewBag.Companydata = companyModel;
                ViewBag.BillData = ob;
                ViewBag.BillDetil = _billDetailList;
                return View(_BilldetailViewModel);
            }
            catch (Exception)
            {
                throw;
            }
        }


        public bool AddTransaction(BillDetailViewModel invoiceViewModel)
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
                    _TransactionTable.TransictionRefrenceId = invoiceViewModel.BilID.ToString();
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


        public bool Transaction(BillDetailViewModel invoiceViewModel, string TransType)
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
                    if (CommonController.DeleteFromTransactionTableByRefrenceId((int)invoiceViewModel.BilID))
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
        public ActionResult SaveDraft(BillDetailViewModel billDetailViewModel)
        {
            HttpResponseMessage responsses = new HttpResponseMessage();

            BillTable billtable = new BillTable();
            BillDetailViewModel billviewModel = new BillDetailViewModel();
            try
            {
                billtable.CompanyId = billDetailViewModel.CompanyId;
                billtable.UserId = Convert.ToInt32(Session["LoginUserID"]);
                billtable.Bill_ID = billDetailViewModel.Bill_ID.ToString();
                billtable.VenderId = billDetailViewModel.VenderId;
                billtable.RefNumber = billDetailViewModel.RefNumber;
                billtable.BillDate = billDetailViewModel.BillDate;
                billtable.BillDueDate = billDetailViewModel.BillDueDate;
                billtable.SubTotal = billDetailViewModel.SubTotal;
                billtable.DiscountAmount = billDetailViewModel.DiscountAmount;
                billtable.TotalAmount = billDetailViewModel.TotalAmount;
                billtable.CustomerNote = billDetailViewModel.CustomerNote;
                billtable.TotalVat6 = billDetailViewModel.TotalVat6;
                billtable.TotalVat21 = billDetailViewModel.TotalVat21;
                billtable.Status = "accepted";
                billtable.Type = StatusEnum.Goods.ToString();
                // bill Api
                HttpResponseMessage response = GlobalVeriables.WebApiClient.PostAsJsonAsync("AddBill", billtable).Result;
                BillTable billviewmodel = response.Content.ReadAsAsync<BillTable>().Result;

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    billviewModel.BilID = billviewmodel.BilID;
                    billDetailViewModel.BilID = billviewmodel.BilID;

                    if (billDetailViewModel.BillDetail != null)
                    {
                        foreach (BillDetailTable item in billDetailViewModel.BillDetail)
                        {
                            BillDetailTable billdetailtable = new BillDetailTable();

                            billdetailtable.BillID = billviewmodel.BilID;
                            billdetailtable.ItemId = item.ItemId;
                            billdetailtable.Description = item.Description;
                            billdetailtable.Quantity = item.Quantity;
                            billdetailtable.Rate = item.Rate;
                            billdetailtable.Total = item.Total;
                            billdetailtable.Type = item.Type;
                            billdetailtable.RowSubTotal = item.RowSubTotal;
                            billdetailtable.Vat = item.Vat;
                            billdetailtable.ServiceDate = item.ServiceDate;
                            // APIBill   
                            responsses = GlobalVeriables.WebApiClient.PostAsJsonAsync("AddBillDetail", billdetailtable).Result;
                            if (responsses.StatusCode != System.Net.HttpStatusCode.OK)
                            {
                                return new JsonResult { Data = new { Status = "Fail", BillId = billviewModel.BilID } };
                            }                           
                        }
                        if (Transaction(billDetailViewModel, "Add"))
                        {

                            // return new JsonResult { Data = new { Status = "Success", BillId = billviewModel.BilID } };
                        }

                        if (billDetailViewModel.file23[0] != null)
                        {
                            CreatDirectoryClass.UploadFileToDirectoryCommon(billviewModel.BilID, "Bill", billDetailViewModel.file23, "Bill");
                        }

                    }
                }
                else
                {
                    return new JsonResult { Data = new { Status = "Fail", BillId = billviewModel.BilID } };
                }
            }
            catch (Exception ex)
            {
                return new JsonResult { Data = new { Status = "Fail", Message = ex.Message.ToString() } };
            }
            return new JsonResult { Data = new { Status = "Success", BillId = billviewModel.BilID } };
        }




        [HttpPost]
        public JsonResult GetPurchaseId(int Id)
        {
            TempData["PurchaseOrderList"] = "true";

            try
            {
                MvcBillModel _BillDetailModel = new MvcBillModel();

                HttpResponseMessage responseInvoice = GlobalVeriables.WebApiClient.GetAsync("GetBillIdbyPurchaseId/" + Id).Result;
                _BillDetailModel = responseInvoice.Content.ReadAsAsync<MvcBillModel>().Result;
                if (responseInvoice.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return new JsonResult { Data = new { Status = "Success", Id = _BillDetailModel.BilID } };
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



        [HttpPost]
        public ActionResult SaveEmail(BillDetailViewModel billDetailViewModel)
        {
            HttpResponseMessage responsses = new HttpResponseMessage();

            BillTable billtable = new BillTable();
            BillDetailViewModel billviewModel = new BillDetailViewModel();
            try
            {
                billtable.CompanyId = billDetailViewModel.CompanyId;
                billtable.UserId = Convert.ToInt32(Session["LoginUserID"]);
                billtable.Bill_ID = billDetailViewModel.Bill_ID.ToString();
                billtable.VenderId = billDetailViewModel.VenderId;
                billtable.RefNumber = billDetailViewModel.RefNumber;
                billtable.BillDate = billDetailViewModel.BillDate;
                billtable.BillDueDate = billDetailViewModel.BillDueDate;
                billtable.SubTotal = billDetailViewModel.SubTotal;
                billtable.DiscountAmount = billDetailViewModel.DiscountAmount;
                billtable.TotalAmount = billDetailViewModel.TotalAmount;
                billtable.CustomerNote = billDetailViewModel.CustomerNote;
                billtable.TotalVat6 = billDetailViewModel.TotalVat6;
                billtable.TotalVat21 = billDetailViewModel.TotalVat21;
                billtable.Status = "accepted";
                billtable.Type = StatusEnum.Goods.ToString();
                // bill Api
                HttpResponseMessage response = GlobalVeriables.WebApiClient.PostAsJsonAsync("AddBill", billtable).Result;
                BillTable billviewmodel = response.Content.ReadAsAsync<BillTable>().Result;
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    billviewModel.BilID = billviewmodel.BilID;
                    billDetailViewModel.BilID = billviewmodel.BilID;
                    if (billDetailViewModel.BillDetail != null)
                    {
                        foreach (BillDetailTable item in billDetailViewModel.BillDetail)
                        {
                            BillDetailTable billdetailtable = new BillDetailTable();
                            billdetailtable.BillID = billviewmodel.BilID;
                            billdetailtable.ItemId = item.ItemId;
                            billdetailtable.Description = item.Description;
                            billdetailtable.Quantity = item.Quantity;
                            billdetailtable.Rate = item.Rate;
                            billdetailtable.Total = item.Total;
                            billdetailtable.Type = item.Type;
                            billdetailtable.RowSubTotal = item.RowSubTotal;
                            billdetailtable.Vat = item.Vat;
                            billdetailtable.ServiceDate = item.ServiceDate;
                            // APIBill   
                            responsses = GlobalVeriables.WebApiClient.PostAsJsonAsync("AddBillDetail", billdetailtable).Result;

                            if (responsses.StatusCode != System.Net.HttpStatusCode.OK)
                            {
                                return new JsonResult { Data = new { Status = "Fail", BillId = billviewModel.BilID } };
                            }
                        }

                        if (Transaction(billDetailViewModel, "Add"))
                        {

                        }
                        else
                        {
                            return new JsonResult { Data = new { Status = "Fail", BillId = billviewModel.BilID } };
                        }
                    }
                }
                else
                {
                    return new JsonResult { Data = new { Status = "Fail", BillId = billviewModel.BilID } };
                }

                if (billDetailViewModel.file23[0] != null)
                {
                    CreatDirectoryClass.UploadFileToDirectoryCommon(billDetailViewModel.BilID, "Bill", billDetailViewModel.file23, "Bill");
                }
            }
            catch (Exception ex)
            {

                return new JsonResult { Data = new { Status = "Fail", Message = ex.Message.ToString() } };
            }

            return new JsonResult { Data = new { Status = "Success", BillId = billviewModel.BilID } };
        }


        public ActionResult EmailBill(int Id)
        {

            EmailModel email = new EmailModel();
            var List = CreatDirectoryClass.GetFileDirectiory((int)Id, "Bill");
            List<Selected> _list = new List<Selected>();

            foreach (var Item in List)
            {
                _list.Add(new Selected { IsSelected = true, FileName = Item.DirectoryPath, Directory = Item.FileFolderPathe + "/" + Item.DirectoryPath });
            }

            email.SelectList = _list;



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


                MVCContactModel mvcContactModel = new MVCContactModel();


                if (TempData["ComtactModel"] != null)
                {
                    mvcContactModel = TempData["ComtactModel"] as MVCContactModel;
                }

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

                return RedirectToAction("ViewBilDetail", new { BillId = email.invoiceId });
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

        public string PrintView(int id)
        {
            string pdfname;
            try
            {
                ViewBag.FILE = CreatDirectoryClass.GetFileDirectiory(id, "Bill");
                HttpResponseMessage res = GlobalVeriables.WebApiClient.GetAsync("GetbillDetail/" + id).Result;
                MvcBillModel ob = res.Content.ReadAsAsync<MvcBillModel>().Result;
                ob.BilID = id;

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

                TempData["ComtactModel"] = contectmodel;

                HttpResponseMessage responseCompany = GlobalVeriables.WebApiClient.GetAsync("APIComapny/" + ob.CompanyId.ToString()).Result;
                MVCCompanyInfoModel companyModel = responseCompany.Content.ReadAsAsync<MVCCompanyInfoModel>().Result;

                HttpResponseMessage billdetailresponse = GlobalVeriables.WebApiClient.GetAsync("GetBillDetailTablebyId/" + id.ToString()).Result;
                List<BillDetailViewModel> _billDetailList = billdetailresponse.Content.ReadAsAsync<List<BillDetailViewModel>>().Result;

                ViewBag.Contentdata = contectmodel;
                ViewBag.Companydata = companyModel;
                ViewBag.BilllData = ob;
                ViewBag.BillDetail = _billDetailList;

                DateTime BILLDATE = ob.BillDate; //mm/dd/yyyy
                DateTime DUEDATE = ob.BillDueDate;//mm/dd/yyyy
                TimeSpan ts = BILLDATE.Subtract(BILLDATE);
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
        #endregion

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
                            purchadeDetail.ServiceDate =item.ServiceDate;
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


        

        public ActionResult Print(int? id)
        {
            try
            {
                HttpResponseMessage res = GlobalVeriables.WebApiClient.GetAsync("GetbillDetail/" + id).Result;
                MvcBillModel ob = res.Content.ReadAsAsync<MvcBillModel>().Result;
                ob.BilID = id;

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

                TempData["ComtactModel"] = contectmodel;

                HttpResponseMessage responseCompany = GlobalVeriables.WebApiClient.GetAsync("APIComapny/" + ob.CompanyId.ToString()).Result;
                MVCCompanyInfoModel companyModel = responseCompany.Content.ReadAsAsync<MVCCompanyInfoModel>().Result;

                HttpResponseMessage billdetailresponse = GlobalVeriables.WebApiClient.GetAsync("GetBillDetailTablebyId/" + id.ToString()).Result;
                List<BillDetailViewModel> _billDetailList = billdetailresponse.Content.ReadAsAsync<List<BillDetailViewModel>>().Result;

                ViewBag.Contentdata = contectmodel;
                ViewBag.Companydata = companyModel;
                ViewBag.BilllData = ob;
                ViewBag.BillDetail = _billDetailList;
                string PdfName = id + "-" + companyModel.CompanyName + ".pdf";


                return new Rotativa.PartialViewAsPdf("~/Views/Bills/PrintPartialView.cshtml")
                {
                    PageSize = Rotativa.Options.Size.A4,
                    MinimumFontSize = 16,
                    FileName = PdfName,
                    PageHeight = 40,
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



        [HttpPost]
        public ActionResult Edit(MvcPurchaseViewModel purchaseViewModel)
        {
            HttpResponseMessage responsses = new HttpResponseMessage();

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
                        purchadeDetail.ServiceDate = Convert.ToDateTime(item.ServiceDate);
                        purchadeDetail.PurchaseTotal = item.PurchaseTotal;
                        purchadeDetail.PurchaseVatPercentage = item.PurchaseVatPercentage;
                        purchadeDetail.PurchaseId = purchaseViewModel.PurchaseOrderID;

                        if (purchadeDetail.PurchaseOrderDetailsId == 0)
                        {
                            responsses = GlobalVeriables.WebApiClient.PostAsJsonAsync("APIPurchaseDetail", purchadeDetail).Result;
                            if (responsses.StatusCode != System.Net.HttpStatusCode.OK)
                            {
                                return new JsonResult { Data = new { Status = "Fail", Message = "Fail Process" } };
                            }
                        }
                        else
                        {
                            responsses = GlobalVeriables.WebApiClient.PutAsJsonAsync("APIPurchaseDetail/" + purchadeDetail.PurchaseOrderDetailsId, purchadeDetail).Result;

                            if (responsses.StatusCode != System.Net.HttpStatusCode.OK)
                            {
                                return new JsonResult { Data = new { Status = "Fail", Message = "Fail Process" } };
                            }
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


    }
}