using InvoiceDiskLast.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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

        [Route("api/GetbillDetails/{CompanyID:int}")]
        [ResponseType(typeof(MvcBillModel))]
        public IHttpActionResult GetBillDetails(int CompanyID)
        {
            List<BillDetailViewModel> BillList = new List<BillDetailViewModel>();

            BillList = db.BillTables.Where(q => q.CompanyId == CompanyID).Select(c => new BillDetailViewModel
            {
                BilID = c.BilID,
                Bill_ID = c.Bill_ID,
                BillDate = c.BillDate,
                BillDueDate = c.BillDueDate,
                RefNumber = c.RefNumber,
                SubTotal = c.SubTotal,
                TotalVat6 = c.TotalVat6,
                TotalVat21 = c.TotalVat21,
                DiscountAmount = c.DiscountAmount,
                TotalAmount = c.TotalAmount,
                CustomerNote = c.CustomerNote,
                Status = c.Status,
                UserId = c.UserId,
                ContactName = c.ContactsTable.ContactName,
                SalePerson = c.UserTable.Username,
                VatAmount= c.TotalVat21+ c.TotalVat6,
                Total=c.TotalAmount,
                CompanyId = c.CompanyId,
                VenderId = c.VenderId,
                Type = c.Type,

            }).ToList();


            if (BillList == null)
            {
                return NotFound();
            }

            return Ok(BillList);
        }

        [Route("api/GenrateBilNumber")]
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

        [Route("api/AddBill")]
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
                BillDate = c.BillDate,
                BillDueDate = c.BillDueDate,
                RefNumber = c.RefNumber,
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

        [ResponseType(typeof(void))]
        public IHttpActionResult PutPurchaseTable(int id, BillTable billTable)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != billTable.BilID)
            {
                return BadRequest();
            }

            db.Entry(billTable).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
                return Ok(billTable);
            }
            catch (Exception ex)
            {
                return NotFound();
            }


        }

        [Route("api/GetBillIdbyPurchaseId/{Id:int}")]
        public IHttpActionResult GetbillIdbyPurchaseId(int Id)
        {
            try
            {
                MvcBillModel _BillDetailModel = new MvcBillModel();
                _BillDetailModel.BilID = db.BillTables.Where(I => I.PurchaseId == Id).FirstOrDefault().BilID;
                return Ok(_BillDetailModel);

            }
            catch (Exception)
            {
                NotFound();
                throw;
            }
        }

    }
}
