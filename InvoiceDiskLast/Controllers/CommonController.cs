using InvoiceDiskLast.MISC;
using InvoiceDiskLast.Models;
using Logger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace InvoiceDiskLast.Controllers
{
    [RouteNotFoundAttribute]
    [SessionExpireAttribute]
    public class CommonController : Controller
    {
        private Ilog _iLog;
        // GET: Common
        public ActionResult Index()
        {
            return View();
        }
        public CommonController()
        {
            _iLog = Log.GetInstance;
        }

        [NonAction]
        public static double CalculateVat(float vat, float Total)
        {
            Double Result = 0;
            try
            {
                Result = Convert.ToDouble((Total / 100) * vat);

                return Result;
            }
            catch (Exception ex)
            {
                return Result;
            }
        }

        public ActionResult DeleteFile(string FileName)
        {
            try
            {
                if (CreatDirectoryClass.DeleteFileFromPDF(FileName))
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
                throw;
            }


        }

        [HttpPost]
        public ActionResult UploadFileToPDF()
        {
            try
            {
                HttpFileCollectionBase files = Request.Files;
                for (int i = 0; i < files.Count; i++)
                {
                    HttpPostedFileBase file = files[i];

                    string FileName = CreatDirectoryClass.UploadToPDFCommon(file);

                    return Json(FileName, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {

                throw;
            }

            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public ActionResult ViewDirecory(int Id, string DName)
        {
            string d = "";
            try
            {
                List<DirectoryViewModel> _DirectoryList = new List<DirectoryViewModel>();
                DirectoryViewModel _Directory = new DirectoryViewModel();
                HttpResponseMessage directory = GlobalVeriables.WebApiClient.GetAsync("GetDirectory/" + Id + "/" + DName).Result;
                _Directory = directory.Content.ReadAsAsync<DirectoryViewModel>().Result;

                if (directory.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    CreatDirectoryClass.CreateDirecotyFolder(Id, DName, DName);
                    directory = GlobalVeriables.WebApiClient.GetAsync("GetDirectory/" + Id + "/" + DName).Result;
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

            return View("~/Views/Shared/PartialViews/ViewDirectory.cshtml");
        }

        [HttpPost]
        public JsonResult ProductPricebyId(int ProductId)
        {

            HttpResponseMessage responsep = GlobalVeriables.WebApiClient.GetAsync("APIProductByProductID/" + ProductId.ToString()).Result;
            MVCProductModel productModel = responsep.Content.ReadAsAsync<MVCProductModel>().Result;

            decimal price =Convert.ToDecimal(productModel.SalePrice);
            return Json(price, JsonRequestBehavior.AllowGet);
        }

        public static int GetAcccountId(TransactionModel _Model)
        {
            try
            {
                HttpResponseMessage response = GlobalVeriables.WebApiClient.PostAsJsonAsync("POSTransactionModel/", _Model).Result;
                TransactionModel _Transaction = response.Content.ReadAsAsync<TransactionModel>().Result;
                return _Transaction.Id;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static bool DeleteFromTransactionTableByRefrenceId(int RefId)
        {
            bool Result = true;
            try
            {
                HttpResponseMessage deleteresponse = GlobalVeriables.WebApiClient.GetAsync("GetDeleteRecordFromTransactionbyrefId/" + RefId).Result;
                if (deleteresponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Result= true;
                }
                else
                {
                    return Result=false;
                }              
            }
            catch (Exception)
            {
                Result = false;
                throw;
            }
            return Result;
        }
    }
}