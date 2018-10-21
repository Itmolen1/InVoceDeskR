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
    public class APIPurchaseDetailController : ApiController
    {
        private DBEntities db = new DBEntities();

        public IQueryable<PurchaseOrderDetailsTable> GetQutationDetailsTables()
        {
            return db.PurchaseOrderDetailsTables;
        }

        // GET: api/APIQutationDetail/5
        [ResponseType(typeof(List<MvcPurchaseViewModel>))]
        public IHttpActionResult GetPurchaseDetailsTable(int id)
        {

            IEnumerable<string> HeaderValue;
            var PDID = "";
            if (GlobalVeriables.WebApiClient.DefaultRequestHeaders.TryGetValues("PDID", out HeaderValue))
            {
                PDID = HeaderValue.FirstOrDefault();
            }
            int PDIDs = (PDID != "" ? Convert.ToInt32(PDID) : 0);

            if (PDIDs != 0)
            {
                var query = db.PurchaseOrderDetailsTables.ToList().Where(c => c.PurchaseOrderDetailsId == PDIDs).ToList();

                return Ok(query);
            }
            else
            {

                // List<MVCQutationViewModel> qutationDetailsTable = new List<MVCQutationViewModel>();
                //   var obj = db.QutationDetailsTables.ToList().Where(c => c.QutationID == QviewModel.QutationID).ToList();

                var query = (from pd in db.PurchaseOrderDetailsTables
                             join p in db.ProductTables on pd.PurchaseItemId equals p.ProductId
                             where pd.PurchaseId == id
                             select new MvcPurchaseViewModel
                             {
                                 PurchaseItemId = pd.PurchaseItemId,
                                // PurchaseID = pd.PurchaseID,
                                 PurchaseItemRate = pd.PurchaseItemRate,
                                 PurchaseQuantity = pd.PurchaseQuantity,
                                 PurchaseVatPercentage = pd.PurchaseVatPercentage,
                                 PurchaseItemName = p.ProductName,
                                 PurchaseTotal = pd.PurchaseTotal,
                                 PurchaseOrderDetailsId = pd.PurchaseOrderDetailsId
                             }).ToList();


                if (query == null)
                {
                    return NotFound();
                }
                return Ok(query);
            }

        }



        // GET: api/APIQutationDetail
        [ResponseType(typeof(List<MVCQutationDetailsModel>))]
        public IHttpActionResult GetQutationDetailsTable(int id, string id1)
        {

            var query = db.QutationDetailsTables.ToList().Where(c => c.QutationID == id).Select(c => new MVCQutationDetailsModel
            {

                QutationDetailId = c.QutationDetailId,
                Quantity = c.Quantity,
                ItemId = c.ItemId,
                QutationID = c.QutationID,
                Rate = c.Rate,
                Total = c.Total,

            }).ToList();

            if (query == null)
            {
                return NotFound();
            }
            return Ok(query);
        }



        // PUT: api/APIQutationDetail/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutQutationDetailsTable(int id, QutationDetailsTable qutationDetailsTable)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != qutationDetailsTable.QutationDetailId)
            {
                return BadRequest();
            }

            db.Entry(qutationDetailsTable).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
                return StatusCode(HttpStatusCode.OK);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!QutationDetailsTableExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }


        }

        // POST: api/APIQutationDetail
        [ResponseType(typeof(QutationDetailsTable))]
        public IHttpActionResult PostQutationDetailsTable(QutationDetailsTable qutationDetailsTable)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.QutationDetailsTables.Add(qutationDetailsTable);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = qutationDetailsTable.QutationDetailId }, qutationDetailsTable);
        }

        // DELETE: api/APIQutationDetail/5
        [ResponseType(typeof(QutationDetailsTable))]
        public IHttpActionResult DeleteQutationDetailsTable(int id)
        {
            QutationDetailsTable qutationDetailsTable = db.QutationDetailsTables.Find(id);
            if (qutationDetailsTable == null)
            {
                return NotFound();
            }

            db.QutationDetailsTables.Remove(qutationDetailsTable);
            db.SaveChanges();

            return Ok(qutationDetailsTable);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool QutationDetailsTableExists(int id)
        {
            return db.QutationDetailsTables.Count(e => e.QutationDetailId == id) > 0;
        }
    }
}

   