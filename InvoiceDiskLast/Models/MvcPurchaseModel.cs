using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InvoiceDiskLast.Models
{
    public class MvcPurchaseModel
    {
        public int? PurchaseOrderID { get; set; }
        public string PurchaseID { get; set; }
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

        public int? VenderId { get; set; }

        public Nullable<double> Vat6 { get; set; }
        public Nullable<double> Vat21 { get; set; }

        public double ? PurchaseTotal { get; set; }

      

        public string Type { get; set; }


    }
}