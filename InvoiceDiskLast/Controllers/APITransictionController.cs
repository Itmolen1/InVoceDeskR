using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using InvoiceDiskLast.Models;

namespace InvoiceDiskLast.Controllers
{
    public class APITransictionController : ApiController
    {
        DBEntities db = new DBEntities();

        [Route("api/GetTransiction")]
        public IHttpActionResult GetTransition(int CompanyId)
        {
            List<AccountTransictionTable> TransictionList = new List<AccountTransictionTable>();
            try
            {
                TransictionList = db.AccountTransictionTables.Where(x => x.FK_CompanyId == CompanyId).Select(c => new AccountTransictionTable {

                    TransictionId = c.TransictionId,
                    TransictionNumber = c.TransictionNumber,
                    TransictionDate = c.TransictionDate,
                    TransictionRefrenceId = c.TransictionRefrenceId,
                    TransictionType = c.TransictionType


                }).ToList();

                return Ok(TransictionList);
            }
            catch(Exception)
            {

            }
            return Ok(TransictionList);
        }
    }
}
