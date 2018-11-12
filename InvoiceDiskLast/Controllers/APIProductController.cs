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


        [Route("api/APIProduct/{CompanyId:int}")]
        [ResponseType(typeof(List<MVCProductModel>))]
        
        public IHttpActionResult GetProductTables(int CompanyId)
        {
           
            List<ProductTable> i = new List<ProductTable>();
           
            // DBLIST = db.ProductTables.Where(x => x.Company_ID == id).ToList();
            

          var ob=   db.ProductTables.Where(x => x.Company_ID == CompanyId).Select(c => new MVCProductModel
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
            }).ToList();

            return Ok(ob);
        }


        [Route("api/APIProductByProductID/{id:int}")]
        // GET: api/APIProduct/5
        [ResponseType(typeof(ProductTable))]
        public IHttpActionResult GetProductTable(int id)
        {

            try
            {
                MVCProductModel productTable = db.ProductTables.Where(x => x.ProductId == id).Select(c => new MVCProductModel
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
                    ProductUnit = c.ProductUnit
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
            catch(Exception EX)
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

            return StatusCode(HttpStatusCode.NoContent);
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

            return CreatedAtRoute("DefaultApi", new { id = productTable.ProductId }, productTable);
        }

        // DELETE: api/APIProduct/5
        [Route("api/DeleteProduct/{id:int}")]
        [ResponseType(typeof(ProductTable))]
        public IHttpActionResult DeleteProductTable(int id)
        {
            ProductTable productTable = db.ProductTables.Find(id);
            if (productTable == null)
            {
                return NotFound();
            }
            return Ok(productTable);
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
    }
}
