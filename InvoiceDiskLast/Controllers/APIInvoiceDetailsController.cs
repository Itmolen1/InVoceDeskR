using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using InvoiceDiskLast.Models;
using System.Web.Http.Description;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace InvoiceDiskLast.Controllers
{

    public class APIInvoiceDetailsController : ApiController
    {
        private DBEntities db = new DBEntities();



        [Route("api/PostinvoiceDetails")]
        [ResponseType(typeof(QutationDetailsTable))]
        public IHttpActionResult PostQutationDetailsTable(InvoiceDetailsTable invoiceDetailsTable)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                invoiceDetailsTable = db.InvoiceDetailsTables.Add(invoiceDetailsTable);
                db.SaveChanges();

                return Ok();
            }

            catch (Exception ex)
            {
                return BadRequest();
            }
        }


        [Route("api/GetInvoiceDetails/{id:int}")]
        [ResponseType(typeof(List<InvoiceViewModel>))]
        public IHttpActionResult GetInvoiceDetailsTable(int id)
        {
            List<InvoiceViewModel> InvoiceDetaislList = new List<InvoiceViewModel>();
            if (id != 0)
            {
                try
                {
                    InvoiceDetaislList = db.InvoiceDetailsTables.ToList().Where(c => c.InvoiceId == id).Select(pd => new InvoiceViewModel
                    {

                        ItemId = pd.ItemId,
                        InvoiceID = pd.InvoiceId,
                        Rate = pd.Rate,
                        ServiceDate=pd.ServiceDate,
                        Quantity = pd.Quantity,
                        Vat = pd.Vat,
                        Total = pd.Total,
                        Type = pd.Type,
                        ItemName = pd.ProductTable.ProductName,
                        InvoiceDetailId = pd.InvoiceDetailId,
                        RowSubTotal = pd.RowSubTotal

                    }).ToList();                    

                    return Ok(InvoiceDetaislList);
                }
                catch (Exception ex)
                {
                    return BadRequest();
                }

            }
            return Ok(InvoiceDetaislList);

        }



        [Route("api/DeleteInvoiceDetails/{id:int}")]
        [ResponseType(typeof(InvoiceDetailsTable))]
        public IHttpActionResult DeleteInvoiceDtailsTable(int id)
        {
             InvoiceDetailsTable invoiceDetailsTable = db.InvoiceDetailsTables.Find(id);
            if (invoiceDetailsTable == null)
            {
                return NotFound();
            }

            db.InvoiceDetailsTables.Remove(invoiceDetailsTable);
            db.SaveChanges();

            return Ok(invoiceDetailsTable);
        }


        private bool InvoiceDetailsTableExists(int id)
        {
            return db.InvoiceDetailsTables.Count(e => e.InvoiceDetailId == id) > 0;
        }

        [Route("api/UpdateInvoiceDetails/{id:int}")]
        [ResponseType(typeof(InvoiceDetailsTable))]
        public IHttpActionResult PutQutationDetailsTable(int id, InvoiceDetailsTable InvoiceDetailsTable)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != InvoiceDetailsTable.InvoiceDetailId)
            {
                return BadRequest();
            }

             db.Entry(InvoiceDetailsTable).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
                return Ok(InvoiceDetailsTable);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InvoiceDetailsTableExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }


        }
    }
}
