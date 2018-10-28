using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using InvoiceDiskLast.Models;

namespace InvoiceDiskLast.Controllers
{


    public class MVCClientController : Controller
    {
        // GET: MVCClient
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetContacts()
        {
            int CompanyId = Convert.ToInt32(Session["CompayID"]);
            GlobalVeriables.WebApiClient.DefaultRequestHeaders.Clear();
            GlobalVeriables.WebApiClient.DefaultRequestHeaders.Add("CompayID", CompanyId.ToString());
            GlobalVeriables.WebApiClient.DefaultRequestHeaders.Remove("CustomerStatus");

            GlobalVeriables.WebApiClient.DefaultRequestHeaders.Add("CustomerStatus", "Customer");
            HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("ApiConatacts").Result;


         
        
            var ProductList = response.Content.ReadAsAsync<IEnumerable<MVCContactModel>>().Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return Json(ProductList, JsonRequestBehavior.AllowGet);
            }

            return View();
        }

        [HttpPost]
        public JsonResult GetContactList()
        {
            IEnumerable<MVCContactModel> ContactsList;
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

                HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("ApiConatacts").Result;
                ContactsList = response.Content.ReadAsAsync<IEnumerable<MVCContactModel>>().Result;


                if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
                {
                    // Apply search  on multiple field  
                    ContactsList = ContactsList.Where(p => p.ContactsId.ToString().Contains(search) ||
                    p.ContactName.ToLower().Contains(search.ToLower()) ||

                    //p.BillingCity.ToLower().Contains(search.ToLower()) ||
                    p.Type.ToLower().Contains(search.ToLower()) ||
                    p.ContactAddress.ToLower().ToString().Contains(search.ToLower())).ToList();
                }


                switch (sortColumn)
                {
                    case "ContactName":
                        ContactsList = ContactsList.OrderBy(c => c.ContactName);
                        break;
                    case "Type":
                        ContactsList = ContactsList.OrderBy(c => c.Type);
                        break;


                

                    default:
                        ContactsList = ContactsList.OrderByDescending(c => c.ContactsId);
                        break;
                }


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

        [HttpGet]
        public ActionResult AddOrEdit(int id = 0)
        {
            if (id == 0)
            {

                return View(new MVCContactModel());
            }
            else
            {
                HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("ApiConatacts/" + id.ToString()).Result;
                MVCContactModel mvcContactModel = response.Content.ReadAsAsync<MVCContactModel>().Result;

                Session["ClientID"] = mvcContactModel.ContactsId;
   

                return Json(mvcContactModel, JsonRequestBehavior.AllowGet);
            }
        }


        public JsonResult AddOrEditQutation(int Qutationid)
        {

            MVCQutationModel m = new Models.MVCQutationModel();

            HttpResponseMessage response1 = GlobalVeriables.WebApiClient.GetAsync("APIQutation/" + Qutationid.ToString()).Result;
            m = response1.Content.ReadAsAsync<MVCQutationModel>().Result;

            Session["ClientID"] = m.ContactId;
           
            return Json("", JsonRequestBehavior.AllowGet);

        }


        [HttpPost]
        public ActionResult AddOrEdit(MVCContactModel mvcContactModel)
        {
            //if (!ModelState.IsValid)
            //{
            //    return View(mvcContactModel);
            //}
            mvcContactModel.Addeddate = Convert.ToDateTime(DateTime.Now.ToShortDateString());

          
                if (mvcContactModel.ContactsId == null)
                {
                    mvcContactModel.Company_Id = Convert.ToInt32(Session["CompayID"]);
                    mvcContactModel.UserId = 1;
                    mvcContactModel.Addeddate = Convert.ToDateTime(System.DateTime.Now.ToShortDateString());
                    mvcContactModel.Type = mvcContactModel.Type;
                    HttpResponseMessage response = GlobalVeriables.WebApiClient.PostAsJsonAsync("ApiConatacts", mvcContactModel).Result;

                    return Json(response.StatusCode, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    mvcContactModel.Company_Id = Convert.ToInt32(Session["CompayID"]);
                    mvcContactModel.UserId = 1;
                    mvcContactModel.Addeddate = Convert.ToDateTime(System.DateTime.Now.ToShortDateString());
                    HttpResponseMessage response = GlobalVeriables.WebApiClient.PutAsJsonAsync("ApiConatacts/" + mvcContactModel.ContactsId, mvcContactModel).Result;
                    TempData["SuccessMessage"] = "Updated Successfully";
                }
            
            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            try
            {

                HttpResponseMessage response = GlobalVeriables.WebApiClient.DeleteAsync("ApiConatacts/" + id.ToString()).Result;
                TempData["SuccessMessage"] = "Delete Successfully";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return View();
        }

        public ActionResult Details(int id)
        {
            try
            {
                HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("ApiConatacts/" + id.ToString()).Result;
                return View(response.Content.ReadAsAsync<MVCContactModel>().Result);
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return View();
        }


        public string C()
        {
            var id = Session["SessionID"];
            return id.ToString();
        }
    }
}