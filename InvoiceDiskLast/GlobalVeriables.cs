using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;

namespace InvoiceDiskLast
{
    public class GlobalVeriables
    {
        public static HttpClient WebApiClient = new HttpClient();
        static GlobalVeriables()
        {
            string url = System.Configuration.ConfigurationManager.AppSettings["url"];
            WebApiClient.BaseAddress = new Uri(url+"/api/");

            //WebApiClient.BaseAddress = new Uri("http://uurtjefactuur.nl/api/");
          
            WebApiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
          

        }
    }
}