using InvoiceDiskLast.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace InvoiceDiskLast.Controllers
{
    public class MVCProductController : Controller
    {
        // GET: MVCProduct
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetProductlist()
        {
            


            IEnumerable<MVCProductModel> ProductList;

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

                GlobalVeriables.WebApiClient.DefaultRequestHeaders.Clear();
                GlobalVeriables.WebApiClient.DefaultRequestHeaders.Add("CompayID", CompanyId.ToString());
                HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("APIProduct").Result;
                ProductList = response.Content.ReadAsAsync<IEnumerable<MVCProductModel>>().Result;

                if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
                {
                    // Apply search  on multiple field  
                    ProductList = ProductList.Where(p => p.ProductId.ToString().Contains(search) ||
                     p.ProductName.ToLower().Contains(search.ToLower()) ||
                     p.Description.ToString().ToLower().Contains(search.ToLower()) ||
                     p.SalePrice.ToString().Contains(search.ToLower()) ||
                     p.PurchasePrice.ToString().Contains(search) ||
                     p.Type.ToString().ToLower().Contains(search.ToLower()) ||
                     p.OpeningQuantity.ToString().Contains(search.ToLower())).ToList();
                }
                switch (sortColumn)
                {
                    case "ProductId":
                        ProductList = ProductList.OrderBy(c => c.ProductId);
                        break;
                    case "ProductName":
                        ProductList = ProductList.OrderBy(c => c.ProductName);
                        break;
                    case "Description":
                        ProductList = ProductList.OrderBy(c => c.Description);
                        break;

                    case "SalePrice":
                        ProductList = ProductList.OrderBy(c => c.SalePrice);
                        break;

                    case "PurchasePrice":
                        ProductList = ProductList.OrderBy(c => c.PurchasePrice);
                        break;

                    case "Type":

                        ProductList = ProductList.OrderBy(c => c.Type);
                        break;

                    case "OpeningQuantity":

                        ProductList = ProductList.OrderBy(c => c.OpeningQuantity);
                        break;


                    default:
                        ProductList = ProductList.OrderByDescending(c => c.ProductId);
                        break;
                }

                //if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                //{
                //    var v = CompanyList.OrderBy(c=>c.);

                //}
                int recordsTotal = recordsTotal = ProductList.Count();
                var data = ProductList.Skip(skip).Take(pageSize).ToList();
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return View();
        }


        [HttpGet]
        public ActionResult GetProduct()
        {

            int CompanyId = Convert.ToInt32(Session["CompayID"]);

            GlobalVeriables.WebApiClient.DefaultRequestHeaders.Clear();
            GlobalVeriables.WebApiClient.DefaultRequestHeaders.Add("CompayID", CompanyId.ToString());
            HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("APIProduct").Result;
            List<MVCProductModel>  ProductList= response.Content.ReadAsAsync<List<MVCProductModel>>().Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return Json(ProductList, JsonRequestBehavior.AllowGet);
            }

            return View();
        }

        [HttpGet]
        public ActionResult AddOrEdit(int id = 0)
        {
            if (id == 0)
            {

                return View(new MVCProductModel());
            }
            else
            {
                HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("APIProduct/" + id.ToString()).Result;
                return Json(response.Content.ReadAsAsync<MVCProductModel>().Result,JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult AddOrEdit(MVCProductModel ProductModel)
        {
            if (!ModelState.IsValid)
            {
                return View(ProductModel);
            }
            else
            {
                if (ProductModel.ProductId == null)
                {
                    int CompanyId = Convert.ToInt32(Session["CompayID"]);
                    GlobalVeriables.WebApiClient.DefaultRequestHeaders.Clear();
                    GlobalVeriables.WebApiClient.DefaultRequestHeaders.Add("CompayID", CompanyId.ToString());
                    ProductModel.AddedBy = 1;
                    ProductModel.Company_ID = CompanyId;
                    ProductModel.AddedDate = Convert.ToDateTime(System.DateTime.Now.ToShortDateString());

                    HttpResponseMessage response = GlobalVeriables.WebApiClient.PostAsJsonAsync("APIProduct", ProductModel).Result;
                    TempData["SuccessMessage"] = "Saved Successfully";
                }
                else
                {
                    HttpResponseMessage response = GlobalVeriables.WebApiClient.PutAsJsonAsync("APIProduct/" + ProductModel.ProductId, ProductModel).Result;
                    TempData["SuccessMessage"] = "Updated Successfully";
                }
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            try
            {

                HttpResponseMessage response = GlobalVeriables.WebApiClient.DeleteAsync("APIProduct/" + id.ToString()).Result;
                TempData["SuccessMessage"] = "Delete Successfully";
                return Json("Delete",JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return View();
        }

        public ActionResult Details(int id)
        {
            try
            {
                HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("APIProduct/" + id.ToString()).Result;
                return View(response.Content.ReadAsAsync<MVCProductModel>().Result);
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return View();
        }
    }
}