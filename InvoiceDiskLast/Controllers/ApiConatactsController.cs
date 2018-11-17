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

    
    public class ApiConatactsController : ApiController
    {
        private DBEntities db = new DBEntities();


        // GET: api/ApiConatacts
        [Route("api/ApiConatacts/{companyID:int}/{contactStatus:alpha}")]
        public IHttpActionResult GetContactsTables(int companyID, string contactStatus)
        {            
            try
            {
                if (contactStatus != "All")
                {
                    var obvender = db.ContactsTables.Where(x => x.Company_Id == companyID && x.Type == contactStatus && x.ContactsId != null).Select(c => new MVCContactModel
                    {
                        ContactsId = c.ContactsId,
                        ContactName = c.ContactName,                       
                        Type = c.Type,                      
                        Status = c.Status,
                    }).ToList();

                    return Ok(obvender);
                }

                else
                {

                    var obContact = db.ContactsTables.Where(x => x.Company_Id == companyID).Select(c => new MVCContactModel
                    {
                        ContactsId = c.ContactsId,
                        ContactName = c.ContactName,
                        ContactAddress = c.ContactAddress,
                        City = c.City,
                        PostalCode = c.PostalCode,
                        Mobile = c.Mobile,
                        Company_Id = c.Company_Id,
                        UserId = c.UserId,
                        telephone=c.telephone,                    
                        Type = c.Type,
                        StreetNumber = c.StreetNumber,                      
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
                    City = c.City,
                    StreetNumber = c.StreetNumber,
                    PostalCode = c.PostalCode,
                    LandLine = c.LandLine,
                    telephone = c.telephone,
                    Mobile = c.Mobile,
                    Website = c.Website,
                    BillingEmail = c.BillingEmail,
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
        [Route("api/DeleteConatcts/{id:int}/{status:alpha}")]
        [ResponseType(typeof(ContactsTable))]
        public IHttpActionResult DeleteContactsTable(int id,bool status)
        {
            ContactsTable contactsTable = db.ContactsTables.Find(id);
                       
            if (contactsTable == null || contactsTable.ToString() == null)
            {
                return NotFound();
            }

            ContactsTable contactstble = db.ContactsTables.Where(c => c.ContactsId == id).FirstOrDefault();
            contactstble.Status = status;

            db.Entry(contactstble).State = EntityState.Modified;
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

        private bool ContactsTableExists(int id)
        {
            return db.ContactsTables.Count(e => e.ContactsId == id) > 0;
        }
    }
}