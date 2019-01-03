using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using InvoiceDiskLast.Models;
using System.IO;
using InvoiceDiskLast.MISC;
using Logger;

namespace InvoiceDiskLast.Controllers
{

    [SessionExpireAttribute]
    [RouteNotFoundAttribute]
    public class MVCClientController : Controller
    {
        private Ilog _iLog;
        public MVCClientController()
        {
            _iLog = Log.GetInstance;
        }
        // GET: MVCClient
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetContacts()
        {
            int CompanyId = Convert.ToInt32(Session["CompayID"]);
           
            HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("ApiConatacts/" + CompanyId + "/Customer").Result;

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

                // GlobalVeriables.WebApiClient.DefaultRequestHeaders.Clear();
                int CompanyId = Convert.ToInt32(Session["CompayID"]);
                //  GlobalVeriables.WebApiClient.DefaultRequestHeaders.Add("CompayID", CompanyId.ToString());


                HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("ApiConatacts/" + CompanyId + "/All").Result;
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
                    case "City":
                        ContactsList = ContactsList.OrderBy(c => c.City);
                        break;
                    case "PostalCode":
                        ContactsList = ContactsList.OrderBy(c => c.PostalCode);
                        break;
                    case "Mobile":
                        ContactsList = ContactsList.OrderBy(c => c.Mobile);
                        break;
                    case "Status":
                        ContactsList = ContactsList.OrderBy(c => c.Status);
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
               
                mvcContactModel.UserId = 1;
                mvcContactModel.Addeddate = Convert.ToDateTime(System.DateTime.Now.ToShortDateString());
                HttpResponseMessage response = GlobalVeriables.WebApiClient.PutAsJsonAsync("ApiConatacts/" + mvcContactModel.ContactsId, mvcContactModel).Result;
                HttpResponseMessage directory = GlobalVeriables.WebApiClient.GetAsync("CheckForDirectoryExist/" + mvcContactModel.ContactsId).Result;
                DirectoryTable _Directory = new DirectoryTable();
                if (directory.StatusCode == System.Net.HttpStatusCode.OK)
                {
                }
                else
                {
                    _Directory.IsActive = true;
                    _Directory.DirectoryPath = CreatDirectoryClass.CreateDirecotyFolder((int)mvcContactModel.ContactsId, mvcContactModel.ContactName,"Client");
                    _Directory.RefrenceId = mvcContactModel.ContactsId;

                    HttpResponseMessage directoryResponse = GlobalVeriables.WebApiClient.PostAsJsonAsync("CreateDirecoty", _Directory).Result;

                }
            }

            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id, bool status)
        {
            try
            {
                if (status == true)
                {
                    status = false;
                }
                else
                {
                    status = true;
                }
                HttpResponseMessage response = GlobalVeriables.WebApiClient.DeleteAsync("DeleteConatcts/" + id + "/" + status).Result;

                return Json("ok", JsonRequestBehavior.AllowGet);
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


        public class IFormFile
        {
            public HttpPostedFile file { get; set; }
        }
        
        public ActionResult ViewDirecory(int Id, string DName)
        {
            string d = "";
            try
            {
                List<DirectoryViewModel> _DirectoryList = new List<DirectoryViewModel>();
                DirectoryViewModel _Directory = new DirectoryViewModel();
                HttpResponseMessage directory = GlobalVeriables.WebApiClient.GetAsync("GetDirectory/" + Id).Result;
                _Directory = directory.Content.ReadAsAsync<DirectoryViewModel>().Result;

                if (directory.StatusCode != System.Net.HttpStatusCode.OK)
                { 
                      CreatDirectoryClass.CreateDirecotyFolder(Id, DName);
                    directory = GlobalVeriables.WebApiClient.GetAsync("GetDirectory/" + Id).Result;
                    _Directory = directory.Content.ReadAsAsync<DirectoryViewModel>().Result;

                    d = _Directory.DirectoryPath.ToString();
                    string F = _Directory.DirectoryPath.ToString();
                    d = d.Substring(17);
                    ViewBag.Name = d.Replace("/", "");

                    if (_Directory.DirectoryPath != null)
                    {
                        DirectoryInfo dir = new DirectoryInfo(Server.MapPath(_Directory.DirectoryPath));
                        FileInfo[] info = dir.GetFiles("*.*");

                        foreach (FileInfo f in info)
                        {
                            string Name = f.Name;
                            _DirectoryList.Add(new DirectoryViewModel { DirectoryPath = f.Name, FileFolderPathe = F });
                        }
                        ViewBag.TreeView = _DirectoryList;
                    }
                }
                else
                {
                    d = _Directory.DirectoryPath.ToString();
                    string F = _Directory.DirectoryPath.ToString();
                    d = d.Substring(17);
                    ViewBag.Name = d.Replace("/", "");

                    if (_Directory.DirectoryPath != null)
                    {
                        DirectoryInfo dir = new DirectoryInfo(Server.MapPath(_Directory.DirectoryPath));
                        FileInfo[] info = dir.GetFiles("*.*");

                        foreach (FileInfo f in info)
                        {
                            string Name = f.Name;
                            _DirectoryList.Add(new DirectoryViewModel { DirectoryPath = f.Name, FileFolderPathe = F });
                        }
                        ViewBag.TreeView = _DirectoryList;
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }

            return View();
        }

        [HttpPost]
        public ActionResult UploadFiles(MVCContactModel mvcContactModel)
        {
            try
            {
                HttpFileCollectionBase files = Request.Files;

                if (CreatDirectoryClass.UploadFileAndCreateDirectory((int)mvcContactModel.ContactsId, mvcContactModel.ContactName, files, "Decription"))
                {
                    return Json("Success", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("Fail", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {

                return Json("Fail", JsonRequestBehavior.AllowGet);
            }
            

        }


        //try
        //{
        //       if (mvcContactModel.ContactsId != null)
        //    {
        //        int Client = Convert.ToInt32(Session["ClientId"]);
        //        HttpResponseMessage directory = GlobalVeriables.WebApiClient.GetAsync("GetDirectory/" + mvcContactModel.ContactsId).Result;
        //        _Directory = directory.Content.ReadAsAsync<DirectoryViewModel>().Result;

        //        if (_Directory.DirectoryPath != null)
        //        {
        //            string folderPAth = Server.MapPath(_Directory.DirectoryPath);
        //            HttpFileCollectionBase files = Request.Files;
        //            if (System.IO.Directory.Exists(folderPAth))
        //            {
        //                for (int i = 0; i < files.Count; i++)
        //                {
        //                    HttpPostedFileBase file = files[i];
        //                    FileInfo fi = new FileInfo(file.FileName);
        //                    string ext = fi.Extension;
        //                    if (allowedExtensions.Contains(ext))
        //                    {
        //                        string dateTime = DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();

        //                        var FileName = file.FileName.Replace(ext, "");

        //                        string FileNameSetting = FileName + dateTime + ext;

        //                        file.SaveAs(folderPAth + FileNameSetting);
        //                    }
        //                }
        //            }
        //            return Json("UploadSuccess", JsonRequestBehavior.AllowGet);

        //        }
        //        else
        //        {
        //            return Json("NotExist", JsonRequestBehavior.AllowGet);
        //        }
        //    }
        //    else
        //    {
        //        return Json("NotExist", JsonRequestBehavior.AllowGet);
        //    }

        //}


        [HttpPost]
        public ActionResult UploadFile(FormCollection from)
        {



            var file = from["file"];


            if (Request.Form["file"] != null)
            {
                var fileName2 = Request.Form["file"];

                string[] valueArray = fileName2.Split(',');

                if (valueArray != null && valueArray.Count() > 0)
                {

                    foreach (var itemm in valueArray)
                    {
                        if (itemm.EndsWith("doc") || itemm.EndsWith("docx") || itemm.EndsWith("jpg") || itemm.EndsWith("png") || itemm.EndsWith("txt"))
                        {

                        }
                    }
                }
            }



            //for (int i = 0; i < Request.Files.Count; i++)
            //{
            //    var file = Request.Files[i];

            //    if (file != null && file.ContentLength > 0)
            //    {
            //        var fileName = Path.GetFileName(file.FileName);
            //FileDetail fileDetail = new FileDetail()
            //{
            //    FileName = fileName,
            //    Extension = Path.GetExtension(fileName),
            //    Id = Guid.NewGuid()
            //};
            // fileDetails.Add(fileDetail);
            // var path = Path.Combine(Server.MapPath("~/App_Data/Upload/"), fileDetail.Id + fileDetail.Extension);
            // file.SaveAs(path);


            return View();
        }
        
        public string C()
        {
            var id = Session["SessionID"];
            return id.ToString();
        }
    }
}