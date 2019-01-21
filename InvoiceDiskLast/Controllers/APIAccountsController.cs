using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using InvoiceDiskLast.Models;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;

namespace InvoiceDiskLast.Controllers
{
    public class APIAccountsController : ApiController
    {
        private DBEntities db = new DBEntities();
        [Route("api/AccountByAccountID/{HeadAcID:int}/{companyId:int}")]
        public IHttpActionResult GetHeadAccount(int HeadAcID, int companyId)
        {

            if (HeadAcID == 0)
            {

                try
                {
                    List<MVCAccountTableModel> AccountObj = db.AccountTables.Where(x => x.FK_CompanyId == companyId).Select(c => new MVCAccountTableModel
                    {
                        AccountId = c.AccountId,
                        AccountTitle = c.AccountTitle,
                        AccountDescription = c.AccountDescription,
                        FK_HeadAccountId = c.FK_HeadAccountId

                    }).ToList();

                    return Ok(AccountObj);
                }
                catch (Exception )
                {
                    return NotFound();
                }
            }
            else
            {
                try
                {
                    List<MVCAccountTableModel> AccountObj = db.AccountTables.Where(x => x.FK_CompanyId == companyId && x.FK_HeadAccountId == HeadAcID).Select(c => new MVCAccountTableModel
                    {
                        AccountId = c.AccountId,
                        AccountTitle = c.AccountTitle,
                        AccountDescription = c.AccountDescription,
                        FK_HeadAccountId = c.FK_HeadAccountId

                    }).ToList();

                    return Ok(AccountObj);
                }
                catch (Exception ex)
                {
                    return NotFound();
                }
            }
        }




        [Route("api/AccountTitle")]
        public IHttpActionResult PostCheckHeadTitie(AccountTable accountable)
        {
            bool found = db.AccountTables.Any(x => x.AccountTitle == accountable.AccountTitle &&
                                                  x.FK_CompanyId == accountable.FK_CompanyId &&
                                                  x.FK_HeadAccountId == accountable.FK_HeadAccountId);

            if (found)
            {
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }



        [Route("api/PostAccount")]

        public IHttpActionResult PostHeadAccount(AccountTable AccountTable)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.AccountTables.Add(AccountTable);
            db.SaveChanges();

            return Created("DefaultAPi", AccountTable);

            // return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }


        [Route("api/UpdateAccount/{id:int}")]
        public IHttpActionResult PutHeadAccount(int id, AccountTable AccountTable)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != AccountTable.AccountId)
            {
                return BadRequest();
            }

            db.Entry(AccountTable).State = EntityState.Modified;

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


        

        [Route("api/GetAllAccountControl/{companyId:int}")]
        public IHttpActionResult GetAllAccountControl(int companyId)
        {

            try
            {
                List<MVCAccountTableModel> AccountObj = new List<MVCAccountTableModel>();

                AccountObj = db.AccountTables.Where(x => x.FK_CompanyId == companyId).Select(c => new MVCAccountTableModel
                {
                    AccountId = c.AccountId,
                    AccountTitle = c.AccountTitle,
                    AccountCode = c.AccountCode

                }).ToList();

                return Ok(AccountObj);

            }
            catch (Exception)
            {
                return NotFound();
            }

        }


        [Route("api/GetExpense/{companyId:int}")]
        public IHttpActionResult GetExpenseAccount(int companyId)
        {
          
                try
                {
                List<MVCAccountTableModel> AccountObj = new List<MVCAccountTableModel>();

                   AccountObj = db.AccountTables.Where(x => x.FK_CompanyId == companyId && x.FK_HeadAccountId == 188).Select(c => new MVCAccountTableModel
                    {
                        AccountId = c.AccountId,
                        AccountTitle = c.AccountTitle,                                      
                        AccountCode = c.AccountCode

                    }).ToList();

                    return Ok(AccountObj);

                }
                catch (Exception)
                {
                    return NotFound();
                }
           
        }




        [Route("api/AccountByID/{ACid:int}/{companyId:int}")]
        public IHttpActionResult GetHeadAccountByID(int ACid, int companyId)
        {

            if (ACid > 0)
            {
                try
                {
                    MVCAccountTableModel AccountObj = db.AccountTables.Where(x => x.FK_CompanyId == companyId && x.AccountId == ACid).Select(c => new MVCAccountTableModel
                    {
                        AccountId = c.AccountId,
                        AccountTitle = c.AccountTitle,
                        AccountDescription = c.AccountDescription,
                        FK_HeadAccountId = c.FK_HeadAccountId,
                        AccountCode = c.AccountCode

                    }).FirstOrDefault();

                    return Ok(AccountObj);

                }
                catch (Exception)
                {
                    return NotFound();
                }
            }
            else
            {
                return NotFound();
            }
        }


    


        [Route("api/AccountCheckStatus")]
        public IHttpActionResult PostUserEmailConformed(UserModel user)
        {
            try
            {

                bool IsConformed = db.AspNetUsers.Count(x => x.UserName == user.Username && x.EmailConfirmed == true) > 0;

                if (IsConformed == true)
                {
                    return Ok(user);
                }
                else
                {
                    return NotFound();
                }

            }
            catch (Exception ex)
            {
                return NotFound();
            }

        }


        [Route("api/AccountUpdateAspNetUserConformed/{code:alpha}")]
        public IHttpActionResult PutUpdateAspNetUser(string code, AspNetUser aspnetuser)
        {
            AspNetUser asp = db.AspNetUsers.Where(c => c.Id == code).FirstOrDefault();
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
