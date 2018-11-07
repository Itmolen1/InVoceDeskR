using InvoiceDiskLast.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;

namespace InvoiceDiskLast.Controllers
{
    [Authorize]
    [RoutePrefix("api/ApiCompanyStatus")]
    public class ApiCompanyStatusController : ApiController
    {       

        private DBEntities db = new DBEntities();
        // Exist: api/ApiCompanyStatus/id      
       

        public IHttpActionResult Getcompany()
        {
            return null;
        }

        [Route("{name:alpha}")]
        public object GetComapnyInfoes(string name)
        {
            IEnumerable<string> headerValues;
          
            string em = "";
            if (GlobalVeriables.WebApiClient.DefaultRequestHeaders.TryGetValues("name", out headerValues))
            {
                em = headerValues.FirstOrDefault();
               
            }

            bool Result= db.ComapnyInfoes.Count(e => e.UserName == em) > 0;
            object ob = new object();
            ob = 0;            
            if (Result == true)
            {
                ob = db.ComapnyInfoes.Where(u => u.UserName == em.ToString()).Select(c =>(int) c.CompanyID).FirstOrDefault();
   
                return ob;
                
            }
            return ob;
        }


       

    }
}
