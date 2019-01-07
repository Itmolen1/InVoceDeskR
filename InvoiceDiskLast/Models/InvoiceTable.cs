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
    
    public partial class InvoiceTable
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public InvoiceTable()
        {
            this.InvoiceDetailsTables = new HashSet<InvoiceDetailsTable>();
        }
    
        public int InvoiceID { get; set; }
        public string Invoice_ID { get; set; }
        public string RefNumber { get; set; }
        public Nullable<int> QutationId { get; set; }
        public Nullable<System.DateTime> InvoiceDate { get; set; }
        public Nullable<System.DateTime> InvoiceDueDate { get; set; }
        public Nullable<double> SubTotal { get; set; }
        public Nullable<double> TotalVat6 { get; set; }
        public Nullable<double> TotalVat21 { get; set; }
        public Nullable<double> DiscountAmount { get; set; }
        public Nullable<double> TotalAmount { get; set; }
        public string CustomerNote { get; set; }
        public string Status { get; set; }
        public Nullable<int> UserId { get; set; }
        public Nullable<int> CompanyId { get; set; }
        public Nullable<int> ContactId { get; set; }
        public string Type { get; set; }
        public string InvoiceDescription { get; set; }
    
        public virtual ComapnyInfo ComapnyInfo { get; set; }
        public virtual ContactsTable ContactsTable { get; set; }
        public virtual UserTable UserTable { get; set; }
        public virtual UserTable UserTable1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<InvoiceDetailsTable> InvoiceDetailsTables { get; set; }
    }
}
