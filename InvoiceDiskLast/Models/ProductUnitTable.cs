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
    
    public partial class ProductUnitTable
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ProductUnitTable()
        {
            this.ProductTables = new HashSet<ProductTable>();
        }
    
        public int? ProductUnitID { get; set; }
        public string ProductUnit { get; set; }
        public Nullable<bool> Status { get; set; }
        public Nullable<int> CompanyId { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProductTable> ProductTables { get; set; }
    }
}
