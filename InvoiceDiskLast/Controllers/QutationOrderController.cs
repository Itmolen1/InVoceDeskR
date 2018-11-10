using InvoiceDiskLast.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace InvoiceDiskLast.Controllers
{

    [SessionExpire]
    public class QutationOrderController : Controller
    {
        // GET: QutationOrder
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult QutationOrderList()
        {
            PendingModel model = new PendingModel();
            model.FromDate = DateTime.Now;
            model.ToDate = DateTime.Now;

            return View(model);
        }

        [HttpPost]
        public ActionResult AddPending(PendingModel pendingModel)
        {

            try
            {
                HttpResponseMessage res = GlobalVeriables.WebApiClient.GetAsync("APIQutation/" + pendingModel.Purchase_QuataionId.ToString()).Result;
                MVCQutationModel puchasemodel = res.Content.ReadAsAsync<MVCQutationModel>().Result;
                puchasemodel.Status = pendingModel.Status;

                if (puchasemodel == null)
                {
                    return new JsonResult { Data = new { Status = "Fail", Message = "No record Found" } };
                }
                HttpResponseMessage response = GlobalVeriables.WebApiClient.PutAsJsonAsync("APIQutation/" + pendingModel.Purchase_QuataionId, puchasemodel).Result;

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
        public JsonResult UpdateOrderStaus(int QutationId, string Status)
        {
            QutationOrderStatusTable orderstatusModel = new QutationOrderStatusTable();

            try
            {
                HttpResponseMessage res = GlobalVeriables.WebApiClient.GetAsync("APIQutation/" + QutationId.ToString()).Result;
                MVCQutationModel qutationModel = res.Content.ReadAsAsync<MVCQutationModel>().Result;
                qutationModel.Status = Status;

                if (qutationModel == null)
                {
                    return new JsonResult { Data = new { Status = "Fail", Message = "No record Found" } };
                }

                HttpResponseMessage response = GlobalVeriables.WebApiClient.PutAsJsonAsync("APIQutation/" + QutationId, qutationModel).Result;

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    GlobalVeriables.WebApiClient.DefaultRequestHeaders.Clear();
                    if (qutationModel != null)
                    {
                        orderstatusModel.Fk_QutationID = qutationModel.QutationID;
                        orderstatusModel.Status = Status;
                        orderstatusModel.CreateDate = DateTime.Now;
                        orderstatusModel.CompanyId = Convert.ToInt32(Session["CompayID"]);
                        orderstatusModel.UserId = 1;
                        orderstatusModel.Type = "Qutation";
                        HttpResponseMessage response2 = GlobalVeriables.WebApiClient.PostAsJsonAsync("postOrderStatus", orderstatusModel).Result;

                        if (response2.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            return new JsonResult { Data = new { Status = "Success", Message = "Status is change successfully" } };
                          
                        }
                        else
                        {
                            return new JsonResult { Data = new { Status = "Fail", Message = "Fail to change status" } };
                          
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



        [HttpPost]
        public JsonResult GetQutationOrderList(string status)
        {

            if (status == "" || status == null)
            {
                status = "Open";
            }

            IEnumerable<MVCQutationModel> QutationOrderList;
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

                HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("QutationOrderListByStatus/" + status).Result;
                QutationOrderList = response.Content.ReadAsAsync<IEnumerable<MVCQutationModel>>().Result;

                if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
                {
                    QutationOrderList = QutationOrderList.Where(p => p.QutationID.ToString().Contains(search)
                  || p.RefNumber != null && p.RefNumber.ToLower().Contains(search.ToLower())
                  || p.QutationDate != null && p.QutationDate.ToString().ToLower().Contains(search.ToLower())
                  || p.DueDate != null && p.DueDate.ToString().ToLower().Contains(search.ToLower())
                  || p.Status != null && p.Status.ToString().ToLower().Contains(search.ToLower())
                  || p.SubTotal != null && p.SubTotal.ToString().ToLower().Contains(search.ToLower())
                  || p.Status != null && p.Status.ToString().ToLower().Contains(search.ToLower())).ToList();
                }

                int recordsTotal = recordsTotal = QutationOrderList.Count();
                var data = QutationOrderList.Skip(skip).Take(pageSize).ToList();
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
        public JsonResult GetPendingQutationDetail(int QutationId)
        {
            MVCQutationViewModel quutionviewModel = new MVCQutationViewModel();
            int Contectid, CompanyID = 0;
            MvcPurchaseViewModel purchaseviewModel = new MvcPurchaseViewModel();

            try
            {
                var idd = Session["ClientID"];
                var cdd = Session["CompayID"];


                if (Session["ClientID"] != null && Session["CompayID"] != null)
                {
                    Contectid = Convert.ToInt32(Session["ClientID"]);
                    CompanyID = Convert.ToInt32(Session["CompayID"]);


                    HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("ApiConatacts/" + Contectid.ToString()).Result;
                    MVCContactModel contectmodel = response.Content.ReadAsAsync<MVCContactModel>().Result;



                    HttpResponseMessage responseCompany = GlobalVeriables.WebApiClient.GetAsync("APIComapny/" + CompanyID.ToString()).Result;
                    MVCCompanyInfoModel companyModel = responseCompany.Content.ReadAsAsync<MVCCompanyInfoModel>().Result;


                    HttpResponseMessage responsep = GlobalVeriables.WebApiClient.GetAsync("APIProduct/" + CompanyID).Result;
                    List<MVCProductModel> productModel = responsep.Content.ReadAsAsync<List<MVCProductModel>>().Result;
                    ViewBag.Product = productModel;

                    HttpResponseMessage res = GlobalVeriables.WebApiClient.GetAsync("APIQutation/" + QutationId.ToString()).Result;
                    MVCQutationModel ob = res.Content.ReadAsAsync<MVCQutationModel>().Result;

                    quutionviewModel.QutationID = ob.QutationID;
                    quutionviewModel.Qutation_ID = ob.Qutation_ID;
                    quutionviewModel.QutationDate = ob.QutationDate;
                    quutionviewModel.RefNumber = ob.RefNumber;
                    quutionviewModel.DueDate = ob.DueDate;
                    quutionviewModel.CustomerNote = ob.CustomerNote;
                    quutionviewModel.SubTotal = ob.SubTotal;
                    quutionviewModel.TotalAmount = ob.TotalAmount;
                    quutionviewModel.TotalVat21 = (ob.TotalVat21 != null ? (float)(ob.TotalVat21) : (float)0.00);
                    quutionviewModel.TotalVat6 = (ob.TotalVat6 != null ? (float)(ob.TotalVat6) : (float)0.00);
                    quutionviewModel.ConatctId = (int)ob.ContactId;
                    HttpResponseMessage responseQutationDetailsList = GlobalVeriables.WebApiClient.GetAsync("APIQutationDetails/" + QutationId.ToString()).Result;
                    List<MVCQutationViewModel> QutationModelDetailsList = responseQutationDetailsList.Content.ReadAsAsync<List<MVCQutationViewModel>>().Result;
                    ViewBag.Contentdata = contectmodel;
                    ViewBag.Companydata = companyModel;
                    ViewBag.QutationDatailsList = QutationModelDetailsList;

                    return new JsonResult { Data = new { QutationList = QutationModelDetailsList, ContactData = contectmodel, CompanyDta = companyModel, purchase = ob } };
                }
            }
            catch (Exception ex)
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }

            return Json("", JsonRequestBehavior.AllowGet);

        }

    }
}