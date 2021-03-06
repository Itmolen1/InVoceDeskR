﻿using InvoiceDiskLast.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace InvoiceDiskLast.Controllers
{
    public class APIQutationController : ApiController
    {

        private DBEntities db = new DBEntities();

        // GET: api/APIQutation
        public IHttpActionResult GetQutationTables()
        {
            List<QutationIndexViewModel> QutationModelObj = new List<QutationIndexViewModel>();
            IEnumerable<string> headerValues;
            //var DBLIST = "";
            var IDS = "";
            int id = 0;
            if (GlobalVeriables.WebApiClient.DefaultRequestHeaders.TryGetValues("CompayID", out headerValues))
            {
                IDS = headerValues.FirstOrDefault();
                id = Convert.ToInt32(IDS);
            }
            var Qutationob = db.QutationTables.Where(x => x.CompanyId == id && x.Status != "accepted").Select(c => new QutationIndexViewModel
            {

                QutationID = c.QutationID,
                Qutation_ID = c.Qutation_ID,
                RefNumber = c.RefNumber,
                QutationDate = c.QutationDate,
                DueDate = c.DueDate,
                Vat = c.TotalVat6 + c.TotalVat21,
                TotalAmount = c.TotalAmount,
                Status = (c.Status).Trim(),
                //UserName = c.UserTable.UserFname + " " + c.UserTable.UserLname,
                UserName = (c.UserTable.UserFname + " " + c.UserTable.UserLname != "" ? c.UserTable.UserFname + " " + c.UserTable.UserLname : c.UserTable.Username),
                CustomerName = c.ContactsTable.ContactName

            }).ToList();

            return Ok(Qutationob);


        }



        [Route("api/GetQuotationReport/{QuotationId:int}")]
        public IHttpActionResult GetQuotationReport(int QuotationId)
        {
            try
            {





                //var Query = db.SpQuotationReport(QuotationId).ToList();
                ////{                  
                ////    Qutation_ID = C.Qutation_ID,
                ////    RefNumber = C.RefNumber,
                ////    QutationDate = C.QutationDate,
                ////    DueDate = C.DueDate,
                ////    SubTotal = C.SubTotal,
                ////    TotalVat6 = C.TotalVat6,
                ////    TotalVat21 = C.TotalVat21,
                ////    TotalAmount = C.TotalAmount,
                ////    CustomerNote = C.CustomerNote,

                ////    Type = C.Type,

                ////}).ToList();

                return Ok();
            }
            catch (Exception)
            {
                NotFound();
                throw;
            }
        }



        // GET: api/APIQutation/5
        [ResponseType(typeof(QutationTable))]
        public IHttpActionResult GetQutationTable(int id)
        {
            MVCQutationModel qutationmodel = new MVCQutationModel();

            qutationmodel = db.QutationTables.Where(q => q.QutationID == id).Select(c => new MVCQutationModel
            {
                QutationID = c.QutationID,
                Qutation_ID = c.Qutation_ID,
                RefNumber = c.RefNumber,
                QutationDate = c.QutationDate,
                DueDate = c.DueDate,
                SubTotal = c.SubTotal,
                TotalVat6 = c.TotalVat6,
                TotalVat21 = c.TotalVat21,
                DiscountAmount = c.DiscountAmount,
                TotalAmount = c.TotalAmount,
                CustomerNote = c.CustomerNote,
                Status = c.Status,
                UserId = c.UserId,
                CompanyId = c.CompanyId,
                ContactId = c.ContactId,
                Type = c.Type,

            }).FirstOrDefault();


            if (qutationmodel == null)
            {
                return NotFound();
            }

            return Ok(qutationmodel);
        }

        // PUT: api/APIQutation/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutQutationTable(int id, QutationTable qutationTable)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != qutationTable.QutationID)
            {
                return BadRequest();
            }

            db.Entry(qutationTable).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
                return StatusCode(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [ResponseType(typeof(MVCQutationViewModel))]
        public IHttpActionResult PostQutationTable([FromBody] QutationTable qutationtable)
        {
            using (DBEntities entities = new DBEntities())
            {
                try
                {
                    qutationtable = entities.QutationTables.Add(qutationtable);
                    entities.SaveChanges();

                    return Ok(qutationtable);
                }
                catch (Exception ex)
                {
                    return BadRequest();
                }

            }
        }


        //[ResponseType(typeof(QutationTable))]
        //public IHttpActionResult PostQutationTable([FromBody] QutationTable qutationtable)
        //{
        //    using (DBEntities entities = new DBEntities())
        //    {
        //        try
        //        {
        //            qutationtable = entities.QutationTables.Add(qutationtable);
        //            entities.SaveChanges();

        //            return Ok(qutationtable);
        //        }
        //        catch (Exception ex)
        //        {
        //            return BadRequest();
        //        }

        //    }
        //}


        [ResponseType(typeof(QutationTable))]
        public IHttpActionResult DeleteQutationTable(int id)
        {
            QutationTable qutationTable = db.QutationTables.Find(id);
            if (qutationTable == null)
            {
                return NotFound();
            }

            db.QutationTables.Remove(qutationTable);
            db.SaveChanges();

            return Ok(qutationTable);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool QutationTableExists(int id)
        {
            return db.QutationTables.Count(e => e.QutationID == id) > 0;
        }


        [Route("api/QutationOrderListByStatus/{Status:alpha}")]
        public IHttpActionResult GetQutationOrderListByStatus(string Status)
        {
            object ob = new object();
            try
            {
                var IDS = "";
                int id = 0;
                IEnumerable<string> headerValues;
                if (GlobalVeriables.WebApiClient.DefaultRequestHeaders.TryGetValues("CompayID", out headerValues))
                {
                    IDS = headerValues.FirstOrDefault();
                    id = Convert.ToInt32(IDS);
                }
                ob = db.QutationTables.Where(p => p.CompanyId == id && p.Status.ToLower() == Status.ToLower()).Select(p => new MVCQutationModel
                {
                    QutationID = p.QutationID,
                    Qutation_ID = p.Qutation_ID,
                    QutationDate = p.QutationDate,
                    DueDate = p.DueDate,
                    RefNumber = p.RefNumber,
                    SubTotal = p.SubTotal,
                    ContactName = p.ContactsTable.ContactName,
                    SalePerson = (p.UserTable.UserFname + " " + p.UserTable.UserLname != "" ? p.UserTable.UserFname + " " + p.UserTable.UserLname : p.UserTable.Username),

                    TotalVat = p.TotalVat21 + p.TotalVat6,
                    Status = p.Status,
                    TotalAmount=p.TotalAmount,
                    CompanyId = p.CompanyId,
                    UserId = p.UserId,
                    Type = p.Type,


                }).ToList();

                return Ok(ob);
            }

            catch (Exception)
            {

                throw;
            }
        }







        [Route("api/GetAcceptedInvoices/{companyId:int}")]
        public IHttpActionResult GetQutationOrderListByStatus(int companyId)
        {
            object ob = new object();
            try
            {

                ob = db.InvoiceTables.Where(p => p.CompanyId == companyId).Select(p => new InvoiceViewModel
                {
                    InvoiceID = p.InvoiceID,
                    InvoiceDate = p.InvoiceDate,
                    InvoiceDueDate = p.InvoiceDueDate,
                    CustomerName = p.ContactsTable.ContactName,
                    RefNumber = p.RefNumber,
                    SubTotal = p.SubTotal,
                    UserName = (p.UserTable.UserFname != null && p.UserTable.UserLname != null ? p.UserTable.UserFname + " " + p.UserTable.UserLname : p.UserTable.Username),
                    Vat = p.TotalVat21 + p.TotalVat6,
                    Status = p.Status,
                    TotalAmount = p.TotalAmount,
                    CompanyId = p.CompanyId,
                    UserId = p.UserId,
                    Type = p.Type,


                }).ToList();

                return Ok(ob);
            }

            catch (Exception)
            {

                throw;
            }
        }





        [Route("api/QutationOrderListByStatus12/{Status:alpha}")]
        public IHttpActionResult GetQutationOrderListByStatus12(string Status)
        {
            object ob = new object();
            try
            {
                var IDS = "";
                int id = 0;
                IEnumerable<string> headerValues;
                if (GlobalVeriables.WebApiClient.DefaultRequestHeaders.TryGetValues("CompayID", out headerValues))
                {
                    IDS = headerValues.FirstOrDefault();
                    id = Convert.ToInt32(IDS);
                }
                ob = db.QutationTables.Where(p => p.CompanyId == id && p.Status.ToLower() == Status.ToLower()).ToList().Select(p => new MVCQutationModel
                {
                    QutationID = Convert.ToInt32(p.QutationID),
                    Qutation_ID = p.Qutation_ID,
                    QutationDate = (DateTime)p.QutationDate,
                    DueDate = p.DueDate,
                    RefNumber = p.RefNumber,
                    SubTotal = p.SubTotal,
                    Status = p.Status,
                    CompanyId = p.CompanyId,
                    UserId = p.UserId,
                    Type = p.Type,


                }).ToList();

                return Ok(ob);
            }

            catch (Exception)
            {

                throw;
            }
        }

        #region
        [Route("api/GetQuatationSerViceList/{Type:alpha}/{CompanyId:int}")]
        public IHttpActionResult GetQutationListForInvoiceService(string Type, int CompanyId)
        {
            List<MVCQutationViewModel> _qutationList = new List<MVCQutationViewModel>();
            try
            {
                _qutationList = db.QutationTables.Where(q => q.Type == Type && q.CompanyId == CompanyId && q.Status == "accepted").Select(p => new MVCQutationViewModel
                {
                    QutationID = p.QutationID,
                    Qutation_ID = p.Qutation_ID,
                    QutationDate = p.QutationDate,
                    DueDate = p.DueDate,
                    RefNumber = p.RefNumber,
                    SubTotal = p.SubTotal,
                    Status = (p.Status).Trim(),
                    CompanyId = p.CompanyId,
                    UserId = p.UserId,
                }).ToList();
            }
            catch (Exception)
            {
                return BadRequest();

            }

            return Ok(_qutationList);


        }

        #endregion

        [Route("api/CheckProductQuantity/{ProductId:int}")]
        public IHttpActionResult GetCheckQuantity(int ProductId)
        {
            MVCQutationViewModel _MvcQuatation = new MVCQutationViewModel();

            int PurchaseQuantity = 0;
            int qutationquantity = 0;

            try
            {
                PurchaseQuantity = Convert.ToInt32(db.PurchaseOrderDetailsTables.Where(t => t.PurchaseItemId == ProductId).Sum(i => i.PurchaseQuantity));
                qutationquantity = Convert.ToInt32(db.QutationDetailsTables.Where(t => t.ItemId == ProductId).Sum(i => i.Quantity));
                _MvcQuatation.QuantityRemaing = PurchaseQuantity - qutationquantity;

                return Ok(_MvcQuatation);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }

        }



        [Route("api/GetProductQuantit/{ProductId:int}")]
        public IHttpActionResult GetProductQuantityPurchase(int ProductId)
        {
            MVCQutationViewModel _MvcQuatation = new MVCQutationViewModel();

            int PurchaseQuantity = 0;

            try
            {

                var q = (from p in db.PurchaseOrderTables
                         join pd in db.PurchaseOrderDetailsTables on p.PurchaseOrderID equals pd.PurchaseId
                         where p.Status == "accepted" && pd.PurchaseItemId == ProductId
                         select pd).Sum(i => i.PurchaseQuantity);



                // PurchaseQuantity = Convert.ToInt32(db.PurchaseOrderDetailsTables.Where(t => t.PurchaseItemId == ProductId).Sum(i => i.PurchaseQuantity));
                _MvcQuatation.QuantityRemaing = q;

                return Ok(_MvcQuatation);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }

        }


        [Route("api/GetQuatationListForQuantityCheck/{QutationId:int}")]
        public IHttpActionResult GetQutationDetailList(int QutationId)
        {
            List<MVCQutationViewModel> _MvcQutationViewModel = new List<MVCQutationViewModel>();

            try
            {
                _MvcQutationViewModel = (from c in db.QutationDetailsTables
                                         where c.QutationID == QutationId
                                         group c by c.ItemId into g
                                         select new MVCQutationViewModel
                                         {
                                             ItemId = g.Key,
                                             QuantityRemaing = g.Sum(oi => oi.Quantity),

                                         }).ToList();



                return Ok(_MvcQutationViewModel);
            }
            catch (Exception)
            {
            }

            return Ok();
        }

        [Route("api/GetSaleItemQty/{id:int}/{companyid:int}")]
        public IHttpActionResult GetSaleQuantity(int id, int companyid)
        {
            MVCQutationViewModel _MvcQutationViewModel = new MVCQutationViewModel();
            try
            {

                var q = (from p in db.QutationTables
                         join pd in db.QutationDetailsTables on p.QutationID equals pd.QutationID
                         where p.Status == "accepted" && pd.QutationID == id && p.CompanyId == companyid
                         select pd).Sum(i => i.Quantity);

                _MvcQutationViewModel.QuantityRemaing = q;

                return Ok(_MvcQutationViewModel);
            }

            catch (Exception ex)
            {
                return NotFound();
            }

        }

        [Route("api/GetUpdateQuatationStatus/{Id:int}")]
        public IHttpActionResult GetUpdateQuatationStatus(int Id)
        {
            QutationTable qt = new QutationTable();

            try
            {
                qt = db.QutationTables.Where(q => q.QutationID == Id).FirstOrDefault();

                if (qt != null)
                {
                    qt.Status = "accepted";
                    db.Entry(qt).State = EntityState.Modified;
                    db.SaveChanges();
                    return Ok();
                }
                return NotFound();
            }
            catch (Exception)
            {
                return BadRequest();
                throw;
            }
        }



    }
}
