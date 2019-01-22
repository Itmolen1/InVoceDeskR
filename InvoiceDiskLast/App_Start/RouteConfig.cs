﻿using InvoiceDiskLast.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace InvoiceDiskLast
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {

          
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{*allaspx}", new { allaspx = @".*(CrystalImageHandler).*" });
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "landing", action = "Index", id = UrlParameter.Optional }
            );
           
         
            //routes.MapRoute(
            //  name: "Bill",
            //  url: "{controller}/{action}/{id}",
            //  defaults: new { controller = "Bills", action = "Create", id = UrlParameter.Optional }
            //  );


        }
    }
}
