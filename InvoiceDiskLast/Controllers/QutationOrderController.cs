using InvoiceDiskLast.MISC;
using InvoiceDiskLast.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace InvoiceDiskLast.Controllers
{

    [SessionExpireAttribute]
    [RouteNotFoundAttribute]
    public class QutationOrderController : Controller
    {

        // GET: QutationOrder
        int Contectid, CompanyID = 0;

        public ActionResult Index()
        {
            return View();
        }


        public ActionResult QutationOrderList(string status)
        {
            PendingModel model = new PendingModel();
            model.FromDate = DateTime.Now;
            model.ToDate = DateTime.Now;
            ViewBag.ststus = status;
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




        public List<MVCQutationViewModel> GetQuatationDetailListById(int QutationId)
        {

            HttpResponseMessage responseQutationDetailsList = GlobalVeriables.WebApiClient.GetAsync("APIQutationDetails/" + QutationId.ToString()).Result;
            List<MVCQutationViewModel> QutationModelDetailsList = responseQutationDetailsList.Content.ReadAsAsync<List<MVCQutationViewModel>>().Result;
            return QutationModelDetailsList;
        }


        public MVCQutationModel GetQutationById(int QutationId)
        {
            HttpResponseMessage responseQutation = GlobalVeriables.WebApiClient.GetAsync("APIQutation/" + QutationId.ToString()).Result;
            MVCQutationModel QutationModel = responseQutation.Content.ReadAsAsync<MVCQutationModel>().Result;
            return QutationModel;
        }



        public bool UpdateQuatationStation(int QuatationId)
        {

            HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("GetUpdateQuatationStatus/" + QuatationId).Result;
            QutationTable qtable = response.Content.ReadAsAsync<QutationTable>().Result;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                return true;
            else
                return false;
        }

        [HttpPost]
        public JsonResult Proceeds(int QutationId, string Status, string Type)
        {
            MVCQutationModel QutationModel = new MVCQutationModel();
            List<MVCQutationViewModel> _QuatationDetailList = new List<MVCQutationViewModel>();

            InvoiceTable InvoiceTable = new InvoiceTable();
            MVCInvoiceModel mvcInvoiceModel = new MVCInvoiceModel();
            try
            {
                QutationModel = GetQutationById(QutationId);
                _QuatationDetailList = GetQuatationDetailListById(QutationId);
                if (QutationModel != null)
                {
                    mvcInvoiceModel.Invoice_ID = QutationModel.Qutation_ID;
                    mvcInvoiceModel.QutationId = QutationModel.QutationID;
                    mvcInvoiceModel.CompanyId = QutationModel.CompanyId;
                    mvcInvoiceModel.UserId = Convert.ToInt32(Session["LoginUserID"]);
                    mvcInvoiceModel.ContactId = QutationModel.ContactId;
                    mvcInvoiceModel.InvoiceID = 0;
                    mvcInvoiceModel.RefNumber = QutationModel.RefNumber;
                    mvcInvoiceModel.InvoiceDate = QutationModel.QutationDate;
                    mvcInvoiceModel.InvoiceDueDate = QutationModel.DueDate;
                    mvcInvoiceModel.SubTotal = QutationModel.SubTotal;
                    mvcInvoiceModel.DiscountAmount = QutationModel.DiscountAmount;
                    mvcInvoiceModel.TotalAmount = QutationModel.TotalAmount;
                    mvcInvoiceModel.CustomerNote = QutationModel.CustomerNote;
                    mvcInvoiceModel.TotalVat21 = QutationModel.TotalVat21;
                    mvcInvoiceModel.TotalVat6 = QutationModel.TotalVat6;
                    mvcInvoiceModel.Type = StatusEnum.Goods.ToString();
                    mvcInvoiceModel.Status = "accepted";
                    HttpResponseMessage InvoiceResponse = GlobalVeriables.WebApiClient.PostAsJsonAsync("PostInvoice", mvcInvoiceModel).Result;
                    InvoiceTable = InvoiceResponse.Content.ReadAsAsync<InvoiceTable>().Result;

                    if (InvoiceResponse.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        if (_QuatationDetailList != null)
                        {
                            foreach (var item in _QuatationDetailList)
                            {
                                InvoiceDetailsTable InvoiceDetails = new InvoiceDetailsTable();
                                InvoiceDetails.ItemId = Convert.ToInt32(item.ItemId);
                                InvoiceDetails.InvoiceId = InvoiceTable.InvoiceID;
                                InvoiceDetails.Description = item.Description;
                                InvoiceDetails.Quantity = item.Quantity;
                                InvoiceDetails.Rate = Convert.ToDouble(item.Rate);
                                InvoiceDetails.Total = Convert.ToDouble(item.Total);
                                InvoiceDetails.ServiceDate = item.ServiceDate;
                                InvoiceDetails.RowSubTotal = item.RowSubTotal;
                                InvoiceDetails.Vat = Convert.ToDouble(item.Vat);
                                InvoiceDetails.Type = item.Type;
                                HttpResponseMessage responsses = GlobalVeriables.WebApiClient.PostAsJsonAsync("PostinvoiceDetails", InvoiceDetails).Result;

                                if (responsses.StatusCode != System.Net.HttpStatusCode.OK)
                                {
                                    return new JsonResult { Data = new { Status = "Fail", Message = "Fail to Proceed" } };
                                }

                            }

                            if (UpdateQuatationStation(QutationId))
                            {
                                return new JsonResult { Data = new { Status = "Success", Message = "Proceed successfullly" } };
                            }
                        }
                    }
                    else
                    {
                        return new JsonResult { Data = new { Status = "Fail", Message = "Fail Proceed successfullly" } };
                    }
                }
            }

            catch (Exception)
            {
                throw;
            }

            return new JsonResult { Data = new { Status = "Success", Message = "Proceed successfullly" } };
        }



        [HttpPost]
        public JsonResult UpdateOrderStaus(int QutationId, string Status, string Type)
        {

            List<MVCQutationViewModel> _QuatationModel = new List<MVCQutationViewModel>();
            QutationOrderStatusTable orderstatusModel = new QutationOrderStatusTable();
            List<MVCQutationViewModel> _QutationList = new List<MVCQutationViewModel>();
            MVCQutationViewModel _mvcQuatationViewModel23 = new MVCQutationViewModel();

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







    }

}
