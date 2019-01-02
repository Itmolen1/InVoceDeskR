using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InvoiceDiskLast.Models
{
    public class MVCInvoiceDetailsModel
    {
        public int InvoiceDetailId { get; set; }
        public Nullable<int> ItemId { get; set; }
        public string Description { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Nullable<double> Rate { get; set; }
        public Nullable<double> Total { get; set; }
        public Nullable<double> Vat { get; set; }
        public Nullable<double> RowSubTotal { get; set; }
        public Nullable<int> InvoiceId { get; set; }
        public string Type { get; set; }
        public Nullable<System.DateTime> ServiceDate { get; set; }
    }
}