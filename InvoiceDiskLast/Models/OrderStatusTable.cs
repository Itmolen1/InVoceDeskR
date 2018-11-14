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
    
    public partial class OrderStatusTable
    {
        public int? OrderStatusId { get; set; }
        public Nullable<int> PurchaseOrderId { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> CompanyId { get; set; }
        public Nullable<int> UserId { get; set; }
    
        public virtual ComapnyInfo ComapnyInfo { get; set; }
        public virtual PurchaseOrderTable PurchaseOrderTable { get; set; }
    }
}
