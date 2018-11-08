using InvoiceDiskLast.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace InvoiceDiskLast.Controllers
{

    [SessionExpireAttribute]
    public class MVCProductUnitController : Controller
    {
        // GET: MVCProductUnit
        public ActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public ActionResult GetProductUnit()
        {
            int CompanyId = Convert.ToInt32(Session["CompayID"]);

            HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("GetProductUnit/"+ CompanyId).Result;

            var ProductUnitList = response.Content.ReadAsAsync<IEnumerable<MVCProductUnitModel>>().Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return Json(ProductUnitList, JsonRequestBehavior.AllowGet);
            }

            return View();
        }

        [HttpGet]
        public ActionResult AddorEdit(int id =0)
        {
            if(id == 0)
            { 

                return View(new MVCProductUnitModel());
            }
            else
            {
                HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("GetProductUnitByID/" + id).Result;
                MVCProductUnitModel MvcUnitModel = response.Content.ReadAsAsync<MVCProductUnitModel>().Result;
                
                return Json(MvcUnitModel, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult AddorEdit(MVCProductUnitModel unitmodel)
        {
            if (unitmodel.ProductUnitID == null)
            {
                    unitmodel.CompanyId = Convert.ToInt32(Session["CompayID"]);                  
                  
                    HttpResponseMessage response = GlobalVeriables.WebApiClient.PostAsJsonAsync("PostProductUnit", unitmodel).Result;

                    return Json(response.StatusCode, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    unitmodel.CompanyId = Convert.ToInt32(Session["CompayID"]);                
                    HttpResponseMessage response = GlobalVeriables.WebApiClient.PutAsJsonAsync("PutProductUnit/" + unitmodel.ProductUnitID, unitmodel).Result;

                    return Json(response.StatusCode, JsonRequestBehavior.AllowGet);
            }
           
        }


        [HttpPost]
        public ActionResult CheckUnitStatus(string name)
        {
            
            List<MVCProductUnitModel> ProductUnitmodel = new List<MVCProductUnitModel>();
            

            if (name != null)
            {
                CheckUnit1 objectcount = new CheckUnit1();
                int CompanyId = Convert.ToInt32(Session["CompayID"]);              
                HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("APIProductUnitByName/" + name.ToString() + "/" + CompanyId).Result;

                objectcount = response.Content.ReadAsAsync<CheckUnit1>().Result;
                
                if (objectcount.count > 0)
                {
                    return Json("Found", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("NotFound", JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json("NotFound Name", JsonRequestBehavior.AllowGet);
            }

        }

    }

    public class CheckUnit
    {
        public int countResult{ get; set; }
    }
}