using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using InvoiceDiskLast.Models;

namespace InvoiceDiskLast.Controllers
{
    public class APIErrorLogController : ApiController
    {

        [Route("api/PostErrorLog")]
       
        public IHttpActionResult PostErrorLog(ExceptionLogger exceptionLogger)
        {
            DBEntities db = new DBEntities();

            try
            {
                db.ExceptionLoggers.Add(exceptionLogger);
                db.SaveChanges();

                return Ok(exceptionLogger);
            }
            catch(Exception ex)
            {
                return BadRequest();
            }
        }

    }
}
