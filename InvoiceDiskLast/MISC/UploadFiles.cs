using InvoiceDiskLast.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace InvoiceDiskLast.MISC
{
    public class UploadFiles
    {
        public static  bool UploadFile(int? Id, string FileName, HttpPostedFileWrapper[] file)
        {
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

                    }
                }
            }

            catch (Exception)
            {

                throw;
            }

            return true;
        }
    }
}