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


        [Route("Api/AddBill")]
        [ResponseType(typeof(BillTable))]
        public IHttpActionResult PostBillDetail([FromBody] BillTable billtable)
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

        [Route("api/GetbillDetail/{id:int}")]
        [ResponseType(typeof(MvcBillModel))]
        public IHttpActionResult GetBillDetail(int id)
        {
            MvcBillModel billModel = new MvcBillModel();

            billModel = db.BillTables.Where(q => q.BilID == id).Select(c => new MvcBillModel
            {
                BilID = c.BilID,
                Bill_ID = c.Bill_ID,
                BillDate =Convert.ToDateTime(c.BillDate),
                BillDueDate = Convert.ToDateTime(c.BillDueDate),
                SubTotal = c.SubTotal,
                TotalVat6 = c.TotalVat6,
                TotalVat21 = c.TotalVat21,
                DiscountAmount = c.DiscountAmount,
                TotalAmount = c.TotalAmount,
                CustomerNote = c.CustomerNote,
                Status = c.Status,
                UserId = c.UserId,
                CompanyId = c.CompanyId,
                VenderId = c.VenderId,
                Type = c.Type,

            }).FirstOrDefault();


            if (billModel == null)
            {
                return NotFound();
            }

            return Ok(billModel);
        }

    }
}
