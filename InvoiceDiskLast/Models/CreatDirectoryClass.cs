using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InvoiceDiskLast.Models
{
    public static class CreatDirectoryClass
    {

        static string Result = "";

        public static string CreateDirecotyFolder(int refrenceId, string Name)
        {
            try
            {
                if (!System.IO.Directory.Exists(HttpContext.Current.Server.MapPath("/DirectoryFolder/")))
                {
                    System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath("/DirectoryFolder/"));
                }

                var cLIENTfOLDER = HttpContext.Current.Server.MapPath("/DirectoryFolder/");
                var FOLDERp = refrenceId + Name;

                if (!System.IO.Directory.Exists(HttpContext.Current.Server.MapPath("/DirectoryFolder/" + FOLDERp)))
                {
                    System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath("/DirectoryFolder/" + FOLDERp));

                    Result ="/DirectoryFolder/" + FOLDERp+"/";
                }
            }
            catch (Exception)
            {

                throw;
            }


            return Result;
        }


    }
}