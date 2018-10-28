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
        public IHttpActionResult GetContactsTables()
        {
            List<ContactsTable> ContactModel = new List<ContactsTable>();
            IEnumerable<string> headerValues;
            //var DBLIST = "";
            var IDS = "";
            if (GlobalVeriables.WebApiClient.DefaultRequestHeaders.TryGetValues("CompayID", out headerValues))
            {
                IDS = headerValues.FirstOrDefault();
            }
            int id = Convert.ToInt32(IDS);
            string status = "";
            if (GlobalVeriables.WebApiClient.DefaultRequestHeaders.TryGetValues("CustomerStatus", out headerValues))
            {
                status = headerValues.FirstOrDefault();
            }

            // DBLIST = db.ProductTables.Where(x => x.Company_ID == id).ToList();
            try
            {
                if (status != "")
                {
                    var obvender = db.ContactsTables.Where(x => x.Company_Id == id && x.Type == status && x.ContactsId != null).Select(c => new MVCContactModel
                    {
                        ContactsId = c.ContactsId,
                        ContactName = c.ContactName,
                        Status = c.Status,
                    }).ToList();

                    return Ok(obvender);
                }
               
                else
                {

                    var obContact = db.ContactsTables.Where(x => x.Company_Id == id).Select(c => new MVCContactModel
                    {
                        ContactsId = c.ContactsId,
                        ContactName = c.ContactName,
                        ContactAddress = c.ContactAddress,
                        Company_Id = c.Company_Id,
                        UserId = c.UserId,
                        Type = c.Type,                      
                     
                        Addeddate = c.Addeddate,
                        Status = c.Status,
                    }).ToList();

                    return Ok(obContact);
                }
            }
            catch (Exception ex)
            {
                return null;

            }

        }

        // GET: api/ApiConatacts/5
        [ResponseType(typeof(ContactsTable))]
        public IHttpActionResult GetContactsTable(int id)
        {
            //var Contactmodel = new MVCContactModel(); 

            try
            {

                MVCContactModel Contactmodel = db.ContactsTables.Where(x => x.ContactsId == id).Select(c => new MVCContactModel
                {
                    ContactsId = c.ContactsId,
                    ContactName = c.ContactName,
                    ContactAddress = c.ContactAddress,
                    Company_Id = c.Company_Id,
                    UserId = c.UserId,
                    Type = c.Type,                   
                   
                    Addeddate = c.Addeddate,
                    Status = c.Status,
                }).FirstOrDefault();
                             

                if (Contactmodel == null)
                {
                    return NotFound();
                }
                return Ok(Contactmodel);
            }
            catch (Exception ex)
            {
                return NotFound();
            }          
            
           
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