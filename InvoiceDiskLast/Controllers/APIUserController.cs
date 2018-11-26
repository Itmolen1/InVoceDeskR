using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using InvoiceDiskLast.Models;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace InvoiceDiskLast.Controllers
{
    public class APIUserController : ApiController
    {
        DBEntities db = new DBEntities();

        [Route("api/GetUserbyEmail/{username:alpha}")]
        public IHttpActionResult GetUserByEmail(string username)
        {           
            try
            {
                var RegEmailId = (from u in db.AspNetUsers
                                  where u.UserName.ToUpper() == username.ToUpper()
                                  select new { username }).FirstOrDefault();

                bool status;
                if (RegEmailId != null)
                {
                    return Ok(RegEmailId);
                    //Already registered                  
                }
                else
                {
                    //Available to use  
                    return BadRequest();
                }

              
            }

            catch (Exception ex)
            {
                return BadRequest();
            }        
        }


        [Route("api/GetUserInfo/{companyId:int}")]
        public IHttpActionResult GetUserInfo(int companyId)
        {
            try
            {
                UserModel userModel = db.UserTables.Where(x => x.CompanyId == companyId).Select(c => new UserModel
                {
                    UserId = c.UserId,
                    UserFname = c.UserFname,
                    Insertion = c.Insertion,
                    UserLname = c.UserLname,
                    Username = c.ComapnyInfo.UserName,
                    Gender = c.Gender,
                    DOB = c.DOB,
                    ImageUrl = c.ImageUrl,
                    AddedDate = c.AddedDate

                }).FirstOrDefault();

                return Ok(userModel);
            }
            catch(Exception ex)
            {
                return NotFound();
            }
        }


        [Route("api/PutUserInfo")]
        public IHttpActionResult PustuserInfo(UserTable userTabe)
        {
            int companyid = Convert.ToInt32(userTabe.CompanyId);
            if (companyid != userTabe.CompanyId)
            {
                return BadRequest();
            }

            db.Entry(userTabe).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
                return Ok(userTabe);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserTableExists(companyid))
                {
                    return NotFound();
                }
                else
                {
                    return NotFound();
                }
            }
        }


        [Route("api/UpdateUserImage")]
        public IHttpActionResult PutUserImage(UserTable usertable)
        {
            UserTable usertble = db.UserTables.Where(c => c.UserId == usertable.UserId).FirstOrDefault();
            usertble.ImageUrl = usertable.ImageUrl;
            db.Entry(usertble).State = EntityState.Modified;
            try
            {
                db.SaveChanges();
                return Ok(usertable);
            }
            catch (DbUpdateConcurrencyException)
            {
                return NotFound();

            }
        }

        private bool UserTableExists(int id)
        {
            return db.UserTables.Count(e => e.CompanyId == id) > 0;
        }
    }
}

