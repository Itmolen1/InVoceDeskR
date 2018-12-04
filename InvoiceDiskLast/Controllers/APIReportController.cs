using InvoiceDiskLast.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace InvoiceDiskLast.Controllers
{
    // [RoutePrefix("api/APIReport")]
    public class APIReportController : ApiController
    {
        private DBEntities db = new DBEntities();


        [Route("api/GetJournal/{ToDate:long}/{FromDate:long}")]
        public IHttpActionResult Getjournal(long FromDate, long ToDate)
        {
            if (FromDate.ToString() != null)
            {

                var FDate = ConvertLongToDate(FromDate);
                var TDate = ConvertLongToDate(ToDate);
                try
                {
                    List<TransactionModel> Journal = db.AccountTransictionTables.Where(t => t.TransictionDate <= FDate && t.TransictionDate >= TDate).Select(c => new TransactionModel
                    {
                        TranDate = c.TransictionDate,
                        AmountDebit = c.Dr,
                        AmountCredit = c.Cr,
                        AccountCode = c.AccountTable.AccountCode,
                        AccountTitle = c.AccountTable.AccountTitle

                    }).ToList();

                    return Ok(Journal);
                }
                catch (Exception ex)
                {
                    throw ex;

                    // return NotFound();

                }
            }
            else
            {
                return null;
            }

        }

        [Route("api/GetTrialBalance/{fromDate:long}/{Todate:long}")]
        public IHttpActionResult GetTrialBalance(long Date)
        {

            //Select T.FK_AccountID, Ac.AccountTitle,T.TransictionDate,SUM(T.Dr) As TotalDebit, SUM(T.Cr) as TotalCredit from AccountTransictionTable as T
            //inner join AccountTable as Ac on T.FK_AccountID = Ac.AccountId
            //group by T.FK_AccountID, Ac.AccountTitle, T.TransictionDate

            List<TransactionModel> _TransactionList = new List<TransactionModel>();
            try
            {

                DateTime DateConverted = new DateTime();

                DateConverted = ConvertLongToDate(Date);

               _TransactionList = (from T in db.AccountTransictionTables
                                    join Ac in db.AccountTables on T.FK_AccountID equals Ac.AccountId
                                    where T.TransictionDate== DateConverted
                                    select new { T.FK_AccountID, Ac.AccountTitle, T.TransictionDate, T.Dr, T.Cr } into x
                                    group x by new { x.FK_AccountID, x.AccountTitle, x.TransictionDate } into g
                                    select new TransactionModel
                                    {
                                        TranDate = g.Key.TransictionDate,
                                        AccountTitle = g.Key.AccountTitle,
                                        AmountCredit = g.Sum(i => i.Cr),
                                        AmountDebit = g.Sum(i => i.Dr),
                                    }).ToList();

                return Ok(_TransactionList);

            }
            catch (Exception)
            {
                return BadRequest();
                throw;
            }

            return Ok();
        }

        public DateTime ConvertLongToDate(long Date)
        {
            DateTime _date = new DateTime(Date);

            return _date;

        }


        //[Route("api/GetReports/{FromDate:alpha}/{ToDate:alpha}")]

        //public IHttpActionResult Getjournal(string FromDate, string ToDate)
        //{

        //    _Prameter = new SqlParameter();
        //    _Prameter.ParameterName = "@FromDate";
        //    _Prameter.SqlDbType = SqlDbType.DateTime;
        //    _Prameter.Value = FromDate;
        //    _Prameter.ParameterName = "@ToDate";
        //    _Prameter.SqlDbType = SqlDbType.DateTime;
        //    _Prameter.Value = ToDate;
        //    try
        //    {
        //        var Journal = db.Database.SqlQuery<TransactionModel>("[exec Sp_GetJournal] @FromDate,@ToDate", _Prameter).ToList<TransactionModel>();
        //        return Ok(Journal);
        //    }
        //    catch (Exception)
        //    {
        //        return NotFound();

        //    }

        //    return Ok();
        //}

    }
}
