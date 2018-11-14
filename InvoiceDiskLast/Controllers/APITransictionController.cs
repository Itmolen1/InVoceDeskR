using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using InvoiceDiskLast.Models;
using System.Data.Entity;

namespace InvoiceDiskLast.Controllers
{
    [RoutePrefix("api/Account")]
    public class APITransictionController : ApiController
    {

        
        DBEntities db = new DBEntities();

        [Route("GetTransiction/{CompanyId:int}")]
        [HttpGet]
        public IHttpActionResult GetTransition(int CompanyId)
        {
            List<AccountTransictionTable> TransictionList = new List<AccountTransictionTable>();
            try
            {
                TransictionList = db.AccountTransictionTables.Where(x => x.FK_CompanyId == CompanyId).Select(c => new AccountTransictionTable {

                    TransictionId = c.TransictionId,
                    TransictionNumber = c.TransictionNumber,
                    TransictionDate = c.TransictionDate,
                    TransictionRefrenceId = c.TransictionRefrenceId,
                    TransictionType = c.TransictionType,
                    Cr = c.Cr,
                    Dr = c.Dr,
                    FK_AccountID = c.FK_AccountID,
                    CreationTime = c.CreationTime,
                    FK_CompanyId = c.FK_CompanyId,
                    Description = c.Description

                }).ToList();

                return Ok(TransictionList);
            }
            catch(Exception)
            {

            }
            return Ok(TransictionList);
        }


        [Route("PostTransiction")]
        public IHttpActionResult PostTransiction(AccountTransictionTable transictiontable)
        {
            //if (ModelState.IsValid)
            //{
            //    return BadRequest();
            //}

            try
            {

               
                db.AccountTransictionTables.Add(transictiontable);
                db.SaveChanges();
                return Ok(transictiontable);

            }
            catch (Exception ex)
            {
                ex.ToString();
            }
           
            return BadRequest();
        }


    

        [Route("PutTransiction")]
        public IHttpActionResult PutTransiction(int id, AccountTransictionTable accountTransictionTable)
        {
          
               if(!ModelState.IsValid)
                {
                    return BadRequest();
                }

               if(id != accountTransictionTable.TransictionId)
                {
                    return BadRequest();
                }

            try
            {
                db.Entry(accountTransictionTable).State = EntityState.Modified;

                db.SaveChanges();

                return Ok(accountTransictionTable);
            }
            catch(Exception)
            {

            }
            return BadRequest();
        }
    }
}
