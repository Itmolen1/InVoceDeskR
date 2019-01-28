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

        string base64Guid = Convert.ToBase64String(Guid.NewGuid().ToByteArray());

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
                    VatAmount = p.Vat21 + p.Vat6,
                    CustomerName = p.ContactsTable.ContactName,
                    SalePerson = (p.UserTable.UserFname + " " + p.UserTable.UserLname != "" ? p.UserTable.UserFname + " " + p.UserTable.UserLname : p.UserTable.Username),

                    TotalAmount = p.PurchaseTotoalAmount,


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
                    CustomerName = p.ContactsTable.ContactName,
                    SalePerson = (p.UserTable.UserFname + " " + p.UserTable.UserLname != "" ? p.UserTable.UserFname + " " + p.UserTable.UserLname : p.UserTable.Username),
                    VatAmount = p.Vat21 + p.Vat6,
                    TotalAmount = p.PurchaseTotoalAmount,
                    Type = p.Type,
                    CompanyId = p.CompanyId,
                    UserId = p.UserId,
                    AddedDate = p.AddedDate,
                }).ToList();
            }
            catch (Exception ex)
            {
                throw ex;

            }

            return Ok(ob);

        }


        [Route("api/GetUpdate/{Id:int}")]
        public IHttpActionResult GetUpdatePurchaseStatus1(int Id)
        {
            PurchaseOrderTable po = new PurchaseOrderTable();
            try
            {
                po = db.PurchaseOrderTables.Where(q => q.PurchaseOrderID == Id).FirstOrDefault();

                if (po != null)
                {
                    po.Status = "accepted";
                    db.Entry(po).State = EntityState.Modified;
                    db.SaveChanges();

                    return Ok();
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception)
            {
                return BadRequest();
                throw;
            }
        }

        // PUT: api/APIQutation/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutPurchaseTable(int id, MvcPurchaseViewModel PViewModel)
        {
            string Ref = PViewModel.PurchaseOrderID.ToString();
            //accountTransictionTable.CreationTime = null;

            PurchaseOrderTable PTable = new PurchaseOrderTable();

            using (DBEntities _dbcotext = new DBEntities())
            {
                using (DbContextTransaction transaction = _dbcotext.Database.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
                {
                    try
                    {

                        PTable.PurchaseID = PViewModel.PurchaseId.ToString();
                        PTable.PurchaseOrderID = Convert.ToInt32(PViewModel.PurchaseOrderID);
                        PTable.PurchaseRefNumber = PViewModel.PurchaseRefNumber;
                        PTable.CompanyId = PViewModel.CompanyId;
                        PTable.VenderId = PViewModel.VenderId;
                        PTable.UserId = PViewModel.UserId;
                        PTable.PurchaseDate = PViewModel.PurchaseDate;
                        PTable.PurchaseDueDate = PViewModel.PurchaseDueDate;
                        PTable.PurchaseSubTotal = PViewModel.PurchaseSubTotal;
                        PTable.PurchaseDiscountAmount = PViewModel.PurchaseDiscountAmount;
                        PTable.PurchaseTotoalAmount = PViewModel.PurchaseTotoalAmount;
                        PTable.PurchaseVenderNote = PViewModel.PurchaseVenderNote;
                        PTable.Vat6 = PViewModel.Vat6;
                        PTable.Vat21 = PViewModel.Vat21;
                        PTable.Status = "open";
                        PTable.Type = StatusEnum.Goods.ToString();

                        _dbcotext.Entry(PTable).State = EntityState.Modified;
                        _dbcotext.SaveChanges();

                        if (PViewModel.PurchaseOrderList != null)
                        {
                            foreach (PurchaseOrderDetailsTable item in PViewModel.PurchaseOrderList)
                            {
                                PurchaseOrderDetailsTable DTable = new PurchaseOrderDetailsTable();
                                DTable.PurchaseOrderDetailsId = item.PurchaseOrderDetailsId;
                                DTable.PurchaseItemId = item.PurchaseItemId;
                                DTable.PurchaseDescription = item.PurchaseDescription;
                                DTable.PurchaseQuantity = item.PurchaseQuantity;
                                DTable.PurchaseItemRate = item.PurchaseItemRate;
                                DTable.Type = item.Type;
                                DTable.RowSubTotal = item.RowSubTotal;
                                DTable.PurchaseDescription = item.PurchaseDescription;
                                PTable.VenderId = PViewModel.VenderId;
                                DTable.ServiceDate = item.ServiceDate;
                                PTable.CompanyId = PViewModel.CompanyId;
                                DTable.PurchaseTotal = item.PurchaseTotal;
                                DTable.PurchaseVatPercentage = item.PurchaseVatPercentage;
                                DTable.PurchaseId = PViewModel.PurchaseOrderID;

                                if (DTable.PurchaseOrderDetailsId == 0)
                                {
                                    _dbcotext.PurchaseOrderDetailsTables.Add(DTable);
                                    _dbcotext.SaveChanges();
                                }
                                else
                                {
                                    _dbcotext.Entry(DTable).State = EntityState.Modified;
                                    _dbcotext.SaveChanges();
                                }
                            }

                            AccountTransictionTable Account = new AccountTransictionTable();
                            Account.TransictionDate = DateTime.Now;
                            Account.TransictionNumber = base64Guid;
                            Account.TransictionType = "Purchase";
                            Account.CreationTime = DateTime.Now.TimeOfDay;
                            Account.AddedBy = PViewModel.UserId;
                            Account.FK_CompanyId = PViewModel.CompanyId;
                            Account.FKPaymentTerm = 1;
                            Account.TransictionRefrenceId = PTable.PurchaseOrderID.ToString();

                            //Accounts payable  Transaction
                            List<AccountTransictionTable> List = new List<AccountTransictionTable>();
                            List = _dbcotext.AccountTransictionTables.Where(t => t.TransictionRefrenceId == Ref.ToString()).ToList();
                            _dbcotext.AccountTransictionTables.RemoveRange(_dbcotext.AccountTransictionTables.Where(F => F.TransictionRefrenceId == Ref.ToString()));
                            _dbcotext.SaveChanges();

                            int Accountpayble = AccountIdByName("Accounts payable", (int)PViewModel.CompanyId);

                            if (Accountpayble != 0)
                            {
                                Account.Dr = 0.00;
                                Account.Cr = PViewModel.PurchaseTotoalAmount;
                                Account.FK_AccountID = Accountpayble;
                                _dbcotext.AccountTransictionTables.Add(Account);
                                _dbcotext.SaveChanges();
                            }

                            // Cost Of Goods Transaction
                            int CostOfGood = AccountIdByName("Cost Of Goods", (int)PViewModel.CompanyId);
                            if (CostOfGood != 0)
                            {
                                Account.Cr = 0.00;
                                Account.Dr = PViewModel.PurchaseSubTotal;
                                Account.FK_AccountID = CostOfGood;
                                _dbcotext.AccountTransictionTables.Add(Account);
                                _dbcotext.SaveChanges();
                            }

                            // Input VAT Trnsaction
                            int VatAccountId = AccountIdByName("Input VAT", (int)PViewModel.CompanyId);

                            if (VatAccountId != 0)
                            {
                                Account.Cr = 0.00;
                                Account.Dr = PViewModel.Vat21 + PViewModel.Vat6;
                                Account.FK_AccountID = VatAccountId;
                                _dbcotext.AccountTransictionTables.Add(Account);
                                int Id = _dbcotext.SaveChanges();
                            }

                            MvcPurchaseModel pModel = new MvcPurchaseModel();
                            pModel.PurchaseOrderID = PViewModel.PurchaseOrderID;
                            transaction.Commit();
                            return Ok(pModel);
                        }
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        return BadRequest();
                    }
                    return BadRequest();
                }
            }
            /////db.Entry(PurchaseTable).State = EntityState.Modified;          
            //db.SaveChanges();
            //        return Ok(PurchaseTable);
        }


        private bool PurchaseTableExists(int id)
        {
            return db.PurchaseOrderTables.Count(e => e.PurchaseOrderID == id) > 0;
        }


        public int AccountIdByName(string Title, int CompanyId)
        {
            int AccountId = 0;
            try
            {
                AccountTable Act = db.AccountTables.Where(Ac => Ac.AccountTitle.ToLower() == Title.ToLower() && Ac.FK_CompanyId == CompanyId).FirstOrDefault();
                if (Act != null)
                {
                    AccountId = Act.AccountId;
                }
            }
            catch (Exception)
            {

                throw;
            }
            return AccountId;
        }



        [ResponseType(typeof(PurchaseOrderTable))]
        public IHttpActionResult PostPurchase([FromBody] MvcPurchaseViewModel PurchaseViewModel)
        {
            using (DBEntities entities = new DBEntities())
            {
                using (DbContextTransaction transaction = entities.Database.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
                {
                    try
                    {
                        PurchaseOrderTable _PTable = new PurchaseOrderTable();

                        _PTable.CompanyId = PurchaseViewModel.CompanyId;
                        _PTable.UserId = PurchaseViewModel.UserId;
                        _PTable.PurchaseID = PurchaseViewModel.PurchaseId.ToString();
                        _PTable.VenderId = PurchaseViewModel.VenderId;
                        _PTable.PurchaseRefNumber = PurchaseViewModel.PurchaseRefNumber;
                        _PTable.PurchaseDate = Convert.ToDateTime(PurchaseViewModel.PurchaseDate);
                        _PTable.PurchaseDueDate = PurchaseViewModel.PurchaseDueDate;
                        _PTable.PurchaseSubTotal = PurchaseViewModel.PurchaseSubTotal;
                        _PTable.PurchaseDiscountAmount = PurchaseViewModel.PurchaseDiscountAmount;
                        _PTable.PurchaseTotoalAmount = PurchaseViewModel.PurchaseTotoalAmount;
                        _PTable.PurchaseVenderNote = PurchaseViewModel.PurchaseVenderNote;
                        _PTable.Vat6 = PurchaseViewModel.Vat6;
                        _PTable.Vat21 = PurchaseViewModel.Vat21;
                        _PTable.Status = "open";
                        _PTable.Type = StatusEnum.Goods.ToString();

                        _PTable = entities.PurchaseOrderTables.Add(_PTable);
                        entities.SaveChanges();

                        if (PurchaseViewModel.PurchaseOrderList != null)
                        {
                            foreach (PurchaseOrderDetailsTable item in PurchaseViewModel.PurchaseOrderList)
                            {
                                PurchaseOrderDetailsTable purchadeDetail = new PurchaseOrderDetailsTable();
                                purchadeDetail.PurchaseOrderDetailsId = item.PurchaseOrderDetailsId;
                                purchadeDetail.PurchaseItemId = item.PurchaseItemId;
                                purchadeDetail.PurchaseDescription = item.PurchaseDescription;
                                purchadeDetail.PurchaseQuantity = item.PurchaseQuantity;
                                purchadeDetail.PurchaseItemRate = item.PurchaseItemRate;
                                purchadeDetail.PurchaseTotal = item.PurchaseTotal;
                                purchadeDetail.Type = item.Type;
                                purchadeDetail.RowSubTotal = item.RowSubTotal;
                                purchadeDetail.PurchaseVatPercentage = item.PurchaseVatPercentage;
                                purchadeDetail.PurchaseId = _PTable.PurchaseOrderID;
                                purchadeDetail.ServiceDate = item.ServiceDate;
                                entities.PurchaseOrderDetailsTables.Add(purchadeDetail);
                                entities.SaveChanges();
                            }
                        }

                        AccountTransictionTable Account = new AccountTransictionTable();
                        Account.TransictionDate = DateTime.Now;
                        Account.TransictionNumber = base64Guid;
                        Account.TransictionType = "Purchase";
                        Account.CreationTime = DateTime.Now.TimeOfDay;
                        Account.AddedBy = 1;
                        Account.FK_CompanyId = PurchaseViewModel.CompanyId;
                        Account.FKPaymentTerm = 1;
                        Account.TransictionRefrenceId = _PTable.PurchaseOrderID.ToString();
                        //Accounts payable  Transaction
                        int Accountpayble = AccountIdByName("Accounts payable", (int)PurchaseViewModel.CompanyId);

                        if (Accountpayble != 0)
                        {
                            Account.Dr = 0.00;
                            Account.Cr = PurchaseViewModel.PurchaseTotoalAmount;
                            Account.FK_AccountID = Accountpayble;
                            entities.AccountTransictionTables.Add(Account);
                            entities.SaveChanges();
                        }
                        // Cost Of Goods Transaction
                        int CostOfGood = AccountIdByName("Cost Of Goods", (int)PurchaseViewModel.CompanyId);
                        if (CostOfGood != 0)
                        {
                            Account.Cr = 0.00;
                            Account.Dr = PurchaseViewModel.PurchaseSubTotal;
                            Account.FK_AccountID = CostOfGood;
                            entities.AccountTransictionTables.Add(Account);
                            entities.SaveChanges();
                        }

                        // Input VAT Trnsaction
                        int VatAccountId = AccountIdByName("Input VAT", (int)PurchaseViewModel.CompanyId);

                        if (VatAccountId != 0)
                        {
                            Account.Cr = 0.00;
                            Account.Dr = PurchaseViewModel.Vat21 + PurchaseViewModel.Vat6;
                            Account.FK_AccountID = VatAccountId;
                            entities.AccountTransictionTables.Add(Account);
                            int Id = entities.SaveChanges();
                        }

                        MvcPurchaseModel pModel = new MvcPurchaseModel();
                        pModel.PurchaseOrderID = _PTable.PurchaseOrderID;
                        transaction.Commit();
                        return Ok(pModel);
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        return BadRequest();
                    }
                }
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
