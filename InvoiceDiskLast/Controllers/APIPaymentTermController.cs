using InvoiceDiskLast.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace InvoiceDiskLast.Controllers
{
    public class APIPaymentTermController : ApiController
    {
        private DBEntities db = new DBEntities();


        [Route("api/GetPaymentTerm/{CompanyID:int}")]
        public IHttpActionResult GetPaymentTerm(int CompanyID)
        {
            try
            {
                List<PaymentTermModel> model = db.PaymentTermTables.Where(C => C.FK_CompanyID == CompanyID).Select(c => new PaymentTermModel
                {
                    PayementTremId = c.PayementTremId,
                    PaymentTerm = c.PaymentTerm,
                }).ToList();

                return Ok(model);
            }
            catch (Exception)
            {
                return NotFound();
                throw;
            }
        }


    }
}
