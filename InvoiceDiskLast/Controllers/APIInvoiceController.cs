using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using InvoiceDiskLast.Models;
using System.Web.Http.Description;
using System.Data.Entity;

namespace InvoiceDiskLast.Controllers
{
    public class APIInvoiceController : ApiController
    {
        private DBEntities db = new DBEntities();

        [Route("api/GetInvoiceTable/{CompayID:int}")]
        public IHttpActionResult GetInvoiceTables(int CompayID)
        {
            List<InvoiceViewModel> invoiceViewModel = new List<InvoiceViewModel>();


            invoiceViewModel = db.InvoiceTables.Where(x => x.CompanyId == CompayID).Select(c => new InvoiceViewModel
            {

                InvoiceID = c.InvoiceID,
                Invoice_ID = c.Invoice_ID,
                RefNumber = c.RefNumber,
                InvoiceDate = c.InvoiceDate,
                InvoiceDueDate = c.InvoiceDueDate,
                Vat = c.TotalVat6 + c.TotalVat21,
                TotalAmount = c.TotalAmount,
                Status = (c.Status).Trim(),
                UserName = (c.UserTable.UserFname + " " + c.UserTable.UserLname != "" ? c.UserTable.UserFname + " " + c.UserTable.UserLname : c.UserTable.Username),
                CustomerName = c.ContactsTable.ContactName

            }).ToList();

            return Ok(invoiceViewModel);


        }

        [Route("api/GetInvoiceCount")]
        public IHttpActionResult GetInvoiceCount()
        {
            MVCInvoiceModel Invoice = new MVCInvoiceModel();

            int InvoiceId = db.InvoiceTables.ToList().Count() + 1;
            Invoice.Invoice_ID = InvoiceId.ToString();

            if (InvoiceId == 0)
            {
                return Ok(Invoice);
            }
            else
            {
                return Ok(Invoice);
            }
        }


        [Route("api/PostInvoice")]
        [ResponseType(typeof(QutationTable))]
        public IHttpActionResult PostInvoice([FromBody] InvoiceTable invoiceTable)
        {
            using (DBEntities entities = new DBEntities())
            {
               
                try
                {
                    invoiceTable = entities.InvoiceTables.Add(invoiceTable);

                    entities.SaveChanges();

                    return Ok(invoiceTable);
                }
                catch(Exception ex)
                {
                    return BadRequest();
                }

            }
        }


        [Route("api/GetInvoiceById/{id:int}")]
        [ResponseType(typeof(QutationTable))]
        public IHttpActionResult GetInvoiceTable(int id)
        {
            MVCInvoiceModel invoiceModel = new MVCInvoiceModel();

            invoiceModel = db.InvoiceTables.Where(q => q.InvoiceID == id).Select(c => new MVCInvoiceModel
            {
                InvoiceID = c.InvoiceID,
                Invoice_ID = c.Invoice_ID,
                RefNumber = c.RefNumber,
                InvoiceDate = c.InvoiceDate,
                InvoiceDueDate = c.InvoiceDueDate,
                SubTotal = c.SubTotal,
                TotalVat6 = c.TotalVat6,
                TotalVat21 = c.TotalVat21,
                DiscountAmount = c.DiscountAmount,
                TotalAmount = c.TotalAmount,
                CustomerNote = c.CustomerNote,
                Status = c.Status,
                UserId = c.UserId,
                CompanyId = c.CompanyId,
                ContactId = c.ContactId,
                Type = c.Type,

            }).FirstOrDefault();


            if (invoiceModel == null)
            {
                return NotFound();
            }

            return Ok(invoiceModel);
        }

        [Route("api/UpdateInvoice/{id:int}")]
        [ResponseType(typeof(void))]
        public IHttpActionResult PutInvoiceTable(int id, InvoiceTable invoiceTable)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != invoiceTable.InvoiceID)
            {
                return BadRequest();
            }

            db.Entry(invoiceTable).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
                return Ok(invoiceTable);
            }
            catch (Exception ex)
            {

                return BadRequest();
            }


        }
    }
}
