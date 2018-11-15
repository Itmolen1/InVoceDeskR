﻿using InvoiceDiskLast.Models;
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
                       || p.Description != null && p.Description.ToLower().Contains(search.ToLower())
                       || p.SalePrice != null && p.SalePrice.ToString().ToLower().Contains(search.ToLower())
                        //|| p.Prod != null && p.AddedDate.ToString().ToLower().Contains(search.ToLower())
                       || p.PurchasePrice != null && p.PurchasePrice.ToString().ToLower().Contains(search.ToLower())
                       || p.Type != null && p.Type.ToString().ToLower().Contains(search.ToLower())
                       || p.OpeningQuantity != null && p.OpeningQuantity.ToString().ToLower().Contains(search.ToLower())).ToList();
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
                    case "Description":
                        ProductList = ProductList.OrderBy(c => c.Description).ToList();
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

                    case "OpeningQuantity":

                        ProductList = ProductList.OrderBy(c => c.OpeningQuantity).ToList();
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
                return Json(response.Content.ReadAsAsync<MVCProductModel>().Result, JsonRequestBehavior.AllowGet);
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
                    HttpResponseMessage response = GlobalVeriables.WebApiClient.PostAsJsonAsync("PostProduct", ProductModel).Result;

                   
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        
                        ProductTable PModel = response.Content.ReadAsAsync<ProductTable>().Result;

                       

                        string base64Guid = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                        AccountTransictionTable accountTransictiontable = new AccountTransictionTable();

                        accountTransictiontable.TransictionDate = PModel.AddedDate;
                        accountTransictiontable.FK_AccountID = 4002;
                        accountTransictiontable.Cr = ProductModel.OpeningQuantity * PModel.PurchasePrice;
                        accountTransictiontable.Dr = 0.00;
                        accountTransictiontable.TransictionNumber = base64Guid;
                        accountTransictiontable.TransictionRefrenceId = PModel.ProductId.ToString();
                        accountTransictiontable.TransictionType = "Purchase";
                        accountTransictiontable.CreationTime = DateTime.Now.TimeOfDay;
                        accountTransictiontable.AddedBy = 1;
                        accountTransictiontable.FK_CompanyId = CompanyId;
                        accountTransictiontable.FKPaymentTerm = 1;
                        accountTransictiontable.Description = "Purchase created at product" + PModel.ProductName.ToString() + "First time";
                                               
                        HttpResponseMessage responses = GlobalVeriables.WebApiClient.PostAsJsonAsync("APIAccountTransiction", accountTransictiontable).Result;

                        if(responses.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            AccountTransictionTable accountTransictiontable1 = new AccountTransictionTable();
                           // accountTransictiontable1.




                            return Json("Created", JsonRequestBehavior.AllowGet);
                        }                       
                    }
                }
                else
                {
                    HttpResponseMessage response = GlobalVeriables.WebApiClient.PutAsJsonAsync("PutAPIProduct/" + ProductModel.ProductId, ProductModel).Result;
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            try
            {

                HttpResponseMessage response = GlobalVeriables.WebApiClient.DeleteAsync("DeleteProduct/" + id.ToString()).Result;

                return Json("Delete", JsonRequestBehavior.AllowGet);
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
    }
}