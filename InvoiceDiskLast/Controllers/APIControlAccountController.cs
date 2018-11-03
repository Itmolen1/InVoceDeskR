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

        [Route("api/GetControlAccount")]
        public IHttpActionResult GetControlAccount()
        {

           List<MVCControlAccountModel> table =  new List<MVCControlAccountModel>();
            // return  table = db.ControlAccountTables.ToList();


           // table = db.ControlAccountTables.ToList();
            table = db.ControlAccountTables.Select(c => new MVCControlAccountModel
            {

                ControlAccountId = c.ControlAccountId,
                ControleAccountTitile = c.ControleAccountTitile,
                ControlAcooountDescription = c.ControlAcooountDescription

            }).ToList();


            return Ok(table);
        }
    }
}

