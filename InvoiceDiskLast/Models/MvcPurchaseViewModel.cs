﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InvoiceDiskLast.Models
{
    public class MvcPurchaseViewModel
    {

        public int ?  PurchaseOrderID { get; set; }
        public string Purchase_ID { get; set; }
        public Nullable<DateTime> PurchaseDate { get; set; }
        public Nullable<System.DateTime> PurchaseDueDate { get; set; }
        public string PurchaseRefNumber { get; set; }
        public Nullable<double> PurchaseSubTotal { get; set; }
        public Nullable<double> PurchaseDiscountPercenteage { get; set; }
        public Nullable<double> PurchaseDiscountAmount { get; set; }
        public Nullable<double> PurchaseVatPercentage { get; set; }
        public Nullable<double> PurchaseTotoalAmount { get; set; }
        public string PurchaseVenderNote { get; set; }
        public string Status { get; set; }
        public Nullable<int> CompanyId { get; set; }
        public Nullable<int> UserId { get; set; }
        public Nullable<System.DateTime> AddedDate { get; set; }

        public string PurchaseItemName { get; set; }


        public int PurchaseOrderDetailsId { get; set; }
        public Nullable<int> PurchaseItemId { get; set; }
        public string PurchaseDescription { get; set; }
        public Nullable<int> PurchaseQuantity { get; set; }
        public Nullable<double> PurchaseItemRate { get; set; }
        public Nullable<double> PurchaseTotal { get; set; }
        
        public Nullable<int> PurchaseId { get; set; }

        public Nullable<double> Vat6 { get; set; }
        public Nullable<double> Vat21 { get; set; }

        public List<PurchaseOrderDetailsTable> PurchaseDetailslist { get; set; }

    }
}