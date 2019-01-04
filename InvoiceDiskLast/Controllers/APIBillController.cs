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
    public class APIBillController : ApiController
    {
        private DBEntities db = new DBEntities();



        [Route("Api/GenrateBilNumber")]
        [ResponseType(typeof(MvcBillModel))]
        public IHttpActionResult GetBillNumber()
        {
            MvcBillModel _billModel = new MvcBillModel();

            int biId = db.BillTables.ToList().Count() + 1;
            _billModel.Bill_ID = biId.ToString();

            if (biId == 0)
            {
                return Ok(_billModel);
            }
            else
            {
                return Ok(_billModel);
            }
        }


        [ResponseType(typeof(BillTable))]
        public IHttpActionResult PostPurchase([FromBody] BillTable billtable)
        {
            using (DBEntities entities = new DBEntities())
            {
                try
                {
                    billtable = entities.BillTables.Add(billtable);
                    entities.SaveChanges();
                    return Ok(billtable);
                }
                catch (Exception)
                {
                    return BadRequest();

                }
            }
        }
    }
}
