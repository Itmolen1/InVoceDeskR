﻿using System;
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
    public class PaymentTermDurationsController : ApiController
    {
        private DBEntities db = new DBEntities();

        // GET: api/PaymentTermDurations
        [Route("api/GetPaymentTermDurations/{id:int}")]
        public IHttpActionResult GetPaymentTermDurations(int id)
        {
            
            List<PaymentTermUdrationModel> paymentTermDurations = db.PaymentTermDurations.Where(x => x.CompanyId == id && x.Status == true).Select(x => new PaymentTermUdrationModel
            {

                PaymentDurationId = x.PaymentDurationId,
                PaymentDuration = x.PaymentDuration

            }).ToList();

            return Ok(paymentTermDurations);
        }

        // GET: api/PaymentTermDurations/5
     
        [ResponseType(typeof(PaymentTermDuration))]
        public IHttpActionResult GetPaymentTermDuration(int id)
        {
            PaymentTermDuration paymentTermDuration = db.PaymentTermDurations.Find(id);
            if (paymentTermDuration == null)
            {
                return NotFound();
            }

            return Ok(paymentTermDuration);
        }

        // PUT: api/PaymentTermDurations/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutPaymentTermDuration(int id, PaymentTermDuration paymentTermDuration)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != paymentTermDuration.PaymentDurationId)
            {
                return BadRequest();
            }

            db.Entry(paymentTermDuration).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
                return Ok(paymentTermDuration);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PaymentTermDurationExists(id))
                {
                    return NotFound();
                }
                else
                {
                    return StatusCode(HttpStatusCode.NoContent);
                }
            }            
        }

        [Route("api/PaymentTermDurations")]
        [ResponseType(typeof(PaymentTermDuration))]
        public IHttpActionResult PostPaymentTermDuration(PaymentTermDuration paymentTermDuration)
        {
            try
            {
                paymentTermDuration = db.PaymentTermDurations.Add(paymentTermDuration);
                db.SaveChanges();

                return Ok(paymentTermDuration);
            }
            catch(Exception ex)
            {
                return BadRequest();
            }
        }

        // DELETE: api/PaymentTermDurations/5
        [ResponseType(typeof(PaymentTermDuration))]
        public IHttpActionResult DeletePaymentTermDuration(int id)
        {
            PaymentTermDuration paymentTermDuration = db.PaymentTermDurations.Find(id);
            if (paymentTermDuration == null)
            {
                return NotFound();
            }

            db.PaymentTermDurations.Remove(paymentTermDuration);
            db.SaveChanges();

            return Ok(paymentTermDuration);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool PaymentTermDurationExists(int id)
        {
            return db.PaymentTermDurations.Count(e => e.PaymentDurationId == id) > 0;
        }
    }
}