//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace InvoiceDiskLast.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class BillDetailTable
    {
        public int BillDetailId { get; set; }
        public Nullable<int> ItemId { get; set; }
        public string Description { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Nullable<double> Rate { get; set; }
        public Nullable<double> Total { get; set; }
        public Nullable<double> Vat { get; set; }
        public Nullable<double> RowSubTotal { get; set; }
        public Nullable<int> BillID { get; set; }
        public string Type { get; set; }
        public Nullable<System.DateTime> ServiceDate { get; set; }
    
        public virtual BillTable BillTable { get; set; }
        public virtual ProductTable ProductTable { get; set; }
    }
}
