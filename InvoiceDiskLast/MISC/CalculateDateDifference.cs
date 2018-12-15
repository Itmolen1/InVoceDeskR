using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InvoiceDiskLast.MISC
{
    public class CalculateDateDifference
    {

        public string DateDiff(DateTime FromDate, DateTime ToDate)
        {

            System.TimeSpan diff = FromDate.Subtract(FromDate);
            System.TimeSpan diff1 = ToDate - FromDate;

            string diff2 = (ToDate - FromDate).TotalDays.ToString();

            return diff2;
        }
    }
}