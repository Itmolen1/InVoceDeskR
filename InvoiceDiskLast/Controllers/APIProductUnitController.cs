using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using InvoiceDiskLast.Models;

namespace InvoiceDiskLast.Controllers
{
    
    public class APIProductUnitController : ApiController
    {
        
        private DBEntities db = new DBEntities();
       
        // GET: api/APIProductUnit
        public IHttpActionResult GetProductUnitTables()
        {
            
            IEnumerable<string> headerValues;
            //var DBLIST = "";
            var IDS = "";
            if (GlobalVeriables.WebApiClient.DefaultRequestHeaders.TryGetValues("CompayID", out headerValues))
            {
                IDS = headerValues.FirstOrDefault();
            }
            int id = Convert.ToInt32(IDS);           

          try
            {
               List<MVCProductUnitModel> obProductUnit = db.ProductUnitTables.Where(x => x.CompanyId == id && x.Status == true).Select(c => new MVCProductUnitModel
                    {
                        ProductUnitID = c.ProductUnitID,
                        ProductUnit = c.ProductUnit,
                        Status = c.Status,
                    }).ToList();

                    return Ok(obProductUnit);
                
            }
            catch (Exception ex)
            {
                return NotFound();

            }
           
        }
       

        // GET: api/APIProductUnit/5


        // PUT: api/APIProductUnit/5
        [ResponseType(typeof(void))]
       
        public IHttpActionResult PutProductUnitTable(int id, ProductUnitTable productUnitTable)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != productUnitTable.ProductUnitID)
            {
                return BadRequest();
            }

            db.Entry(productUnitTable).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
                return Ok();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductUnitTableExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

           
        }

        // POST: api/APIProductUnit
        [ResponseType(typeof(ProductUnitTable))]
        public IHttpActionResult PostProductUnitTable(ProductUnitTable productUnitTable)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.ProductUnitTables.Add(productUnitTable);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = productUnitTable.ProductUnitID }, productUnitTable);
        }

        // DELETE: api/APIProductUnit/5
        [ResponseType(typeof(ProductUnitTable))]
        public IHttpActionResult DeleteProductUnitTable(int id)
        {
            ProductUnitTable productUnitTable = db.ProductUnitTables.Find(id);
            if (productUnitTable == null)
            {
                return NotFound();
            }

            db.ProductUnitTables.Remove(productUnitTable);
            db.SaveChanges();

            return Ok(productUnitTable);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ProductUnitTableExists(int id)
        {
            return db.ProductUnitTables.Count(e => e.ProductUnitID == id) > 0;
        }
    }

    public class CheckUnit1
    {
        public int count { get; set; }
    }
}