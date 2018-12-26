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



        public static bool DeleteFile(string FilePath)
        {
            bool Result = true;
            try
            {


                var CompleterPath = HttpContext.Current.Server.MapPath("/DirectoryFoleder/" + FilePath);
                if (System.IO.File.Exists(HttpContext.Current.Server.MapPath(FilePath)))
                {
                    System.IO.File.Delete(HttpContext.Current.Server.MapPath(FilePath));
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
                    //directory = GlobalVeriables.WebApiClient.GetAsync("GetDirectory/" + Id).Result;
                    //_Directory = directory.Content.ReadAsAsync<DirectoryViewModel>().Result;

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




    }
}