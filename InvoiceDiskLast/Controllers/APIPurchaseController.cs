using InvoiceDiskLast.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace InvoiceDiskLast.Controllers
{
    public class APIPurchaseController : ApiController
    {
        private DBEntities db = new DBEntities();


    
        //get list
        public IHttpActionResult GetPurchaseOrder()
        {
            object ob = new object();
            try
            {

                IEnumerable<string> headerValues;
                //var DBLIST = "";
                var IDS = "";
                if (GlobalVeriables.WebApiClient.DefaultRequestHeaders.TryGetValues("CompayID", out headerValues))
                {
                    IDS = headerValues.FirstOrDefault();
                }
                int id = Convert.ToInt32(IDS);

                ob = db.PurchaseOrderTables.Where(p=>p.CompanyId== id).ToList().Select(p => new MvcPurchaseModel
                {
                    PurchaseOrderID = Convert.ToInt32(p.PurchaseOrderID),
                    PurchaseID = p.PurchaseID,
                    PurchaseDate = p.PurchaseDate,
                    PurchaseDueDate = p.PurchaseDueDate,
                    PurchaseRefNumber = p.PurchaseRefNumber,
                    PurchaseSubTotal = p.PurchaseSubTotal,
                    PurchaseDiscountPercenteage = p.PurchaseDiscountPercenteage,
                    PurchaseDiscountAmount = p.PurchaseOrderID,
                    PurchaseVatPercentage = p.PurchaseVatPercentage,
                    PurchaseTotoalAmount = p.PurchaseTotoalAmount,
                    PurchaseVenderNote = p.PurchaseVenderNote,
                    Status = p.Status,
                    CompanyId = p.CompanyId,
                    UserId = p.UserId,
                    AddedDate = p.AddedDate,
                }).ToList();
            }
            catch (Exception ex)
            {

            }

            return Ok(ob);

        }
        //// GET: api/APIQutation/5
        //[ResponseType(typeof(PurchaseOrderTable))]
        //public IHttpActionResult GetPurchaseTable(int id)
        //{
        //    PurchaseOrderTable PurchaseTable = db.PurchaseOrderTables.Find(id);
        //    if (PurchaseTable == null)
        //    {
        //        return NotFound();
        //    }

        //    return Ok(PurchaseTable);
        //}

        // PUT: api/APIQutation/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutPurchaseTable(int id, PurchaseOrderTable PurchaseTable)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != PurchaseTable.PurchaseOrderID)
            {
                return BadRequest();
            }

            db.Entry(PurchaseTable).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
                return StatusCode(HttpStatusCode.OK);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PurchaseTableExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }


        }


        private bool PurchaseTableExists(int id)
        {
            return db.PurchaseOrderTables.Count(e => e.PurchaseOrderID == id) > 0;
        }


        // POST: api/APIQutation
        [ResponseType(typeof(PurchaseOrderTable))]
        public HttpResponseMessage PostQutationTable([FromBody] PurchaseOrderTable Purchasetable)
        {
            using (DBEntities entities = new DBEntities())
            {
                entities.PurchaseOrderTables.Add(Purchasetable);
                entities.SaveChanges();

                var massage = Request.CreateResponse(HttpStatusCode.Created, Purchasetable);
                massage.Headers.Location = new Uri(Request.RequestUri + Purchasetable.PurchaseOrderID.ToString());
                massage.Content.Headers.Add("idd", Purchasetable.PurchaseOrderID.ToString());
                massage.RequestMessage.Headers.Add("idd", Purchasetable.PurchaseOrderID.ToString());
                return massage;

            }
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


        [ResponseType(typeof(PurchaseOrderTable))]
        public IHttpActionResult DeleteQutationTable(int id)
        {
            PurchaseOrderTable PurchaseTable = db.PurchaseOrderTables.Find(id);
            if (PurchaseTable == null)
            {
                return NotFound();
            }
            db.PurchaseOrderTables.Remove(PurchaseTable);
            db.SaveChanges();

            return Ok(PurchaseTable);
        }

    }
}
