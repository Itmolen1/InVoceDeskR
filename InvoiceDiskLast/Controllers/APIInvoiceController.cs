using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using InvoiceDiskLast.Models;
using System.Web.Http.Description;
using System.Data.Entity;

namespace InvoiceDiskLast.Controllers
{
    public class APIInvoiceController : ApiController
    {
        string base64Guid = Convert.ToBase64String(Guid.NewGuid().ToByteArray());

        private DBEntities db = new DBEntities();

        [Route("api/GetInvoiceTable/{CompayID:int}")]
        public IHttpActionResult GetInvoiceTables(int CompayID)
        {
            List<InvoiceViewModel> invoiceViewModel = new List<InvoiceViewModel>();


            invoiceViewModel = db.InvoiceTables.Where(x => x.CompanyId == CompayID).Select(c => new InvoiceViewModel
            {

                InvoiceID = c.InvoiceID,
                Invoice_ID = c.Invoice_ID,
                RefNumber = c.RefNumber,
                InvoiceDate = c.InvoiceDate,
                InvoiceDueDate = c.InvoiceDueDate,
                Vat = c.TotalVat6 + c.TotalVat21,
                TotalAmount = c.TotalAmount,
                Status = (c.Status).Trim(),
                UserName = (c.UserTable.UserFname + " " + c.UserTable.UserLname != "" ? c.UserTable.UserFname + " " + c.UserTable.UserLname : c.UserTable.Username),
                CustomerName = c.ContactsTable.ContactName,
                InvoiceDescription = c.InvoiceDescription

            }).ToList();

            return Ok(invoiceViewModel);


        }





