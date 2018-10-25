using InvoiceDiskLast.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace InvoiceDiskLast.Controllers
{
    public class GetQuationCountController : ApiController
    {
        private DBEntities db = new DBEntities();

     
        [ResponseType(typeof(MVCQutationModel))]
        public IHttpActionResult GetQutationCount()
        {
            MVCQutationModel q = new MVCQutationModel();

            int quataionId = db.QutationTables.ToList().Count()+1;
            q.Qutation_ID = quataionId.ToString();

            if (quataionId == 0)
            {
                return Ok(q);
            }
            else
            {
                return Ok(q);
            }
        }
    }
}
