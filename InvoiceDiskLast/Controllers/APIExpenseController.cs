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
    public class APIExpenseController : ApiController
    {

        private DBEntities db = new DBEntities();

        string base64Guid = Convert.ToBase64String(Guid.NewGuid().ToByteArray());

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

        [Route("api/PostExpense")]
        public IHttpActionResult PostExpense(ExpenseViewModel ExpenseViewModel)
        {

            EXPENSE expenseTable = new EXPENSE();
            ExpenseModel _ExpeseModel = new ExpenseModel();

            AccountTransictionTable AccountTable = new AccountTransictionTable();
            AccountTable.TransictionDate = DateTime.Now;
            AccountTable.TransictionNumber = base64Guid;
            AccountTable.TransictionType = "Expence";
            AccountTable.CreationTime = DateTime.Now.TimeOfDay;
            AccountTable.FK_CompanyId = ExpenseViewModel.comapny_id;
            AccountTable.FKPaymentTerm = 1;
            AccountTable.AddedBy = ExpenseViewModel.user_id;

            using (DBEntities context = new DBEntities())
            {
                using (DbContextTransaction transaction = context.Database.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
                {
                    try
                    {
                        EXPENSE expense = new EXPENSE();

                        expense.REFERENCEno = ExpenseViewModel.REFERENCEno;
                        expense.ACCOUNT_ID = ExpenseViewModel.ACCOUNT_ID;
                        expense.VENDOR_ID = ExpenseViewModel.VENDOR_ID;
                        expense.notes = ExpenseViewModel.notes;
                        expense.user_id = ExpenseViewModel.user_id;
                        expense.SUBTOTAL = ExpenseViewModel.SUBTOTAL;
                        expense.VAT_AMOUNT = ExpenseViewModel.VAT_AMOUNT;
                        expense.GRAND_TOTAL = ExpenseViewModel.GRAND_TOTAL;
                        expense.AddedDate = ExpenseViewModel.AddedDate;
                        expense.comapny_id = ExpenseViewModel.comapny_id;
                        expense.Vat6 = ExpenseViewModel.Vat6;
                        expense.Vat21 = ExpenseViewModel.Vat21;
                        expenseTable = context.EXPENSEs.Add(expense);
                        context.SaveChanges();

                        if (ExpenseViewModel.ExpensenDetailList != null)
                        {
                            foreach (ExpenseDetail item in ExpenseViewModel.ExpensenDetailList)
                            {
                                ExpenseDetail expenseDetailModel = new ExpenseDetail();
                                expenseDetailModel.expense_id = expenseTable.Id;
                                expenseDetailModel.EXPENSE_ACCOUNT_ID = item.EXPENSE_ACCOUNT_ID;
                                expenseDetailModel.DESCRIPTION = item.DESCRIPTION;
                                expenseDetailModel.AMOUNT = item.AMOUNT;
                                expenseDetailModel.TAX_PERCENT = item.TAX_PERCENT;
                                expenseDetailModel.TAX_AMOUNT = item.TAX_AMOUNT;
                                expenseDetailModel.SUBTOTAL = item.SUBTOTAL;
                                expenseDetailModel.user_id = ExpenseViewModel.user_id;
                                expenseDetailModel.comapny_id = ExpenseViewModel.comapny_id;
                                AccountTable.Cr = 0.00;
                                AccountTable.Dr = item.AMOUNT;
                                AccountTable.FK_AccountID = item.EXPENSE_ACCOUNT_ID;

                                context.ExpenseDetails.Add(expenseDetailModel);
                                context.AccountTransictionTables.Add(AccountTable);
                                context.SaveChanges();
                            }
                        }

                        //Accounts Petty Cash  Transaction
                        int Accountpayble = AccountIdByName("Petty Cash", (int)ExpenseViewModel.comapny_id);

                        if (Accountpayble != 0)
                        {
                            AccountTable.Dr = 0.00;
                            AccountTable.Cr = ExpenseViewModel.GRAND_TOTAL;
                            AccountTable.FK_AccountID = Accountpayble;
                            context.AccountTransictionTables.Add(AccountTable);
                            context.SaveChanges();
                        }
                        // Cost Of Goods Transaction
                        int CostOfGood = AccountIdByName("Input VAT", (int)ExpenseViewModel.comapny_id);
                        if (CostOfGood != 0)
                        {
                            AccountTable.Cr = 0.00;
                            AccountTable.Dr = ExpenseViewModel.Vat21 + ExpenseViewModel.Vat6; ;
                            AccountTable.FK_AccountID = CostOfGood;
                            context.AccountTransictionTables.Add(AccountTable);
                            context.SaveChanges();
                        }
                        _ExpeseModel.Id = expenseTable.Id;
                        transaction.Commit();
                        return Ok(_ExpeseModel);

                    }

                    catch (Exception ex)
                    {
                        transaction.Commit();
                              
                        return BadRequest();
                    }

                }
            }


        }


        [Route("api/GetExpenseById/{ExpenseId:int}")]
        public IHttpActionResult GetExpenseById(int ExpenseId)
        {
            try
            {
                ExpenseViewModel _ExpenseModel = db.EXPENSEs.Where(C => C.Id == ExpenseId).Select(Ex => new ExpenseViewModel
                {
                    Id = Ex.Id,
                    REFERENCEno = Ex.REFERENCEno,
                    ACCOUNT_ID = Ex.ACCOUNT_ID,
                    VENDOR_ID = Ex.VENDOR_ID,
                    notes = Ex.notes,
                    Vat21 = Ex.Vat21,
                    Vat6 = Ex.Vat6,
                    SUBTOTAL = Ex.SUBTOTAL,
                    VAT_AMOUNT = Ex.VAT_AMOUNT,
                    GRAND_TOTAL = Ex.GRAND_TOTAL,
                    user_id = Ex.user_id,
                    comapny_id = Ex.comapny_id,
                    AddedDate = Ex.AddedDate,

                }).FirstOrDefault();

                return Ok(_ExpenseModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [Route("api/GetExpenseDetailById/{ExpenseId:int}")]
        public IHttpActionResult GetExpenseDetailById(int ExpenseId)
        {
            try
            {
                List<ExpenseViewModel> ExpenseDetail = new List<ExpenseViewModel>();
                ExpenseDetail = db.ExpenseDetails.Where(C => C.expense_id == ExpenseId).Select(Ex => new ExpenseViewModel
                {
                    Id = Ex.Id,
                    EXPENSE_ACCOUNT_ID = (int)Ex.EXPENSE_ACCOUNT_ID,
                    DESCRIPTION = Ex.DESCRIPTION,
                    AMOUNT = Ex.AMOUNT,
                    TAX_PERCENT = Ex.TAX_PERCENT,
                    TAX_AMOUNT = Ex.TAX_AMOUNT,
                    SUBTOTAL = Ex.SUBTOTAL,
                    AccountTitle = Ex.AccountTable.AccountTitle,
                    expense_id = Ex.expense_id,
                    user_id = Ex.user_id,
                    comapny_id = Ex.comapny_id,

                }).ToList();

                return Ok(ExpenseDetail);
            }
            catch (Exception)
            {
                throw;
            }
        }


        [Route("Api/GetExpenseDetailList122/{CompanyId:int}/{Search:alpha}/{skip:int}/{pageSize:int}")]
        public IHttpActionResult GetExpenselist(int CompanyId, string Search, int skip, int pageSize)

        {
            try
            {
                if (Search == "NoSearch")
                {
                    List<ExpenseViewModel> ExpensList = new List<ExpenseViewModel>();
                    ExpensList = (from Ex in db.EXPENSEs
                                  join u in db.UserTables on Ex.user_id equals u.UserId
                                  join con in db.ContactsTables on Ex.VENDOR_ID equals con.ContactsId
                                  join Acc in db.AccountTables on Ex.ACCOUNT_ID equals Acc.AccountId
                                  where (Ex.comapny_id == CompanyId || Ex.REFERENCEno != null && Ex.REFERENCEno.ToLower().Contains(Search.ToLower()) ||
                                   Ex.AddedDate != null && Ex.AddedDate.ToString().ToLower().Contains(Search.ToLower()) ||
                                  u.Username != null && u.Username.ToLower().Contains(Search.ToLower()))
                                  select new ExpenseViewModel()
                                  {
                                      Id = Ex.Id,
                                      REFERENCEno = Ex.REFERENCEno,
                                      ACCOUNT_ID = Ex.ACCOUNT_ID,
                                      VENDOR_ID = Ex.VENDOR_ID,
                                      notes = Ex.notes,
                                      TotalRecord = db.EXPENSEs.ToList().Count(),
                                      PaidThrougAccount = Acc.AccountTitle,
                                      VenderName = con.ContactName,
                                      SUBTOTAL = Ex.SUBTOTAL,
                                      VAT_AMOUNT = Ex.VAT_AMOUNT,
                                      GRAND_TOTAL = Ex.GRAND_TOTAL,
                                      AddedDate = Ex.AddedDate,
                                  }).OrderByDescending(x => x.Id).ToList().Skip(skip).Take(pageSize).ToList();

                    return Ok(ExpensList);

                }
                else
                {
                    List<ExpenseViewModel> ExpensList = new List<ExpenseViewModel>();
                    ExpensList = (from Ex in db.EXPENSEs
                                  join u in db.UserTables on Ex.user_id equals u.UserId
                                  join con in db.ContactsTables on Ex.VENDOR_ID equals con.ContactsId
                                  join Acc in db.AccountTables on Ex.ACCOUNT_ID equals Acc.AccountId
                                  where (Ex.comapny_id == CompanyId || Ex.REFERENCEno != null && Ex.REFERENCEno.ToLower().Contains(Search.ToLower()) ||
                                   Ex.AddedDate != null && Ex.AddedDate.ToString().ToLower().Contains(Search.ToLower()) ||
                                  u.Username != null && u.Username.ToLower().Contains(Search.ToLower()))
                                  select new ExpenseViewModel()
                                  {
                                      Id = Ex.Id,
                                      REFERENCEno = Ex.REFERENCEno,
                                      ACCOUNT_ID = Ex.ACCOUNT_ID,
                                      VENDOR_ID = Ex.ACCOUNT_ID,
                                      notes = Ex.notes,
                                      PaidThrougAccount = Acc.AccountTitle,
                                      VenderName = con.ContactName,
                                      SUBTOTAL = Ex.SUBTOTAL,
                                      VAT_AMOUNT = Ex.VAT_AMOUNT,
                                      GRAND_TOTAL = Ex.GRAND_TOTAL,
                                      AddedDate = Ex.AddedDate,
                                  }).OrderByDescending(x => x.Id).ToList().Skip(skip).Take(pageSize).ToList();


                    return Ok(ExpensList);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        [Route("api/PutExpense/{id:int}")]
        public IHttpActionResult PutExpense(int id, EXPENSE ExpenseTable)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (id != ExpenseTable.Id)
            {
                return BadRequest();
            }
            db.Entry(ExpenseTable).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
                return Ok(ExpenseTable);
            }
            catch (Exception ex)
            {
                BadRequest();
                throw;
            }
        }

        [Route("api/DeleteExpenseDetails/{id:int}")]
        [ResponseType(typeof(ExpenseDetail))]
        public IHttpActionResult DeleteInvoiceDtailsTable(int id)
        {
            ExpenseDetail expensedetail = db.ExpenseDetails.Find(id);
            if (expensedetail == null)
            {
                return NotFound();
            }
            db.ExpenseDetails.Remove(expensedetail);
            db.SaveChanges();

            return Ok(expensedetail);
        }

        [Route("api/PutExpenseDetail/{id:int}")]
        public IHttpActionResult PutExpenseDetailTable(int id, ExpenseDetail expensedetail)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != expensedetail.expense_id)
            {
                return BadRequest();
            }
            db.Entry(expensedetail).State = EntityState.Modified;
            try
            {
                db.SaveChanges();
                return StatusCode(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                throw ex;

                return NotFound();
            }
        }

        [Route("api/PostExpenseDetail")]
        public IHttpActionResult PostEpenseDetail([FromBody] ExpenseDetail ExpenseDetail)
        {
            using (DBEntities entities = new DBEntities())
            {
                try
                {
                    ExpenseDetail = entities.ExpenseDetails.Add(ExpenseDetail);
                    entities.SaveChanges();
                    return Ok(ExpenseDetail);
                }
                catch (Exception ex)
                {
                    return BadRequest();
                }

            }
        }

    }
}
