using InvoiceDiskLast.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace InvoiceDiskLast.Controllers
{
    public class APIControlAccountController : ApiController
    {
        private DBEntities db = new DBEntities();

        [Route("api/ControlAccount")]
        public IQueryable<ControlAccountTable> GetControlAccount()
        {

            return db.ControlAccountTables;
        }
    }
}

