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
using System.Web;

namespace InvoiceDiskLast.Controllers
{
    public class APIComapnyController : ApiController
    {
        private DBEntities db = new DBEntities();

        // GET: api/APIComapny
        public IQueryable<ComapnyInfo> GetComapnyInfoes()
        {
            return db.ComapnyInfoes;
        }

       
        // GET: api/APIComapny/5
        [ResponseType(typeof(ComapnyInfo))]
        public IHttpActionResult GetComapnyInfo(int id)
        {
            try
            {
                MVCCompanyInfoModel comapnyInfo = db.ComapnyInfoes.Where(x => x.CompanyID == id).Select(c => new MVCCompanyInfoModel
                {

                    CompanyID = c.CompanyID,
                    CompanyName = c.CompanyName,
                    CompanyAddress = c.CompanyAddress,
                    CompanyPhone = c.CompanyPhone,
                    CompanyCell = c.CompanyCell,
                    CompanyEmail = c.CompanyEmail,
                    CompanyLogo = c.CompanyLogo,
                    CompanyTRN = c.CompanyTRN,
                    ComapnyFax = c.ComapnyFax,
                    CompanySubTitile = c.CompanySubTitile,
                    CompanyCity = c.CompanyCity,
                    CompanyState = c.CompanyState,
                    CompanyCountry = c.CompanyCountry,
                    AddedBy = c.AddedBy,
                    AddedDate = c.AddedDate,
                    UserName = c.UserName

                }).FirstOrDefault();

                if (comapnyInfo != null)
                {
                    Request.Content.Headers.Add("CompayID", comapnyInfo.CompanyID.ToString());
                    Request.Headers.Add("CompayID", comapnyInfo.CompanyID.ToString());
                    return Ok(comapnyInfo);
                }
                else
                {
                    return null;
                }
            }
            catch(Exception ex)
            {
                return null;
            }
                   
           
        }

        // PUT: api/APIComapny/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutComapnyInfo(int id, ComapnyInfo comapnyInfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != comapnyInfo.CompanyID)
            {
                return BadRequest();
            }

            db.Entry(comapnyInfo).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ComapnyInfoExists(id))
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

        // POST: api/APIComapny
        [HttpPost]
        [ResponseType(typeof(ComapnyInfo))]
        public IHttpActionResult PostComapnyInfo(ComapnyInfo comapnyInfo)
         {
            
            if (!ModelState.IsValid)
            {

                return BadRequest(ModelState);
            }

            db.ComapnyInfoes.Add(comapnyInfo);
            db.SaveChanges();



            return CreatedAtRoute("DefaultApi", new { id = comapnyInfo.CompanyID }, comapnyInfo);
        }

       

        // DELETE: api/APIComapny/5
        [ResponseType(typeof(ComapnyInfo))]
        public IHttpActionResult DeleteComapnyInfo(int id)
        {
            ComapnyInfo comapnyInfo = db.ComapnyInfoes.Find(id);
            if (comapnyInfo == null)
            {
                return NotFound();
            }

            db.ComapnyInfoes.Remove(comapnyInfo);
            db.SaveChanges();

            return Ok(comapnyInfo);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ComapnyInfoExists(int id)
        {
            return db.ComapnyInfoes.Count(e => e.CompanyID == id) > 0;
        }
    }
}