using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InvoiceDiskLast.Models
{
    public class BillDetailViewModel
    {
        public int? BilID { get; set; }

        public int? BillDetailId { get; set; }
        public string Bill_ID { get; set; }
        public string RefNumber { get; set; }
        public Nullable<int> PurchaseId { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{dd/MM/yyyy}")]
        [DataType(DataType.Date)]
        public DateTime BillDueDate { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{dd/MM/yyyy}")]
        [DataType(DataType.Date)]
        public DateTime BillDate { get; set; }
        public string Description { get; set; }

        public int? Quantity { get; set; }


        public Nullable<double> SubTotal { get; set; }
        public Nullable<double> TotalVat6 { get; set; }
        public Nullable<double> TotalVat21 { get; set; }
        public Nullable<double> Vat { get; set; }
        public Nullable<double> RowSubTotal { get; set; }
        public Nullable<double> Total { get; set; }

        public Nullable<double> Rate { get; set; }
        public int? ItemId { get; set; }
        public string ItemName { get; set; }
        public Nullable<double> DiscountAmount { get; set; }
        public Nullable<double> TotalAmount { get; set; }
        public string CustomerNote { get; set; }
        public string Status { get; set; }

        public Nullable<DateTime> ServiceDate { get; set; }

        public Nullable<int> UserId { get; set; }
        public Nullable<int> CompanyId { get; set; }
        public Nullable<int> VenderId { get; set; }
        public string Type { get; set; }
        public HttpPostedFileWrapper[] file23 { get; set; }
        public BillDetailTable[] BillDetail { get; set; }

    }
}