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
    public class ApiCompanyStatusController : ApiController
    {       

        private DBEntities db = new DBEntities();
        // Exist: api/ApiCompanyStatus/id      
        public int GetComapnyInfoes(string id)
        {
            bool Result= db.ComapnyInfoes.Count(e => e.UserName == id) > 0;

            int companyId = 0;
            if (Result == true)
            {
                companyId = db.ComapnyInfoes.Where(u => u.UserName == id.ToString()).Select(c =>(int) c.CompanyID).FirstOrDefault();
   
                return companyId;
                
            }
            return companyId;
        }


       

    }
}
