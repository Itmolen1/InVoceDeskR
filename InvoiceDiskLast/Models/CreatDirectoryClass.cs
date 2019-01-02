using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace InvoiceDiskLast.Models
{
    public static class CreatDirectoryClass
    {
        public static DirectoryViewModel _Directory = new DirectoryViewModel();
        static string Result = "";
        public static string CreateDirecotyFolder(int? refrenceId, string Name)
        {
            try
            {
                if (!System.IO.Directory.Exists(HttpContext.Current.Server.MapPath("/DirectoryFolder/")))
                {
                    System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath("/DirectoryFolder/"));
                }

                var cLIENTfOLDER = HttpContext.Current.Server.MapPath("/DirectoryFolder/");
                var FOLDERp = refrenceId + Name;
                string d = HttpContext.Current.Server.MapPath("/DirectoryFolder/" + FOLDERp);

                if (!System.IO.Directory.Exists(HttpContext.Current.Server.MapPath("/DirectoryFolder/" + FOLDERp)))
                {
                    System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath("/DirectoryFolder/" + FOLDERp));
                    Result = "/DirectoryFolder/" + FOLDERp + "/";
                    AddDirectory(refrenceId, Result);
                }
            }
            catch (Exception)
            {
                throw;
            }

            return Result;
        }
        
        public static void AddDirectory(int? Id, string Path)
        {
            try
            {
                DirectoryTable _Directory = new DirectoryTable();
                _Directory.IsActive = true;
                _Directory.DirectoryPath = Path;
                _Directory.RefrenceId = Id;
                HttpResponseMessage directoryResponse = GlobalVeriables.WebApiClient.PostAsJsonAsync("CreateDirecoty", _Directory).Result;
            }
            catch (Exception)
            {

                throw;
            }

        }
                
        public static bool UploadFileAndCreateDirectory(int Id, string Name, HttpFileCollectionBase files)
        {

            bool Result = true;

            var allowedExtensions = new string[] { ".doc", ".docx", ".pdf", ".jpg", ".png", ".JPEG", ".JFIF", ".PNG", ".txt" };

            try
            {
                if (Id != 0)
                {
                    DirectoryViewModel _Directory = new DirectoryViewModel();

                    HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("GetDirectory/" + Id).Result;
                    _Directory = response.Content.ReadAsAsync<DirectoryViewModel>().Result;


                    if (_Directory != null)
                    {
                        string folderPAth = HttpContext.Current.Server.MapPath(_Directory.DirectoryPath);

                        if (System.IO.Directory.Exists(folderPAth))
                        {
                            for (int i = 0; i < files.Count; i++)
                            {
                                HttpPostedFileBase file = files[i];
                                FileInfo fi = new FileInfo(file.FileName);
                                string ext = fi.Extension;
                                if (allowedExtensions.Contains(ext))
                                {
                                    string dateTime = DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();
                                    var FileName = file.FileName.Replace(ext, "");
                                    string FileNameSetting = FileName + dateTime + ext;
                                    file.SaveAs(folderPAth + FileNameSetting);
                                }
                            }
                        }
                    }
                    else
                    {
                        string folderPAth2 = CreateDirecotyFolder(Id, Name);

                        if (System.IO.Directory.Exists(HttpContext.Current.Server.MapPath(folderPAth2)))
                        {
                            for (int i = 0; i < files.Count; i++)
                            {
                                HttpPostedFileBase file = files[i];
                                FileInfo fi = new FileInfo(file.FileName);
                                string ext = fi.Extension;
                                if (allowedExtensions.Contains(ext))
                                {
                                    string dateTime = DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();
                                    var FileName = file.FileName.Replace(ext, "");
                                    string FileNameSetting = FileName + dateTime + ext;
                                    file.SaveAs(HttpContext.Current.Server.MapPath(folderPAth2) + FileNameSetting);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                Result = false;
                throw;
            }

            return Result;



        }

        public static bool DeleteFileFromPDF(string FislePath)
        {
            bool Result = true;
            try
            {


                var CompleterPath = HttpContext.Current.Server.MapPath("/PDF/" + FislePath);
                if (System.IO.File.Exists(HttpContext.Current.Server.MapPath(FislePath)))
                {
                    System.IO.File.Delete(HttpContext.Current.Server.MapPath(FislePath));
                    return Result = true;
                }


            }
            catch (Exception)
            {
                return Result = false;
                throw;
            }

            return Result;
        }
        
        public static List<DirectoryViewModel> GetFileDirectiory(int Id)
        {
            List<DirectoryViewModel> _object = new List<DirectoryViewModel>();
            string d = "";
            try
            {
                List<DirectoryViewModel> _DirectoryList = new List<DirectoryViewModel>();
                DirectoryViewModel _Directory = new DirectoryViewModel();

                HttpResponseMessage directory = GlobalVeriables.WebApiClient.GetAsync("GetDirectory/" + Id).Result;
                _Directory = directory.Content.ReadAsAsync<DirectoryViewModel>().Result;

                if (directory.StatusCode == System.Net.HttpStatusCode.OK)
                {

                    d = _Directory.DirectoryPath.ToString();
                    string F = _Directory.DirectoryPath.ToString();
                    d = d.Substring(17);
                    if (_Directory.DirectoryPath != null)
                    {
                        DirectoryInfo dir = new DirectoryInfo(HttpContext.Current.Server.MapPath(_Directory.DirectoryPath));
                        FileInfo[] info = dir.GetFiles("*.*");

                        foreach (FileInfo f in info)
                        {
                            string Name = f.Name;
                            _object.Add(new DirectoryViewModel { DirectoryPath = f.Name, FileFolderPathe = F });
                        }
                    }
                }

            }
            catch (Exception)
            {

                throw;
            }

            return _object;
        }
        
        public static DirectoryViewModel GetDiretoryPathById(int Id)
        {
            try
            {
                HttpResponseMessage directory = GlobalVeriables.WebApiClient.GetAsync("GetDirectory/" + Id).Result;
                _Directory = directory.Content.ReadAsAsync<DirectoryViewModel>().Result;
            }
            catch (Exception)
            {
                _Directory = null;
                throw;
            }

            return _Directory;
        }


        public static bool Delete(int Id, string FileName)
        {
            bool Issuccess = true;

            try
            {
                _Directory = GetDiretoryPathById((int)Id);

                if (_Directory != null)
                {

                    string FilePath = HttpContext.Current.Server.MapPath(_Directory.DirectoryPath + FileName);
                    if (System.IO.File.Exists(FilePath))
                    {
                        System.IO.File.Delete(HttpContext.Current.Server.MapPath(_Directory.DirectoryPath + FileName));
                    }
                }

            }
            catch (Exception)
            {

                throw;
            }

            return Issuccess;
        }

        public static string UploadFileToDirectory(HttpPostedFileBase files, int? Id, string Name)
        {

            string DirevtoryPath = "";

            DirectoryViewModel _Directory12 = new DirectoryViewModel();
            try
            {
                _Directory = GetDiretoryPathById((int)Id);

                if (_Directory == null)
                {
                    string p = CreateDirecotyFolder(Id, Name);
                    _Directory12.DirectoryPath = p;
                }

                if (_Directory12 != null)
                {

                    if (files.ContentLength != 0)
                    {
                        if (_Directory12.DirectoryPath != null)
                        {
                            DirevtoryPath = HttpContext.Current.Server.MapPath(_Directory12.DirectoryPath);
                        }
                        else
                        {
                            DirevtoryPath = HttpContext.Current.Server.MapPath(_Directory.DirectoryPath);
                        }

                        FileInfo fi = new FileInfo(files.FileName);
                        string ext = fi.Extension;
                        string dateTime = DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();
                        var FileName = files.FileName.Replace(ext, "");
                        string FileNameSetting = FileName + dateTime + ext;
                        files.SaveAs(DirevtoryPath + FileNameSetting);
                        DirevtoryPath = FileNameSetting;
                    }
                }


            }
            catch (Exception)
            {

                throw;
            }

            return DirevtoryPath;
        }


        //upload file and create directory for Create and Update [4 places (Quotation,Invoice,Purchase,Bill)]
        public static string UploadFileToDirectoryCommon(int? Id, string FileName, HttpPostedFileWrapper[] file)
        {
            string FilePath12 = "";

            try
            {
                var allowedExtensions = new string[] { ".doc", ".docx", ".pdf", ".jpg", ".png", ".JPEG", ".JFIF", ".PNG", ".txt" };
                string FilePath = CreatDirectoryClass.CreateDirecotyFolder(Id, FileName);

                string fap = HttpContext.Current.Server.MapPath(FilePath);

                for (int i = 0; i < file.Count(); i++)
                {
                    HttpPostedFileBase f = file[i];
                    FileInfo fi = new FileInfo(f.FileName);
                    string ext = fi.Extension;
                    if (allowedExtensions.Contains(ext))
                    {
                        string dateTime = DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();

                        string FileName1 = f.FileName.Replace(ext, "");

                        string FileNameSetting = FileName1 + dateTime + ext;

                        f.SaveAs(fap + FileNameSetting);

                        return FilePath12 = FileNameSetting;
                    }
                }
            }

            catch (Exception)
            {

                throw;
            }

            return FilePath12;
        }
        
        public static string UploadToPDFCommon(HttpPostedFileBase file)
        {
            string FilePAth = "";

            try
            {
                if (!System.IO.Directory.Exists(HttpContext.Current.Server.MapPath("/PDF/")))
                {
                    Directory.CreateDirectory(HttpContext.Current.Server.MapPath("/PDF/"));
                }
                string DirevtoryPath = HttpContext.Current.Server.MapPath("/PDF/");

                if (file.ContentLength != 0)
                {
                    FileInfo fi = new FileInfo(file.FileName);
                    string ext = fi.Extension;
                    string dateTime = DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();
                    var FileName = file.FileName.Replace(ext, "");
                    string FileNameSetting = FileName + dateTime + ext;
                    file.SaveAs(DirevtoryPath + FileNameSetting);
                    return FileNameSetting;
                }
            }
            catch (Exception)
            {

                throw;
            }

            return FilePAth;
        }

    }
}