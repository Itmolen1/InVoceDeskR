using InvoiceDiskLast.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace InvoiceDiskLast.Controllers
{
    public class PurchaseController : Controller
    {
        // GET: Purchase
        public ActionResult Index()
        {

            return View();
        }






        [HttpPost]
        public JsonResult GetPurchaseList()
        {
            IEnumerable<MvcPurchaseModel> ContactsList;
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

                GlobalVeriables.WebApiClient.DefaultRequestHeaders.Clear();
                int CompanyId = Convert.ToInt32(Session["CompayID"]);
                GlobalVeriables.WebApiClient.DefaultRequestHeaders.Add("CompayID", CompanyId.ToString());

                HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("APIPurchase").Result;
                ContactsList = response.Content.ReadAsAsync<IEnumerable<MvcPurchaseModel>>().Result;


                //if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
                //{
                //    // Apply search  on multiple field  
                //    ContactsList = ContactsList.Where(p => p.ContactsId.ToString().Contains(search) ||
                //    p.ContactName.ToLower().Contains(search.ToLower()) ||
                //    p.BillingCountry.ToLower().ToString().ToLower().Contains(search.ToLower()) ||
                //    p.BillingCity.ToLower().Contains(search.ToLower()) ||
                //    p.Type.ToLower().Contains(search.ToLower()) ||
                //    p.ContactAddress.ToLower().ToString().Contains(search.ToLower()) ||
                //    p.BillingCompanyName.ToLower().ToString().ToLower().Contains(search.ToLower()) ||
                //    p.BillingVatTRN.Contains(search.ToLower())).ToList();
                //}


                //switch (sortColumn)
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


                int recordsTotal = recordsTotal = ContactsList.Count();
                var data = ContactsList.Skip(skip).Take(pageSize).ToList();
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Response.Write(ex.ToString());
                Json(new { draw = 0, recordsFiltered = 0, recordsTotal = 0, data = 0 }, JsonRequestBehavior.AllowGet);
            }
            return Json(null, JsonRequestBehavior.AllowGet);

        }





        public ActionResult GetVender()
        {
            var ProductList = new List<MVCContactModel>();
            try
            {
                int CompanyId = Convert.ToInt32(Session["CompayID"]);
                GlobalVeriables.WebApiClient.DefaultRequestHeaders.Clear();
                GlobalVeriables.WebApiClient.DefaultRequestHeaders.Add("CompayID", CompanyId.ToString());

                GlobalVeriables.WebApiClient.DefaultRequestHeaders.Add("CustomerStatus", "Vender");
                HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("ApiConatacts").Result;
                ProductList = response.Content.ReadAsAsync<List<MVCContactModel>>().Result;

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return Json(ProductList, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception)
            {

                throw;
            }

            return Json(ProductList, JsonRequestBehavior.AllowGet);

        }



        [HttpPost]
        public ActionResult savePrintAndSentItToYouronsave(MvcPurchaseViewModel purchaseViewModel)
        {
            var Qutationid = "";
            int Qid = 0;
            MvcPurchaseModel purchaseModel = new MvcPurchaseModel();
            try
            {
                if (purchaseViewModel.PurchaseId == null)
                {
                   purchaseModel.CompanyId = 2;
                    purchaseModel.UserId = 1;
                    purchaseModel.PurchaseID = purchaseViewModel.PurchaseID;
                    purchaseModel.PurchaseOrderID = purchaseViewModel.PurchaseOrderID;
                    purchaseModel.PurchaseRefNumber = purchaseViewModel.PurchaseRefNumber;
                    purchaseModel.PurchaseDate = purchaseViewModel.PurchaseDate;
                    purchaseModel.PurchaseDueDate = purchaseViewModel.PurchaseDueDate;
                    purchaseModel.PurchaseSubTotal = purchaseViewModel.PurchaseSubTotal;
                    purchaseModel.PurchaseDiscountAmount = purchaseViewModel.PurchaseDiscountAmount;
                    purchaseModel.PurchaseTotoalAmount = purchaseViewModel.PurchaseTotoalAmount;
                    purchaseModel.PurchaseVenderNote = purchaseModel.PurchaseVenderNote;
                    purchaseModel.Status = "Open";
                    HttpResponseMessage response = GlobalVeriables.WebApiClient.PostAsJsonAsync("APIPurchase/" + purchaseModel.PurchaseOrderID, purchaseModel).Result;

                    IEnumerable<string> headerValues;
                    var userId = string.Empty;

                    if (response.Headers.TryGetValues("idd", out headerValues))
                    {
                        Qutationid = headerValues.FirstOrDefault();
                    }

                    Qid = Convert.ToInt32(Qutationid);

                    if (response.StatusCode == System.Net.HttpStatusCode.Created)
                    {
                        foreach (var item in purchaseViewModel.PurchaseDetailslist)
                        {
                            PurchaseOrderDetailsTable purchadeDetail = new PurchaseOrderDetailsTable();
                            //purchadeDetail.PurchaseOrderDetailsId =item.PurchaseId
                            purchadeDetail.PurchaseItemId = item.PurchaseItemId;
                            purchadeDetail.PurchaseDescription = item.PurchaseDescription;
                            purchadeDetail.PurchaseQuantity = item.PurchaseQuantity;
                            purchadeDetail.PurchaseItemRate = item.PurchaseItemRate;
                            purchadeDetail.PurchaseTotal = item.PurchaseTotal;
                            purchadeDetail.PurchaseVatPercentage = item.PurchaseVatPercentage;
                            purchadeDetail.PurchaseId = item.PurchaseId;

                            if (purchadeDetail.PurchaseId == 0)
                            {
                                HttpResponseMessage responsses = GlobalVeriables.WebApiClient.PostAsJsonAsync("APIPurchaseDetail", purchadeDetail).Result;
                            }
                            else
                            {
                                HttpResponseMessage responsses = GlobalVeriables.WebApiClient.PutAsJsonAsync("APIQutationDetail/" + purchadeDetail.PurchaseId, purchadeDetail).Result;
                            }
                        }

                        return new JsonResult { Data = new { Status = "Success", QutationId = Qid } };

                    }
                    else
                    {
                        return Json("Fail", JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {

                return new JsonResult { Data = new { Status = "Fail", Message = ex.Message.ToString() } };
            }
            return new JsonResult { Data = new { Status = "Success", QutationId = Qid } };
        }



        int Contectid = 0;
        int CompanyID = 0;

        [HttpGet]
        public ActionResult AddOrEdit(int id = 0)
        {
            MvcPurchaseViewModel purchaseviewModel = new MvcPurchaseViewModel();
            try
            {

                var idd = Session["ClientID"];
                var cdd = Session["CompayID"];

                if (Session["ClientID"] != null && Session["CompayID"] != null)
                {
                    Contectid = Convert.ToInt32(Session["ClientID"]);
                    CompanyID = Convert.ToInt32(Session["CompayID"]);
                }
                else
                {
                    return RedirectToAction("Index", "Login");
                }


                HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("ApiConatacts/" + Contectid.ToString()).Result;
                MVCContactModel contectmodel = response.Content.ReadAsAsync<MVCContactModel>().Result;


                HttpResponseMessage responseCompany = GlobalVeriables.WebApiClient.GetAsync("APIComapny/" + CompanyID.ToString()).Result;
                MVCCompanyInfoModel companyModel = responseCompany.Content.ReadAsAsync<MVCCompanyInfoModel>().Result;

                ViewBag.Contentdata = contectmodel;
                ViewBag.Companydata = companyModel;

                if (id == 0)
                {
                    return View(new MvcPurchaseViewModel());
                }
                else
                {
                    ViewBag.edit = "true";

                    List<VatModel> model = new List<VatModel>();
                    model.Add(new VatModel() { Vat1 = 0, Name = "0" });

                    model.Add(new VatModel() { Vat1 = 6, Name = "6" });
                    model.Add(new VatModel() { Vat1 = 21, Name = "21" });

                    ViewBag.VatDrop = model;


                    HttpResponseMessage responsep = GlobalVeriables.WebApiClient.GetAsync("APIProduct").Result;
                    List<MVCProductModel> productModel = responsep.Content.ReadAsAsync<List<MVCProductModel>>().Result;
                    ViewBag.Product = productModel;

                    HttpResponseMessage res = GlobalVeriables.WebApiClient.GetAsync("APIPurchase/" + id.ToString()).Result;
                    MvcPurchaseModel ob = res.Content.ReadAsAsync<MvcPurchaseModel>().Result;

                    purchaseviewModel.PurchaseOrderID = purchaseviewModel.PurchaseOrderID;
                    purchaseviewModel.PurchaseID = ob.PurchaseID;
                    purchaseviewModel.PurchaseDate = ob.PurchaseDate;
                    purchaseviewModel.PurchaseDueDate = ob.PurchaseDueDate;
                    purchaseviewModel.PurchaseRefNumber = ob.PurchaseRefNumber;
                    purchaseviewModel.PurchaseSubTotal = ob.PurchaseSubTotal;
                    purchaseviewModel.PurchaseDiscountPercenteage = ob.PurchaseDiscountPercenteage;
                    purchaseviewModel.PurchaseDiscountAmount = ob.PurchaseDiscountAmount;
                    purchaseviewModel.PurchaseVatPercentage = ob.PurchaseVatPercentage;
                    purchaseviewModel.PurchaseTotoalAmount = ob.PurchaseTotoalAmount;
                    purchaseviewModel.PurchaseVenderNote = ob.PurchaseVenderNote;
                    purchaseviewModel.Status = ob.Status;
                    purchaseviewModel.CompanyId = ob.CompanyId;
                    purchaseviewModel.UserId = ob.UserId;
                    purchaseviewModel.AddedDate = ob.AddedDate;


                    HttpResponseMessage responseQutationDetailsList = GlobalVeriables.WebApiClient.GetAsync("APIQutationDetail/" + id.ToString()).Result;
                    List<MVCQutationDetailsModel> QutationModelDetailsList = responseQutationDetailsList.Content.ReadAsAsync<List<MVCQutationDetailsModel>>().Result;
                    ViewBag.Contentdata = contectmodel;
                    ViewBag.Companydata = companyModel;
                    ViewBag.PurchaseDatailsList = QutationModelDetailsList;


                    return View(purchaseviewModel);
                }

            }
            catch (Exception ex)
            {

                ViewBag.Message = ex.ToString();
            }
            return View(purchaseviewModel);

        }



        public class VatModel
        {
            public int Vat1 { get; set; }
            public string Name { get; set; }

        }
    }

}