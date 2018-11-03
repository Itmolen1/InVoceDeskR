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
        public ActionResult GetHeadAccountList(int ControleAccountId = 1)
        {
            
            List<HeadAccountTable> HeadAccount = new List<HeadAccountTable>();
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


                int CompanyId = Convert.ToInt32(Session["CompayID"]);

                //  IEnumerable<string> token;
                //   GlobalVeriables.WebApiClient.DefaultRequestHeaders.TryGetValues("accessToken",out token);                
               
                HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("HeadAccount/"+ ControleAccountId + "/" + CompanyId).Result;
                HeadAccount = response.Content.ReadAsAsync<List<HeadAccountTable>>().Result;

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
    }
}