using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InvoiceDiskLast.Models
{
    public class MVCProductUnitModel
    {
        public int? ProductUnitID { get; set; }
        public string ProductUnit { get; set; }
        public Nullable<bool> Status { get; set; }
        public Nullable<int> CompanyId { get; set; }
    }
}