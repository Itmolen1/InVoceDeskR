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
           WebApiClient.BaseAddress = new Uri("http://localhost:63861/api/");
          //WebApiClient.BaseAddress = new Uri("http://uurtjefactuur.nl/api/");
            WebApiClient.DefaultRequestHeaders.Clear();
            WebApiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
           // WebApiClient.DefaultRequestHeaders.Add("authorization", "Bearer <access_token>");

        }
    }
}