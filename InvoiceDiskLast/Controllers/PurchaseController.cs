using InvoiceDiskLast.Models;
using System;
using System.Collections.Generic;
using System.IO;
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

                GlobalVeriables.WebApiClient.DefaultRequestHeaders.Remove("CustomerStatus");

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
                if (purchaseViewModel.PurchaseOrderID == 0)
                {
                    purchaseModel.CompanyId = 2;
                    purchaseModel.UserId = 1;
                    purchaseModel.PurchaseID = purchaseViewModel.Purchase_ID;
                    purchaseModel.PurchaseOrderID = purchaseViewModel.PurchaseOrderID;
                    purchaseModel.PurchaseRefNumber = purchaseViewModel.PurchaseRefNumber;
                    purchaseModel.PurchaseDate = (DateTime)purchaseViewModel.PurchaseDate;
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

                var idd = Session["ClientId"];
                var cdd = Session["CompayID"];

                if (Session["ClientId"] != null && Session["CompayID"] != null)
                {
                    Contectid = Convert.ToInt32(Session["ClientId"]);
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
                DateTime InvoiceDate = new DateTime();
                InvoiceDate = DateTime.Now;
                purchaseviewModel.PurchaseDate = InvoiceDate;
                purchaseviewModel.PurchaseDueDate = InvoiceDate.AddDays(+15);

                if (id == 0)
                {
                    MvcPurchaseModel q = new MvcPurchaseModel();
                    HttpResponseMessage response1 = GlobalVeriables.WebApiClient.GetAsync("GenrateInvoice/").Result;
                    q = response1.Content.ReadAsAsync<MvcPurchaseModel>().Result;
                    purchaseviewModel.Purchase_ID = q.PurchaseID;
                    return View(purchaseviewModel);
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

                    purchaseviewModel.PurchaseOrderID = ob.PurchaseOrderID;
                    purchaseviewModel.Purchase_ID = ob.PurchaseID;
                    purchaseviewModel.PurchaseDate = ob.PurchaseDate;
                    purchaseviewModel.PurchaseDueDate = (DateTime)ob.PurchaseDueDate;
                    purchaseviewModel.PurchaseRefNumber = ob.PurchaseRefNumber;
                    purchaseviewModel.PurchaseSubTotal = ob.PurchaseSubTotal;
                    purchaseviewModel.PurchaseDiscountPercenteage = ob.PurchaseDiscountPercenteage;
                    purchaseviewModel.PurchaseDiscountAmount = ob.PurchaseDiscountAmount;
                    purchaseviewModel.PurchaseVatPercentage = ob.PurchaseVatPercentage;
                    purchaseviewModel.PurchaseTotoalAmount = ob.PurchaseTotoalAmount;
                    purchaseviewModel.PurchaseVenderNote = ob.PurchaseVenderNote;
                    purchaseviewModel.Status = ob.Status;
                    purchaseviewModel.Vat21 =(int)ob.Vat21;
                    purchaseviewModel.Vat6 = (int)ob.Vat6;
                    purchaseviewModel.CompanyId = ob.CompanyId;
                    purchaseviewModel.UserId = ob.UserId;
                 //   purchaseviewModel.AddedDate = (DateTime)ob.AddedDate;


                    HttpResponseMessage responseQutationDetailsList = GlobalVeriables.WebApiClient.GetAsync("APIPurchaseDetail/" + id.ToString()).Result;
                    List<MvcPurchaseViewModel> QutationModelDetailsList = responseQutationDetailsList.Content.ReadAsAsync<List<MvcPurchaseViewModel>>().Result;
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


        public JsonResult AddOrEditPurchase(int purchaseId)
        {
            PurchaseOrderTable m = new Models.PurchaseOrderTable();
            HttpResponseMessage response1 = GlobalVeriables.WebApiClient.GetAsync("APIPurchase/" + purchaseId.ToString()).Result;
            m = response1.Content.ReadAsAsync<PurchaseOrderTable>().Result;
            Session["ClientId"] = m.VenderId;
            return Json("", JsonRequestBehavior.AllowGet);

        }



        [HttpPost]
        public ActionResult save(MvcPurchaseViewModel purchaseViewModel)
        {
            var purchaseorderId = "";
            int intpurchaseorderId = 0;
            int companyId = 0;
            PurchaseOrderTable purchasemodel = new PurchaseOrderTable();
            try
            {
                if (Session["ClientId"] != null && Session["CompayID"] != null)
                {
                    Contectid = Convert.ToInt32(Session["ClientId"]);
                    CompanyID = Convert.ToInt32(Session["CompayID"]);
                }
                else
                {
                    return RedirectToAction("Index", "Login");
                }

               
                purchasemodel.CompanyId = CompanyID;
                purchasemodel.UserId = 1;
                purchasemodel.PurchaseID = purchaseViewModel.PurchaseId.ToString();
                    
                purchasemodel.PurchaseOrderID = (purchaseViewModel.PurchaseOrderID != null ? purchaseViewModel.PurchaseOrderID : 0);
                purchasemodel.PurchaseRefNumber = purchaseViewModel.PurchaseRefNumber;
                purchasemodel.PurchaseDate = (DateTime)purchaseViewModel.PurchaseDate;
                purchasemodel.PurchaseDueDate = purchaseViewModel.PurchaseDueDate;
                purchasemodel.PurchaseSubTotal = purchaseViewModel.PurchaseSubTotal;
                purchasemodel.PurchaseDiscountAmount = purchaseViewModel.PurchaseDiscountAmount;
                purchasemodel.PurchaseTotoalAmount = purchaseViewModel.PurchaseTotoalAmount;
                purchasemodel.PurchaseVenderNote = purchaseViewModel.PurchaseVenderNote;
                purchasemodel.Vat6 = purchaseViewModel.Vat6;
                purchasemodel.VenderId = Contectid;
                purchasemodel.Vat21 = purchaseViewModel.Vat21;
                purchasemodel.Status = "Open";

                if (purchaseViewModel.PurchaseOrderID == 0)
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
                            if (purchadeDetail.PurchaseOrderDetailsId == 0)
                            {
                                HttpResponseMessage responsses = GlobalVeriables.WebApiClient.PostAsJsonAsync("APIPurchaseDetail", purchadeDetail).Result;
                            }
                            else
                            {
                                HttpResponseMessage responsses = GlobalVeriables.WebApiClient.PutAsJsonAsync("APIPurchaseDetail/" + purchadeDetail.PurchaseOrderDetailsId, purchadeDetail).Result;
                            }
                        }
                    }
                }

                else
                {
                    if (purchaseViewModel.PurchaseOrderID != 0)
                    {
                        HttpResponseMessage response2 = GlobalVeriables.WebApiClient.PutAsJsonAsync("APIPurchase/"+ purchasemodel.PurchaseOrderID , purchasemodel).Result;
                        intpurchaseorderId =(int)purchaseViewModel.PurchaseOrderID;
                        if (response2.StatusCode == System.Net.HttpStatusCode.OK)
                        {
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
                                if (purchadeDetail.PurchaseOrderDetailsId == 0)
                                {
                                    HttpResponseMessage responsses = GlobalVeriables.WebApiClient.PostAsJsonAsync("APIPurchaseDetail", purchadeDetail).Result;
                                }
                                else
                                {
                                    HttpResponseMessage responsses = GlobalVeriables.WebApiClient.PutAsJsonAsync("APIPurchaseDetail/" + purchadeDetail.PurchaseOrderDetailsId, purchadeDetail).Result;
                                }
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {

                return new JsonResult { Data = new { Status = "Fail", Message = ex.Message.ToString() } };
            }
            return new JsonResult { Data = new { Status = "Success", purchaseId = intpurchaseorderId } };
        }



        [HttpPost]
        public ActionResult saveEmail(MvcPurchaseViewModel purchaseViewModel)
        {
            var purchaseorderId = "";
            int intpurchaseorderId = 0;
            int companyId = 0;
            PurchaseOrderTable purchasemodel = new PurchaseOrderTable();
            try
            {
                if (Session["ClientId"] != null && Session["CompayID"] != null)
                {
                    Contectid = Convert.ToInt32(Session["ClientId"]);
                    CompanyID = Convert.ToInt32(Session["CompayID"]);
                }
                else
                {
                    return RedirectToAction("Index", "Login");
                }


                purchasemodel.CompanyId = CompanyID;
                purchasemodel.UserId = 1;
                purchasemodel.PurchaseID = purchaseViewModel.PurchaseId.ToString();

                purchasemodel.PurchaseOrderID = (purchaseViewModel.PurchaseOrderID != null ? purchaseViewModel.PurchaseOrderID : 0);
                purchasemodel.PurchaseRefNumber = purchaseViewModel.PurchaseRefNumber;
                purchasemodel.PurchaseDate = (DateTime)purchaseViewModel.PurchaseDate;
                purchasemodel.PurchaseDueDate = purchaseViewModel.PurchaseDueDate;
                purchasemodel.PurchaseSubTotal = purchaseViewModel.PurchaseSubTotal;
                purchasemodel.PurchaseDiscountAmount = purchaseViewModel.PurchaseDiscountAmount;
                purchasemodel.PurchaseTotoalAmount = purchaseViewModel.PurchaseTotoalAmount;
                purchasemodel.PurchaseVenderNote = purchaseViewModel.PurchaseVenderNote;
                purchasemodel.Vat6 = purchaseViewModel.Vat6;
                purchasemodel.VenderId = Contectid;
                purchasemodel.Vat21 = purchaseViewModel.Vat21;
                purchasemodel.Status = "Open";

                if (purchaseViewModel.PurchaseOrderID == 0)
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
                            if (purchadeDetail.PurchaseOrderDetailsId == 0)
                            {
                                HttpResponseMessage responsses = GlobalVeriables.WebApiClient.PostAsJsonAsync("APIPurchaseDetail", purchadeDetail).Result;
                            }
                            else
                            {
                                HttpResponseMessage responsses = GlobalVeriables.WebApiClient.PutAsJsonAsync("APIPurchaseDetail/" + purchadeDetail.PurchaseOrderDetailsId, purchadeDetail).Result;
                            }
                        }
                    }
                }

                else
                {
                    if (purchaseViewModel.PurchaseOrderID != 0)
                    {
                        HttpResponseMessage response2 = GlobalVeriables.WebApiClient.PutAsJsonAsync("APIPurchase/" + purchasemodel.PurchaseOrderID, purchasemodel).Result;

                        if (response2.StatusCode == System.Net.HttpStatusCode.OK)
                        {
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
                                if (purchadeDetail.PurchaseOrderDetailsId == 0)
                                {
                                    HttpResponseMessage responsses = GlobalVeriables.WebApiClient.PostAsJsonAsync("APIPurchaseDetail", purchadeDetail).Result;
                                }
                                else
                                {
                                    HttpResponseMessage responsses = GlobalVeriables.WebApiClient.PutAsJsonAsync("APIPurchaseDetail/" + purchadeDetail.PurchaseOrderDetailsId, purchadeDetail).Result;
                                }
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {

                return new JsonResult { Data = new { Status = "Fail", Message = ex.Message.ToString() } };
            }
            return new JsonResult { Data = new { Status = "Success", QutationId = intpurchaseorderId } };
        }



        [HttpPost]
        public ActionResult saveEmailPrint(MvcPurchaseViewModel purchaseViewModel)
        {
            var purchaseorderId = "";
            int intpurchaseorderId = 0;
            int companyId = 0;
            PurchaseOrderTable purchasemodel = new PurchaseOrderTable();
            try
            {
                if (Session["ClientId"] != null && Session["CompayID"] != null)
                {
                    Contectid = Convert.ToInt32(Session["VenderId"]);
                    CompanyID = Convert.ToInt32(Session["CompayID"]);
                }
                else
                {
                    return RedirectToAction("Index", "Login");
                }


                purchasemodel.CompanyId = CompanyID;
                purchasemodel.UserId = 1;
                purchasemodel.PurchaseID = purchaseViewModel.PurchaseId.ToString();

                purchasemodel.PurchaseOrderID = (purchaseViewModel.PurchaseOrderID != null ? purchaseViewModel.PurchaseOrderID : 0);
                purchasemodel.PurchaseRefNumber = purchaseViewModel.PurchaseRefNumber;
                purchasemodel.PurchaseDate = (DateTime)purchaseViewModel.PurchaseDate;
                purchasemodel.PurchaseDueDate = purchaseViewModel.PurchaseDueDate;
                purchasemodel.PurchaseSubTotal = purchaseViewModel.PurchaseSubTotal;
                purchasemodel.PurchaseDiscountAmount = purchaseViewModel.PurchaseDiscountAmount;
                purchasemodel.PurchaseTotoalAmount = purchaseViewModel.PurchaseTotoalAmount;
                purchasemodel.PurchaseVenderNote = purchaseViewModel.PurchaseVenderNote;
                purchasemodel.Vat6 = purchaseViewModel.Vat6;
                purchasemodel.VenderId = Contectid;
                purchasemodel.Vat21 = purchaseViewModel.Vat21;
                purchasemodel.Status = "Open";

                if (purchaseViewModel.PurchaseOrderID == 0)
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
                            if (purchadeDetail.PurchaseOrderDetailsId == 0)
                            {
                                HttpResponseMessage responsses = GlobalVeriables.WebApiClient.PostAsJsonAsync("APIPurchaseDetail", purchadeDetail).Result;
                            }
                            else
                            {
                                HttpResponseMessage responsses = GlobalVeriables.WebApiClient.PutAsJsonAsync("APIPurchaseDetail/" + purchadeDetail.PurchaseOrderDetailsId, purchadeDetail).Result;
                            }
                        }
                    }
                }

                else
                {
                    if (purchaseViewModel.PurchaseOrderID != 0)
                    {
                        HttpResponseMessage response2 = GlobalVeriables.WebApiClient.PutAsJsonAsync("APIPurchase/" + purchasemodel.PurchaseOrderID, purchasemodel).Result;

                        if (response2.StatusCode == System.Net.HttpStatusCode.OK)
                        {
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
                                if (purchadeDetail.PurchaseOrderDetailsId == 0)
                                {
                                    HttpResponseMessage responsses = GlobalVeriables.WebApiClient.PostAsJsonAsync("APIPurchaseDetail", purchadeDetail).Result;
                                }
                                else
                                {
                                    HttpResponseMessage responsses = GlobalVeriables.WebApiClient.PutAsJsonAsync("APIPurchaseDetail/" + purchadeDetail.PurchaseOrderDetailsId, purchadeDetail).Result;
                                }
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return new JsonResult { Data = new { Status = "Fail", Message = ex.Message.ToString() } };
            }
            return new JsonResult { Data = new { Status = "Success", QutationId = intpurchaseorderId } };
        }

        public ActionResult Viewinvoice(int? purchaseOrderId)
        {
            try
            {
                var idd = Session["ClientID"];
                var cdd = Session["CompayID"];


                if (Session["ClientID"] != null && Session["CompayID"] != null)
                {
                    Contectid = Convert.ToInt32(Session["ClientID"]);
                    CompanyID = Convert.ToInt32(Session["CompayID"]);
                }

                HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("ApiConatacts/" + Contectid.ToString()).Result;
                MVCContactModel contectmodel = response.Content.ReadAsAsync<MVCContactModel>().Result;

                HttpResponseMessage responseCompany = GlobalVeriables.WebApiClient.GetAsync("APIComapny/" + CompanyID.ToString()).Result;
                MVCCompanyInfoModel companyModel = responseCompany.Content.ReadAsAsync<MVCCompanyInfoModel>().Result;

                HttpResponseMessage res = GlobalVeriables.WebApiClient.GetAsync("APIPurchase/" + purchaseOrderId.ToString()).Result;
                MvcPurchaseModel ob = res.Content.ReadAsAsync<MvcPurchaseModel>().Result;

                GlobalVeriables.WebApiClient.DefaultRequestHeaders.Clear();

                HttpResponseMessage responseQutationDetailsList = GlobalVeriables.WebApiClient.GetAsync("APIPurchaseDetail/" + purchaseOrderId.ToString()).Result;
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



        public ActionResult InvoicebyEmail(int? purchaseOrderId)
        {



            EmailModel email = new EmailModel();
            try
            {

                email.Attachment = PrintView((int)purchaseOrderId);

                if (Session["CompayID"] == null)
                {
                    return RedirectToAction("Index", "Login");
                }

                var CompanyName = Session["CompanyName"];
                var contact = Session["CompanyContact"];
                var companyEmail = Session["CompanyEmail"];
                if (contact == null)
                {
                    contact = "Company Contact";
                }
                if (companyEmail == null)
                {
                    companyEmail = "Company Email";
                }

                int id = 0;
                if (Session["ClientID"] != null)
                {
                    id = Convert.ToInt32(Session["ClientID"]);
                }


                HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("ApiConatacts/" + id.ToString()).Result;
                MVCContactModel mvcContactModel = response.Content.ReadAsAsync<MVCContactModel>().Result;

            
                email.EmailText = @"Geachte heer" + mvcContactModel.ContactName + "." +


                ".Hierbij ontvangt u onze offerte 10 zoals besproken,." +

                "." + "Graag horen we of u hiermee akkoord gaat." +

                "." + "De offerte vindt u als bijlage bij deze email." +


                "..Met vriendelijke groet." +

                mvcContactModel.ContactName + "." +

                CompanyName.ToString() + "." +

                contact.ToString() + "." +

                companyEmail.ToString();

                string strToProcess = email.EmailText;
                string result = strToProcess.Replace(".", " \r\n");

                email.EmailText = result;


                email.invoiceId = (int)purchaseOrderId;
                email.From = "samarbudhni@gmail.com";
            }
            catch (Exception)
            {

                throw;
            }





            return View(email);
        }


        [HttpPost]
        public ActionResult InvoicebyEmail(EmailModel email)
        {

            TempData["EmailMessge"] = "";

            EmailModel emailModel = new EmailModel();
            var fileName = email.Attachment;
            try
            {
                if (email.Attachment.Contains(".pdf"))
                {
                    email.Attachment = email.Attachment.Replace(".pdf", "");
                }

                if (email.ToEmail.Contains(','))
                {
                    var p = email.Attachment.Split('.');

                    var root = Server.MapPath("~/PDF/");
                    var pdfname = String.Format("{0}.pdf", p);
                    var path = Path.Combine(root, pdfname);
                    email.Attachment = path;

                    string[] EmailArray = email.ToEmail.Split(',');
                    if (EmailArray.Count() > 0)
                    {
                        foreach (var item in EmailArray)
                        {
                            emailModel.From = email.From;
                            emailModel.ToEmail = item;
                            emailModel.Attachment = email.Attachment;
                            emailModel.EmailBody = email.EmailText;
                            bool result = EmailController.email(emailModel);
                        }
                    }
                }
                else
                {
                    var root = Server.MapPath("~/PDF/");
                    var pdfname = String.Format("{0}.pdf", email.Attachment);
                    var path = Path.Combine(root, pdfname);
                    email.Attachment = path;
                    emailModel.From = email.From;
                    emailModel.ToEmail = email.ToEmail;
                    emailModel.Attachment = email.Attachment;
                    emailModel.EmailBody = email.EmailText;
                    bool result = EmailController.email(emailModel);

                    TempData["EmailMessge"] = "Email Send successfully";

                }
                //return RedirectToAction("ViewQuation", "MVCQutation", new { @id = id });
                return RedirectToAction("Viewinvoice", new { purchaseOrderId = email.invoiceId });


            }
            catch (Exception ex)
            {
                TempData["EmailMessge"] = ex.Message.ToString();
                TempData["Error"] = ex.Message.ToString();
            }

            if (TempData["Path"] == null)
            {
                TempData["Path"] = fileName;
            }

            TempData["Message"] = "Email Send Succssfully";


            return View(email);
        }



        public string PrintView(int purchaseOrderId)
        {
            string pdfname;
            try
            {
                var idd = Session["ClientID"];
                var cdd = Session["CompayID"];


                if (Session["ClientID"] != null && Session["CompayID"] != null)
                {
                    Contectid = Convert.ToInt32(Session["ClientID"]);
                    CompanyID = Convert.ToInt32(Session["CompayID"]);
                }
               


                HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("ApiConatacts/" + Contectid.ToString()).Result;
                MVCContactModel contectmodel = response.Content.ReadAsAsync<MVCContactModel>().Result;

                HttpResponseMessage responseCompany = GlobalVeriables.WebApiClient.GetAsync("APIComapny/" + CompanyID.ToString()).Result;
                MVCCompanyInfoModel companyModel = responseCompany.Content.ReadAsAsync<MVCCompanyInfoModel>().Result;

                HttpResponseMessage res = GlobalVeriables.WebApiClient.GetAsync("APIPurchase/" + purchaseOrderId.ToString()).Result;
                MvcPurchaseModel ob = res.Content.ReadAsAsync<MvcPurchaseModel>().Result;

                GlobalVeriables.WebApiClient.DefaultRequestHeaders.Clear();

                HttpResponseMessage responseQutationDetailsList = GlobalVeriables.WebApiClient.GetAsync("APIPurchaseDetail/" + purchaseOrderId.ToString()).Result;
                List<MvcPurchaseViewModel> PuchaseModelDetailsList = responseQutationDetailsList.Content.ReadAsAsync<List<MvcPurchaseViewModel>>().Result;


                ViewBag.Contentdata = contectmodel;
                ViewBag.Companydata = companyModel;
                ViewBag.Purchase = ob;
                ViewBag.PurchaseDatailsList = PuchaseModelDetailsList;


                string companyName = purchaseOrderId + "-" + companyModel.CompanyName;
                var root = Server.MapPath("~/PDF/");
                pdfname = String.Format("{0}.pdf", companyName);
                var path = Path.Combine(root, pdfname);
                path = Path.GetFullPath(path);

                string subPath = "~/PDF"; // your code goes here
                bool exists = System.IO.Directory.Exists(Server.MapPath(subPath));

                if (!exists)
                {
                    System.IO.Directory.CreateDirectory(Server.MapPath(subPath));
                }
                if (System.IO.File.Exists(path))
                {
                    try
                    {

                        if (System.IO.File.Exists(path))
                        {
                            FileInfo info = new FileInfo(path);

                            if (!IsFileLocked(info)) info.Delete();

                        }
                    }
                    catch (System.IO.IOException e)
                    {

                    }
                }

                var pdfResult = new Rotativa.PartialViewAsPdf("~/Views/Purchase/Viewpp.cshtml")
                {
                    SaveOnServerPath = path, // Save your place
                    PageWidth = 200,
                    PageHeight = 350,
                };
                // This section allows you to save without downloading 
                pdfResult.BuildPdf(this.ControllerContext);
            }
            catch (Exception)
            {

                throw;
            }

            return pdfname;
        }


        public static Boolean IsFileLocked(FileInfo file)
        {
            FileStream stream = null;

            try
            {
                //Don't change FileAccess to ReadWrite, 
                //because if a file is in readOnly, it fails.
                stream = file.Open
                (
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.None
                );
            }
            catch (IOException ex)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            //file is not locked
            return false;
        }



        public class VatModel
        {
            public int Vat1 { get; set; }
            public string Name { get; set; }

        }
    }

}