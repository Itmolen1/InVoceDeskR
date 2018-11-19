using InvoiceDiskLast.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace InvoiceDiskLast.Controllers
{
    public class APIHeadAccountController : ApiController
    {
        private DBEntities db = new DBEntities();

        [Route("api/HeadAccount/{ControlAccountId:int}/{CompanyID:int}")]
        public IHttpActionResult GetControlAccount(int ControlAccountId, int CompanyID)
        {
            try
            {
                List<MVCHeadAccountModel> HeadAccountObj = db.HeadAccountTables.Where(x => x.FK_CompanyId == CompanyID && x.FK_ControlAccountID == ControlAccountId).Select(c => new MVCHeadAccountModel
                {
                    HeadAccountId = c.HeadAccountId,
                    HeadAccountTitle = c.HeadAccountTitle,
                    HeadAccountDescription = c.HeadAccountDescription,

                }).ToList();

                return Ok(HeadAccountObj);
            }
            catch (Exception ex)
            {
                return NotFound();

            }
        }

        [Route("api/HeadAccountbyId/{companyid:int}")]
        public IHttpActionResult GetControlAccount(int companyid)
        {

            try
            {
                List<MVCHeadAccountModel> HeadAccountObj = db.HeadAccountTables.Where(x => x.FK_CompanyId == companyid).Select(c => new MVCHeadAccountModel
                {
                    HeadAccountId = c.HeadAccountId,
                    HeadAccountTitle = c.HeadAccountTitle,
                    HeadAccountDescription = c.HeadAccountDescription,

                }).ToList();

                return Ok(HeadAccountObj);
            }
            catch (Exception ex)
            {
                return NotFound();

            }
        }
        
        [Route("api/PostHeadAccount")]

        public IHttpActionResult PostHeadAccount(HeadAccountTable headAccountTable)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.HeadAccountTables.Add(headAccountTable);
            db.SaveChanges();

            return Created("DefaultAPi", headAccountTable);

            // return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }


        [Route("api/HeadAccountTitle")]
        public IHttpActionResult PostCheckHeadTitie(HeadAccountTable headaccountable)
        {
            bool found = db.HeadAccountTables.Any(x => x.HeadAccountTitle == headaccountable.HeadAccountTitle &&
                                                  x.FK_CompanyId == headaccountable.FK_CompanyId && 
                                                  x.FK_ControlAccountID == headaccountable.FK_ControlAccountID);
            
            if(found)
            {
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }



        [Route("api/HeadAccountByAccountID/{headACid:int}/{companyId:int}")]
        public IHttpActionResult GetHeadAccountByID(int headACid,int companyId)
        {

            if(headACid > 0)
            {
                try
                {
                    MVCHeadAccountModel HeadAccountObj = db.HeadAccountTables.Where(x => x.FK_CompanyId == companyId && x.HeadAccountId == headACid).Select(c => new MVCHeadAccountModel
                    {
                        HeadAccountId = c.HeadAccountId,
                        HeadAccountTitle = c.HeadAccountTitle,
                        HeadAccountDescription = c.HeadAccountDescription,
                        FK_ControlAccountID = c.FK_ControlAccountID

                    }).FirstOrDefault();

                    return Ok(HeadAccountObj);

                }
                catch(Exception)
                {
                    return NotFound();
                }
            }
            else
            {
                return NotFound();
            }
        }


        // PUT: api/APIComapny/5
      
        [Route("api/UpdateHeadAccount/{id:int}")]
        public IHttpActionResult PutHeadAccount(int id, HeadAccountTable headAccount)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != headAccount.HeadAccountId)
            {
                return BadRequest();
            }

            db.Entry(headAccount).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
                return StatusCode(HttpStatusCode.OK);
            }
            catch (DbUpdateConcurrencyException)
            {

                throw;
            }


        }


        [Route("api/HeadAccountList/{companyId:int}")]

        public IHttpActionResult GetHeadAccount(int companyId)
        {
            try
            {
                List<MVCHeadAccountModel> HeadAccountObj = db.HeadAccountTables.Where(x => x.FK_CompanyId == companyId).Select(c => new MVCHeadAccountModel
                {
                    HeadAccountId = c.HeadAccountId,
                    HeadAccountTitle = c.HeadAccountTitle,
                    HeadAccountDescription = c.HeadAccountDescription,

                }).ToList();

                return Ok(HeadAccountObj);
            }
            catch(Exception ex)
            {
                return NotFound();
            }
        }

        //[Route("api/students/{name:alpha}")]
        //public IHttpActionResult GetProductUnitTables(string name)
        //{
        //    return Ok(name);
        //}









        [Route("api/GetHeadAccount/{HeadAcID:int}/{companyId:int}")]
        public IHttpActionResult GetHeadAccounts(int HeadAcID, int companyId)
        {

            if (HeadAcID == 0)
            {

                try
                {
                    List<MVCHeadAccountModel> AccountObj = db.HeadAccountTables.Where(x => x.FK_CompanyId == companyId && x.FK_ControlAccountID == 1).Select(c => new MVCHeadAccountModel
                    {
                        HeadAccountId = c.HeadAccountId,
                        HeadAccountTitle = c.HeadAccountTitle,
                    }).ToList();

                    return Ok(AccountObj);
                }
                catch (Exception ex)
                {
                    return NotFound();
                }
            }
            else
            {
                try
                {
                    List<MVCAccountTableModel> AccountObj2 = db.AccountTables.Where(x => x.FK_CompanyId == companyId && x.FK_HeadAccountId == HeadAcID).Select(c => new MVCAccountTableModel
                    {
                        AccountId = c.AccountId,
                        AccountTitle = c.AccountTitle,

                    }).ToList();

                    return Ok(AccountObj2);
                }
                catch (Exception ex)
                {
                    return NotFound();
                }
            }
        }
    }
}
    