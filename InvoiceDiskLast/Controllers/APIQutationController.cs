﻿using InvoiceDiskLast.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace InvoiceDiskLast.Controllers
{
    public class APIQutationController : ApiController
    {

        private DBEntities db = new DBEntities();

        //Get: api/GetQutationCount

       

        
        // GET: api/APIQutation
        public IHttpActionResult GetQutationTables()
        {
            List<QutationTable> QutationModelObj = new List<QutationTable>();
            IEnumerable<string> headerValues;
            //var DBLIST = "";
            var IDS = "";
            int id = 0;
            if (GlobalVeriables.WebApiClient.DefaultRequestHeaders.TryGetValues("CompayID", out headerValues))
            {
                IDS = headerValues.FirstOrDefault();
                 id = Convert.ToInt32(IDS);
            }        

            var Qutationob = db.QutationTables.Where(x => x.CompanyId == id).Select(c => new MVCQutationModel
            {

                QutationID = c.QutationID,
                Qutation_ID = c.Qutation_ID,
                RefNumber = c.RefNumber,
                QutationDate = c.QutationDate,
                DueDate = c.DueDate,
                SubTotal = c.SubTotal,
                TotalVat6 = c.TotalVat6,
                TotalVat21 = c.TotalVat21,
                DiscountAmount = c.DiscountAmount,
                TotalAmount = c.TotalAmount,
                CustomerNote = c.CustomerNote,
                Status = c.Status,
                UserId = c.UserId,
                ContactId = c.ContactId

            }).ToList();

            return Ok(Qutationob);


        }



        // GET: api/APIQutation/5
        [ResponseType(typeof(QutationTable))]
        public IHttpActionResult GetQutationTable(int id)
        {
            MVCQutationModel qutationmodel = new MVCQutationModel();

            var ob = db.QutationTables.Find(id);
            qutationmodel.QutationID = ob.QutationID;
            qutationmodel.Qutation_ID = ob.Qutation_ID;
            qutationmodel.RefNumber = ob.RefNumber;
            qutationmodel.QutationDate = ob.QutationDate;
            qutationmodel.DueDate = ob.DueDate;
            qutationmodel.SubTotal = ob.SubTotal;
            qutationmodel.TotalVat6 = ob.TotalVat6;
            qutationmodel.TotalVat21 = ob.TotalVat21;
            qutationmodel.DiscountAmount = ob.DiscountAmount;
            qutationmodel.TotalAmount = ob.TotalAmount;
            qutationmodel.CustomerNote = ob.CustomerNote;
            qutationmodel.Status = ob.Status;
            qutationmodel.UserId = ob.UserId;
            qutationmodel.CompanyId = ob.CompanyId;
            qutationmodel.ContactId = ob.ContactId;

            if (qutationmodel == null)
            {
                return NotFound();
            }

            return Ok(qutationmodel);
        }

        // PUT: api/APIQutation/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutQutationTable(int id, QutationTable qutationTable)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != qutationTable.QutationID)
            {
                return BadRequest();
            }

            db.Entry(qutationTable).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
                return StatusCode(HttpStatusCode.OK);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!QutationTableExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }


        }

        // POST: api/APIQutation
        [ResponseType(typeof(QutationTable))]
        public HttpResponseMessage PostQutationTable([FromBody] QutationTable qutationtable)
        {
            using (DBEntities entities = new DBEntities())
            {
                entities.QutationTables.Add(qutationtable);
                entities.SaveChanges();

                var massage = Request.CreateResponse(HttpStatusCode.Created, qutationtable);
                massage.Headers.Location = new Uri(Request.RequestUri + qutationtable.QutationID.ToString());
                massage.Content.Headers.Add("idd", qutationtable.QutationID.ToString());
                massage.RequestMessage.Headers.Add("idd", qutationtable.QutationID.ToString());
                return massage;

            }
        }



        //public HttpResponseMessage Post([FromBody] QutationTable qutationTable)
        //{
        //    try
        //    {
        //        using (QutationEntities entities = new QutationEntities())
        //        {
        //            entities.QutationTables.Add(qutationTable);
        //            entities.SaveChanges();

        //            var message = Request.CreateResponse(HttpStatusCode.Created, qutationTable);
        //            message.Headers.Location = new Uri(Request.RequestUri +
        //                qutationTable.QutationID.ToString());

        //            return message;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
        //    }
        //}

        // DELETE: api/APIQutation/5
        [ResponseType(typeof(QutationTable))]
        public IHttpActionResult DeleteQutationTable(int id)
        {
            QutationTable qutationTable = db.QutationTables.Find(id);
            if (qutationTable == null)
            {
                return NotFound();
            }

            db.QutationTables.Remove(qutationTable);
            db.SaveChanges();

            return Ok(qutationTable);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool QutationTableExists(int id)
        {
            return db.QutationTables.Count(e => e.QutationID == id) > 0;
        }
    }
}
