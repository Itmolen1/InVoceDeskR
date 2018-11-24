using InvoiceDiskLast.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Policy;
using System.Web;
using System.Web.Mvc;

namespace InvoiceDiskLast.Controllers
{

    [SessionExpireAttribute]
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
            IEnumerable<MvcPurchaseModel> PurchaseList;
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

                int companyId = Convert.ToInt32(Session["CompayID"]);

                HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("GetPurchaseInvoiceList/" + StatusEnum.Goods + "/" + companyId).Result;
                PurchaseList = response.Content.ReadAsAsync<IEnumerable<MvcPurchaseModel>>().Result;


                if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
                {
                    PurchaseList = PurchaseList.Where(p => p.PurchaseOrderID.ToString().Contains(search)
                  || p.PurchaseRefNumber != null && p.PurchaseRefNumber.ToLower().Contains(search.ToLower())
                  || p.PurchaseDate != null && p.PurchaseDate.ToString().ToLower().Contains(search.ToLower())
                  || p.PurchaseDueDate != null && p.PurchaseDueDate.ToString().ToLower().Contains(search.ToLower())
                  || p.Status != null && p.Status.ToString().ToLower().Contains(search.ToLower())
                  || p.PurchaseTotoalAmount != null && p.PurchaseTotoalAmount.ToString().ToLower().Contains(search.ToLower())
                  || p.Status != null && p.Status.ToString().ToLower().Contains(search.ToLower())).ToList();
                }


                //    switch (sortColumn)
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


