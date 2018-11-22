using InvoiceDiskLast.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace InvoiceDiskLast.Controllers
{
    public class APIReportController : ApiController
    {
        private DBEntities db = new DBEntities();

        [Route("api/GetJournal")]
        public IHttpActionResult GetJournalReport()
        {
            try
            {           
                var Journal = db.Database.SqlQuery<TransactionModel>("exec Sp_GetJournal").ToList<TransactionModel>();
                return Ok(Journal);
            }
            catch (Exception)
            {
                return NotFound();
                
            }

            return Ok();
        }

    }
}
