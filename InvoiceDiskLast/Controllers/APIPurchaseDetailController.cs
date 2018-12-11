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
                var query = db.PurchaseOrderDetailsTables.ToList().Where(c => c.PurchaseId == PDIDs).Select(pd => new MvcPurchaseViewModel
                {

                    PurchaseOrderDetailsId =(int)pd.PurchaseOrderDetailsId,
                    PurchaseItemId = pd.PurchaseOrderDetailsId,
                    PurchaseDescription = pd.PurchaseDescription,
                    PurchaseQuantity = pd.PurchaseQuantity,
                    PurchaseItemRate = pd.PurchaseItemRate,
                    PurchaseTotal = pd.PurchaseTotal,
                    PurchaseVatPercentage = pd.PurchaseVatPercentage,
                    PurchaseId = pd.PurchaseId,
                    Purchase_ID = pd.PurchaseOrderTable.PurchaseID
                    
                }).ToList();

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
                                 PurchaseId = pd.PurchaseId,
                                 PurchaseItemRate = pd.PurchaseItemRate,
                                 PurchaseQuantity = pd.PurchaseQuantity,
                                 PurchaseVatPercentage = pd.PurchaseVatPercentage,
                                 PurchaseItemName = p.ProductName,
                                 PurchaseTotal = pd.PurchaseTotal,
                                 PurchaseOrderID = (int)pd.PurchaseId,

                                 PurchaseOrderDetailsId =(int)pd.PurchaseOrderDetailsId
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




        [ResponseType(typeof(void))]
        public IHttpActionResult PutPurchaseDetailsTable(int id, PurchaseOrderDetailsTable purchasedetail)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != purchasedetail.PurchaseOrderDetailsId)
            {
                return BadRequest();
            }
            db.Entry(purchasedetail).State = EntityState.Modified;
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

        // POST: api/APIQutationDetail
        [ResponseType(typeof(PurchaseOrderDetailsTable))]
        public IHttpActionResult PostPurchaseDetailsTable(PurchaseOrderDetailsTable purchaseOrderDetail)
        {
            try
            {

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                db.PurchaseOrderDetailsTables.Add(purchaseOrderDetail);
                db.SaveChanges();

                return Ok();
            }
            catch (Exception ex)
            {
                throw ex;
                return BadRequest();

            }

        }

        // DELETE: api/APIQutationDetail/5
        [ResponseType(typeof(PurchaseOrderDetailsTable))]
        public IHttpActionResult DeleteQutationDetailsTable(int id)
        {
            PurchaseOrderDetailsTable purchaseOrderDetailsTable = db.PurchaseOrderDetailsTables.Find(id);
            if (purchaseOrderDetailsTable == null)
            {
                return NotFound();
            }

            db.PurchaseOrderDetailsTables.Remove(purchaseOrderDetailsTable);
            db.SaveChanges();

            return Ok(purchaseOrderDetailsTable);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool PurchaseTableExists(int id)
        {
            return db.PurchaseOrderDetailsTables.Count(P => P.PurchaseOrderDetailsId == id) > 0;
        }
    }
}

