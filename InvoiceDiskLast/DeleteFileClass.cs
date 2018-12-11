using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InvoiceDiskLast
{
    public class DeleteFileClass : ActionFilterAttribute
    {

        public string Attachment { get; set; }

        public string Roles { get; set; }

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            filterContext.HttpContext.Response.Flush();
            string something = (string)filterContext.HttpContext.Items["FilePath"];
            if (System.IO.File.Exists(something))
            {
                System.IO.File.Delete(something);
            }

        }
    }
}