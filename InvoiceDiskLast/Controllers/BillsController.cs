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

        public ActionResult Print(int id)
        {
            try
            {
                HttpResponseMessage res = GlobalVeriables.WebApiClient.GetAsync("APIPurchase/" + id.ToString()).Result;
                MvcPurchaseModel ob = res.Content.ReadAsAsync<MvcPurchaseModel>().Result;

                int? Contectid = ob.VenderId;
                int? CompanyID = ob.CompanyId;

                HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("ApiConatacts/" + Contectid.ToString()).Result;
                MVCContactModel contectmodel = response.Content.ReadAsAsync<MVCContactModel>().Result;

                HttpResponseMessage responseCompany = GlobalVeriables.WebApiClient.GetAsync("APIComapny/" + CompanyID.ToString()).Result;
                MVCCompanyInfoModel companyModel = responseCompany.Content.ReadAsAsync<MVCCompanyInfoModel>().Result;

                HttpResponseMessage responseQutationDetailsList = GlobalVeriables.WebApiClient.GetAsync("APIPurchaseDetail/" + id.ToString()).Result;
                List<MvcPurchaseViewModel> PuchaseModelDetailsList = responseQutationDetailsList.Content.ReadAsAsync<List<MvcPurchaseViewModel>>().Result;

                DateTime PurchaseDueDate = Convert.ToDateTime(ob.PurchaseDueDate); //mm/dd/yyyy
                DateTime PurchaseDate = Convert.ToDateTime(ob.PurchaseDate);//mm/dd/yyyy
                TimeSpan ts = PurchaseDueDate.Subtract(PurchaseDate);
                string diffDate = ts.Days.ToString();

                ViewBag.Contentdata = contectmodel;
                ViewBag.Companydata = companyModel;
                ViewBag.Purchase = ob;
                ViewBag.PurchaseDatailsList = PuchaseModelDetailsList;

                string PdfName = id + "-" + companyModel.CompanyName + ".pdf";

                string cutomswitches = "";
                cutomswitches = "--footer-center \"" + "Wilt u zo vriendelijk zijn om het verschuldigde bedrag binnen " + diffDate + " dagen over te maken naar IBAN: \n " + companyModel.IBANNumber + " ten name van IT Molen o.v.v.bovenstaande factuurnummer. \n (Op al onze diensten en producten zijn onze algemene voorwaarden van toepassing.Deze kunt u downloaden van onze website.)" + " \n Printed date: " +
                   DateTime.Now.Date.ToString("MM/dd/yyyy") + "  Page: [page]/[toPage]\"" +
                  " --footer-line --footer-font-size \"10\" --footer-spacing 6 --footer-font-name \"calibri light\"";

                return new Rotativa.PartialViewAsPdf("~/Views/Bills/Viewapp.cshtml")
                {
                    PageSize = Rotativa.Options.Size.A4,
                    MinimumFontSize = 16,

                    FileName = PdfName,

                    PageHeight = 40,
                    CustomSwitches = cutomswitches,
                    PageMargins = new Rotativa.Options.Margins(10, 12, 20, 3)
                };
            }
            catch (Exception ex)
            {
                return null;

            }
        }

        [NonAction]
        public List<VatModel> GetVatList()
        {
            List<VatModel> model = new List<VatModel>();
            model.Add(new VatModel() { Vat1 = 0, Name = "0" });
            model.Add(new VatModel() { Vat1 = 6, Name = "6" });
            model.Add(new VatModel() { Vat1 = 21, Name = "21" });
            return model;
        }

        public ActionResult Edit(int Id)
        {
            MvcPurchaseViewModel purchaseviewModel = new MvcPurchaseViewModel();

            try
            {
                ViewBag.VatDrop = GetVatList();

                HttpResponseMessage res = GlobalVeriables.WebApiClient.GetAsync("APIPurchase/" + Id.ToString()).Result;
                MvcPurchaseModel ob = res.Content.ReadAsAsync<MvcPurchaseModel>().Result;

                int? contactid = ob.VenderId;
                int? CompanyId = ob.CompanyId;

                HttpResponseMessage responsep = GlobalVeriables.WebApiClient.GetAsync("APIProduct/" + CompanyId + "/All").Result;
                List<MVCProductModel> productModel = responsep.Content.ReadAsAsync<List<MVCProductModel>>().Result;
                ViewBag.Product = productModel;

                HttpResponseMessage GoodResponse = GlobalVeriables.WebApiClient.GetAsync("APIProduct/" + CompanyId + "/Good").Result;
                List<MVCProductModel> GoodModel = GoodResponse.Content.ReadAsAsync<List<MVCProductModel>>().Result;
                ViewBag.Good = GoodModel;

                HttpResponseMessage Services = GlobalVeriables.WebApiClient.GetAsync("APIProduct/" + CompanyId + "/Services").Result;
                List<MVCProductModel> ServiceModel = Services.Content.ReadAsAsync<List<MVCProductModel>>().Result;
                ViewBag.Service = ServiceModel;

                HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("ApiConatacts/" + contactid.ToString()).Result;
                MVCContactModel contectmodel = response.Content.ReadAsAsync<MVCContactModel>().Result;

                HttpResponseMessage responseCompany = GlobalVeriables.WebApiClient.GetAsync("APIComapny/" + CompanyId.ToString()).Result;
                MVCCompanyInfoModel companyModel = responseCompany.Content.ReadAsAsync<MVCCompanyInfoModel>().Result;

               

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

                HttpResponseMessage responseQutationDetailsList = GlobalVeriables.WebApiClient.GetAsync("APIPurchaseDetail/" + Id.ToString()).Result;
                List<MvcPurchaseViewModel> QutationModelDetailsList = responseQutationDetailsList.Content.ReadAsAsync<List<MvcPurchaseViewModel>>().Result;
                ViewBag.Contentdata = contectmodel;
                ViewBag.Companydata = companyModel;
                ViewBag.Purchase = ob;
                ViewBag.PurchaseDatailsList = QutationModelDetailsList;

                return View(purchaseviewModel);
            }
            catch (Exception)
            {

                throw;
            }

        }

        [HttpPost]
        public ActionResult Edit(MvcPurchaseViewModel purchaseViewModel)
        {


            PurchaseOrderTable purchasemodel = new PurchaseOrderTable();
            try
            {
               
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

                HttpResponseMessage response = GlobalVeriables.WebApiClient.PutAsJsonAsync("APIPurchase/" + purchasemodel.PurchaseOrderID, purchasemodel).Result;

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    foreach (var item in purchaseViewModel.PurchaseDetailslist)
                    {
                        PurchaseOrderDetailsTable purchadeDetail = new PurchaseOrderDetailsTable();
                        purchadeDetail.PurchaseOrderDetailsId = item.PurchaseOrderDetailsId;
                        purchadeDetail.PurchaseItemId = item.PurchaseItemId;
                        purchadeDetail.PurchaseDescription = item.PurchaseDescription;
                        purchadeDetail.PurchaseQuantity = item.PurchaseQuantity;
                        purchadeDetail.PurchaseItemRate = item.PurchaseItemRate;
                        purchadeDetail.Type = item.Type;
                        purchadeDetail.RowSubTotal = item.RowSubTotal;
                        purchadeDetail.PurchaseDescription = item.PurchaseDescription;
                        purchasemodel.VenderId = 1;
                        purchadeDetail.ServiceDate = item.ServiceDate;
                        purchasemodel.CompanyId = 1;
                        purchadeDetail.PurchaseTotal = item.PurchaseTotal;
                        purchadeDetail.PurchaseVatPercentage = item.PurchaseVatPercentage;
                        purchadeDetail.PurchaseId = purchaseViewModel.PurchaseOrderID;

                        if (purchadeDetail.PurchaseOrderDetailsId == 0)
                        {


                            HttpResponseMessage responsses = GlobalVeriables.WebApiClient.PostAsJsonAsync("APIPurchaseDetail", purchadeDetail).Result;
                        }
                        else
                        {
                            HttpResponseMessage responsses = GlobalVeriables.WebApiClient.PutAsJsonAsync("APIPurchaseDetail/" + purchadeDetail.PurchaseOrderDetailsId, purchadeDetail).Result;
                        }
                    }

                    return new JsonResult { Data = new { Status = "Success", purchaseId = purchasemodel.PurchaseOrderID } };

                }
            }
            catch (Exception ex)
            {
                return new JsonResult { Data = new { Status = "Fail", Message = ex.Message.ToString() } };
            }
            return View();
        }
    }
}