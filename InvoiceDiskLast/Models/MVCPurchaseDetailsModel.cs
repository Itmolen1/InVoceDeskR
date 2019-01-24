using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InvoiceDiskLast.Models
{
    public class MVCPurchaseDetailsModel
    {
        public int PurchaseOrderDetailsId { get; set; }
        public Nullable<int> PurchaseItemId { get; set; }
        public string PurchaseDescription { get; set; }
        public Nullable<int> PurchaseQuantity { get; set; }
        public Nullable<double> PurchaseItemRate { get; set; }
        public Nullable<double> PurchaseTotal { get; set; }
        public Nullable<double> PurchaseVatPercentage { get; set; }
        public Nullable<int> PurchaseId { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{dd/mm/yy}")]
        public Nullable<DateTime> ServiceDate { get; set; }

        public Nullable<double> RowSubTotal { get; set; }
        public string Type { get; set; }

    }
}