        [Route("api/GetInvoiceCount")]
        public IHttpActionResult GetInvoiceCount()
        {
            MVCInvoiceModel Invoice = new MVCInvoiceModel();

            int InvoiceId = db.InvoiceTables.ToList().Count() + 1;
            Invoice.Invoice_ID = InvoiceId.ToString();

            if (InvoiceId == 0)
            {
                return Ok(Invoice);
            }
            else
            {
                return Ok(Invoice);
            }
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

        [Route("api/PostInvoice")]
        [ResponseType(typeof(QutationTable))]
        public IHttpActionResult PostInvoice([FromBody] InvoiceViewModel invoiceViewModel)
        {
            InvoiceTable Table = new InvoiceTable();

            using (DBEntities context = new DBEntities())
            {
                using (DbContextTransaction transaction = context.Database.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
                {
                    try
                    {
                        Table.Invoice_ID = invoiceViewModel.Invoice_ID;
                        Table.CompanyId = invoiceViewModel.CompanyId;
                        Table.UserId = invoiceViewModel.UserId;
                        Table.ContactId = invoiceViewModel.ContactId;
                        Table.RefNumber = invoiceViewModel.RefNumber;
                        Table.InvoiceDate = invoiceViewModel.InvoiceDate;
                        Table.InvoiceDueDate = invoiceViewModel.InvoiceDueDate;
                        Table.SubTotal = invoiceViewModel.SubTotal;
                        Table.DiscountAmount = invoiceViewModel.DiscountAmount;
                        Table.TotalAmount = invoiceViewModel.TotalAmount;
                        Table.CustomerNote = invoiceViewModel.CustomerNote;
                        Table.TotalVat21 = invoiceViewModel.TotalVat21;
                        Table.TotalVat6 = invoiceViewModel.TotalVat6;
                        Table.Type = StatusEnum.Goods.ToString();
                        Table.Status = "accepted";
                        Table.InvoiceDescription = invoiceViewModel.InvoiceDescription;

                        if (Table.TotalVat6 != null)
                        {
                            double vat61 = Math.Round((double)Table.TotalVat6, 2, MidpointRounding.AwayFromZero);
                            Table.TotalVat6 = vat61;
                        }
                        if (Table.TotalVat21 != null)
                        {
                            double vat21 = Math.Round((double)Table.TotalVat21, 2, MidpointRounding.AwayFromZero);
                            Table.TotalVat21 = vat21;
                        }

                        Table = context.InvoiceTables.Add(Table);
                        context.SaveChanges();

                        if (invoiceViewModel.InvoiceDetailsTable != null)
                        {
                            foreach (InvoiceDetailsTable InvoiceDetailsList in invoiceViewModel.InvoiceDetailsTable)
                            {
                                InvoiceDetailsTable InvoiceDetails = new InvoiceDetailsTable();
                                InvoiceDetails.ItemId = Convert.ToInt32(InvoiceDetailsList.ItemId);
                                InvoiceDetails.InvoiceId = Table.InvoiceID;
                                InvoiceDetails.Description = InvoiceDetailsList.Description;
                                InvoiceDetails.Quantity = InvoiceDetailsList.Quantity;
                                InvoiceDetails.Rate = Convert.ToDouble(InvoiceDetailsList.Rate);
                                InvoiceDetails.Total = Convert.ToDouble(InvoiceDetailsList.Total);
                                InvoiceDetails.ServiceDate = InvoiceDetailsList.ServiceDate;
                                InvoiceDetails.RowSubTotal = InvoiceDetailsList.RowSubTotal;
                                InvoiceDetails.Vat = Convert.ToDouble(InvoiceDetailsList.Vat);
                                InvoiceDetails.Type = InvoiceDetailsList.Type;

                                if (InvoiceDetails.InvoiceDetailId == 0)
                                {
                                    InvoiceDetails = context.InvoiceDetailsTables.Add(InvoiceDetails);
                                    context.SaveChanges();
                                }
                            }
                        }

                        AccountTransictionTable Account = new AccountTransictionTable();
                        Account.TransictionDate = DateTime.Now;
                        Account.TransictionNumber = base64Guid;
                        Account.TransictionType = "Invoice";
                        Account.CreationTime = DateTime.Now.TimeOfDay;
                        Account.AddedBy = 1;
                        Account.FK_CompanyId = invoiceViewModel.CompanyId;
                        Account.FKPaymentTerm = 1;

                        //Cash Account Transaction
                        int CashtAccountId = AccountIdByName("Cash on hand", (int)invoiceViewModel.CompanyId);

                        if (CashtAccountId != 0)
                        {
                            Account.Dr = invoiceViewModel.TotalAmount;
                            Account.Cr = 0.00;
                            Account.FK_AccountID = CashtAccountId;
                            context.AccountTransictionTables.Add(Account);
                            context.SaveChanges();
                        }
                        // Sale Account Transaction
                        int SaleAccount = AccountIdByName("Seles", (int)invoiceViewModel.CompanyId);
                        if (SaleAccount != 0)
                        {
                            Account.Cr = invoiceViewModel.SubTotal;
                            Account.Dr = 0.00;
                            Account.FK_AccountID = SaleAccount;
                            context.AccountTransictionTables.Add(Account);
                            context.SaveChanges();
                        }
                        // vat Out Put Trnsaction
                        int VatAccountId = AccountIdByName("VAT Payable", (int)invoiceViewModel.CompanyId);
                        if (VatAccountId != 0)
                        {
                            Account.Cr = invoiceViewModel.TotalVat6 + invoiceViewModel.TotalVat21;
                            Account.Dr = 0.00;
                            Account.FK_AccountID = VatAccountId;
                            context.AccountTransictionTables.Add(Account);
                            int Id = context.SaveChanges();
                        }
                        InvoiceModel inc = new InvoiceModel();
                        inc.InvoiceID = Table.InvoiceID;
                        transaction.Commit();
                        return Ok(inc);
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        return BadRequest();
                    }
                }
            }
        }


        [Route("api/GetInvoiceById/{id:int}")]
        [ResponseType(typeof(QutationTable))]
        public IHttpActionResult GetInvoiceTable(int id)
        {
            MVCInvoiceModel invoiceModel = new MVCInvoiceModel();

            invoiceModel = db.InvoiceTables.Where(q => q.InvoiceID == id).Select(c => new MVCInvoiceModel
            {
                InvoiceID = c.InvoiceID,
                Invoice_ID = c.Invoice_ID,
                RefNumber = c.RefNumber,
                InvoiceDate = c.InvoiceDate,
                InvoiceDueDate = c.InvoiceDueDate,
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
                InvoiceDescription = c.InvoiceDescription

            }).FirstOrDefault();


            if (invoiceModel == null)
            {
                return NotFound();
            }

            return Ok(invoiceModel);
        }




        [Route("api/GetId/{Id:int}")]
        public IHttpActionResult GetId12(int Id)
        {
            try
            {
                MVCInvoiceModel inoiceTabe = new MVCInvoiceModel();

                inoiceTabe.InvoiceID = db.InvoiceTables.Where(I => I.QutationId == Id).FirstOrDefault().InvoiceID;

                return Ok(inoiceTabe);

            }
            catch (Exception)
            {
                NotFound();
                throw;
            }
        }






        [Route("api/UpdateInvoice/{id:int}")]
        [ResponseType(typeof(void))]
        public IHttpActionResult PutInvoiceTable(int id, InvoiceTable invoiceTable)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != invoiceTable.InvoiceID)
            {
                return BadRequest();
            }

            db.Entry(invoiceTable).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
                return Ok(invoiceTable);
            }
            catch (Exception ex)
            {

                return BadRequest();
            }


        }
    }
}
