using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InvoiceDiskLast.Models
{
    public class PaymentTermModel
    {
        public int? PayementTremId { get; set; }
        public string PaymentTerm { get; set; }
        public Nullable<int> FK_CompanyID { get; set; }
        public Nullable<int> AddebBy { get; set; }
        public Nullable<System.DateTime> AddedDate { get; set; }
    }
}