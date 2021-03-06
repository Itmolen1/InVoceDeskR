﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InvoiceDiskLast.Models
{
    public class MvcPurchaseViewModel
    {

        public int? PurchaseOrderID { get; set; }
        public string Purchase_ID { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{dd/mm/yyyy}")]
        [DataType(DataType.Date)]
        public Nullable<System.DateTime> PurchaseDate { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{dd/mm/yyyy}")]
        [DataType(DataType.Date)]
        public Nullable<System.DateTime> PurchaseDueDate { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{dd/mm/yyyy}")]
        [DataType(DataType.Date)]
        public Nullable<System.DateTime> PDueDate { get; set; }

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
        public string Type { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{dd/mm/yy}")]
        public Nullable<DateTime> ServiceDate { get; set; }

        public int PurchaseOrderDetailsId { get; set; }
        public Nullable<int> PurchaseItemId { get; set; }
        public string PurchaseDescription { get; set; }
        public Nullable<int> PurchaseQuantity { get; set; }
        public Nullable<double> PurchaseItemRate { get; set; }
        public Nullable<double> PurchaseTotal { get; set; }

        public Nullable<int> PurchaseId { get; set; }
        public int VenderId { get; set; }

        public Nullable<double> Vat6 { get; set; }
        public Nullable<double> Vat21 { get; set; }
        public Nullable<double> RowSubTotal { get; set; }
       
        public PurchaseOrderDetailsTable[] PurchaseOrderList { get; set; }
        public List<MVCPurchaseDetailsModel> PurchaseDetailslist { get; set; }

    }
}