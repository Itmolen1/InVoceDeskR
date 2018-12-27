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
    
    public partial class UserTable
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public UserTable()
        {
            this.QutationOrderStatusTables = new HashSet<QutationOrderStatusTable>();
            this.QutationTables = new HashSet<QutationTable>();
        }
    
        public int UserId { get; set; }
        public string UserFname { get; set; }
        public string Insertion { get; set; }
        public string UserLname { get; set; }
        public Nullable<int> Gender { get; set; }
        public string DOB { get; set; }
        public Nullable<System.DateTime> AddedDate { get; set; }
        public Nullable<int> CompanyId { get; set; }
        public string ImageUrl { get; set; }
    
        public virtual ComapnyInfo ComapnyInfo { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<QutationOrderStatusTable> QutationOrderStatusTables { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<QutationTable> QutationTables { get; set; }
    }
}
