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
        SqlParameter _Prameter;


   

        [Route("api/GetJournal/{ToDate:long}/{FromDate:long}")]
        public IHttpActionResult Getjournal(long FromDate, long ToDate)
        {

            DateTime dt = dt = ConvertLongToDate(FromDate);

            string Date = dt.ToString("yyyy-MM-dd");



            var FDate = new SqlParameter("FromDate", ConvertLongToDate(FromDate).ToString("yyyy-MM-dd"));
            var TDate = new SqlParameter("ToDate", ConvertLongToDate(ToDate).ToString("yyyy-MM-dd"));

            try
            {
                var Journal = db.Database.SqlQuery<TransactionModel>("exec dbo.Sp_GetJournal  @FromDate , @ToDate", FDate, TDate).ToList<TransactionModel>();
                return Ok(Journal);
            }
            catch (Exception ex)
            {
                throw ex;

                // return NotFound();

            }
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
