using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InvoiceDiskLast.Models
{
    public class GoodsTable
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public double Rate { get; set; }
        public double Total { get; set; }
        public double Vat { get; set; }
        public double RowSubTotal { get; set; }
    }



    public class ServicesTables
    {
        public String Date { get; set; }
        public string ProductNames { get; set; }

        public string Descriptions { get; set; }
        public int Quantitys { get; set; }
        public double Rates { get; set; }
        public double Totals { get; set; }
        public double Vats { get; set; }
        public double RowSubTotals { get; set; }


      
    }
}