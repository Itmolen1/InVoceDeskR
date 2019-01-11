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
            try
            {
                HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("GetUpdateQuatationStatus/" + QuatationId).Result;
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    return true;
                else
                    return false;

            }
            catch (Exception)
            {

                throw;
            }

        }



        public bool AddTransaction(MVCQutationModel quatationModel)
        {
            bool result = false;
            try
            {
                TransactionModel model = new TransactionModel();
                model.CompanyId = (int)quatationModel.CompanyId;
                model.AccountTitle = "Accounts receivable";
                int AccountId = CommonController.GetAcccountId(model);
                if (AccountId > 0)
                {
                    AccountTransictionTable _TransactionTable = new AccountTransictionTable();
                    _TransactionTable.FK_AccountID = AccountId;
                    _TransactionTable.Dr = quatationModel.SubTotal;
                    _TransactionTable.Cr = 0;
                    _TransactionTable.TransictionNumber = Guid.NewGuid().ToString();
                    _TransactionTable.TransictionRefrenceId = quatationModel.QutationID.ToString();
                    _TransactionTable.CreationTime = DateTime.Now.TimeOfDay;
                    _TransactionTable.AddedBy = Convert.ToInt32(Session["LoginUserID"]);
                    _TransactionTable.FK_CompanyId = quatationModel.CompanyId;
                    _TransactionTable.TransictionType = "Dr";
                    _TransactionTable.FKPaymentTerm = null;
                    _TransactionTable.TransictionDate = DateTime.Now;
                    _TransactionTable.Description = "invoice Creating Time Transaction";

                    if (TransactionClass.PerformTransaction(_TransactionTable))
                    {
                        result = true;
                        model.AccountTitle = "Input VAT";
                        int AccountId1 = CommonController.GetAcccountId(model);
                        _TransactionTable.FK_AccountID = AccountId1;
                        _TransactionTable.Dr = quatationModel.TotalVat21 + quatationModel.TotalVat6;

                        if (TransactionClass.PerformTransaction(_TransactionTable))
                        {
                            result = true;
                            model.AccountTitle = "Cash on hand";
                            int AccountId12 = CommonController.GetAcccountId(model);
                            _TransactionTable.FK_AccountID = AccountId12;
                            _TransactionTable.Cr = quatationModel.TotalAmount;
                            _TransactionTable.Dr = 0;
                            _TransactionTable.TransictionType = "Cr";
                            if (TransactionClass.PerformTransaction(_TransactionTable))
                            {
                                result = true;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            return result;

        }


        public bool Transaction(MVCQutationModel quatationModel, string TransType)
        {
            bool result = false;

            try
            {
                if (TransType == "Add")
                {
                    if (AddTransaction(quatationModel))
                    {
                        return result = true;
                    }
                    else
                    {
                        return result = false;
                    }
                }

                else
                {
                    if (CommonController.DeleteFromTransactionTableByRefrenceId((int)quatationModel.QutationID))
                    {
                        if (AddTransaction(quatationModel))
                        {
                            return result = true;
                        }
                        else
                        {
                            return result = false;
                        }

                    }

                }

            }
            catch (Exception)
            {
                result = false;
                throw;
            }
            return result;

        }



        [HttpPost]
        public JsonResult Proceeds(int QutationId, string Status, string Type)
        {
            HttpResponseMessage responsses = new HttpResponseMessage();

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
                                responsses = GlobalVeriables.WebApiClient.PostAsJsonAsync("PostinvoiceDetails", InvoiceDetails).Result;

                                if (responsses.StatusCode != System.Net.HttpStatusCode.OK)
                                {
                                    return new JsonResult { Data = new { Status = "Fail", Message = "Fail to Proceed" } };
                                }
                            }

                            if (UpdateQuatationStation(QutationId))
                            {

                                if (Transaction(QutationModel, "Add"))
                                {
                                    return new JsonResult { Data = new { Status = "Success", Message = "Proceed successfullly" } };
                                }


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
        public JsonResult GetQutationOrderList(string status)
        {

            if (status == "" || status == null)
            {
                status = "open";
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
