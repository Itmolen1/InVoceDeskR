using InvoiceDiskLast.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace InvoiceDiskLast.Controllers
{
    public class APIBillDetailController : ApiController
    {
        private DBEntities db = new DBEntities();

        // POST: api/APIQutationDetail
        [Route("Api/AddBillDetail")]
        [ResponseType(typeof(BillDetailTable))]
        public IHttpActionResult PostBillDetail(BillDetailTable billdetailmodel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                db.BillDetailTables.Add(billdetailmodel);
                db.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest();

            }
        }






        [Route("Api/GetBillDetailTablebyId/{id:int}")]
        [ResponseType(typeof(List<BillDetailViewModel>))]
        public IHttpActionResult GetBillDetail(int id)
        {
            IEnumerable<string> HeaderValue;
            var QTID = "";
            if (GlobalVeriables.WebApiClient.DefaultRequestHeaders.TryGetValues("QTID", out HeaderValue))
            {
                QTID = HeaderValue.FirstOrDefault();
            }
            int QTIDs = (QTID != "" ? Convert.ToInt32(QTID) : 0);

            if (QTIDs != 0)
            {
                var query = db.BillDetailTables.ToList().Where(c => c.BillDetailId == QTIDs).Select(pd => new
                {
                    BillID = pd.BillID,
                    BillDetailId = pd.BillDetailId,
                    ItemId = pd.ItemId,
                    Rate = pd.Rate,
                    Description = pd.Description,
                    Quantity = pd.Quantity,
                    Vat = pd.Vat,
                    Total = pd.Total,
                    Type = pd.Type,
                    RowSubTotal = pd.RowSubTotal,
                }).ToList();


                return Ok(query);
            }
            else
            {               

                var query = (from pd in db.BillDetailTables
                             join p in db.ProductTables on pd.ItemId equals p.ProductId
                             where pd.BillID == id
                             select new BillDetailViewModel
                             {
                                 ServiceDate = pd.ServiceDate,
                                 BilID = pd.BillID,

                                 BillDetailId = pd.BillDetailId,
                                 ItemId = pd.ItemId,
                                 Rate = pd.Rate,
                                 Description = pd.Description,
                                 Quantity = pd.Quantity,
                                 Vat = pd.Vat,
                                 Total = pd.Total,
                                 Type = pd.Type,
                                 ItemName = p.ProductName,
                                 RowSubTotal = pd.RowSubTotal,


                             }).ToList();


                if (query == null)
                {
                    return NotFound();
                }
                return Ok(query);
            }

        }



        [ResponseType(typeof(void))]
        public IHttpActionResult PutQutationDetailsTable(int id, BillDetailTable billDetailTable)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != billDetailTable.BillDetailId)
            {
                return BadRequest();
            }

            db.Entry(billDetailTable).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
                return StatusCode(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                throw ex;

            }


        }







        [Route("api/DeleteDetails/{id:int}")]
        [ResponseType(typeof(InvoiceDetailsTable))]
        public IHttpActionResult DeleteBillDtailsTable(int id)
        {
            BillDetailTable billdetailtable = db.BillDetailTables.Find(id);
            try
            {
                if (billdetailtable == null)
                {
                    return NotFound();
                }
                db.BillDetailTables.Remove(billdetailtable);
                db.SaveChanges();
                return Ok(billdetailtable);

            }
            catch (Exception)
            {

                throw;
            }


        }

    }
}
