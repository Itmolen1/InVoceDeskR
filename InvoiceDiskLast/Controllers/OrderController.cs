using InvoiceDiskLast.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls.WebParts;

namespace InvoiceDiskLast.Controllers
{


    [SessionExpireAttribute]
    public class OrderController : Controller
    {
        // GET: OrderStatu
        public ActionResult Index()
        {
            return View();
        }



        public ActionResult PurchaseOrderList()
        {
            return View();
        }



        [HttpPost]
        public JsonResult GetPurderOrderList(string status)
        {

            if(status=="" || status == null)
            {
                status = "Open";
            }

            IEnumerable<MvcPurchaseModel> PurchaseList;
            try
            {
                var draw = Request.Form.GetValues("draw").FirstOrDefault();
                var start = Request.Form.GetValues("start").FirstOrDefault();
                var length = Request.Form.GetValues("length").FirstOrDefault();
                var sortColumn = Request.Form.GetValues("columns[" +
                Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
                string search = Request.Form.GetValues("search[value]")[0];

                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;

                int CompanyId = Convert.ToInt32(Session["CompayID"]);
                GlobalVeriables.WebApiClient.DefaultRequestHeaders.Add("CompayID", CompanyId.ToString());

               

                HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("OrderListByStatus/" + status).Result;
                PurchaseList = response.Content.ReadAsAsync<IEnumerable<MvcPurchaseModel>>().Result;

                if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
                {
                    PurchaseList = PurchaseList.Where(p => p.PurchaseOrderID.ToString().Contains(search)
                  || p.PurchaseRefNumber != null && p.PurchaseRefNumber.ToLower().Contains(search.ToLower())
                  || p.PurchaseDate != null && p.PurchaseDate.ToString().ToLower().Contains(search.ToLower())
                  || p.PurchaseDueDate != null && p.PurchaseDueDate.ToString().ToLower().Contains(search.ToLower())
                  || p.Status != null && p.Status.ToString().ToLower().Contains(search.ToLower())
                  || p.PurchaseTotoalAmount != null && p.PurchaseTotoalAmount.ToString().ToLower().Contains(search.ToLower())
                  || p.Status != null && p.Status.ToString().ToLower().Contains(search.ToLower())).ToList();
                }


                //    switch (sortColumn)
                //{
                //    case "ContactName":
                //        ContactsList = ContactsList.OrderBy(c => c.ContactName);
                //        break;
                //    case "Type":
                //        ContactsList = ContactsList.OrderBy(c => c.Type);
                //        break;


                //    case "BillingPersonName":
                //        ContactsList = ContactsList.OrderBy(c => c.BillingPersonName);
                //        break;

                //    case "BillingCompanyName":
                //        ContactsList = ContactsList.OrderBy(c => c.BillingCompanyName);
                //        break;

                //    case "BillingVatTRN":

                //        ContactsList = ContactsList.OrderBy(c => c.BillingVatTRN);
                //        break;

                //    default:
                //        ContactsList = ContactsList.OrderByDescending(c => c.ContactsId);
                //        break;
                //}


                int recordsTotal = recordsTotal = PurchaseList.Count();
                var data = PurchaseList.Skip(skip).Take(pageSize).ToList();
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Response.Write(ex.ToString());
                Json(new { draw = 0, recordsFiltered = 0, recordsTotal = 0, data = 0 }, JsonRequestBehavior.AllowGet);
            }
            return Json(null, JsonRequestBehavior.AllowGet);

        }



        [HttpPost]
        public JsonResult UpdateOrderStaus(int PurchaseId, string Status)
        {
            OrderStatusTable orderstatusModel = new OrderStatusTable();

            try
            {
                HttpResponseMessage res = GlobalVeriables.WebApiClient.GetAsync("APIPurchase/" + PurchaseId.ToString()).Result;
                MvcPurchaseModel puchasemodel = res.Content.ReadAsAsync<MvcPurchaseModel>().Result;                          
                puchasemodel.Status = Status;

                if (puchasemodel == null)
                {
                    return new JsonResult { Data = new { Status = "Fail",Message="No record Found" } };
                }

                  HttpResponseMessage response = GlobalVeriables.WebApiClient.PutAsJsonAsync("APIPurchase/" + PurchaseId, puchasemodel).Result;

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    GlobalVeriables.WebApiClient.DefaultRequestHeaders.Clear();

                    if (puchasemodel != null)
                    {
                        orderstatusModel.PurchaseOrderId = puchasemodel.PurchaseOrderID;
                        orderstatusModel.Status = Status;
                        orderstatusModel.CreatedDate = DateTime.Now;
                        orderstatusModel.CompanyId = Convert.ToInt32(Session["CompayID"]);
                        orderstatusModel.UserId = 1;
                        orderstatusModel.Type = "Purchase";
                        HttpResponseMessage  response2 = GlobalVeriables.WebApiClient.PostAsJsonAsync("APIOrderStatus", orderstatusModel).Result;

                        if (response2.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            return new JsonResult { Data = new { Status = "Success"} };
                        }
                        else
                        {
                            return new JsonResult { Data = new { Status= "Fail"} };
                        }

                    }               
                }
                
            }
            catch (Exception ex)
            {

                return new JsonResult { Data = new { Status = "Fail", ex.Message} };
            }

            return Json("", JsonRequestBehavior.AllowGet);
        }







    }
}