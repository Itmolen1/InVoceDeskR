using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InvoiceDiskLast.Models
{
    public class MVCQutationDetailsModel
    {
        public int? QutationDetailId { get; set; }
        public Nullable<int> ItemId { get; set; }
        public string Description { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Nullable<double> Rate { get; set; }
        public Nullable<double> Total { get; set; }
        public Nullable<double> Vat { get; set; }
        public Nullable<int> QutationID { get; set; }
    }
}