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
                        public ActionResult AddOrEdit(MvcPurchaseViewModel MVCQutationViewModel)
                        {
                            MVCQutationModel mvcQutationModel = new MVCQutationModel();
                            try
                            {
                                if (MVCQutationViewModel.QutationID != null)
                                {
                                    mvcQutationModel.Qutation_ID = MVCQutationViewModel.Qutation_ID;
                                    mvcQutationModel.CompanyId = 2;
                                    mvcQutationModel.UserId = 1;
                                    mvcQutationModel.QutationID = MVCQutationViewModel.QutationID;
                                    mvcQutationModel.RefNumber = MVCQutationViewModel.RefNumber;
                                    mvcQutationModel.QutationDate = MVCQutationViewModel.QutationDate;
                                    mvcQutationModel.DueDate = MVCQutationViewModel.DueDate;
                                    mvcQutationModel.SubTotal = MVCQutationViewModel.SubTotal;
                                    mvcQutationModel.DiscountAmount = MVCQutationViewModel.DiscountAmount;
                                    mvcQutationModel.TotalAmount = MVCQutationViewModel.TotalAmount;
                                    mvcQutationModel.CustomerNote = MVCQutationViewModel.CustomerNote;
                                    mvcQutationModel.Qutation_ID = MVCQutationViewModel.Qutation_ID;
                                    mvcQutationModel.TotalVat6 = MVCQutationViewModel.TotalVat6;
                                    mvcQutationModel.TotalVat21 = MVCQutationViewModel.TotalVat21;
                                    mvcQutationModel.Qutation_ID = MVCQutationViewModel.Qutation_ID;
                                    mvcQutationModel.Status = MVCQutationViewModel.Status;
                                    HttpResponseMessage response = GlobalVeriables.WebApiClient.PutAsJsonAsync("APIQutation/" + mvcQutationModel.QutationID, mvcQutationModel).Result;
                                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                                    {
                                        foreach (QutationDetailsTable QDTList in MVCQutationViewModel.QutationDetailslist)
                                        {
                                            QutationDetailsTable QtDetails = new QutationDetailsTable();
                                            QtDetails.ItemId = Convert.ToInt32(QDTList.ItemId);
                                            QtDetails.QutationID = QDTList.QutationID;
                                            QtDetails.Description = QDTList.Description;
                                            QtDetails.QutationDetailId = QDTList.QutationDetailId;
                                            QtDetails.Quantity = QDTList.Quantity;
                                            QtDetails.Rate = Convert.ToDouble(QDTList.Rate);
                                            QtDetails.Total = Convert.ToDouble(QDTList.Total);
                                            QtDetails.Vat = Convert.ToDouble(QDTList.Vat);
                                            if (QtDetails.QutationDetailId == 0)
                                            {
                                                HttpResponseMessage responsses = GlobalVeriables.WebApiClient.PostAsJsonAsync("APIQutationDetail", QtDetails).Result;
                                            }
                                            else
                                            {
                                                HttpResponseMessage responsses = GlobalVeriables.WebApiClient.PutAsJsonAsync("APIQutationDetail/" + QtDetails.QutationDetailId, QtDetails).Result;
                                            }
                                        }
                                        return new JsonResult { Data = new { Status = "Success", QutationId = MVCQutationViewModel.QutationID } };
                                    }
                                    else
                                    {
                                        return new JsonResult { Data = new { Status = "Fail", QutationId = MVCQutationViewModel.QutationID } };

                                    }
                                }
                            }
                            catch (Exception ex)
                            {

                                return new JsonResult { Data = new { Status = "Fail", Message = ex.Message.ToString() } };
                            }
                            return new JsonResult { Data = new { Status = "Success", QutationId = MVCQutationViewModel.QutationID } };
                        }



                        int Contectid = 0;
                        int CompanyID = 0;

                        [HttpGet]
                        public ActionResult AddOrEdit(int id = 0)
                        {
                            MvcPurchaseViewModel quutionviewModel = new MvcPurchaseViewModel();
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

                                    HttpResponseMessage res = GlobalVeriables.WebApiClient.GetAsync("APIQutation/" + id.ToString()).Result;
                                    MVCQutationModel ob = res.Content.ReadAsAsync<MVCQutationModel>().Result;

                                    quutionviewModel.QutationID = ob.QutationID;
                                    quutionviewModel.QutationDate = ob.QutationDate;
                                    quutionviewModel.RefNumber = ob.RefNumber;
                                    quutionviewModel.DueDate = ob.DueDate;
                                    quutionviewModel.CustomerNote = ob.CustomerNote;
                                    quutionviewModel.SubTotal = ob.SubTotal;
                                    quutionviewModel.TotalAmount = ob.TotalAmount;
                                    quutionviewModel.TotalVat21 = (ob.TotalVat21 != null ? (float)(ob.TotalVat21) : (float)0.00);
                                    quutionviewModel.TotalVat6 = (ob.TotalVat6 != null ? (float)(ob.TotalVat6) : (float)0.00);


                                    HttpResponseMessage responseQutationDetailsList = GlobalVeriables.WebApiClient.GetAsync("APIQutationDetail/" + id.ToString()).Result;
                                    List<MVCQutationDetailsModel> QutationModelDetailsList = responseQutationDetailsList.Content.ReadAsAsync<List<MVCQutationDetailsModel>>().Result;
                                    ViewBag.Contentdata = contectmodel;
                                    ViewBag.Companydata = companyModel;
                                    ViewBag.QutationDatailsList = QutationModelDetailsList;


                                    return View(quutionviewModel);
                                }

                            }
                            catch (Exception ex)
                            {

                                ViewBag.Message = ex.ToString();
                            }
                            return View(quutionviewModel);

                        }



                        public class VatModel
                        {
                            public int Vat1 { get; set; }
                            public string Name { get; set; }

                        }
                    }

                }