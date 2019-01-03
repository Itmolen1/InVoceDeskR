using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using InvoiceDiskLast.Models;
using System.Web.Http.Description;

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
        [ResponseType(typeof(List<MVCQutationViewModel>))]
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
                        Quantity = pd.Quantity,
                        Vat = pd.Vat,
                        Total = pd.Total,
                        Type = pd.Type,
                        ItemName = pd.ProductTable.ProductName,
                        InvoiceDetailId = pd.InvoiceDetailId
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

    }
}
