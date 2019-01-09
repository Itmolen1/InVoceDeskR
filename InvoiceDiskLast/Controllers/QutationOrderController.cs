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



        public ActionResult Accepted()
        {
            return View();
        }

        public JsonResult GetAcceptedInvoice()
        {

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

                HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("QutationOrderListByStatus").Result;
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
            catch (Exception)
            {

                throw;
            }
        }


    }




    //[HttpPost]
    //public JsonResult GetQutationOrderList(string status)
    //{

    //    if (status == "" || status == null)
    //    {
    //        status = "open";
    //    }

    //    IEnumerable<MVCQutationModel> QutationOrderList;
    //    try
    //    {
    //        var draw = Request.Form.GetValues("draw").FirstOrDefault();
    //        var start = Request.Form.GetValues("start").FirstOrDefault();
    //        var length = Request.Form.GetValues("length").FirstOrDefault();
    //        var sortColumn = Request.Form.GetValues("columns[" +
    //        Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
    //        var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
    //        string search = Request.Form.GetValues("search[value]")[0];

    //        int pageSize = length != null ? Convert.ToInt32(length) : 0;
    //        int skip = start != null ? Convert.ToInt32(start) : 0;

    //        int CompanyId = Convert.ToInt32(Session["CompayID"]);
    //        GlobalVeriables.WebApiClient.DefaultRequestHeaders.Add("CompayID", CompanyId.ToString());

    //        HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("QutationOrderListByStatus/" + status).Result;
    //        QutationOrderList = response.Content.ReadAsAsync<IEnumerable<MVCQutationModel>>().Result;

    //        if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
    //        {
    //            QutationOrderList = QutationOrderList.Where(p => p.QutationID.ToString().Contains(search)
    //          || p.RefNumber != null && p.RefNumber.ToLower().Contains(search.ToLower())
    //          || p.QutationDate != null && p.QutationDate.ToString().ToLower().Contains(search.ToLower())
    //          || p.DueDate != null && p.DueDate.ToString().ToLower().Contains(search.ToLower())
    //          || p.Status != null && p.Status.ToString().ToLower().Contains(search.ToLower())
    //          || p.SubTotal != null && p.SubTotal.ToString().ToLower().Contains(search.ToLower())
    //          || p.Status != null && p.Status.ToString().ToLower().Contains(search.ToLower())).ToList();
    //        }

    //        int recordsTotal = recordsTotal = QutationOrderList.Count();
    //        var data = QutationOrderList.Skip(skip).Take(pageSize).ToList();
    //         return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data }, JsonRequestBehavior.AllowGet);
    //    }

    //    catch (Exception ex)
    //    {
    //        //Response.Write(ex.ToString());
    //        //return Json(new { draw = 0, recordsFiltered = 0, recordsTotal = 0, data = 0 }, JsonRequestBehavior.AllowGet);
    //        return null;
    //    }
    //   // return Json(null, JsonRequestBehavior.AllowGet);

    //}

    //[HttpPost]
    //public JsonResult GetPendingQutationDetail(int QutationId)
    //{
    //    MVCQutationViewModel quutionviewModel = new MVCQutationViewModel();
    //    int Contectid, CompanyID = 0;
    //    MvcPurchaseViewModel purchaseviewModel = new MvcPurchaseViewModel();

    //    try
    //    {
    //        var idd = Session["ClientID"];
    //        var cdd = Session["CompayID"];


    //        if (Session["ClientID"] != null && Session["CompayID"] != null)
    //        {
    //            Contectid = Convert.ToInt32(Session["ClientID"]);
    //            CompanyID = Convert.ToInt32(Session["CompayID"]);


    //            HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("ApiConatacts/" + Contectid.ToString()).Result;
    //            MVCContactModel contectmodel = response.Content.ReadAsAsync<MVCContactModel>().Result;



    //            HttpResponseMessage responseCompany = GlobalVeriables.WebApiClient.GetAsync("APIComapny/" + CompanyID.ToString()).Result;
    //            MVCCompanyInfoModel companyModel = responseCompany.Content.ReadAsAsync<MVCCompanyInfoModel>().Result;


    //            HttpResponseMessage responsep = GlobalVeriables.WebApiClient.GetAsync("APIProduct/" + CompanyID).Result;
    //            List<MVCProductModel> productModel = responsep.Content.ReadAsAsync<List<MVCProductModel>>().Result;
    //            ViewBag.Product = productModel;

    //            HttpResponseMessage res = GlobalVeriables.WebApiClient.GetAsync("APIQutation/" + QutationId.ToString()).Result;
    //            MVCQutationModel ob = res.Content.ReadAsAsync<MVCQutationModel>().Result;

    //            quutionviewModel.QutationID = ob.QutationID;
    //            quutionviewModel.Qutation_ID = ob.Qutation_ID;
    //            quutionviewModel.QutationDate = ob.QutationDate;
    //            quutionviewModel.RefNumber = ob.RefNumber;
    //            quutionviewModel.DueDate = ob.DueDate;
    //            quutionviewModel.CustomerNote = ob.CustomerNote;
    //            quutionviewModel.SubTotal = ob.SubTotal;
    //            quutionviewModel.TotalAmount = ob.TotalAmount;
    //            quutionviewModel.TotalVat21 = (ob.TotalVat21 != null ? (float)(ob.TotalVat21) : (float)0.00);
    //            quutionviewModel.TotalVat6 = (ob.TotalVat6 != null ? (float)(ob.TotalVat6) : (float)0.00);
    //            quutionviewModel.ConatctId = (int)ob.ContactId;
    //            HttpResponseMessage responseQutationDetailsList = GlobalVeriables.WebApiClient.GetAsync("APIQutationDetails/" + QutationId.ToString()).Result;
    //            List<MVCQutationViewModel> QutationModelDetailsList = responseQutationDetailsList.Content.ReadAsAsync<List<MVCQutationViewModel>>().Result;
    //            ViewBag.Contentdata = contectmodel;
    //            ViewBag.Companydata = companyModel;
    //            ViewBag.QutationDatailsList = QutationModelDetailsList;

    //            return new JsonResult { Data = new { QutationList = QutationModelDetailsList, ContactData = contectmodel, CompanyDta = companyModel, purchase = ob } };
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        return Json("", JsonRequestBehavior.AllowGet);
    //    }

    //    return Json("", JsonRequestBehavior.AllowGet);

    //}


    //[HttpPost]
    //public ActionResult DeleteQuatation(int QutationId, int QutationDetailID, int vat, decimal total)
    //{
    //    try
    //    {

    //        MVCQutationViewModel viewModel = new MVCQutationViewModel();
    //        viewModel.QutationDetailId = QutationDetailID;
    //        HttpResponseMessage responseQutation = GlobalVeriables.WebApiClient.GetAsync("APIQutation/" + QutationId.ToString()).Result;
    //        MVCQutationModel QutationModel = responseQutation.Content.ReadAsAsync<MVCQutationModel>().Result;

    //        GlobalVeriables.WebApiClient.DefaultRequestHeaders.Clear();

    //        GlobalVeriables.WebApiClient.DefaultRequestHeaders.Add("QTID", QutationId.ToString());
    //        //GlobalVeriables.WebApiClient.DefaultRequestHeaders.Add("QutationDetailID1", QutationDetailID1);



    //        HttpResponseMessage responseQutationDetailsList = GlobalVeriables.WebApiClient.GetAsync("APIQutationDetails/" + QutationId.ToString()).Result;
    //        List<MVCQutationDetailsModel> QutationModelDetailsList = responseQutationDetailsList.Content.ReadAsAsync<List<MVCQutationDetailsModel>>().Result;
    //        MVCQutationDetailsModel CMODEL = new MVCQutationDetailsModel();

    //        if (QutationModelDetailsList.Count() > 1)
    //        {
    //            QutationModelDetailsList = QutationModelDetailsList.Where(C => C.QutationDetailId == QutationDetailID).ToList();

    //            #region
    //            if (QutationModelDetailsList.Count() != 0)
    //            {
    //                CMODEL.Vat = QutationModelDetailsList[0].Vat;
    //                CMODEL.Total = QutationModelDetailsList[0].Total;
    //                QutationModel.SubTotal = QutationModel.SubTotal - CMODEL.Total;
    //                QutationModel.TotalAmount = QutationModel.TotalAmount - (CMODEL.Vat + CMODEL.Total);

    //                QutationModel.QutationID = QutationModel.QutationID;



    //                QutationModel.QutationDate = QutationModel.QutationDate;
    //                QutationModel.CustomerNote = QutationModel.CustomerNote;
    //                QutationModel.Qutation_ID = QutationModel.Qutation_ID;
    //                QutationModel.DueDate = QutationModel.DueDate;
    //                QutationModel.Status = QutationModel.Status;
    //                QutationModel.CompanyId = QutationModel.CompanyId;
    //                if (vat == 6)
    //                    QutationModel.TotalVat6 = QutationModel.TotalVat6 - 6;
    //                else
    //                    QutationModel.TotalVat21 = QutationModel.TotalVat21 - 21;
    //                HttpResponseMessage response = GlobalVeriables.WebApiClient.PutAsJsonAsync("APIQutation/" + QutationId, QutationModel).Result;
    //                if (response.StatusCode == System.Net.HttpStatusCode.OK)
    //                {
    //                    HttpResponseMessage deleteQuaution2 = GlobalVeriables.WebApiClient.DeleteAsync("APIQutationDetails/" + QutationDetailID).Result;
    //                    if (deleteQuaution2.StatusCode == System.Net.HttpStatusCode.OK)
    //                    {
    //                        GlobalVeriables.WebApiClient.DefaultRequestHeaders.Clear();
    //                        return new JsonResult { Data = new { Status = "Success" } };
    //                    }
    //                }
    //                else
    //                {
    //                    return new JsonResult { Data = new { Status = "Fail" } };


    //                }
    //                #endregion
    //            }
    //        }

    //        else
    //        {

    //            HttpResponseMessage Qdresponse = GlobalVeriables.WebApiClient.DeleteAsync("APIQutationDetails/" + QutationDetailID).Result;

    //            if (Qdresponse.StatusCode == System.Net.HttpStatusCode.OK)
    //            {
    //                QutationModel.SubTotal = 0.00;
    //                QutationModel.TotalAmount = 0.00;
    //                QutationModel.Qutation_ID = QutationModel.Qutation_ID;
    //                QutationModel.QutationID = QutationModel.QutationID;
    //                QutationModel.QutationDate = QutationModel.QutationDate;
    //                QutationModel.CustomerNote = QutationModel.CustomerNote;
    //                QutationModel.Qutation_ID = QutationModel.Qutation_ID;
    //                QutationModel.DueDate = QutationModel.DueDate;
    //                QutationModel.Status = QutationModel.Status;
    //                QutationModel.CompanyId = QutationModel.CompanyId;

    //                QutationModel.TotalVat6 = 0;

    //                QutationModel.TotalVat21 = 0;

    //                HttpResponseMessage response = GlobalVeriables.WebApiClient.PutAsJsonAsync("APIQutation/" + QutationId, QutationModel).Result;
    //                if (response.StatusCode == System.Net.HttpStatusCode.OK)
    //                {
    //                    GlobalVeriables.WebApiClient.DefaultRequestHeaders.Clear();
    //                    return new JsonResult { Data = new { Status = "Success" } };
    //                }
    //            }
    //            else
    //            {
    //                return new JsonResult { Data = new { Status = "Fail" } };


    //            }
    //            //    HttpResponseMessage deleteQuaution = GlobalVeriables.WebApiClient.DeleteAsync("APIQutationDetail/" + QutationId).Result;



    //            //if (deleteQuaution.StatusCode == System.Net.HttpStatusCode.OK)
    //            //{
    //            //  return Json("Success", JsonRequestBehavior.AllowGet);
    //            // }

    //        }



    //    }
    //    catch (Exception)
    //    {

    //        return new JsonResult { Data = new { Status = "Fail" } };
    //    }

    //    return new JsonResult { Data = new { Status = "Success" } };
    //}



    //[HttpPost]
    //public JsonResult GetPendingQutation(int QutationId)
    //{
    //    MVCQutationViewModel quutionviewModel = new MVCQutationViewModel();
    //    try
    //    {

    //        HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("ApiConatacts/" + 1.ToString()).Result;
    //        MVCContactModel contectmodel = response.Content.ReadAsAsync<MVCContactModel>().Result;


    //        HttpResponseMessage responseCompany = GlobalVeriables.WebApiClient.GetAsync("APIComapny/" + 1.ToString()).Result;
    //        MVCCompanyInfoModel companyModel = responseCompany.Content.ReadAsAsync<MVCCompanyInfoModel>().Result;


    //        HttpResponseMessage responsep = GlobalVeriables.WebApiClient.GetAsync("APIProduct/" + 1 + "/Good").Result;
    //        List<MVCProductModel> productModel = responsep.Content.ReadAsAsync<List<MVCProductModel>>().Result;
    //        ViewBag.Product = productModel;

    //        HttpResponseMessage res = GlobalVeriables.WebApiClient.GetAsync("APIQutation/" + QutationId.ToString()).Result;
    //        MVCQutationModel ob = res.Content.ReadAsAsync<MVCQutationModel>().Result;

    //        quutionviewModel.QutationID = ob.QutationID;
    //        quutionviewModel.Qutation_ID = ob.Qutation_ID;
    //        quutionviewModel.QutationDate = ob.QutationDate;
    //        quutionviewModel.RefNumber = ob.RefNumber;
    //        quutionviewModel.DueDate = ob.DueDate;
    //        quutionviewModel.CustomerNote = ob.CustomerNote;
    //        quutionviewModel.SubTotal = ob.SubTotal;
    //        quutionviewModel.TotalAmount = ob.TotalAmount;
    //        quutionviewModel.TotalVat21 = (ob.TotalVat21 != null ? (float)(ob.TotalVat21) : (float)0.00);
    //        quutionviewModel.TotalVat6 = (ob.TotalVat6 != null ? (float)(ob.TotalVat6) : (float)0.00);
    //        quutionviewModel.ConatctId = (int)ob.ContactId;
    //        HttpResponseMessage responseQutationDetailsList = GlobalVeriables.WebApiClient.GetAsync("APIQutationDetails/" + QutationId.ToString()).Result;
    //        List<MVCQutationViewModel> QutationModelDetailsList = responseQutationDetailsList.Content.ReadAsAsync<List<MVCQutationViewModel>>().Result;
    //        ViewBag.Contentdata = contectmodel;
    //        ViewBag.Companydata = companyModel;
    //        ViewBag.QutationDatailsList = QutationModelDetailsList;

    //        return new JsonResult { Data = new { QutationDat = QutationModelDetailsList, ContectDetail = contectmodel, CompanyDta = companyModel, purchase = ob } };
    //    }
    //    catch (Exception ex)
    //    {
    //        return Json("", JsonRequestBehavior.AllowGet);
    //    }

    //    return Json("", JsonRequestBehavior.AllowGet);

    //}

    //[HttpPost]
    //public JsonResult GetPendingItem(int PurchaseId)
    //{
    //    int Contectid, CompanyID = 0;
    //    MvcPurchaseViewModel purchaseviewModel = new MvcPurchaseViewModel();
    //    try
    //    {
    //        var idd = Session["ClientId"];
    //        var cdd = Session["CompayID"];
    //        if (Session["ClientId"] != null && Session["CompayID"] != null)
    //        {
    //            Contectid = Convert.ToInt32(Session["ClientId"]);
    //            CompanyID = Convert.ToInt32(Session["CompayID"]);
    //        }
    //        else
    //        {
    //            //  return RedirectToAction("Index", "Login");
    //        }

    //        GlobalVeriables.WebApiClient.DefaultRequestHeaders.Remove("CompayID");
    //        GlobalVeriables.WebApiClient.DefaultRequestHeaders.Add("CompayID", cdd.ToString());

    //        HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("ApiConatacts/" + idd.ToString()).Result;
    //        MVCContactModel contectmodel = response.Content.ReadAsAsync<MVCContactModel>().Result;

    //        HttpResponseMessage responseCompany = GlobalVeriables.WebApiClient.GetAsync("APIComapny/" + cdd.ToString()).Result;
    //        MVCCompanyInfoModel companyModel = responseCompany.Content.ReadAsAsync<MVCCompanyInfoModel>().Result;

    //        ViewBag.Contentdata = contectmodel;
    //        ViewBag.Companydata = companyModel;
    //        DateTime InvoiceDate = new DateTime();
    //        InvoiceDate = DateTime.Now;
    //        purchaseviewModel.PurchaseDate = InvoiceDate;
    //        purchaseviewModel.PurchaseDueDate = InvoiceDate.AddDays(+15);

    //        HttpResponseMessage res = GlobalVeriables.WebApiClient.GetAsync("APIPurchase/" + PurchaseId.ToString()).Result;
    //        MvcPurchaseModel ob = res.Content.ReadAsAsync<MvcPurchaseModel>().Result;

    //        purchaseviewModel.PurchaseOrderID = ob.PurchaseOrderID;
    //        purchaseviewModel.Purchase_ID = ob.PurchaseID;
    //        purchaseviewModel.PurchaseDate = Convert.ToDateTime(ob.PurchaseDate);
    //        purchaseviewModel.PurchaseDueDate = (DateTime)ob.PurchaseDueDate;
    //        purchaseviewModel.PurchaseRefNumber = ob.PurchaseRefNumber;
    //        purchaseviewModel.PurchaseSubTotal = ob.PurchaseSubTotal;
    //        purchaseviewModel.PurchaseDiscountPercenteage = ob.PurchaseDiscountPercenteage;
    //        purchaseviewModel.PurchaseDiscountAmount = ob.PurchaseDiscountAmount;
    //        purchaseviewModel.PurchaseVatPercentage = ob.PurchaseVatPercentage;
    //        purchaseviewModel.PurchaseTotoalAmount = ob.PurchaseTotoalAmount;
    //        purchaseviewModel.PurchaseVenderNote = ob.PurchaseVenderNote;
    //        purchaseviewModel.Status = ob.Status;
    //        purchaseviewModel.Vat21 = (int)ob.Vat21;
    //        purchaseviewModel.Vat6 = (int)ob.Vat6;
    //        purchaseviewModel.CompanyId = ob.CompanyId;
    //        purchaseviewModel.UserId = ob.UserId;

    //        HttpResponseMessage responseQutationDetailsList = GlobalVeriables.WebApiClient.GetAsync("APIPurchaseDetail/" + PurchaseId.ToString()).Result;
    //        List<MvcPurchaseViewModel> QutationModelDetailsList = responseQutationDetailsList.Content.ReadAsAsync<List<MvcPurchaseViewModel>>().Result;
    //        ViewBag.Contentdata = contectmodel;
    //        ViewBag.Companydata = companyModel;
    //        ViewBag.PurchaseDatailsList = QutationModelDetailsList;

    //        return new JsonResult { Data = new { PurchaseData = QutationModelDetailsList, ContectDetail = contectmodel, CompanyDta = companyModel, purchase = ob } };

    //    }
    //    catch (Exception ex)
    //    {
    //        return Json("", JsonRequestBehavior.AllowGet);
    //    }

    //    return Json("", JsonRequestBehavior.AllowGet);

    //}


    //[HttpPost]
    //public ActionResult Trasactionpayment(List<TransactionModel> TransactionModel)
    //{

    //    try
    //    {

    //        HttpResponseMessage res = GlobalVeriables.WebApiClient.GetAsync("APIQutation/" + TransactionModel[0].PurchaseOrderID.ToString()).Result;
    //        MVCQutationModel ob = res.Content.ReadAsAsync<MVCQutationModel>().Result;
    //        if (res.StatusCode == System.Net.HttpStatusCode.OK)
    //        {

    //            foreach (var item in TransactionModel)
    //            {
    //                string base64Guid1 = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
    //                AccountTransictionTable accountTransictiontable = new AccountTransictionTable();
    //                accountTransictiontable.FK_CompanyId = Convert.ToInt32(Session["CompayID"]);
    //                accountTransictiontable.Description = item.descrition;
    //                accountTransictiontable.FK_AccountID = item.Id;
    //                accountTransictiontable.Description = item.descrition;
    //                accountTransictiontable.FKPaymentTerm = 1;
    //                accountTransictiontable.TransictionRefrenceId = ob.QutationID.ToString();
    //                accountTransictiontable.Dr = item.AmountDebit;
    //                accountTransictiontable.Cr = item.AmountCredit;
    //                accountTransictiontable.TransictionNumber = base64Guid1;
    //                accountTransictiontable.TransictionType = "Qutation";
    //                accountTransictiontable.CreationTime = DateTime.Now.TimeOfDay;
    //                accountTransictiontable.TransictionDate = item.TranDate;
    //                TransactionClass.PerformTransaction(accountTransictiontable);
    //            }
    //        }
    //    }
    //    catch (Exception)
    //    {
    //        return Json("Fail", JsonRequestBehavior.AllowGet);
    //    }

    //    return Json("Success", JsonRequestBehavior.AllowGet);

    //    return View();
    //}


    //public ActionResult QutationByStatus(string status)
    //{
    //    if (status == null)
    //    {
    //        ViewBag.ststus = "accepted";
    //    }
    //    else
    //    {
    //        ViewBag.ststus = status;
    //    }

    //    return View();
    //}

}
