﻿using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace InvoiceDiskLast.Models
{
    public class SessionExpireAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpContext ctx = HttpContext.Current;
            if (HttpContext.Current.Session["CompayID"] == null && HttpContext.Current.Session["ApiAccessToken"] ==null)
            {
                filterContext.Result = new RedirectResult("~/Captcha/Index");
                return;
            }
            base.OnActionExecuting(filterContext);
        }

    }
}