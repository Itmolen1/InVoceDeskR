using InvoiceDiskLast.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace InvoiceDiskLast.Controllers
{
    public class BillController : Controller
    {
        // GET: Bill
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {

            MvcPurchaseViewModel purchaseviewModel = new MvcPurchaseViewModel();
            try
            {

                HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("ApiConatacts/" + 1.ToString()).Result;
                MVCContactModel contectmodel = response.Content.ReadAsAsync<MVCContactModel>().Result;

                HttpResponseMessage responseCompany = GlobalVeriables.WebApiClient.GetAsync("APIComapny/" + 1.ToString()).Result;
                MVCCompanyInfoModel companyModel = responseCompany.Content.ReadAsAsync<MVCCompanyInfoModel>().Result;

                ViewBag.Contentdata = contectmodel;
                ViewBag.Companydata = companyModel;
                DateTime InvoiceDate = new DateTime();
                InvoiceDate = DateTime.Now;
                purchaseviewModel.PurchaseDate = InvoiceDate;
                purchaseviewModel.PurchaseDueDate = InvoiceDate.AddDays(+15);

                MvcPurchaseModel q = new MvcPurchaseModel();
                HttpResponseMessage response1 = GlobalVeriables.WebApiClient.GetAsync("GenrateInvoice/").Result;
                q = response1.Content.ReadAsAsync<MvcPurchaseModel>().Result;
                purchaseviewModel.PurchaseDate = InvoiceDate;
                purchaseviewModel.PurchaseDueDate = InvoiceDate.AddDays(+15);
                purchaseviewModel.Purchase_ID = q.PurchaseID;

                return View(purchaseviewModel);
            }
            catch (Exception ex)
            {
                return null;
            }

        }


        [HttpPost]
        public ActionResult SaveDraft(MvcPurchaseViewModel purchaseViewModel)
        {

            PurchaseOrderTable purchasemodel = new PurchaseOrderTable();
            try
            {
                int CompanyID = Convert.ToInt32(Session["CompayID"]);

                purchasemodel.CompanyId = CompanyID;

                purchasemodel.UserId = 1;
                purchasemodel.PurchaseID = purchaseViewModel.PurchaseId.ToString();
                purchasemodel.PurchaseOrderID = (Convert.ToInt32(purchaseViewModel.PurchaseOrderID != null ? purchaseViewModel.PurchaseOrderID : 0));
                purchasemodel.PurchaseRefNumber = purchaseViewModel.PurchaseRefNumber;
                purchasemodel.PurchaseDate = (DateTime)purchaseViewModel.PurchaseDate;
                purchasemodel.PurchaseDueDate = purchaseViewModel.PurchaseDueDate;
                purchasemodel.PurchaseSubTotal = purchaseViewModel.PurchaseSubTotal;
                purchasemodel.PurchaseDiscountAmount = purchaseViewModel.PurchaseDiscountAmount;
                purchasemodel.PurchaseTotoalAmount = purchaseViewModel.PurchaseTotoalAmount;
                purchasemodel.PurchaseVenderNote = purchaseViewModel.PurchaseVenderNote;
                purchasemodel.Vat6 = purchaseViewModel.Vat6;
                purchasemodel.VenderId = purchaseViewModel.Contactid;
                purchasemodel.Vat21 = purchaseViewModel.Vat21;
                purchasemodel.Type = StatusEnum.Goods.ToString();
                purchasemodel.Status = "accepted";

                if (purchasemodel.Vat6 != null)
                {
                    double vat61 = Math.Round((double)purchasemodel.Vat6, 2, MidpointRounding.AwayFromZero);
                    purchasemodel.Vat6 = vat61;
                }

                if (purchasemodel.Vat21 != null)
                {
                    double vat21 = Math.Round((double)purchasemodel.Vat21, 2, MidpointRounding.AwayFromZero);
                    purchasemodel.Vat21 = vat21;
                }

                purchasemodel.PurchaseID = purchaseViewModel.Purchase_ID;

                HttpResponseMessage response = GlobalVeriables.WebApiClient.PostAsJsonAsync("APIPurchase", purchasemodel).Result;

                purchasemodel = response.Content.ReadAsAsync<PurchaseOrderTable>().Result;



                if (response.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    purchaseViewModel.PurchaseOrderID = purchasemodel.PurchaseOrderID;

                    foreach (var item in purchaseViewModel.PurchaseDetailslist)
                    {
                        PurchaseOrderDetailsTable purchadeDetail = new PurchaseOrderDetailsTable();
                        purchadeDetail.PurchaseOrderDetailsId = item.PurchaseOrderDetailsId;
                        purchadeDetail.PurchaseItemId = item.PurchaseItemId;
                        purchadeDetail.PurchaseDescription = item.PurchaseDescription;
                        purchadeDetail.PurchaseQuantity = item.PurchaseQuantity;
                        purchadeDetail.PurchaseItemRate = item.PurchaseItemRate;
                        purchadeDetail.PurchaseTotal = item.PurchaseTotal;
                        purchadeDetail.PurchaseVatPercentage = item.PurchaseVatPercentage;
                        purchadeDetail.PurchaseId = purchaseViewModel.PurchaseOrderID;

                        purchadeDetail.PurchaseId = purchasemodel.PurchaseOrderID;

                        if (purchadeDetail.PurchaseId == 0)
                        {
                            HttpResponseMessage responsses = GlobalVeriables.WebApiClient.PostAsJsonAsync("APIPurchaseDetail", purchadeDetail).Result;
                        }
                        else
                        {
                            HttpResponseMessage responsses = GlobalVeriables.WebApiClient.PutAsJsonAsync("APIPurchaseDetail/" + purchadeDetail.PurchaseOrderDetailsId, purchadeDetail).Result;
                        }
                    }

                    return new JsonResult { Data = new { Status = "Success", path = "", purchasemodel.PurchaseOrderID } };

                }
                else
                {
                    return Json("Fail", JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {

                return new JsonResult { Data = new { Status = "Fail", Message = ex.Message.ToString() } };
            }
        }
    }
}