                int recordsTotal = recordsTotal = PurchaseList.Count();
                var data = PurchaseList.Skip(skip).Take(pageSize).ToList();
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Response.Write(ex.ToString());
                return Json(new { draw = 0, recordsFiltered = 0, recordsTotal = 0, data = 0 }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { draw = 0, recordsFiltered = 0, recordsTotal = 0, data = 0 }, JsonRequestBehavior.AllowGet);

        }



        public ActionResult GetVender()
        {
            var ProductList = new List<MVCContactModel>();
            try
            {
                int CompanyId = Convert.ToInt32(Session["CompayID"]);
                GlobalVeriables.WebApiClient.DefaultRequestHeaders.Clear();
                GlobalVeriables.WebApiClient.DefaultRequestHeaders.Add("CompayID", CompanyId.ToString());

                //GlobalVeriables.WebApiClient.DefaultRequestHeaders.Remove("CustomerStatus");

                // GlobalVeriables.WebApiClient.DefaultRequestHeaders.Add("CustomerStatus", "Vender");


                HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("ApiConatacts/" + CompanyId + "/Vender").Result;
                //HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("ApiConatacts").Result;
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
            var purchaseorderId = "";
            int intpurchaseorderId = 0;

            PurchaseOrderTable purchasemodel = new PurchaseOrderTable();
            try
            {
                if (Session["ClientId"] != null && Session["CompayID"] != null)
                {
                    Contectid = Convert.ToInt32(Session["ClientId"]);
                    CompanyID = Convert.ToInt32(Session["CompayID"]);
                }
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
                purchasemodel.VenderId = Contectid;
                purchasemodel.Vat21 = purchaseViewModel.Vat21;
                purchasemodel.Status = "open";
                purchasemodel.Type = StatusEnum.Goods.ToString();
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
                            if (purchadeDetail.PurchaseOrderDetailsId == 0 || purchadeDetail.PurchaseOrderDetailsId == null)
                            {
                                purchadeDetail.PurchaseId = intpurchaseorderId;

                                intpurchaseorderId = Convert.ToInt32(purchaseorderId);
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
                        purchasemodel.PurchaseID = purchaseViewModel.Purchase_ID;
                        intpurchaseorderId = Convert.ToInt32(purchaseViewModel.PurchaseOrderID);
                        HttpResponseMessage response2 = GlobalVeriables.WebApiClient.PutAsJsonAsync("APIPurchase/" + purchasemodel.PurchaseOrderID, purchasemodel).Result;
                        intpurchaseorderId = (int)purchaseViewModel.PurchaseOrderID;
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
                                if (purchadeDetail.PurchaseOrderDetailsId == 0 || purchadeDetail.PurchaseOrderDetailsId == null)
                                {
                                    purchadeDetail.PurchaseId = intpurchaseorderId;
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

            //calling printing option
            string path1 = PrintView(intpurchaseorderId);
            var root = Server.MapPath("/PDF/");
            var pdfname = String.Format("{0}", path1);
            var path = Path.Combine(root, pdfname);
            path = Path.GetFullPath(path);
            //DownloadFile(path);
            return new JsonResult { Data = new { Status = "Success", path = path, PurchaseOrderId = intpurchaseorderId } };
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

                GlobalVeriables.WebApiClient.DefaultRequestHeaders.Remove("CompayID");
                GlobalVeriables.WebApiClient.DefaultRequestHeaders.Add("CompayID", cdd.ToString());

                HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("ApiConatacts/" + idd.ToString()).Result;
                MVCContactModel contectmodel = response.Content.ReadAsAsync<MVCContactModel>().Result;

                HttpResponseMessage responseCompany = GlobalVeriables.WebApiClient.GetAsync("APIComapny/" + cdd.ToString()).Result;
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
                    purchaseviewModel.PurchaseDate = InvoiceDate;
                    purchaseviewModel.PurchaseDueDate = InvoiceDate.AddDays(+15);
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


                    HttpResponseMessage responsep = GlobalVeriables.WebApiClient.GetAsync("APIProduct/" + CompanyID + "/All").Result;
                    List<MVCProductModel> productModel = responsep.Content.ReadAsAsync<List<MVCProductModel>>().Result;
                    ViewBag.Product = productModel;

                    HttpResponseMessage res = GlobalVeriables.WebApiClient.GetAsync("APIPurchase/" + id.ToString()).Result;
                    MvcPurchaseModel ob = res.Content.ReadAsAsync<MvcPurchaseModel>().Result;

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
                purchasemodel.PurchaseOrderID = (Convert.ToInt32(purchaseViewModel.PurchaseOrderID != null ? purchaseViewModel.PurchaseOrderID : 0));
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
                purchasemodel.Status = "open";
                purchasemodel.Type = StatusEnum.Goods.ToString();

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
                            purchadeDetail.PurchaseVatPercentage = item.PurchaseVatPercentage;
                            purchadeDetail.PurchaseId = purchaseViewModel.PurchaseOrderID;
                            if (purchadeDetail.PurchaseOrderDetailsId == 0 || purchadeDetail.PurchaseOrderDetailsId == null)
                            {
                                purchadeDetail.PurchaseId = intpurchaseorderId;

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
                        purchasemodel.PurchaseID = purchaseViewModel.Purchase_ID;
                        intpurchaseorderId = Convert.ToInt32(purchaseViewModel.PurchaseOrderID);
                        HttpResponseMessage response2 = GlobalVeriables.WebApiClient.PutAsJsonAsync("APIPurchase/" + purchasemodel.PurchaseOrderID, purchasemodel).Result;
                        intpurchaseorderId = (int)purchaseViewModel.PurchaseOrderID;
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
                                if (purchadeDetail.PurchaseOrderDetailsId == 0 || purchadeDetail.PurchaseOrderDetailsId == null)
                                {
                                    purchadeDetail.PurchaseId = intpurchaseorderId;

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

                purchasemodel.PurchaseOrderID = (Convert.ToInt32(purchaseViewModel.PurchaseOrderID != null ? purchaseViewModel.PurchaseOrderID : 0));
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
                purchasemodel.Status = "open";
                purchasemodel.Type = StatusEnum.Goods.ToString();
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
                            if (purchadeDetail.PurchaseOrderDetailsId == 0 || purchadeDetail.PurchaseOrderDetailsId == null)
                            {
                                purchadeDetail.PurchaseId = intpurchaseorderId;

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
                        purchasemodel.PurchaseID = purchaseViewModel.Purchase_ID;
                        intpurchaseorderId = Convert.ToInt32(purchaseViewModel.PurchaseOrderID);
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
                                if (purchadeDetail.PurchaseOrderDetailsId == 0 || purchadeDetail.PurchaseOrderDetailsId == null)
                                {
                                    purchadeDetail.PurchaseId = intpurchaseorderId;

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
            return new JsonResult { Data = new { Status = "Success", PurchaseOrderId = intpurchaseorderId } };
        }


        [HttpPost]
        public ActionResult saveEmailPrint(MvcPurchaseViewModel purchaseViewModel)
        {
            var purchaseorderId = "";
            int intpurchaseorderId = 0;

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

                purchasemodel.PurchaseOrderID = (Convert.ToInt32(purchaseViewModel.PurchaseOrderID != null ? purchaseViewModel.PurchaseOrderID : 0));
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
                purchasemodel.Status = "open";
                purchasemodel.Type = StatusEnum.Goods.ToString();
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
                            if (purchadeDetail.PurchaseOrderDetailsId == 0 || purchadeDetail.PurchaseOrderDetailsId == null)
                            {
                                purchadeDetail.PurchaseId = intpurchaseorderId;
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
                        purchasemodel.PurchaseID = purchaseViewModel.Purchase_ID;
                        intpurchaseorderId = Convert.ToInt32(purchaseViewModel.PurchaseOrderID);
                        HttpResponseMessage response2 = GlobalVeriables.WebApiClient.PutAsJsonAsync("APIPurchase/" + purchasemodel.PurchaseOrderID, purchasemodel).Result;
                        intpurchaseorderId = (int)purchaseViewModel.PurchaseOrderID;
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
                                if (purchadeDetail.PurchaseOrderDetailsId == 0 || purchadeDetail.PurchaseOrderDetailsId == null)
                                {
                                    purchadeDetail.PurchaseId = intpurchaseorderId;
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
            return new JsonResult { Data = new { Status = "Success", PurchaseOrderId = intpurchaseorderId } };
        }

        public ActionResult Viewinvoice(int? purchaseOrderId)
        {

            if (purchaseOrderId == null)
            {
                return RedirectToAction("Index", "Login");
            }

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

        public string SetPdfName(string FilePath)
        {
            int CompanyId = 0;
            try
            {

                if (Session["CompayID"] != null)
                {
                    CompanyId = Convert.ToInt32(Session["CompayID"]);
                }

                HttpResponseMessage responseCompany = GlobalVeriables.WebApiClient.GetAsync("APIComapny/" + CompanyId.ToString()).Result;
                MVCCompanyInfoModel companyModel = responseCompany.Content.ReadAsAsync<MVCCompanyInfoModel>().Result;
                string[] arrya = FilePath.Split('-');
                string InvoiceNumber = arrya[1];
                string PdfName1 = InvoiceNumber + "-" + companyModel.CompanyName + ".pdf";
                return PdfName1;
            }
            catch (Exception)
            {

                throw;
            }

        }

        [DeleteFileClass]
        [HttpPost]
        public FileResult DownloadFile(string FilePath1)
        {
            string filepath = "";
            string FileName = SetPdfName(FilePath1);
            try
            {
                filepath = System.IO.Path.Combine(Server.MapPath("/PDF/"), FilePath1);
                HttpContext.Items["FilePath"] = FilePath1;

            }
            catch (Exception)
            {
            }

            return File(filepath, MimeMapping.GetMimeMapping(filepath), FileName);
        }

        [AllowAnonymous]
        public ActionResult Footer()
        {
            return View();
        }

        public ActionResult Print(int? purchaseOrderId)
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
                else
                {
                    return RedirectToAction("Index", "Login");
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

                DateTime PurchaseDueDate = Convert.ToDateTime(ob.PurchaseDueDate); //mm/dd/yyyy
                DateTime PurchaseDate = Convert.ToDateTime(ob.PurchaseDate);//mm/dd/yyyy
                TimeSpan ts = PurchaseDueDate.Subtract(PurchaseDate);
                string diffDate = ts.Days.ToString();

                ViewBag.Contentdata = contectmodel;
                ViewBag.Companydata = companyModel;
                ViewBag.Purchase = ob;
                ViewBag.PurchaseDatailsList = PuchaseModelDetailsList;

                string PdfName = purchaseOrderId + "-" + companyModel.CompanyName + ".pdf";


                string cutomswitches = "";
                cutomswitches = "--footer-center \"" + "Wilt u zo vriendelijk zijn om het verschuldigde bedrag binnen " + diffDate + " dagen over te maken naar IBAN: \n " + companyModel.IBANNumber + " ten name van IT Molen o.v.v.bovenstaande factuurnummer. \n (Op al onze diensten en producten zijn onze algemene voorwaarden van toepassing.Deze kunt u downloaden van onze website.)" + " \n Printed date: " +
                   DateTime.Now.Date.ToString("MM/dd/yyyy") + "  Page: [page]/[toPage]\"" +
                  " --footer-line --footer-font-size \"10\" --footer-spacing 6 --footer-font-name \"calibri light\"";


                return new Rotativa.PartialViewAsPdf("~/Views/Purchase/Viewpp.cshtml")
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


                if (CompanyName == null)
                {
                    CompanyName = "Nocompany";
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
                email.From = "infouurtjefactuur@gmail.com";

            }
            catch (Exception)
            {

                throw;
            }

            return View(email);
        }
        public bool PerformTransaction(MvcPurchaseModel purchaseViewModel, int CompanyId)
        {
            bool TransactionResult = false;
            try
            {
                string base64Guid = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                AccountTransictionTable accountTransictiontable = new AccountTransictionTable();
                accountTransictiontable.TransictionDate = DateTime.Now;
                accountTransictiontable.FK_AccountID = 4002;
                //with tax
                accountTransictiontable.Cr = purchaseViewModel.PurchaseTotoalAmount;
                accountTransictiontable.Dr = 0.00;
                accountTransictiontable.TransictionNumber = base64Guid;
                accountTransictiontable.TransictionRefrenceId = purchaseViewModel.PurchaseOrderID.ToString();
                accountTransictiontable.TransictionType = "Purchase";
                accountTransictiontable.CreationTime = DateTime.Now.TimeOfDay;
                accountTransictiontable.AddedBy = 1;
                accountTransictiontable.FK_CompanyId = CompanyId;
                accountTransictiontable.FKPaymentTerm = 1;
                accountTransictiontable.Description = "Total + Invoice ,Invoice created at Invoice" + purchaseViewModel.PurchaseTotoalAmount.ToString() + "On invoice genrattion";
                HttpResponseMessage responses = GlobalVeriables.WebApiClient.PostAsJsonAsync("APIAccountTransiction", accountTransictiontable).Result;
                if (responses.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string base64Guid1 = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                    TransactionResult = true;
                    accountTransictiontable.TransictionDate = DateTime.Now;
                    accountTransictiontable.FK_AccountID = 4003;
                    accountTransictiontable.Dr = purchaseViewModel.PurchaseSubTotal;
                    accountTransictiontable.Cr = 0.00;
                    accountTransictiontable.TransictionNumber = base64Guid1;
                    accountTransictiontable.TransictionRefrenceId = purchaseViewModel.PurchaseOrderID.ToString();
                    accountTransictiontable.TransictionType = "Purchase";
                    accountTransictiontable.CreationTime = DateTime.Now.TimeOfDay;
                    accountTransictiontable.AddedBy = 1;
                    accountTransictiontable.FK_CompanyId = CompanyId;
                    accountTransictiontable.FKPaymentTerm = 1;
                    accountTransictiontable.Description = "Total + Invoice ,Invoice created at Invoice" + purchaseViewModel.PurchaseSubTotal.ToString() + "On invoice genrattion";
                    HttpResponseMessage responses2 = GlobalVeriables.WebApiClient.PostAsJsonAsync("APIAccountTransiction", accountTransictiontable).Result;

                    if (responses2.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        string base64Guid2 = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                        TransactionResult = true;
                        accountTransictiontable.TransictionDate = DateTime.Now;
                        accountTransictiontable.FK_AccountID = 3005;
                        accountTransictiontable.Dr = purchaseViewModel.PurchaseSubTotal;
                        accountTransictiontable.Cr = 0.00;
                        accountTransictiontable.TransictionNumber = base64Guid2;
                        accountTransictiontable.TransictionRefrenceId = purchaseViewModel.PurchaseOrderID.ToString();
                        accountTransictiontable.TransictionType = "Purchase";
                        accountTransictiontable.CreationTime = DateTime.Now.TimeOfDay;
                        accountTransictiontable.AddedBy = 1;
                        accountTransictiontable.FK_CompanyId = CompanyId;
                        accountTransictiontable.FKPaymentTerm = 1;
                        double TotalVat = Convert.ToDouble(purchaseViewModel.Vat21 + purchaseViewModel.Vat6);
                        accountTransictiontable.Dr = TotalVat;
                        accountTransictiontable.Description = "Total + Vat ,Invoice created at Invoice" + TotalVat + "On invoice genrattion";

                        HttpResponseMessage responses3 = GlobalVeriables.WebApiClient.PostAsJsonAsync("APIAccountTransiction", accountTransictiontable).Result;
                        if (responses3.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            return TransactionResult = true;
                        }

                    }
                }
            }


            catch (Exception)
            {

                return TransactionResult = false;
            }
            return TransactionResult;
        }

        [HttpPost]
        public ActionResult InvoicebyEmail(EmailModel email)
        {
            var idd = Session["ClientID"];
            var cdd = Session["CompayID"];
            if (Session["ClientID"] != null && Session["CompayID"] != null)
            {
                Contectid = Convert.ToInt32(Session["ClientID"]);
                CompanyID = Convert.ToInt32(Session["CompayID"]);
            }

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

                    var root = Server.MapPath("/PDF/");
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
                    var root = Server.MapPath("/PDF/");
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

                HttpResponseMessage res = GlobalVeriables.WebApiClient.GetAsync("APIPurchase/" + email.invoiceId.ToString()).Result;
                MvcPurchaseModel ob = res.Content.ReadAsAsync<MvcPurchaseModel>().Result;

                if (PerformTransaction(ob, CompanyID))
                {
                    TempData["EmailMessge"] = "Email Send Succssfully";

                }
                else
                {
                    TempData["EmailMessge"] = "Your transaction is not perform with success";
                }
                return RedirectToAction("Viewinvoice", new { purchaseOrderId = email.invoiceId });
            }
            catch (Exception ex)
            {
                email.Attachment = fileName;
                TempData["EmailMessge"] = ex.Message.ToString();
                TempData["Error"] = ex.Message.ToString();
            }
            if (TempData["Path"] == null)
            {
                TempData["Path"] = fileName;
            }
            TempData["Message"] = "Email Send Succssfully";
            email.Attachment = fileName;

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

                DateTime PurchaseDueDate = Convert.ToDateTime(ob.PurchaseDueDate); //mm/dd/yyyy
                DateTime PurchaseDate = Convert.ToDateTime(ob.PurchaseDate);//mm/dd/yyyy
                TimeSpan ts = PurchaseDueDate.Subtract(PurchaseDate);
                string diffDate = ts.Days.ToString();

                string companyName = purchaseOrderId + "-" + companyModel.CompanyName;
                var root = Server.MapPath("/PDF/");
                pdfname = String.Format("{0}.pdf", companyName);
                var path = Path.Combine(root, pdfname);
                path = Path.GetFullPath(path);

                string subPath = "/PDF"; // your code goes here
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

                    PageSize = Rotativa.Options.Size.A4,
                    MinimumFontSize = 16,
                    PageMargins = new Rotativa.Options.Margins(10, 12, 20, 3),
                    PageHeight = 40,

                    SaveOnServerPath = path,

                    CustomSwitches = "--footer-center \"" + "Wilt u zo vriendelijk zijn om het verschuldigde bedrag binnen " + diffDate + " dagen over te maken naar IBAN: \n " + companyModel.IBANNumber + " ten name van IT Molen o.v.v.bovenstaande factuurnummer. \n (Op al onze diensten en producten zijn onze algemene voorwaarden van toepassing.Deze kunt u downloaden van onze website.)" + " \n Printed date: " +
                    DateTime.Now.Date.ToString("MM/dd/yyyy") + "  Page: [page]/[toPage]\"" +
                   " --footer-line --footer-font-size \"10\" --footer-spacing 6 --footer-font-name \"calibri light\"",
                };
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
                stream = file.Open
                (
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.None
                );
            }
            catch (IOException ex)
            {
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


        [HttpPost]
        public ActionResult DeleteInvoice(int PurchaseOrderId, int purchaseOrderDetailId, int vat, decimal total)
        {
            try
            {

                MvcPurchaseModel mvcpurchaseModel = new MvcPurchaseModel();
                HttpResponseMessage res = GlobalVeriables.WebApiClient.GetAsync("APIPurchase/" + PurchaseOrderId.ToString()).Result;
                mvcpurchaseModel = res.Content.ReadAsAsync<MvcPurchaseModel>().Result;

                MVCPurchaseDetailsModel detailModel = new MVCPurchaseDetailsModel();

                GlobalVeriables.WebApiClient.DefaultRequestHeaders.Clear();

                GlobalVeriables.WebApiClient.DefaultRequestHeaders.Add("PDID", PurchaseOrderId.ToString());
                //GlobalVeriables.WebApiClient.DefaultRequestHeaders.Add("QutationDetailID1", QutationDetailID1);



                HttpResponseMessage responseQutationDetailsList = GlobalVeriables.WebApiClient.GetAsync("APIPurchaseDetail/" + purchaseOrderDetailId.ToString()).Result;
                List<MvcPurchaseViewModel> PurchaseDetailsList = responseQutationDetailsList.Content.ReadAsAsync<List<MvcPurchaseViewModel>>().Result;

                if (PurchaseDetailsList.Count() > 1)
                {
                    PurchaseDetailsList = PurchaseDetailsList.Where(C => C.PurchaseOrderDetailsId == purchaseOrderDetailId).ToList();

                    #region
                    if (PurchaseDetailsList.Count() != 0)
                    {
                        detailModel.PurchaseVatPercentage = PurchaseDetailsList[0].PurchaseVatPercentage;

                        detailModel.PurchaseTotal = PurchaseDetailsList[0].PurchaseTotal;

                        mvcpurchaseModel.PurchaseSubTotal = mvcpurchaseModel.PurchaseSubTotal - detailModel.PurchaseTotal;

                        mvcpurchaseModel.PurchaseTotal = mvcpurchaseModel.PurchaseTotal - detailModel.PurchaseTotal;

                        if (vat == 6)
                            mvcpurchaseModel.Vat6 = detailModel.PurchaseVatPercentage - 6;
                        else
                            mvcpurchaseModel.Vat21 = detailModel.PurchaseVatPercentage - 21;


                        HttpResponseMessage response = GlobalVeriables.WebApiClient.PutAsJsonAsync("APIPurchase/" + PurchaseOrderId, mvcpurchaseModel).Result;
                        if (response.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            HttpResponseMessage deleteQuaution2 = GlobalVeriables.WebApiClient.DeleteAsync("APIPurchaseDetail/" + purchaseOrderDetailId).Result;
                            if (deleteQuaution2.StatusCode == System.Net.HttpStatusCode.OK)
                            {
                                GlobalVeriables.WebApiClient.DefaultRequestHeaders.Clear();
                                return new JsonResult { Data = new { Status = "Success" } };
                            }
                        }
                        else
                        {
                            return new JsonResult { Data = new { Status = "Fail" } };

                        }
                        #endregion
                    }
                }

                else
                {

                    HttpResponseMessage Qdresponse = GlobalVeriables.WebApiClient.DeleteAsync("APIPurchaseDetail/" + purchaseOrderDetailId).Result;

                    if (Qdresponse.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        mvcpurchaseModel.PurchaseSubTotal = 0.00;
                        mvcpurchaseModel.PurchaseTotoalAmount = 0.00;
                        mvcpurchaseModel.PurchaseID = mvcpurchaseModel.PurchaseID;
                        mvcpurchaseModel.PurchaseOrderID = mvcpurchaseModel.PurchaseOrderID;
                        mvcpurchaseModel.PurchaseDueDate = mvcpurchaseModel.PurchaseDueDate;
                        mvcpurchaseModel.PurchaseVenderNote = mvcpurchaseModel.PurchaseVenderNote;
                        mvcpurchaseModel.PurchaseDueDate = mvcpurchaseModel.PurchaseDueDate;
                        mvcpurchaseModel.Status = mvcpurchaseModel.Status;
                        mvcpurchaseModel.CompanyId = mvcpurchaseModel.CompanyId;
                        mvcpurchaseModel.Vat6 = 0;
                        mvcpurchaseModel.Vat21 = 0;

                        HttpResponseMessage response = GlobalVeriables.WebApiClient.PutAsJsonAsync("APIPurchase/" + PurchaseOrderId, mvcpurchaseModel).Result;
                        if (response.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            GlobalVeriables.WebApiClient.DefaultRequestHeaders.Clear();

                            return new JsonResult { Data = new { Status = "Success" } };
                        }
                    }
                    else
                    {
                        return new JsonResult { Data = new { Status = "Fail" } };

                    }
                }

            }
            catch (Exception)
            {

                return new JsonResult { Data = new { Status = "Fail" } };
            }

            return new JsonResult { Data = new { Status = "Success" } };
        }


        public ActionResult Design()
        {
            return View();
        }


        public class VatModel
        {
            public int Vat1 { get; set; }
            public string Name { get; set; }

        }


        public ActionResult PurchaseList()
        {
            return View();

        }




        [HttpPost]
        public ActionResult Trasactionpayment(List<TransactionModel> TransactionModel)
        {

            try
            {

                HttpResponseMessage res = GlobalVeriables.WebApiClient.GetAsync("APIPurchase/" + TransactionModel[0].PurchaseOrderID.ToString()).Result;
                MvcPurchaseModel ob = res.Content.ReadAsAsync<MvcPurchaseModel>().Result;
                if (res.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    
                    foreach (var item in TransactionModel)
                    {
                        string base64Guid1 = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                        AccountTransictionTable accountTransictiontable = new AccountTransictionTable();
                        accountTransictiontable.FK_CompanyId = Convert.ToInt32(Session["CompayID"]);
                        accountTransictiontable.Description = item.descrition;
                        accountTransictiontable.FK_AccountID = item.Id;
                        accountTransictiontable.Description = item.descrition;
                        accountTransictiontable.FKPaymentTerm = 1;
                        accountTransictiontable.TransictionRefrenceId = ob.PurchaseOrderID.ToString();
                        accountTransictiontable.Dr = item.AmountDebit;
                        accountTransictiontable.Cr = item.AmountCredit;
                        accountTransictiontable.TransictionNumber = base64Guid1;
                        accountTransictiontable.TransictionType = "Purchase";
                        accountTransictiontable.CreationTime = DateTime.Now.TimeOfDay;
                        accountTransictiontable.TransictionDate = item.TranDate;
                        TransactionClass.PerformTransaction(accountTransictiontable);
                    }
                }
            }
            catch (Exception)
            {
                return Json("Fail", JsonRequestBehavior.AllowGet);
            }

            return Json("Success", JsonRequestBehavior.AllowGet);

            return View();
        }
    }

}