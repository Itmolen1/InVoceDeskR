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
        public IHttpActionResult PostComapnyInfoes(UserInfo User)
        {
            //object ob = new object();
            UserModel  com = new UserModel();
            //ob = 0;
            try
            {
                bool Result = db.CompanyUsers.Count(e => e.UserID == User.username) > 0;

                if (Result == true)
                {
                    //ob = db.CompanyUsers.Where(u => u.UserID == User.username.ToString())
                    //    .Select(c => (int)c.CompanyID).FirstOrDefault();

                    com = db.UserTables.Where(u => u.Username == User.username.ToString())
                        .Select(c => new UserModel
                        {
                            UserId = c.UserId,
                            CompanyId = c.CompanyId,
                        }).FirstOrDefault();

                    return Ok(com);

                }
            }
            catch (Exception ex)
            {
                return NotFound();
            }
            return  NotFound();
        }
    }
}
