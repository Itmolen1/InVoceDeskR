using InvoiceDiskLast.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace InvoiceDiskLast.Controllers
{
    public class MVCHeadAccountController : Controller
    {
        [SessionExpireAttribute]
        // GET: MVCHeadAccount
        public ActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public ActionResult GetHeadAccountList(int? ControlACid)
        {
            
            List<HeadAccountTable> HeadAccount = new List<HeadAccountTable>();
            if(ControlACid == null)
            {
                ControlACid = 0;
            }
            //List<MVCProductModel> ProductList1 = new List<MVCProductModel>();
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

                //  IEnumerable<string> token;
                //   GlobalVeriables.WebApiClient.DefaultRequestHeaders.TryGetValues("accessToken",out token);  

                if (ControlACid == 0)
                {

                    HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("HeadAccountbyId/" + companyid).Result;
                    HeadAccount = response.Content.ReadAsAsync<List<HeadAccountTable>>().Result;
                }
                else
                {
                    HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("HeadAccount/" + ControlACid + "/" + companyid).Result;
                    HeadAccount = response.Content.ReadAsAsync<List<HeadAccountTable>>().Result;
                }
                if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
                {

                    if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
                    {
                        HeadAccount = HeadAccount.Where(p => p.HeadAccountId.ToString().Contains(search)
                         || p.HeadAccountTitle != null && p.HeadAccountTitle.ToLower().Contains(search.ToLower())).ToList();
                     
                    }
                }
                switch (sortColumn)
                {
                    case "HeadAccountId":
                        HeadAccount = HeadAccount.OrderBy(c => c.HeadAccountId).ToList();
                        break;
                    case "HeadAccountTitle":
                        HeadAccount = HeadAccount.OrderBy(c => c.HeadAccountTitle).ToList();
                        break;
                    default:
                        HeadAccount = HeadAccount.OrderByDescending(c => c.HeadAccountId).ToList();
                        break;
                }

                //if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                //{
                //    var v = CompanyList.OrderBy(c=>c.);

                //}
                int recordsTotal = recordsTotal = HeadAccount.Count();
                var data = HeadAccount.Skip(skip).Take(pageSize).ToList();
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return View();
        }
        

        [HttpGet]
        public ActionResult GetControlAccount()
        {                   HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("GetControlAccount").Result;
            List<ControlAccountTable> ControlAccount = response.Content.ReadAsAsync<List<ControlAccountTable>>().Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return Json(ControlAccount, JsonRequestBehavior.AllowGet);
            }

            return View();
        }




        [HttpGet]
        public ActionResult GePaymentTerm()
        {
            HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("GetControlAccount").Result;
            List<ControlAccountTable> ControlAccount = response.Content.ReadAsAsync<List<ControlAccountTable>>().Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return Json(ControlAccount, JsonRequestBehavior.AllowGet);
            }

            return View();
        }



        [HttpPost]
        public ActionResult AddOrEdit(MVCHeadAccountModel mvcheadAccountModel)
        {
            try
            {
                mvcheadAccountModel.FK_CompanyId = Convert.ToInt32(Session["CompayID"]);
                mvcheadAccountModel.AddedBy = 1;
                if (mvcheadAccountModel.HeadAccountId == null || mvcheadAccountModel.HeadAccountId == 0)
                {
                    HttpResponseMessage response = GlobalVeriables.WebApiClient.PostAsJsonAsync("PostHeadAccount", mvcheadAccountModel).Result;

                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        return Json(response.StatusCode,JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    HttpResponseMessage response = GlobalVeriables.WebApiClient.PutAsJsonAsync("UpdateHeadAccount/" + mvcheadAccountModel.HeadAccountId, mvcheadAccountModel).Result;

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


        [HttpPost]
        public ActionResult CheckHeadAvailibility(string Name, int  controlacid)
        {
            MVCHeadAccountModel mvcHeadAccountModel = new MVCHeadAccountModel();
            mvcHeadAccountModel.FK_CompanyId = Convert.ToInt32(Session["CompayID"]);
            mvcHeadAccountModel.FK_ControlAccountID = controlacid;
            mvcHeadAccountModel.HeadAccountTitle = Name;
           HttpResponseMessage response = GlobalVeriables.WebApiClient.PostAsJsonAsync("HeadAccountTitle", mvcHeadAccountModel).Result;
            
            if(response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return Json("Found", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("NotFound", JsonRequestBehavior.AllowGet);
            }
            

        }


        public ActionResult GetHeadAccountByID(int HeadAccountID =0)
        {
            if(HeadAccountID > 0)
            {
                int companyid = Convert.ToInt32(Session["CompayID"]);

                
                HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("HeadAccountByAccountID/" + HeadAccountID + "/" + companyid).Result;


                MVCHeadAccountModel  HeadAccountmodelObj = response.Content.ReadAsAsync<MVCHeadAccountModel>().Result;
                
                return Json(HeadAccountmodelObj, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("Null", JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult GetHeadAccountList()
        {
            int companyid = Convert.ToInt32(Session["CompayID"]);


            HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("HeadAccountbyId/" + companyid).Result;


            List<MVCHeadAccountModel> HeadAccountmodelObj = response.Content.ReadAsAsync<List<MVCHeadAccountModel>>().Result;

            return Json(HeadAccountmodelObj, JsonRequestBehavior.AllowGet);
        }
    }
}