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
            return View();
        }





        [HttpPost]
        public JsonResult UpdateOrderStaus(int QutationId, string Status)
        {
            OrderStatusTable orderstatusModel = new OrderStatusTable();

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
                        orderstatusModel.PurchaseOrderId = qutationModel.QutationID;
                        orderstatusModel.Status = Status;
                        orderstatusModel.CreatedDate = DateTime.Now;
                        orderstatusModel.CompanyId = Convert.ToInt32(Session["CompayID"]);
                        orderstatusModel.UserId = 1;
                        orderstatusModel.Type = "Qutation";
                        HttpResponseMessage response2 = GlobalVeriables.WebApiClient.PostAsJsonAsync("APIOrderStatus", orderstatusModel).Result;

                        if (response2.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            return new JsonResult { Data = new { Status = "Success" } };
                        }
                        else
                        {
                            return new JsonResult { Data = new { Status = "Fail" } };
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



    }
}