using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using InvoiceDiskLast.Models;
using InvoiceDiskLast.MISC;
using Logger;

namespace InvoiceDiskLast.Controllers
{
    //[SessionExpireAttribute]
    //[RouteNotFoundAttribute]
    public class MVCAccountsController : Controller
    {
        private Ilog _iLog;
        public MVCAccountsController()
        {
            _iLog = Log.GetInstance;
        }
        // GET: MVCAccounts
        public ActionResult Index()
        {
            return View();
        }



        [HttpPost]
        public ActionResult GetAccountList(int HeadAcoountIDs)
        {

            try
            {
                var draw = Request.Form.GetValues("draw").FirstOrDefault();
                var start = Request.Form.GetValues("start").FirstOrDefault();

                var length = Request.Form.GetValues("length").FirstOrDefault();
                var sortColumn = Request.Form.GetValues("columns[" +
                Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                string search = Request.Form.GetValues("search[value]")[0];
                int skip = start != null ? Convert.ToInt32(start) : 0;


                int companyid = Convert.ToInt32(Session["CompayID"]);


                HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("AccountByAccountID/" + HeadAcoountIDs + "/" + companyid).Result;

                List<MVCAccountTableModel> AccountOBj = response.Content.ReadAsAsync<List<MVCAccountTableModel>>().Result;

                if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
                {

                    if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
                    {
                        AccountOBj = AccountOBj.Where(p => p.AccountId.ToString().Contains(search)
                         || p.AccountTitle != null && p.AccountTitle.ToLower().Contains(search.ToLower())).ToList();

                    }
                }
                switch (sortColumn)
                {
                    case "HeadAccountId":
                        AccountOBj = AccountOBj.OrderBy(c => c.AccountId).ToList();
                        break;
                    case "HeadAccountTitle":
                        AccountOBj = AccountOBj.OrderBy(c => c.AccountTitle).ToList();
                        break;
                    default:
                        AccountOBj = AccountOBj.OrderByDescending(c => c.AccountId).ToList();
                        break;
                }

                //if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                //{
                //    var v = CompanyList.OrderBy(c=>c.);

                //}
                int recordsTotal = recordsTotal = AccountOBj.Count();
                var data = AccountOBj.Skip(skip).Take(pageSize).ToList();
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return View();

        }





        [HttpPost]
        public ActionResult CheckAccountAvailibility(string Name, int HdAcountId)
        {
            MVCAccountTableModel mvcAccountModel = new MVCAccountTableModel();
            mvcAccountModel.FK_CompanyId = Convert.ToInt32(Session["CompayID"]);
            mvcAccountModel.FK_HeadAccountId = HdAcountId;
            mvcAccountModel.AccountTitle = Name;
            HttpResponseMessage response = GlobalVeriables.WebApiClient.PostAsJsonAsync("AccountTitle", mvcAccountModel).Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return Json("Found", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("NotFound", JsonRequestBehavior.AllowGet);
            }


        }


        [HttpPost]
        public ActionResult AddOrEdit(MVCAccountTableModel mvcAccountModel)
        {
            try
            {
                mvcAccountModel.FK_CompanyId = Convert.ToInt32(Session["CompayID"]);
                mvcAccountModel.AddedBy = 1;
                if (mvcAccountModel.AccountId == null || mvcAccountModel.AccountId == 0)
                {
                    HttpResponseMessage response = GlobalVeriables.WebApiClient.PostAsJsonAsync("PostAccount", mvcAccountModel).Result;

                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        return Json(response.StatusCode, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    HttpResponseMessage response = GlobalVeriables.WebApiClient.PutAsJsonAsync("UpdateAccount/" + mvcAccountModel.AccountId, mvcAccountModel).Result;

                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        return Json(response.StatusCode, JsonRequestBehavior.AllowGet);
                    }
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public ActionResult GetExpenseAccount()
        {

            int companyid = Convert.ToInt32(Session["CompayID"]);
            HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("GetExpense/" + companyid).Result;
          List<MVCAccountTableModel> AccountmodelObj = response.Content.ReadAsAsync<List<MVCAccountTableModel>>().Result;
            return Json(AccountmodelObj, JsonRequestBehavior.AllowGet);
        }



        public ActionResult GetAccountByID(int AccountID = 0)
        {
            if (AccountID > 0)
            {
                int companyid = Convert.ToInt32(Session["CompayID"]);


                HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("AccountByID/" + AccountID + "/" + companyid).Result;


                MVCAccountTableModel AccountmodelObj = response.Content.ReadAsAsync<MVCAccountTableModel>().Result;

                return Json(AccountmodelObj, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("Null", JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult GetAccount()
        {

            int companyId = Convert.ToInt32(Session["CompayID"]);
            HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("AccountByAccountID/" + 0 + "/" + companyId).Result;
            List<MVCAccountTableModel> AccountOBj = response.Content.ReadAsAsync<List<MVCAccountTableModel>>().Result;
            return Json(AccountOBj, JsonRequestBehavior.AllowGet);
        }



        public ActionResult Account()
        {

            return View();
        }



        [HttpGet]
        public ActionResult GetAssetAccount(int HeadAccountId)
        {
            try
            {
                if (HeadAccountId == 0)
                {
                    int companyId = Convert.ToInt32(Session["CompayID"]);
                    HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("GetHeadAccount/" + 0 + "/" + companyId).Result;
                    List<MVCHeadAccountModel> AccountOBj = response.Content.ReadAsAsync<List<MVCHeadAccountModel>>().Result;
                    return Json(AccountOBj, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    int companyId = Convert.ToInt32(Session["CompayID"]);
                    HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("GetHeadAccount/" + HeadAccountId + "/" + companyId).Result;
                    List<MVCAccountTableModel> AccountOBj = response.Content.ReadAsAsync<List<MVCAccountTableModel>>().Result;
                    return Json(AccountOBj, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {


                return Json("Fail", JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }




        [HttpPost]
        public ActionResult GetAccountHeadAccount()
        {
            int companyId = Convert.ToInt32(Session["CompayID"]);
            HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("GetAssetAccount/" + 0 + "/" + companyId).Result;
            List<MVCAccountTableModel> AccountOBj = response.Content.ReadAsAsync<List<MVCAccountTableModel>>().Result;
            return Json(AccountOBj, JsonRequestBehavior.AllowGet);
        }

    }
}