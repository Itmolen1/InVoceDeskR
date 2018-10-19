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
using System.Net.Http.Headers;
using System.Web;

namespace InvoiceDiskLast.Controllers
{

    //[Authorize]
    public class ApiConatactsController : ApiController
    {
        private DBEntities db = new DBEntities();

        // GET: api/ApiConatacts
        public List<ContactsTable> GetContactsTables()    
        {            
            IEnumerable<string> headerValues;
          
            var IDS = "";
            if (GlobalVeriables.WebApiClient.DefaultRequestHeaders.TryGetValues("CompayID", out headerValues))
            {
                IDS = headerValues.FirstOrDefault();
            }            
           int id = Convert.ToInt32(IDS);
            
            return db.ContactsTables.Where(c => c.Company_Id == id).ToList();

        }

        // GET: api/ApiConatacts/5
        [ResponseType(typeof(ContactsTable))]
        public IHttpActionResult GetContactsTable(int id)
        {
            ContactsTable contactsTable = db.ContactsTables.Find(id);
            if (contactsTable == null)
            {
                return NotFound();
            }

            return Ok(contactsTable);
        }

        // PUT: api/ApiConatacts/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutContactsTable(int id, ContactsTable contactsTable)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != contactsTable.ContactsId)
            {
                return BadRequest();
            }

            db.Entry(contactsTable).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ContactsTableExists(id))
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

        // POST: api/ApiConatacts
        [ResponseType(typeof(ContactsTable))]
        public IHttpActionResult PostContactsTable(ContactsTable contactsTable)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.ContactsTables.Add(contactsTable);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = contactsTable.ContactsId }, contactsTable);
        }

        // DELETE: api/ApiConatacts/5
        [ResponseType(typeof(ContactsTable))]
        public IHttpActionResult DeleteContactsTable(int id)
        {
            ContactsTable contactsTable = db.ContactsTables.Find(id);
            if (contactsTable == null)
            {
                return NotFound();
            }

            db.ContactsTables.Remove(contactsTable);
            db.SaveChanges();

            return Ok(contactsTable);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ContactsTableExists(int id)
        {
            return db.ContactsTables.Count(e => e.ContactsId == id) > 0;
        }
    }
}