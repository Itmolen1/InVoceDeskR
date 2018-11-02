using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InvoiceDiskLast.Models
{
    public class OrderStatusModel
    {
        public int OrderStatusId { get; set; }
        public Nullable<int> PurchaseOrderId { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> CompanyId { get; set; }
        public Nullable<int> UserId { get; set; }


    }
}