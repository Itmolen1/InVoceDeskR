using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using InvoiceDiskLast.Models;

namespace InvoiceDiskLast.Controllers
{
    public class UserController : ApiController
    {
        private DBEntities db = new DBEntities();

        [Authorize]
        public string GetUserCheck()
        {

            string username = GlobalVeriables.WebApiClient.DefaultRequestHeaders.GetValues("Username").First().ToString();
            // string password = GlobalVeriables.WebApiClient.DefaultRequestHeaders.GetValues("Password").ToString();


            //bool IsuserExist = db.UserTables.Any(x => x.Username == username);

            //UserModels userm = db.UserTables.ToList().Where(x => x.Username == username).Select(c => new UserModels
            //{

            //    UserId = c.UserId,
            //    UserFname = c.UserFname
            //}).FirstOrDefault();


            bool IsuserExist = true;
            if (IsuserExist)
            {
                Guid guid = new Guid();
                return guid.ToString();
            }
            else
            {
                return "User does not exist!";
            }
        }
    }
}
