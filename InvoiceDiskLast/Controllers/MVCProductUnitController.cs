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


        public ActionResult ListProductUnit()
        {
            #region

            List<MVCProductUnitModel> ProductunitList = new List<MVCProductUnitModel>();
            
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

                HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("GetProductUnit/" + CompanyId + "/All").Result;
                ProductunitList = response.Content.ReadAsAsync<List<MVCProductUnitModel>>().Result;
                
                if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
                {

                    if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
                    {
                        ProductunitList = ProductunitList.Where(p => p.ProductUnitID.ToString().Contains(search)                       
                       || p.ProductUnit != null && p.ProductUnit.ToString().ToLower().Contains(search.ToLower())).ToList();
                    }
                }
                switch (sortColumn)
                {
                    case "ProductUnitID":
                        ProductunitList = ProductunitList.OrderBy(c => c.ProductUnitID).ToList();
                        break;
                    case "ProductUnit":
                        ProductunitList = ProductunitList.OrderBy(c => c.ProductUnit).ToList();
                        break;  
                    default:
                        ProductunitList = ProductunitList.OrderByDescending(c => c.ProductUnit).ToList();
                        break;
                }


                int recordsTotal = recordsTotal = ProductunitList.Count();
                var data = ProductunitList.Skip(skip).Take(pageSize).ToList();
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return View();
            #endregion

        }

        [HttpGet]
        public ActionResult GetProductUnit()
        {
            int CompanyId = Convert.ToInt32(Session["CompayID"]);

            HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("GetProductUnit/"+ CompanyId +"/status").Result;

            var ProductUnitList = response.Content.ReadAsAsync<IEnumerable<MVCProductUnitModel>>().Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return Json(ProductUnitList, JsonRequestBehavior.AllowGet);
            }

            return View();
        }

        [HttpGet]
        public ActionResult AddorEdit(int id)
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
            MVCProductUnitModel ProductunitModel = new MVCProductUnitModel();
            ProductunitModel.ProductUnit = name;

            if (name != null)
            {
                CheckUnit1 objectcount = new CheckUnit1();
                int CompanyId = Convert.ToInt32(Session["CompayID"]);              
                HttpResponseMessage response = GlobalVeriables.WebApiClient.PostAsJsonAsync("APIProductUnitByName/" + CompanyId, ProductunitModel).Result;

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


        public ActionResult UpdateUnitStatus(int id, bool ProductUnitStatus)
        {
            MVCProductUnitModel ProductunitModel = new MVCProductUnitModel();
            ProductunitModel.ProductUnitID = id;
            
            try
            {
                if (ProductUnitStatus == true)
                {
                    ProductunitModel.Status = false;
                }
                else
                {
                    ProductunitModel.Status = true;
                }
                HttpResponseMessage response = GlobalVeriables.WebApiClient.PostAsJsonAsync("UpdateUnitStatus/", ProductunitModel).Result;

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return Json("Ok", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(response.StatusCode, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return View();
        }

    }

    public class CheckUnit
    {
        public int countResult{ get; set; }
    }
}