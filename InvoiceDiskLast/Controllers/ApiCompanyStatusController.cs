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
    public class ApiCompanyStatusController : ApiController
    {       

        private DBEntities db = new DBEntities();
        // Exist: api/ApiCompanyStatus/id 

        public IHttpActionResult Getcompany()
        {
            return null;
        }

        [Route("api/CompanyExist")]
        public object PostComapnyInfoes(UserInfo User)
        {
            object ob = new object();
            ob = 0;
            try
            {
                bool Result = db.ComapnyInfoes.Count(e => e.UserName == User.username) > 0;                
               
                if (Result == true)
                {
                    ob = db.CompanyUsers.Where(u => u.UserId == User.username.ToString()).Select(c => (int)c.CompanyId).FirstOrDefault();

                    return ob;

                }
            }
            catch (Exception ex)
            {
                return ob;
            }
            return ob;
        }
    }
}
