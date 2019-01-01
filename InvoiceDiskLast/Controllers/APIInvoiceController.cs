using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using InvoiceDiskLast.Models;


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
    }
}
