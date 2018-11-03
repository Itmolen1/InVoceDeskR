using InvoiceDiskLast.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace InvoiceDiskLast.Controllers
{
    public class MVCControlAccountController : ApiController
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
    }
}
    