using InvoiceDiskLast.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Mvc;

namespace InvoiceDiskLast.Controllers
{
    [SessionExpireAttribute]
    public class GenrateInvoiceController : ApiController
    {
        // GET: GenrateInvoice
       
        private DBEntities db = new DBEntities();


        [ResponseType(typeof(MvcPurchaseModel))]
        public IHttpActionResult GetQutationCount()
        {
            MvcPurchaseModel purchase = new MvcPurchaseModel();

             int puchaseId = db.PurchaseOrderTables.ToList().Count() + 1;
             purchase.PurchaseID = puchaseId.ToString();

            if (puchaseId == 0)
            {
                return Ok(purchase);
            }
            else
            {
                return Ok(purchase);
            }
        }

    }
}