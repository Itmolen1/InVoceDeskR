﻿using InvoiceDiskLast.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace InvoiceDiskLast.Controllers
{

    [SessionExpireAttribute]
    public class ExpenceController : Controller
    {
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

                int recordsTotal = recordsTotal = ExpenseList.Count();
                var data = ExpenseList.Skip(skip).Take(pageSize).ToList();
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Response.Write(ex.ToString());
                return Json(new { draw = 0, recordsFiltered = 0, recordsTotal = 0, data = 0 }, JsonRequestBehavior.AllowGet);
            }
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
                List<ExpenseDetailModel> ExpenseDetailList = expenseDetail.Content.ReadAsAsync<List<ExpenseDetailModel>>().Result;

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

                ExpenseMosel.GRAND_TOTAL = Convert.ToDecimal(ExpenseMosel.GRAND_TOTAL) - Convert.ToDecimal(ResultVAT);

                ExpenseMosel.GRAND_TOTAL = Convert.ToDecimal(ExpenseMosel.GRAND_TOTAL) - Convert.ToDecimal(total);

                ExpenseMosel.VAT_AMOUNT = ExpenseMosel.VAT_AMOUNT - Convert.ToDecimal(vat);
                ExpenseMosel.SUBTOTAL = ExpenseMosel.SUBTOTAL - Convert.ToDecimal(total);

                if (vat == 6)
                {
                    ExpenseMosel.Vat6 = (ExpenseMosel.Vat6) - Convert.ToDecimal(ResultVAT);
                }

                if (vat == 21)
                {
                    ExpenseMosel.Vat21 = (ExpenseMosel.Vat21) - Convert.ToDecimal(ResultVAT);
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
        public ActionResult UploadFiles(ExpenseViewModel MVCQutationViewModel)
        {
            try
            {

                string FileName = "";
                HttpFileCollectionBase files = Request.Files;
                for (int i = 0; i < files.Count; i++)
                {
                    HttpPostedFileBase file = files[i];

                    FileName = CreatDirectoryClass.UploadFileToDirectoryCommon(MVCQutationViewModel.Id, "Expense", MVCQutationViewModel.file23, "Expense");
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
                List<ExpenseDetailModel> ExpenseDetailList = expenseDetail.Content.ReadAsAsync<List<ExpenseDetailModel>>().Result;

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
        public ActionResult AddExpence(ExpenseViewModel ExpenseViewModel)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            int CompanyId = 0;
            int UserId = 0;
            EXPENSE expense = new EXPENSE();
            try
            {
                if (Session["CompayID"] != null)
                {
                    CompanyId = Convert.ToInt32(Session["CompayID"]);
                }

                if (Session["LoginUserID"] != null)
                {
                    UserId = Convert.ToInt32(Session["LoginUserID"]);
                }

                expense.REFERENCEno = ExpenseViewModel.REFERENCEno;
                expense.ACCOUNT_ID = ExpenseViewModel.ACCOUNT_ID;
                expense.VENDOR_ID = ExpenseViewModel.VENDOR_ID;
                expense.notes = ExpenseViewModel.notes;
                expense.user_id = UserId;
                expense.SUBTOTAL = ExpenseViewModel.SUBTOTAL;
                expense.VAT_AMOUNT = ExpenseViewModel.VAT_AMOUNT;
                expense.GRAND_TOTAL = ExpenseViewModel.GRAND_TOTAL;
                expense.AddedDate = ExpenseViewModel.AddedDate;
                expense.comapny_id = CompanyId;
                expense.Vat6 = ExpenseViewModel.Vat6;
                expense.Vat21 = ExpenseViewModel.Vat21;

                response = GlobalVeriables.WebApiClient.PostAsJsonAsync("PostExpense", expense).Result;
                EXPENSE Purchasetable = response.Content.ReadAsAsync<EXPENSE>().Result;

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    ExpenseViewModel.Id = Purchasetable.Id;

                    if (ExpenseViewModel.ExpensenDetailList != null)
                    {
                        foreach (ExpenseDetail item in ExpenseViewModel.ExpensenDetailList)
                        {
                            ExpenseDetail expenseDetailModel = new ExpenseDetail();
                            expenseDetailModel.expense_id = ExpenseViewModel.Id;
                            expenseDetailModel.EXPENSE_ACCOUNT_ID = item.EXPENSE_ACCOUNT_ID;
                            expenseDetailModel.DESCRIPTION = item.DESCRIPTION;
                            expenseDetailModel.AMOUNT = item.AMOUNT;
                            expenseDetailModel.TAX_PERCENT = item.TAX_PERCENT;
                            expenseDetailModel.TAX_AMOUNT = item.TAX_AMOUNT;
                            expenseDetailModel.SUBTOTAL = item.SUBTOTAL;
                            expenseDetailModel.user_id = UserId;
                            expenseDetailModel.comapny_id = CompanyId;
                            response = GlobalVeriables.WebApiClient.PostAsJsonAsync("PostExpenseDetail", expenseDetailModel).Result;

                            if (response.StatusCode != System.Net.HttpStatusCode.OK)
                            {
                                return new JsonResult { Data = new { Status = "Fail" } };
                            }
                        }
                    }
                }

                if (ExpenseViewModel.file23[0] != null)
                {
                    CreatDirectoryClass.UploadFileToDirectoryCommon(ExpenseViewModel.Id, "Expense", ExpenseViewModel.file23, "Expense");
                }
            }
            catch (Exception ex)
            {

                return new JsonResult { Data = new { Status = "Fail", Message = ex.Message.ToString() } };
            }
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return new JsonResult { Data = new { Status = "Success" } };
            }
            else
            {
                return new JsonResult { Data = new { Status = "Fail" } };
            }

        }

    }
}
