using InvoiceDiskLast.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace InvoiceDiskLast.Controllers
{
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
            GlobalVeriables.WebApiClient.DefaultRequestHeaders.Clear();
            GlobalVeriables.WebApiClient.DefaultRequestHeaders.Add("CompayID", CompanyId.ToString());
           
            HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("APIProductUnit").Result;

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
                HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("APIProductUnit/" + id.ToString()).Result;
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
                  
                    HttpResponseMessage response = GlobalVeriables.WebApiClient.PostAsJsonAsync("APIProductUnit", unitmodel).Result;

                    return Json(response.StatusCode, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    unitmodel.CompanyId = Convert.ToInt32(Session["CompayID"]);                
                    HttpResponseMessage response = GlobalVeriables.WebApiClient.PutAsJsonAsync("APIProductUnit/" + unitmodel.ProductUnitID, unitmodel).Result;

                    return Json(response.StatusCode, JsonRequestBehavior.AllowGet);
            }
           
        }


        //[HttpGet]
        //public ActionResult AddorEdit1()
        //{
        //    string name = "kg";
        //   HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("APIProductUnit/" + name.ToString()).Result;
           

         
          
        //}
       
    }
}