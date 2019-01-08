using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InvoiceDiskLast.Models
{
    public class MvcBillModel
    {
        public int ? BilID { get; set; }
        public string Bill_ID { get; set; }
        public string RefNumber { get; set; }
        public Nullable<int> PurchaseId { get; set; }
        public DateTime BillDate { get; set; }
        public DateTime BillDueDate { get; set; }
        public Nullable<double> SubTotal { get; set; }
        public Nullable<double> TotalVat6 { get; set; }
        public Nullable<double> TotalVat21 { get; set; }
        public Nullable<double> DiscountAmount { get; set; }
        public Nullable<double> TotalAmount { get; set; }
        public string CustomerNote { get; set; }
        public string Status { get; set; }
        public Nullable<int> UserId { get; set; }
        public Nullable<int> CompanyId { get; set; }
        public Nullable<int> VenderId { get; set; }
        public string Type { get; set; }


    }
}