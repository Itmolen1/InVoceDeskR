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
    public class APIAccountTransictionController : ApiController
    {
        private DBEntities db = new DBEntities();

        // GET: api/APIAccountTransiction
        public IQueryable<AccountTransictionTable> GetAccountTransictionTables()
        {
            return db.AccountTransictionTables;
        }

        // GET: api/APIAccountTransiction/5
        [ResponseType(typeof(AccountTransictionTable))]
        public IHttpActionResult GetAccountTransictionTable(int id)
        {
            AccountTransictionTable accountTransictionTable = db.AccountTransictionTables.Find(id);
            if (accountTransictionTable == null)
            {
                return NotFound();
            }

            return Ok(accountTransictionTable);
        }

        // PUT: api/APIAccountTransiction/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutAccountTransictionTable(int id, AccountTransictionTable accountTransictionTable)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != accountTransictionTable.TransictionId)
            {
                return BadRequest();
            }

            db.Entry(accountTransictionTable).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AccountTransictionTableExists(id))
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

        [Route("api/APIAccountTransiction")]
        // POST: api/APIAccountTransiction
        [ResponseType(typeof(AccountTransictionTable))]
        public IHttpActionResult PostAccountTransictionTable(AccountTransictionTable accountTransictionTable)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //accountTransictionTable.CreationTime = null;
            try
            {
                db.AccountTransictionTables.Add(accountTransictionTable);
                db.SaveChanges();
            }
            catch(Exception ex)
            {
              var exx =  ex.ToString();
            }
            return Ok(accountTransictionTable);
        }

        // DELETE: api/APIAccountTransiction/5
        [ResponseType(typeof(AccountTransictionTable))]
        public IHttpActionResult DeleteAccountTransictionTable(int id)
        {
            AccountTransictionTable accountTransictionTable = db.AccountTransictionTables.Find(id);
            if (accountTransictionTable == null)
            {
                return NotFound();
            }

            db.AccountTransictionTables.Remove(accountTransictionTable);
            db.SaveChanges();

            return Ok(accountTransictionTable);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool AccountTransictionTableExists(int id)
        {
            return db.AccountTransictionTables.Count(e => e.TransictionId == id) > 0;
        }
    }
}