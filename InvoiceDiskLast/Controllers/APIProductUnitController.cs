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
       
       
       [Route("api/GetProductUnit/{CompanyID:int}")]
        public IHttpActionResult GetProductUnitTables(int CompanyID)
        {
          try
            {
               List<MVCProductUnitModel> obProductUnit = db.ProductUnitTables.Where(x => x.CompanyId == CompanyID && x.Status == true).Select(c => new MVCProductUnitModel
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

        //[Route("api/students/{id:int}")]
        //public IHttpActionResult GetProductUnitTables(int id)
        //{
        //    return Ok(id);
        //}

        [Route("api/APIProductUnitByName/{name:alpha}/{CompanyID:int}")]
        public IHttpActionResult GetProductUnitTables(string name,int CompanyID)
        {
            CheckUnit1 ch = new CheckUnit1();
            bool resut =  db.ProductUnitTables.Count(e => e.ProductUnit == name && e.CompanyId == CompanyID) > 0;

            if (resut == true)
            {
                ch.count = 1;
              return Ok(ch);
            }
            else
            {
                ch.count = 0;
               return Ok(ch);
            }
           
        }

        //[Route("api/students/{name:alpha}/{id:int}")]
        //public IHttpActionResult GetProductUnitTables(string name,int id)
        //{
        //    return Ok(name + id);
        //}


        // GET: api/APIProductUnit/5


        [Route("api/PutProductUnit/{id:int}")]
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

       [Route("api/PostProductUnit")]
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


        [Route("api/DeleteProductUnitByID/{id:int}")]
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