using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InvoiceDiskLast.Models
{
    public class InvoiceViewModel
    {
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

        public string UserName { get; set; }
        public string CustomerName { get; set; }




        public int InvoiceDetailId { get; set; }
        public Nullable<int> ItemId { get; set; }
        public string Description { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Nullable<double> Rate { get; set; }
        public Nullable<double> Total { get; set; }
        public Nullable<double> Vat { get; set; }
        public Nullable<double> RowSubTotal { get; set; }       
        public string Type { get; set; }
        public Nullable<System.DateTime> ServiceDate { get; set; }

        public List<InvoiceDetailsTable> InvoiceDetailsTable { get; set; }

    }
}