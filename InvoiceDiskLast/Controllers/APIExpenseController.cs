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

        [Route("api/PostExpense")]
        public IHttpActionResult PostExpense(EXPENSE _ExpsenseMaster)
        {
            EXPENSE _ExpenseModel = new EXPENSE();
            try
            {
                _ExpsenseMaster = db.EXPENSEs.Add(_ExpsenseMaster);
                db.SaveChanges();
                return Ok(_ExpsenseMaster);
            }
            catch (Exception)
            {
                throw;
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
                    EXPENSE_ACCOUNT_ID =(int) Ex.EXPENSE_ACCOUNT_ID,
                    DESCRIPTION = Ex.DESCRIPTION,
                    AMOUNT =Ex.AMOUNT,
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
                                  //where (Ex.comapny_id == CompanyId || Ex.REFERENCEno != null && Ex.REFERENCEno.ToLower().Contains(Search.ToLower()) ||
                                  // Ex.AddedDate != null && Ex.AddedDate.ToString().ToLower().Contains(Search.ToLower()) ||
                                  //u.Username != null && u.Username.ToLower().Contains(Search.ToLower()))
                                  select new ExpenseViewModel()
                                  {
                                      Id = Ex.Id,
                                      REFERENCEno = Ex.REFERENCEno,
                                      ACCOUNT_ID =Ex.ACCOUNT_ID,
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
                                  //where (Ex.comapny_id == CompanyId || Ex.REFERENCEno != null && Ex.REFERENCEno.ToLower().Contains(Search.ToLower()) ||
                                  // Ex.AddedDate != null && Ex.AddedDate.ToString().ToLower().Contains(Search.ToLower()) ||
                                  //u.Username != null && u.Username.ToLower().Contains(Search.ToLower()))
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
