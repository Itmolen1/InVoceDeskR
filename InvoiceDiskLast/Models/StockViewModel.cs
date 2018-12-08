using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InvoiceDiskLast.Models
{
    public class StockViewModel
    {
        public int ? PurchaseItemId { get; set; }
        public string ProductName { get; set; }

        public int ? PurchaseQuantity { get; set; }
        public int ? SaleQuantity { get; set; }
        public int ? RemaingQuantity { get; set; }
    }
}