using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InvoiceDiskLast.Models
{
    public class PaymentTermUdrationModel
    {
        public int PaymentDurationId { get; set; }
        public Nullable<int> PaymentDuration { get; set; }
        public Nullable<int> CompanyId { get; set; }
        public Nullable<bool> Status { get; set; }
    }
}