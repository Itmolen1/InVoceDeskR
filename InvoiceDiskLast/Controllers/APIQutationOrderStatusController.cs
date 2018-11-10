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
    public class APIQutationOrderStatusController : ApiController
    {
        private DBEntities db = new DBEntities();

        [ResponseType(typeof(QutationOrderStatusTable))]
        [Route("api/postOrderStatus")]
        public IHttpActionResult PostProductTable(QutationOrderStatusTable OrderStausTable)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                db.QutationOrderStatusTables.Add(OrderStausTable);
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
