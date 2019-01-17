using InvoiceDiskLast.Models;
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



        //public ActionResult GetExpenseList()
        //{
        //    var draw = Request.Form.GetValues("draw").FirstOrDefault();
        //    var start = Request.Form.GetValues("start").FirstOrDefault();
        //    var length = Request.Form.GetValues("length").FirstOrDefault();
        //    var sortColumn = Request.Form.GetValues("columns[" +
        //    Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
        //    var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
        //    string search = Request.Form.GetValues("search[value]")[0];

        //    int pageSize = length != null ? Convert.ToInt32(length) : 0;
        //    int skip = start != null ? Convert.ToInt32(start) : 0;

        //    GlobalVeriables.WebApiClient.DefaultRequestHeaders.Clear();
        //    int CompanyId = Convert.ToInt32(Session["CompayID"]);
        //    GlobalVeriables.WebApiClient.DefaultRequestHeaders.Add("CompayID", CompanyId.ToString());

        //    int companyId = Convert.ToInt32(Session["CompayID"]);

        //    HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("GetPurchaseInvoiceList/" + StatusEnum.Goods + "/" + companyId).Result;
        //    PurchaseList = response.Content.ReadAsAsync<IEnumerable<MvcPurchaseModel>>().Result;


        //    if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
        //    {
        //        PurchaseList = PurchaseList.Where(p => p.PurchaseOrderID.ToString().Contains(search)
        //      || p.PurchaseRefNumber != null && p.PurchaseRefNumber.ToLower().Contains(search.ToLower())
        //      || p.PurchaseDate != null && p.PurchaseDate.ToString().ToLower().Contains(search.ToLower())
        //      || p.PurchaseDueDate != null && p.PurchaseDueDate.ToString().ToLower().Contains(search.ToLower())
        //      || p.Status != null && p.Status.ToString().ToLower().Contains(search.ToLower())
        //      || p.PurchaseTotoalAmount != null && p.PurchaseTotoalAmount.ToString().ToLower().Contains(search.ToLower())
        //      || p.Status != null && p.Status.ToString().ToLower().Contains(search.ToLower())).ToList();
        //    }

        //    int recordsTotal = recordsTotal = PurchaseList.Count();
        //    var data = PurchaseList.Skip(skip).Take(pageSize).ToList();
        //    return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data }, JsonRequestBehavior.AllowGet);
        //}
        //    catch (Exception ex)
        //    {
        //        Response.Write(ex.ToString());
        //        return Json(new { draw = 0, recordsFiltered = 0, recordsTotal = 0, data = 0 }, JsonRequestBehavior.AllowGet);
        //    }
        //    return Json(new { draw = 0, recordsFiltered = 0, recordsTotal = 0, data = 0 }, JsonRequestBehavior.AllowGet);
        //}


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
                expense.SUBTOTAL = ExpenseViewModel.SUBTOTAL;
                expense.VAT_AMOUNT = ExpenseViewModel.VAT_AMOUNT;
                expense.GRAND_TOTAL = ExpenseViewModel.GRAND_TOTAL;
                expense.AddedDate = ExpenseViewModel.AddedDate;
                expense.comapny_id = CompanyId;

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
