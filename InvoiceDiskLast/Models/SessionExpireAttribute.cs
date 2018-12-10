using System;
using System.Collections.Generic;
using System.IO;
using System.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;


namespace InvoiceDiskLast.Models
{
    public class SessionExpireAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpContext ctx = HttpContext.Current;
            if (HttpContext.Current.Session["CompayID"] == null && HttpContext.Current.Session["ApiAccessToken"] == null)
            {
                filterContext.Result = new RedirectResult("~/Captcha/Index");
                return;
            }
            if (GlobalVeriables.WebApiClient.DefaultRequestHeaders.Authorization == null || GlobalVeriables.WebApiClient.DefaultRequestHeaders.Authorization.ToString() == "")
            {
                GlobalVeriables.WebApiClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", HttpContext.Current.Session["ApiAccessToken"].ToString());
            }

              clearFolder();

            base.OnActionExecuting(filterContext);
        }


        private void clearFolder()
        {
            try
            {
                var folderPath = System.Web.HttpContext.Current.Server.MapPath("/PDF/");

                if (Directory.Exists(folderPath))
                {
                    System.IO.DirectoryInfo di = new DirectoryInfo(folderPath);
                    foreach (FileInfo file in di.GetFiles())
                    {
                        file.Delete();
                    }
                    foreach (DirectoryInfo dir in di.GetDirectories())
                    {
                        dir.Delete(true);
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }          
        }
    }
}