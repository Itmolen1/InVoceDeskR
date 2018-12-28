using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using InvoiceDiskLast.Models;

namespace InvoiceDiskLast.Controllers
{
    public class UsersController : ApiController
    {
        private DBEntities db = new DBEntities();

        [Authorize]
        public IHttpActionResult GetUsers()
        {

            string username = GlobalVeriables.WebApiClient.DefaultRequestHeaders.GetValues("Username").First().ToString();          
            string password = GlobalVeriables.WebApiClient.DefaultRequestHeaders.GetValues("Password").First().ToString();


            //bool IsuserExist = db.UserTables.Any(x => x.Username == username && x.Password == password);            
            bool IsuserExist = true;
            if (IsuserExist)
            {
                UserModels userm = db.UserTables.ToList().Where(x => x.CompanyId == 1).Select(c => new UserModels
                {

                    UserId = c.UserId,
                    UserFname = c.UserFname,
                    Insertion = c.Insertion,
                    UserLname = c.UserLname,
                    //Username = c.aspn.Username,
                    DOB = c.DOB

                }).FirstOrDefault();

                return Ok(userm);
               
            }
            else
            {
                return NotFound();
            }
        }

        [Route("api/GetAuthorityList")]
        public IHttpActionResult GetAuthorityList()
        {
            try
            {
                List<AuthorityModel> AuthoritModel = db.UserAuthorityTables.Select(c => new AuthorityModel
                {

                    UserAuthorityId = c.UserAuthorityId,
                    Authority = c.Authority, 
                    CompanyId = c.CompanyId
                   
                }).ToList();

                return Ok(AuthoritModel);
            }
            catch(Exception)
            {
                return NotFound();
            }
        }
    }
}
