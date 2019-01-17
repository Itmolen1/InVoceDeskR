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


        [Route("Api/GetBillDetailTablebyId/{CompanyId:int}")]
        public IHttpActionResult GetExpenselist(int CompanyId)
        {
            try
            {
                List<ExpenseViewModel> ExpensList = new List<ExpenseViewModel>();

                ExpensList = db.EXPENSEs.Where(Ex => Ex.comapny_id == CompanyId).Select(E => new ExpenseViewModel
                {
                    REFERENCEno = E.REFERENCEno,
                    ACCOUNT_ID = E.ACCOUNT_ID,
                    VENDOR_ID = E.ACCOUNT_ID,
                    notes = E.notes,
                    SUBTOTAL = E.SUBTOTAL,
                    VAT_AMOUNT = E.VAT_AMOUNT,
                    GRAND_TOTAL = E.GRAND_TOTAL,
                    UserName = E.UserTable.Username,
                    CompanyName = E.ComapnyInfo.CompanyName,
                    AddedDate = E.AddedDate,
                }).ToList();

                return Ok(ExpensList);

            }
            catch (Exception)
            {
                throw;
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
