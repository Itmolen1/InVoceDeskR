using InvoiceDiskLast.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace InvoiceDiskLast.Controllers
{


    public class APIPurchaseController : ApiController
    {
        
        private DBEntities db = new DBEntities();

        [ResponseType(typeof(MvcPurchaseModel))]
        public IHttpActionResult GetPurchaseTable(int id)
        {
            MvcPurchaseModel puchaseorder = new MvcPurchaseModel();

            try
            {
                var ob = db.PurchaseOrderTables.Find(id);
                puchaseorder.PurchaseID = ob.PurchaseID;
                puchaseorder.PurchaseOrderID = ob.PurchaseOrderID;
                puchaseorder.PurchaseRefNumber = ob.PurchaseRefNumber;
                puchaseorder.PurchaseDate = ob.PurchaseDate;
                puchaseorder.PurchaseDueDate = ob.PurchaseDueDate;
                puchaseorder.PurchaseSubTotal = ob.PurchaseSubTotal;
                puchaseorder.Vat6 = ob.Vat6;
                puchaseorder.Vat21 = ob.Vat21;
                puchaseorder.PurchaseDiscountAmount = ob.PurchaseDiscountAmount;
                puchaseorder.PurchaseTotoalAmount = ob.PurchaseTotoalAmount;
                puchaseorder.PurchaseVenderNote = ob.PurchaseVenderNote;
                puchaseorder.Status = ob.Status;
                puchaseorder.UserId = ob.UserId;
                puchaseorder.CompanyId = ob.CompanyId;
                puchaseorder.VenderId = ob.VenderId;
                puchaseorder.Type = ob.Type;
                puchaseorder.PurchaseID = ob.PurchaseID;

                if (puchaseorder == null)
                {
                    return NotFound();
                }

                return Ok(puchaseorder);
            }
            catch (Exception)
            {
                return NotFound();

            }


        }


        [Route("api/OrderListByStatus/{Status:alpha}")]
        public IHttpActionResult GetPurchaseOrderListByStatus(string Status)
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

                ob = db.PurchaseOrderTables.Where(p => p.CompanyId == id && p.Status.ToLower() == Status.ToLower()).ToList().Select(p => new MvcPurchaseModel
                {
                    PurchaseOrderID = Convert.ToInt32(p.PurchaseOrderID),
                    PurchaseID = p.PurchaseID,
                    PurchaseDate = (DateTime)p.PurchaseDate,
                    PurchaseDueDate = p.PurchaseDueDate,
                    PurchaseRefNumber = p.PurchaseRefNumber,
                    PurchaseSubTotal = p.PurchaseSubTotal,
                    PurchaseDiscountPercenteage = p.PurchaseDiscountPercenteage,
                    PurchaseDiscountAmount = p.PurchaseOrderID,
                    PurchaseVatPercentage = p.PurchaseVatPercentage,
                    PurchaseTotoalAmount = p.PurchaseTotoalAmount,
                    PurchaseVenderNote = p.PurchaseVenderNote,
                    Status = p.Status,
                    CompanyId = p.CompanyId,
                    Type = p.Type,
                    Vat21 = p.Vat21,
                    Vat6 = p.Vat6,
                    UserId = p.UserId,
                    AddedDate = p.AddedDate,
                }).ToList();

                return Ok(ob);
            }

            catch (Exception)
            {

                throw;
            }


        }


        //get list



        [Route("api/GetPurchaseInvoiceList/{type:alpha}/{CompanyId:int}")]
        public IHttpActionResult GetPurchaseOrder(string type, int CompanyId)
        {
            object ob = new object();
            try
            {

                IEnumerable<string> headerValues;
                //var DBLIST = "";
                var IDS = "";
                var Status = "";

                if (GlobalVeriables.WebApiClient.DefaultRequestHeaders.TryGetValues("CompayID", out headerValues))
                {
                    IDS = headerValues.FirstOrDefault();
                }
                int id = Convert.ToInt32(IDS);

                ob = db.PurchaseOrderTables.Where(p => p.CompanyId == CompanyId && p.Status != "accepted").ToList().Select(p => new MvcPurchaseModel
                {
                    PurchaseOrderID = Convert.ToInt32(p.PurchaseOrderID),
                    PurchaseID = p.PurchaseID,
                    PurchaseDate = (DateTime)p.PurchaseDate,
                    PurchaseDueDate = p.PurchaseDueDate,
                    PurchaseRefNumber = p.PurchaseRefNumber,
                    PurchaseSubTotal = p.PurchaseSubTotal,
                    PurchaseDiscountPercenteage = p.PurchaseDiscountPercenteage,
                    PurchaseDiscountAmount = p.PurchaseOrderID,
                    PurchaseVatPercentage = p.PurchaseVatPercentage,
                    PurchaseTotoalAmount = p.PurchaseTotoalAmount,
                    PurchaseVenderNote = p.PurchaseVenderNote,
                    Status = p.Status,
                    Type = p.Type,
                    CompanyId = p.CompanyId,
                    UserId = p.UserId,

                    AddedDate = p.AddedDate,
                }).ToList();

            }
            catch (Exception ex)
            {

            }

            return Ok(ob);

        }


        //// GET: api/APIQutation/5
        //[ResponseType(typeof(PurchaseOrderTable))]
        //public IHttpActionResult GetPurchaseTable(int id)
        //{
        //    PurchaseOrderTable PurchaseTable = db.PurchaseOrderTables.Find(id);
        //    if (PurchaseTable == null)
        //    {
        //        return NotFound();
        //    }

        //    return Ok(PurchaseTable);
        //}

        // PUT: api/APIQutation/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutPurchaseTable(int id, PurchaseOrderTable PurchaseTable)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != PurchaseTable.PurchaseOrderID)
            {
                return BadRequest();
            }

            db.Entry(PurchaseTable).State = EntityState.Modified;

            try
            {
                 db.SaveChanges();
                return Ok(PurchaseTable);
            }
            catch (Exception ex)
            {
                if (!PurchaseTableExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }


        }


        private bool PurchaseTableExists(int id)
        {
            return db.PurchaseOrderTables.Count(e => e.PurchaseOrderID == id) > 0;
        }


     
        [ResponseType(typeof(PurchaseOrderTable))]
        public IHttpActionResult PostPurchase([FromBody] PurchaseOrderTable Purchasetable)
        {
            using (DBEntities entities = new DBEntities())
            {
                Purchasetable = entities.PurchaseOrderTables.Add(Purchasetable);
                entities.SaveChanges();

                //var massage = Request.CreateResponse(HttpStatusCode.Created, Purchasetable);
                //massage.Headers.Location = new Uri(Request.RequestUri + Purchasetable.PurchaseOrderID.ToString());
                //massage.Content.Headers.Add("idd", Purchasetable.PurchaseOrderID.ToString());
                //massage.RequestMessage.Headers.Add("idd", Purchasetable.PurchaseOrderID.ToString());
                //return massage;

                return Ok(Purchasetable);

            }
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


        [ResponseType(typeof(PurchaseOrderTable))]
        public IHttpActionResult DeleteQutationTable(int id)
        {
            PurchaseOrderTable PurchaseTable = db.PurchaseOrderTables.Find(id);
            if (PurchaseTable == null)
            {
                return NotFound();
            }
            db.PurchaseOrderTables.Remove(PurchaseTable);
            db.SaveChanges();

            return Ok(PurchaseTable);
        }



        [Route("api/OrderListByStatusInvoice/{Status:alpha}")]
        public IHttpActionResult GetPurchaseOrderListBy(string Status)
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


                ob = (from p in db.ProductTables
                      join pdet in db.PurchaseOrderDetailsTables on p.ProductId equals pdet.PurchaseItemId
                      join po in db.PurchaseOrderTables on pdet.PurchaseId equals po.PurchaseOrderID
                      where p.Type == "Sirvice" && po.Status == Status && po.CompanyId == id
                      select new MvcPurchaseModel
                      {
                          PurchaseOrderID = po.PurchaseOrderID,
                          PurchaseID = po.PurchaseID,
                          PurchaseDate = po.PurchaseDate,
                          PurchaseDueDate = po.PurchaseDueDate,
                          PurchaseRefNumber = po.PurchaseRefNumber,
                          PurchaseSubTotal = po.PurchaseSubTotal,
                          PurchaseDiscountPercenteage = po.PurchaseDiscountPercenteage,
                          PurchaseDiscountAmount = po.PurchaseOrderID,
                          PurchaseVatPercentage = po.PurchaseVatPercentage,
                          PurchaseTotoalAmount = po.PurchaseTotoalAmount,
                          PurchaseVenderNote = po.PurchaseVenderNote,
                          Status = po.Status,
                          CompanyId = po.CompanyId,
                          UserId = po.UserId,
                          AddedDate = p.AddedDate,
                      }).ToList();


                return Ok(ob);
            }

            catch (Exception)
            {

                throw;
            }


        }


        [Route("api/GetPurchaseServiceList/{type:alpha}/{CompanyId:int}")]
        public IHttpActionResult GetPurchaseServiceList1(string type, int CompanyId)
        {
            object ob = new object();
            try
            {

                IEnumerable<string> headerValues;
                //var DBLIST = "";
                var IDS = "";
                var Status = "";

                if (GlobalVeriables.WebApiClient.DefaultRequestHeaders.TryGetValues("CompayID", out headerValues))
                {
                    IDS = headerValues.FirstOrDefault();
                }
                int id = Convert.ToInt32(IDS);

                ob = db.PurchaseOrderTables.Where(p => p.CompanyId == CompanyId && p.Type == type && p.Status == "accepted").ToList().Select(p => new MvcPurchaseModel
                {
                    PurchaseOrderID = Convert.ToInt32(p.PurchaseOrderID),
                    PurchaseID = p.PurchaseID,
                    PurchaseDate = (DateTime)p.PurchaseDate,
                    PurchaseDueDate = p.PurchaseDueDate,
                    PurchaseRefNumber = p.PurchaseRefNumber,
                    PurchaseSubTotal = p.PurchaseSubTotal,
                    PurchaseDiscountPercenteage = p.PurchaseDiscountPercenteage,
                    PurchaseDiscountAmount = p.PurchaseOrderID,
                    PurchaseVatPercentage = p.PurchaseVatPercentage,
                    PurchaseTotoalAmount = p.PurchaseTotoalAmount,
                    PurchaseVenderNote = p.PurchaseVenderNote,
                    Status = p.Status,
                    Type = p.Type,
                    CompanyId = p.CompanyId,
                    UserId = p.UserId,

                    AddedDate = p.AddedDate,
                }).ToList();

            }
            catch (Exception ex)
            {

            }

            return Ok(ob);

        }

        [Route("api/GetPurchaseGoodsList/{type:alpha}/{CompanyId:int}")]
        public IHttpActionResult GetPurchaseGoodsList1(string type, int CompanyId)
        {
            object ob = new object();
            try
            {

                IEnumerable<string> headerValues;
                //var DBLIST = "";
                var IDS = "";
                var Status = "";

                if (GlobalVeriables.WebApiClient.DefaultRequestHeaders.TryGetValues("CompayID", out headerValues))
                {
                    IDS = headerValues.FirstOrDefault();
                }
                int id = Convert.ToInt32(IDS);

                ob = db.PurchaseOrderTables.Where(p => p.CompanyId == CompanyId && p.Type == type).ToList().Select(p => new MvcPurchaseModel
                {
                    PurchaseOrderID = Convert.ToInt32(p.PurchaseOrderID),
                    PurchaseID = p.PurchaseID,
                    PurchaseDate = (DateTime)p.PurchaseDate,
                    PurchaseDueDate = p.PurchaseDueDate,
                    PurchaseRefNumber = p.PurchaseRefNumber,
                    PurchaseSubTotal = p.PurchaseSubTotal,
                    PurchaseDiscountPercenteage = p.PurchaseDiscountPercenteage,
                    PurchaseDiscountAmount = p.PurchaseOrderID,
                    PurchaseVatPercentage = p.PurchaseVatPercentage,
                    PurchaseTotoalAmount = p.PurchaseTotoalAmount,
                    PurchaseVenderNote = p.PurchaseVenderNote,
                    Status = p.Status,
                    Type = p.Type,
                    CompanyId = p.CompanyId,
                    UserId = p.UserId,

                    AddedDate = p.AddedDate,
                }).ToList();

            }
            catch (Exception ex)
            {

            }

            return Ok(ob);

        }


        [Route("api/GetPurchaseItemQTY/{id:int}/{companyid:int}")]
        public IHttpActionResult GetSaleQuantity(int id, int companyid)
        {
            MVCPurchaseDetailsModel _MvcPurchaseDetailsModel = new MVCPurchaseDetailsModel();
            try
            {
                var q = (from p in db.PurchaseOrderTables
                         join pd in db.PurchaseOrderDetailsTables on p.PurchaseOrderID equals pd.PurchaseId
                         where p.Status == "accepted" && pd.PurchaseItemId == id && p.CompanyId == companyid
                         select pd).Sum(i => i.PurchaseQuantity);
              
                _MvcPurchaseDetailsModel.PurchaseQuantity = q;

                return Ok(_MvcPurchaseDetailsModel);
            }

            catch (Exception ex)
            {
                return NotFound();
            }

        }

    }
}
