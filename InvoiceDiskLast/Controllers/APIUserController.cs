using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using InvoiceDiskLast.Models;

namespace InvoiceDiskLast.Controllers
{
    public class APIUserController : ApiController
    {
        DBEntities db = new DBEntities();

        [Route("api/GetUserbyEmail/{username:alpha}")]
        public IHttpActionResult GetUserByEmail(string username)
        {
           
            try
            {
                var RegEmailId = (from u in db.AspNetUsers
                                  where u.UserName.ToUpper() == username.ToUpper()
                                  select new { username }).FirstOrDefault();

                bool status;
                if (RegEmailId != null)
                {
                    return Ok(RegEmailId);
                    //Already registered  
                  
                }
                else
                {
                    //Available to use  
                    return BadRequest();
                }

                //return status;
                //string user = db.AspNetUsers.Where(x => x.UserName == username).Select(u => u.UserName).FirstOrDefault();

                //    if (user == username)
                //    {
                //        return Ok(user);

                //    }
                //    else
                //    {
                //        return BadRequest();
                //    }
            }

            catch (Exception ex)
            {
                return BadRequest();
            }
        
        }
    }
}

