using InvoiceDiskLast.MISC;
using InvoiceDiskLast.Models;
using Logger;
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
    [RouteNotFoundAttribute]
    public class OrderController : Controller
    {
        private Ilog _iLog;

        public OrderController()
        {
            _iLog = Log.GetInstance;
        }
        // GET: OrderStatu
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Result()
        {
            return View();
        }
        public ActionResult PurchaseOrderList()
        {

            PendingModel model = new PendingModel();
            model.FromDate = DateTime.Now;
            model.ToDate = DateTime.Now.AddDays(1);
            return View(model);
        }


        [HttpPost]
        public ActionResult AddPending(PendingModel pendingModel)
        {

            try
            {
                HttpResponseMessage res = GlobalVeriables.WebApiClient.GetAsync("APIPurchase/" + pendingModel.Purchase_QuataionId.ToString()).Result;
                MvcPurchaseModel puchasemodel = res.Content.ReadAsAsync<MvcPurchaseModel>().Result;
                puchasemodel.Status = pendingModel.Status;

                if (puchasemodel == null)
                {
                    return new JsonResult { Data = new { Status = "Fail", Message = "No record Found" } };
                }
                HttpResponseMessage response = GlobalVeriables.WebApiClient.PutAsJsonAsync("APIPurchase/" + pendingModel.Purchase_QuataionId, puchasemodel).Result;

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    PendingTable pending = new PendingTable();
                    pending.Purchase_QuataionId = pendingModel.Purchase_QuataionId;
                    pending.FromDate = pendingModel.FromDate;
                    pending.ToDate = pendingModel.ToDate;
                    pending.Description = pendingModel.Description;
                    pending.Status = pendingModel.Status;
                    HttpResponseMessage postPendintTableResponse = GlobalVeriables.WebApiClient.PostAsJsonAsync("Addpending", pending).Result;

                    if (postPendintTableResponse.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        return new JsonResult { Data = new { Status = "Success", Message = "Status is change successfully to pending" } };
                    }

                }
                else
                {
                    return new JsonResult { Data = new { Status = "Fail", Message = "Fail to update status in purchase" } };
                }

            }
            catch (Exception ex)
            {
                return new JsonResult { Data = new { Status = "Fail", Message = ex.Message } };
            }

            return Json("", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetPurderOrderList(string status)
        {

            if (status == "" || status == null)
            {
                status = "open";
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
        public JsonResult GetPendingItem(int PurchaseId)
        {
            int Contectid, CompanyID = 0;
            MvcPurchaseViewModel purchaseviewModel = new MvcPurchaseViewModel();
            try
            {
                var idd = Session["ClientId"];
                var cdd = Session["CompayID"];
                if (Session["ClientId"] != null && Session["CompayID"] != null)
                {
                    Contectid = Convert.ToInt32(Session["ClientId"]);
                    CompanyID = Convert.ToInt32(Session["CompayID"]);
                }
                else
                {
                    //  return RedirectToAction("Index", "Login");
                }

                GlobalVeriables.WebApiClient.DefaultRequestHeaders.Remove("CompayID");
                GlobalVeriables.WebApiClient.DefaultRequestHeaders.Add("CompayID", cdd.ToString());

                HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("ApiConatacts/" + idd.ToString()).Result;
                MVCContactModel contectmodel = response.Content.ReadAsAsync<MVCContactModel>().Result;

                HttpResponseMessage responseCompany = GlobalVeriables.WebApiClient.GetAsync("APIComapny/" + cdd.ToString()).Result;
                MVCCompanyInfoModel companyModel = responseCompany.Content.ReadAsAsync<MVCCompanyInfoModel>().Result;

                ViewBag.Contentdata = contectmodel;
                ViewBag.Companydata = companyModel;
                DateTime InvoiceDate = new DateTime();
                InvoiceDate = DateTime.Now;
                purchaseviewModel.PurchaseDate = InvoiceDate;
                purchaseviewModel.PurchaseDueDate = InvoiceDate.AddDays(+15);

                HttpResponseMessage res = GlobalVeriables.WebApiClient.GetAsync("APIPurchase/" + PurchaseId.ToString()).Result;
                MvcPurchaseModel ob = res.Content.ReadAsAsync<MvcPurchaseModel>().Result;

                purchaseviewModel.PurchaseOrderID = ob.PurchaseOrderID;
                purchaseviewModel.Purchase_ID = ob.PurchaseID;
                purchaseviewModel.PurchaseDate = Convert.ToDateTime(ob.PurchaseDate);
                purchaseviewModel.PurchaseDueDate = (DateTime)ob.PurchaseDueDate;
                purchaseviewModel.PurchaseRefNumber = ob.PurchaseRefNumber;
                purchaseviewModel.PurchaseSubTotal = ob.PurchaseSubTotal;
                purchaseviewModel.PurchaseDiscountPercenteage = ob.PurchaseDiscountPercenteage;
                purchaseviewModel.PurchaseDiscountAmount = ob.PurchaseDiscountAmount;
                purchaseviewModel.PurchaseVatPercentage = ob.PurchaseVatPercentage;
                purchaseviewModel.PurchaseTotoalAmount = ob.PurchaseTotoalAmount;
                purchaseviewModel.PurchaseVenderNote = ob.PurchaseVenderNote;
                purchaseviewModel.Status = ob.Status;
                purchaseviewModel.Vat21 = (int)ob.Vat21;
                purchaseviewModel.Vat6 = (int)ob.Vat6;
                purchaseviewModel.CompanyId = ob.CompanyId;
                purchaseviewModel.UserId = ob.UserId;

                HttpResponseMessage responseQutationDetailsList = GlobalVeriables.WebApiClient.GetAsync("APIPurchaseDetail/" + PurchaseId.ToString()).Result;
                List<MvcPurchaseViewModel> QutationModelDetailsList = responseQutationDetailsList.Content.ReadAsAsync<List<MvcPurchaseViewModel>>().Result;
                ViewBag.Contentdata = contectmodel;
                ViewBag.Companydata = companyModel;
                ViewBag.PurchaseDatailsList = QutationModelDetailsList;

                return new JsonResult { Data = new { PurchaseData = QutationModelDetailsList, ContectDetail = contectmodel, CompanyDta = companyModel, purchase = ob } };

            }
            catch (Exception ex)
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }

            return Json("", JsonRequestBehavior.AllowGet);

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
                    return new JsonResult { Data = new { Status = "Fail", Message = "No record Found" } };
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
                        HttpResponseMessage response2 = GlobalVeriables.WebApiClient.PostAsJsonAsync("InsertOrderStatus", orderstatusModel).Result;

                        if (response2.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            return new JsonResult { Data = new { Status = "Success", Message="Change status to"+ Status } };
                        }
                        else
                        {
                            return new JsonResult { Data = new { Status = "Fail", Message = "fail to change try again" } };
                        }

                    }
                }

            }
            catch (Exception ex)
            {

                return new JsonResult { Data = new { Status = "Fail", ex.Message } };
            }

            return Json("", JsonRequestBehavior.AllowGet);
        }

    }
}