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

        [Authorize]
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
                    CompanyCity = c.CompanyCity,                 
                    CompanyCountry = c.CompanyCountry,
                    AddedBy = c.AddedBy,
                    AddedDate = c.AddedDate,
                    UserName = c.UserName,
                    StreetNumber = c.StreetNumber,
                    PostalCode = c.PostalCode,
                    Website = c.Website,
                    BankName = c.BankName,
                    IBANNumber = c.IBANNumber,
                    BIC = c.BIC,
                    KVK = c.KVK,
                    BTW = c.BTW

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
            catch(Exception )
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
                return StatusCode(HttpStatusCode.OK);
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

           
        }

        // POST: api/APIComapny
        [HttpPost]
        [ResponseType(typeof(ComapnyInfo))]
        public IHttpActionResult PostComapnyInfo(MVCCompanyInfoModel MVCcomapnyInfo)
         {
            ComapnyInfo comapnyInfo = new ComapnyInfo();
            comapnyInfo.CompanyName = MVCcomapnyInfo.CompanyName;
            comapnyInfo.CompanyLogo = MVCcomapnyInfo.CompanyLogo;
            comapnyInfo.CompanyAddress = MVCcomapnyInfo.CompanyAddress;

            comapnyInfo.StreetNumber = MVCcomapnyInfo.StreetNumber;
            comapnyInfo.PostalCode = MVCcomapnyInfo.PostalCode;
            comapnyInfo.CompanyCity = MVCcomapnyInfo.CompanyCity;
            comapnyInfo.CompanyCountry = MVCcomapnyInfo.CompanyCountry;
            comapnyInfo.CompanyPhone = MVCcomapnyInfo.CompanyPhone;
            comapnyInfo.CompanyCell = MVCcomapnyInfo.CompanyCell;           
            comapnyInfo.CompanyEmail = MVCcomapnyInfo.CompanyEmail;
            comapnyInfo.Website = MVCcomapnyInfo.Website;
            comapnyInfo.BankName = MVCcomapnyInfo.BankName;
            comapnyInfo.IBANNumber = MVCcomapnyInfo.IBANNumber;
            comapnyInfo.BIC = MVCcomapnyInfo.BIC;
            comapnyInfo.KVK = MVCcomapnyInfo.KVK;
            comapnyInfo.BTW = MVCcomapnyInfo.BTW;
            comapnyInfo.UserName = MVCcomapnyInfo.UserName;


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