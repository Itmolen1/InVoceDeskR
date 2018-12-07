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
    public class MVCProductController : Controller
    {
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


                foreach (var i in ProductList)
                {
                    i.OpeningStockValue = pt.GetStockValueQuantity(i.ProductId);
                  
                }
                ViewBag.count = 23;

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
                       || p.ProductStatus != null && p.ProductStatus   .ToString().ToLower().Contains(search.ToLower())).ToList();
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
            HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("APIProduct/" + CompanyId + "/" + ProductStatus.ToString()).Result;
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
                foreach(MVCProductModel product in ProductList)
                {
                    if(product.ProductName == ProductName)
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
                int VatValue = ProductModel.VatValue;
                if (ProductModel.ProductId == null)
                {
                    ProductModel.ProductStatus = true;
                    HttpResponseMessage response = GlobalVeriables.WebApiClient.PostAsJsonAsync("PostProduct", ProductModel).Result;

                   
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        
                        ProductTable PModel = response.Content.ReadAsAsync<ProductTable>().Result;


                        double Total = Convert.ToDouble(ProductModel.OpeningQuantity * PModel.PurchasePrice);
                        double totalvat = Convert.ToDouble(Total/100 * VatValue + Total- Total);

                        string base64Guid = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                        AccountTransictionTable accountTransictiontable = new AccountTransictionTable();

                        accountTransictiontable.TransictionDate = PModel.AddedDate;
                        accountTransictiontable.FK_AccountID = 4002;
                        accountTransictiontable.Cr = Math.Round(totalvat + Total,2);
                        accountTransictiontable.Dr = 0.00;
                        accountTransictiontable.TransictionNumber = base64Guid;
                        accountTransictiontable.TransictionRefrenceId = PModel.ProductId.ToString();
                        accountTransictiontable.TransictionType = "Purchase";
                        accountTransictiontable.CreationTime = DateTime.Now.TimeOfDay;
                        accountTransictiontable.AddedBy = 1;
                        accountTransictiontable.FK_CompanyId = CompanyId;
                        accountTransictiontable.FKPaymentTerm = 1;
                        accountTransictiontable.Description = "Total + VAT ,Purchase created at product" + PModel.ProductName.ToString() + "First time added";
                                               
                        HttpResponseMessage responses = GlobalVeriables.WebApiClient.PostAsJsonAsync("APIAccountTransiction", accountTransictiontable).Result;

                        if(responses.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            base64Guid = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                            AccountTransictionTable accountTransictiontable1 = new AccountTransictionTable();
                            accountTransictiontable1.FK_AccountID = 3005;
                            accountTransictiontable1.TransictionDate = PModel.AddedDate;
                            accountTransictiontable1.Dr = Math.Round(totalvat, 2);
                            accountTransictiontable1.Cr = 0.00;
                            accountTransictiontable1.TransictionNumber = base64Guid;
                            accountTransictiontable1.TransictionRefrenceId = PModel.ProductId.ToString();
                            accountTransictiontable1.TransictionType = "Purchase VAT "+VatValue ;
                            accountTransictiontable1.CreationTime = DateTime.Now.TimeOfDay;
                            accountTransictiontable1.AddedBy = 1;
                            accountTransictiontable1.FK_CompanyId = CompanyId;
                            accountTransictiontable1.FKPaymentTerm = 1;
                            accountTransictiontable1.Description = "VAT" + VatValue+", Purchase created at product" + PModel.ProductName.ToString() + "First time added";

                            HttpResponseMessage response1 = GlobalVeriables.WebApiClient.PostAsJsonAsync("APIAccountTransiction", accountTransictiontable1).Result;

                            if (response1.StatusCode == System.Net.HttpStatusCode.OK)
                            {
                                base64Guid = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                                AccountTransictionTable accountTransictiontable2 = new AccountTransictionTable();

                                accountTransictiontable2.FK_AccountID = 4003;
                                accountTransictiontable2.TransictionDate = PModel.AddedDate;
                                accountTransictiontable2.Dr = Math.Round(Total, 2);
                                accountTransictiontable2.Cr = 0.00;
                                accountTransictiontable2.TransictionNumber = base64Guid;
                                accountTransictiontable2.TransictionRefrenceId = PModel.ProductId.ToString();
                                accountTransictiontable2.TransictionType = "Purchase Total";
                                accountTransictiontable2.CreationTime = DateTime.Now.TimeOfDay;
                                accountTransictiontable2.AddedBy = 1;
                                accountTransictiontable2.FK_CompanyId = CompanyId;
                                accountTransictiontable2.FKPaymentTerm = 1;
                                accountTransictiontable2.Description = "Total, Purchase created at product" + PModel.ProductName.ToString() + "First time added";

                                HttpResponseMessage response2 = GlobalVeriables.WebApiClient.PostAsJsonAsync("APIAccountTransiction", accountTransictiontable2).Result;


                                if (response2.StatusCode == System.Net.HttpStatusCode.OK)
                                {
                                    return Json("Created", JsonRequestBehavior.AllowGet);
                                }
                            }
                        }                       
                    }
                }
                else
                {
                    HttpResponseMessage response = GlobalVeriables.WebApiClient.PutAsJsonAsync("PutAPIProduct/" + ProductModel.ProductId, ProductModel).Result;
                    if(response.StatusCode == System.Net.HttpStatusCode.OK)
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
                if(ProductStatus == true)
                {
                    ProductStatus = false;
                }
                else
                {
                    ProductStatus = true;
                }
                HttpResponseMessage response = GlobalVeriables.WebApiClient.DeleteAsync("DeleteProduct/" + id.ToString()+ "/"+ ProductStatus).Result;

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
                HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("GetSaleItemQty/" + id+ "/" + CompanyId).Result;
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

        //api/product
        public ActionResult GetItemStok()
        {
            return View();
        }



    }
}