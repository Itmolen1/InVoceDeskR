using InvoiceDiskLast.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace InvoiceDiskLast.Controllers
{
    public class APIBillDetailController : ApiController
    {
        private DBEntities db = new DBEntities();

        // POST: api/APIQutationDetail
        [ResponseType(typeof(BillDetailTable))]
        public IHttpActionResult PostPurchaseDetailsTable(BillDetailTable billdetailmodel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                  return BadRequest(ModelState);
                }

                db.BillDetailTables.Add(billdetailmodel);
                db.SaveChanges();

                return Ok();
            }
            catch (Exception ex)
            {

                return BadRequest();

            }

        }
    }
}
