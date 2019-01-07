
using InvoiceDiskLast.MISC;
using InvoiceDiskLast.Models;
using Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace InvoiceDiskLast.Controllers
{
    [SessionExpireAttribute]
    [RouteNotFoundAttribute]
    public class MVCProductController : Controller
    {
        private Ilog _iLog;

        public MVCProductController()
        {
            _iLog = Log.GetInstance;
        }
        // GET: MVCProduct
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetProductlist()
        {
            #region
            MVCProductController pt = new MVCProductController();
            List<MVCProductModel> ProductList = new List<MVCProductModel>();
            List<MVCProductModel> ProductList1 = new List<MVCProductModel>();
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

                HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("APIProduct/" + CompanyId + "/All").Result;
                ProductList = response.Content.ReadAsAsync<List<MVCProductModel>>().Result;


                if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
                {

                    if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
                    {
                        ProductList = ProductList.Where(p => p.ProductId.ToString().Contains(search)
                        || p.ProductName != null && p.ProductName.ToLower().Contains(search.ToLower())
                       || p.SalePrice != null && p.SalePrice.ToString().ToLower().Contains(search.ToLower())
                        //|| p.Prod != null && p.AddedDate.ToString().ToLower().Contains(search.ToLower())
                       || p.PurchasePrice != null && p.PurchasePrice.ToString().ToLower().Contains(search.ToLower())
                       || p.Type != null && p.Type.ToString().ToLower().Contains(search.ToLower())
                       || p.ProductStatus != null && p.ProductStatus.ToString().ToLower().Contains(search.ToLower())).ToList();
                    }
                }
                switch (sortColumn)
                {
                    case "ProductId":
                        ProductList = ProductList.OrderBy(c => c.ProductId).ToList();
                        break;
                    case "ProductName":
                        ProductList = ProductList.OrderBy(c => c.ProductName).ToList();
                        break;

                    case "SalePrice":
                        ProductList = ProductList.OrderBy(c => c.SalePrice).ToList();
                        break;

                    case "PurchasePrice":
                        ProductList = ProductList.OrderBy(c => c.PurchasePrice).ToList();
                        break;

                    case "Type":
                        ProductList = ProductList.OrderBy(c => c.Type).ToList();
                        break;
                    case "ProductStatus":
                        ProductList = ProductList.OrderBy(c => c.ProductStatus).ToList();
                        break;
                    default:
                        ProductList = ProductList.OrderByDescending(c => c.ProductId).ToList();
                        break;
                }


                int recordsTotal = recordsTotal = ProductList.Count();
                var data = ProductList.Skip(skip).Take(pageSize).ToList();
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return View();
            #endregion
        }
        //ujhy6t7y67y

        [HttpGet]
        public ActionResult GetProduct(string ProductStatus)
        {


           int CompanyId = Convert.ToInt32(Session["CompayID"]);

           
            HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("APIProductAdd/" + CompanyId + "/" + ProductStatus.ToString()).Result;
            List<MVCProductModel> ProductList = response.Content.ReadAsAsync<List<MVCProductModel>>().Result;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return Json(ProductList, JsonRequestBehavior.AllowGet);
            }

            return View();
        }


        [HttpPost]
        public ActionResult GetProductByName(string ProductName, string ProductStatus)
        {
            string Status = "Not Found";

            int CompanyId = Convert.ToInt32(Session["CompayID"]);
            HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("APIProduct/" + CompanyId + "/" + ProductStatus.ToString()).Result;
            List<MVCProductModel> ProductList = response.Content.ReadAsAsync<List<MVCProductModel>>().Result;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                foreach (MVCProductModel product in ProductList)
                {
                    if (product.ProductName == ProductName)
                    {
                        Status = "Found";
                        break;
                    }

                }
                return Json(Status, JsonRequestBehavior.AllowGet);
            }
            return Json(Status, JsonRequestBehavior.AllowGet);
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
                HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("APIProductByProductID/" + id.ToString()).Result;
                return Json(response.Content.ReadAsAsync<MVCProductViewModel>().Result, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult AddOrEdit(MVCProductModel ProductModel)
        {
            try
            {
                int CompanyId = Convert.ToInt32(Session["CompayID"]);
                ProductModel.AddedBy = 1;
                ProductModel.Company_ID = CompanyId;
                ProductModel.AddedDate = Convert.ToDateTime(System.DateTime.Now.ToShortDateString());
             
                if (ProductModel.ProductId == null)
                {
                    ProductModel.ProductStatus = true;
                    HttpResponseMessage response = GlobalVeriables.WebApiClient.PostAsJsonAsync("PostProduct", ProductModel).Result;

                    return Json("Created",JsonRequestBehavior.AllowGet);
                    
                }
                else
                {
                    ProductModel.ProductStatus = true;
                    HttpResponseMessage response = GlobalVeriables.WebApiClient.PutAsJsonAsync("PutAPIProduct/" + ProductModel.ProductId, ProductModel).Result;
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        return Json("Updated", JsonRequestBehavior.AllowGet);
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
        public ActionResult Delete(int id, bool ProductStatus)
        {
            try
            {
                if (ProductStatus == true)
                {
                    ProductStatus = false;
                }
                else
                {
                    ProductStatus = true;
                }
                HttpResponseMessage response = GlobalVeriables.WebApiClient.DeleteAsync("DeleteProduct/" + id.ToString() + "/" + ProductStatus).Result;

                return Json("ok", JsonRequestBehavior.AllowGet);
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
                HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("APIProductByProductID/" + id.ToString()).Result;
                return View(response.Content.ReadAsAsync<MVCProductModel>().Result);
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return View();
        }


        public int GetStockValueQuantity(int? id)
        {
            try
            {
                int CompanyId = 1003;
                MVCQutationViewModel _QutationList = new MVCQutationViewModel();
                MVCPurchaseDetailsModel PurchaseModel = new MVCPurchaseDetailsModel();

                //api in api/Qutation
                HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("GetSaleItemQty/" + id + "/" + CompanyId).Result;
                _QutationList = response.Content.ReadAsAsync<MVCQutationViewModel>().Result;


                //api in api/purchase
                HttpResponseMessage _response = GlobalVeriables.WebApiClient.GetAsync("GetPurchaseItemQTY/" + id + "/" + CompanyId).Result;
                PurchaseModel = _response.Content.ReadAsAsync<MVCPurchaseDetailsModel>().Result;


                int result = Convert.ToInt32(PurchaseModel.PurchaseQuantity) - Convert.ToInt32(_QutationList.Quantity);

                return result;
            }
            catch (Exception ex)
            {
                return 0;
            }

        }


        public ActionResult StockList()
        {
            return View();
        }

        //api/product/GetStockItemList

        public ActionResult GetItemStok()
        {

            List<StockViewModel> _StockList = new List<StockViewModel>();

            try
            {
                if (Session["CompayId"] != null)
                {
                    int CompanyId = Convert.ToInt32(Session["CompayId"]);
                    HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("GetStockItemList/" + CompanyId.ToString()).Result;
                    _StockList = response.Content.ReadAsAsync<List<StockViewModel>>().Result;
                    
                    int recordsTotal = recordsTotal = _StockList.Count();
                    var data = _StockList.ToList();
                    return Json(new {recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data }, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    return Json(_StockList, JsonRequestBehavior.AllowGet);
                }


            }
            catch (Exception)
            {

                throw;
            }

            
        }



    }
}