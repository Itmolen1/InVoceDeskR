using InvoiceDiskLast.MISC;
using InvoiceDiskLast.Models;
using Logger;
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
    [RouteNotFoundAttribute]
    [RoutePrefix("api/ConfirmEmail")]
    public class ConfirmEmailController : ApiController
    {
        private DBEntities db = new DBEntities();
        private Ilog _iLog;
        public ConfirmEmailController()
        {
            _iLog = Log.GetInstance;
        }
        public IHttpActionResult Get()
        {
            return null;
        }

        //public IHttpActionResult GetDetailbyId(string Email)
        //{
        //    var Result = db.AspNetUsers.Find(Email);

        //    if (Result == null)
        //    {
        //        return NotFound();
        //    }
        //    else
        //    {
        //        return Ok(Result);
        //    }


        [Route("{id:alpha}")]
        [HttpGet]
        public IHttpActionResult GetDetailbyEmail(string id)
        {
            try
            {
                IEnumerable<string> headerValues;
                //var DBLIST = "";
                var email = "";
                if (GlobalVeriables.WebApiClient.DefaultRequestHeaders.TryGetValues("Email", out headerValues))
                {
                    email = headerValues.FirstOrDefault();
                }

                var RESULT = db.AspNetUsers.Where(d => d.Email.ToLower().ToString() == email.ToString().ToLower()).Select(C => new MvcUserModel
                {
                    Id = C.Id,
                    UserName = C.UserName,
                }).FirstOrDefault();

                if (RESULT != null)
                {
                    return Ok(RESULT);
                }
                else
                {
                    return NotFound();
                }

            }
            catch (Exception EX)
            {
                return NotFound();

            }
        }


        [Route("{id:alpha}/{ss:alpha}")]
        [ResponseType(typeof(void))]
        public IHttpActionResult PutProductTable(string id, string ss, AspNetUser productTable)
        {
            IEnumerable<string> headerValues;
            var Code = "";
            if (GlobalVeriables.WebApiClient.DefaultRequestHeaders.TryGetValues("Code", out headerValues))
            {
                Code = headerValues.FirstOrDefault();
            }

            AspNetUser asp = db.AspNetUsers.Where(c => c.Id== Code).FirstOrDefault();
            asp.EmailConfirmed = true;
            db.Entry(asp).State = EntityState.Modified;
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
    }
}

