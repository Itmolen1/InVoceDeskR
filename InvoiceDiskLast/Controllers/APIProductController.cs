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
    [Authorize]
    public class APIProductController : ApiController
    {
        private DBEntities db = new DBEntities();
        // GET: api/APIProduct

        [Route("api/APIProduct/{CompanyId:int}/{status:alpha}")]
        [ResponseType(typeof(List<MVCProductModel>))]

        public IHttpActionResult GetProductTables(int CompanyId, string status)
        {


            List<ProductTable> i = new List<ProductTable>();

            // DBLIST = db.ProductTables.Where(x => x.Company_ID == id).ToList();
            if (status == "All")
            {

                var ob = db.ProductTables.Where(x => x.Company_ID == CompanyId).Select(c => new MVCProductModel
                {

                    ProductId = c.ProductId,
                    ProductName = c.ProductName,
                    Description = c.Description,
                    SalePrice = c.SalePrice,
                    PurchasePrice = c.PurchasePrice,
                    Type = c.Type,
                    OpeningQuantity = c.OpeningQuantity,
                    AddedBy = c.AddedBy,
                    Company_ID = c.Company_ID,
                    AddedDate = c.AddedDate,                  
                    ProductStatus = c.ProductStatus,
                   

                }).ToList();

                return Ok(ob);
            }
            else
            {
                var ob = db.ProductTables.Where(x => x.Company_ID == CompanyId && x.Type.ToLower() == status.ToLower()).Select(c => new MVCProductModel
                {

                    ProductId = c.ProductId,
                    ProductName = c.ProductName,
                    Description = c.Description,
                    SalePrice = c.SalePrice,
                    PurchasePrice = c.PurchasePrice,
                    Type = c.Type,
                    OpeningQuantity = c.OpeningQuantity,
                    AddedBy = c.AddedBy,
                    Company_ID = c.Company_ID,
                    AddedDate = c.AddedDate,                    
                    ProductStatus = c.ProductStatus,
                   
                }).ToList();

                return Ok(ob);
            }
        }




        [Route("api/APIProductByProductID/{id:int}")]
        // GET: api/APIProduct/5
        [ResponseType(typeof(ProductTable))]
        public IHttpActionResult GetProductTable(int id)
        {

            try
            {
                MVCProductViewModel productTable = db.ProductTables.Where(x => x.ProductId == id).Select(c => new MVCProductViewModel
                {

                    ProductId = c.ProductId,
                    ProductName = c.ProductName,
                    Description = c.Description,
                    SalePrice = c.SalePrice,
                    PurchasePrice = c.PurchasePrice,
                    Type = c.Type,
                    OpeningQuantity = c.OpeningQuantity,
                    AddedBy = c.AddedBy,
                    Company_ID = c.Company_ID,
                    AddedDate = c.AddedDate,
                    ProductUnit = c.ProductUnit,
                 
                    ProductUnitName = c.ProductUnitTable.ProductUnit,
                    ProductStatus = c.ProductStatus,
              


                }).FirstOrDefault();
                if (productTable != null)
                {
                    return Ok(productTable);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception EX)
            {
                throw EX;
            }
        }

        [Route("api/PutAPIProduct/{id:int}")]
        [ResponseType(typeof(void))]
        public IHttpActionResult PutProductTable(int id, ProductTable productTable)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != productTable.ProductId)
            {
                return BadRequest();
            }

            db.Entry(productTable).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
                return Ok(productTable);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductTableExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

        }

        // POST: api/APIProduct
        [Route("api/PostProduct")]
        [ResponseType(typeof(ProductTable))]
        public IHttpActionResult PostProductTable(ProductTable productTable)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.ProductTables.Add(productTable);
            db.SaveChanges();


            return Ok(productTable);
            // return CreatedAtRoute("DefaultApi", new { id = productTable.ProductId }, productTable);
        }







        // DELETE: api/APIProduct/5
        [Route("api/DeleteProduct/{id:int}/{ProductStatus:alpha}")]
        [ResponseType(typeof(ProductTable))]
        public IHttpActionResult DeleteProductTable(int id, bool productStatus)
        {
            ProductTable productTable = db.ProductTables.Find(id);
            if (productTable == null || productStatus.ToString() == null)
            {
                return NotFound();
            }

            ProductTable productTables = db.ProductTables.Where(c => c.ProductId == id).FirstOrDefault();
            productTables.ProductStatus = productStatus;

            db.Entry(productTables).State = EntityState.Modified;
            try
            {
                db.SaveChanges();
                return Ok();
            }
            catch (DbUpdateConcurrencyException)
            {
                return NotFound();

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

        private bool ProductTableExists(int id)
        {
            return db.ProductTables.Count(e => e.ProductId == id) > 0;
        }

        [Route("api/GetStockItemList/{CompanyId:int}")]
        public IHttpActionResult GetStockList(int CompanyId)
        {
            //Select pt.PurchaseItemId,p.ProductName, SUM(pt.PurchaseQuantity) as purchaseQuantity,SUM(qt.Quantity) as SaleQuantity,
            //SUM(pt.PurchaseQuantity) - SUM(qt.Quantity) as RemainingQuantity
            //from[dbo].[PurchaseOrderDetailsTable] as pt
            //inner join PurchaseOrderTable as po on po.PurchaseOrderID = pt.PurchaseId
            //inner join QutationDetailsTable as qt on pt.PurchaseItemId = qt.ItemId
            //inner join QutationTable as qtp on qtp.QutationID = qt.QutationID
            //inner join ProductTable as p on p.ProductId = pt.PurchaseItemId
            //where po.Type = 'Goods' And po.Status = 'accepted'
            //and qtp.Type = 'Goods' And po.Status = 'accepted'
            //Group by pt.PurchaseItemId,ProductName

            try
            {
                var Query = db.GetStockItem(CompanyId).Select(C => new StockViewModel
                {

                    PurchaseItemId = C.PurchaseItemId,
                    ProductName=C.ProductName,
                    PurchaseQuantity = C.purchaseQuantity,
                    SaleQuantity = C.SaleQuantity,
                    RemaingQuantity = C.RemainingQuantity,
                }).ToList();

                return Ok(Query);
            }
            catch (Exception)
            {
                NotFound();
                throw;
            }



        }

    }
}
