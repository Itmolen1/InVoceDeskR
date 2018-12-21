using InvoiceDiskLast.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace InvoiceDiskLast.Controllers
{
    public class BillsController : Controller
    {
        // GET: Bills
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Create()
        {
            MvcPurchaseViewModel purchaseviewModel = new MvcPurchaseViewModel();
            try
            {
               
                  int  Contectid = Convert.ToInt32(Session["ClientId"]);
                  int   CompanyID = Convert.ToInt32(Session["CompayID"]);
              

                GlobalVeriables.WebApiClient.DefaultRequestHeaders.Remove("CompayID");
                GlobalVeriables.WebApiClient.DefaultRequestHeaders.Add("CompayID", 1.ToString());

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
        public ActionResult save1(MvcPurchaseViewModel purchaseViewModel)
        {
            var purchaseorderId = "";
            int intpurchaseorderId = 0;
            PurchaseOrderTable purchasemodel = new PurchaseOrderTable();

            try
            {
                //if (Session["ClientId"] != null && Session["CompayID"] != null)
                //{
                //    Contectid = Convert.ToInt32(Session["ClientId"]);
                //    CompanyID = Convert.ToInt32(Session["CompayID"]);
                //}
                //else
                //{
                //    return RedirectToAction("Index", "Login");
                //}


                purchasemodel.CompanyId = 1;
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

                purchasemodel.VenderId = 1;
                purchasemodel.Vat21 = purchaseViewModel.Vat21;
                purchasemodel.Status = "accepted";
                purchasemodel.Type = StatusEnum.Goods.ToString();

                if (purchaseViewModel.PurchaseOrderID == 0 || purchaseViewModel.PurchaseOrderID == null)
                {

                    HttpResponseMessage response = GlobalVeriables.WebApiClient.PostAsJsonAsync("APIPurchase", purchasemodel).Result;
                    IEnumerable<string> headerValues;
                    var userId = string.Empty;
                    if (response.Headers.TryGetValues("idd", out headerValues))
                    {
                        purchaseorderId = headerValues.FirstOrDefault();
                    }
                    intpurchaseorderId = Convert.ToInt32(purchaseorderId);


                    if (response.StatusCode == System.Net.HttpStatusCode.Created)
                    {
                        purchaseViewModel.PurchaseOrderID = intpurchaseorderId;

                        foreach (var item in purchaseViewModel.PurchaseDetailslist)
                        {
                            PurchaseOrderDetailsTable purchadeDetail = new PurchaseOrderDetailsTable();
                            purchadeDetail.PurchaseOrderDetailsId = item.PurchaseOrderDetailsId;
                            purchadeDetail.PurchaseItemId = item.PurchaseItemId;
                            purchadeDetail.PurchaseDescription = item.PurchaseDescription;
                            purchadeDetail.PurchaseQuantity = item.PurchaseQuantity;
                            purchadeDetail.PurchaseItemRate = item.PurchaseItemRate;
                            purchadeDetail.PurchaseTotal = item.PurchaseTotal;
                            purchadeDetail.Type = "accepted";
                            purchadeDetail.RowSubTotal = item.RowSubTotal;
                            purchadeDetail.PurchaseVatPercentage = item.PurchaseVatPercentage;
                            purchadeDetail.PurchaseId = purchaseViewModel.PurchaseOrderID;
                            purchadeDetail.ServiceDate = item.ServiceDate;
                            purchadeDetail.PurchaseId = intpurchaseorderId;
                            HttpResponseMessage responsses = GlobalVeriables.WebApiClient.PostAsJsonAsync("APIPurchaseDetail", purchadeDetail).Result;
                        }

                        return new JsonResult { Data = new { Status = "Success", purchaseId = intpurchaseorderId } };
                    }
                    else
                    {
                        return new JsonResult { Data = new { Status = "Fail", Message = "Error while adding purchase" } };
                    }
                }
            }
            catch (Exception ex)
            {

                return new JsonResult { Data = new { Status = "Fail", Message = ex.Message.ToString() } };
            }

            return new JsonResult { Data = new { Status = "Success", purchaseId = intpurchaseorderId } };
        }


        public ActionResult ViewBill(int? id)
        {
            try
            {

                HttpResponseMessage res = GlobalVeriables.WebApiClient.GetAsync("APIPurchase/" + id.ToString()).Result;
                MvcPurchaseModel ob = res.Content.ReadAsAsync<MvcPurchaseModel>().Result;

                int? Contectid = ob.CompanyId;
                int? CompanyID = ob.VenderId;

                HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("ApiConatacts/" + Contectid.ToString()).Result;
                MVCContactModel contectmodel = response.Content.ReadAsAsync<MVCContactModel>().Result;

                HttpResponseMessage responseCompany = GlobalVeriables.WebApiClient.GetAsync("APIComapny/" + CompanyID.ToString()).Result;
                MVCCompanyInfoModel companyModel = responseCompany.Content.ReadAsAsync<MVCCompanyInfoModel>().Result;

              

                GlobalVeriables.WebApiClient.DefaultRequestHeaders.Clear();

                HttpResponseMessage responseQutationDetailsList = GlobalVeriables.WebApiClient.GetAsync("APIPurchaseDetail/" + id.ToString()).Result;
                List<MvcPurchaseViewModel> PuchaseModelDetailsList = responseQutationDetailsList.Content.ReadAsAsync<List<MvcPurchaseViewModel>>().Result;


                ViewBag.Contentdata = contectmodel;
                ViewBag.Companydata = companyModel;
                ViewBag.Purchase = ob;
                ViewBag.PurchaseDatailsList = PuchaseModelDetailsList;
            }
            catch (Exception ex)
            {
            }
            return View();
        }
    }
}