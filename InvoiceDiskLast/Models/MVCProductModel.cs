﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InvoiceDiskLast.Models
{
    public class MVCProductModel
    {
        public int? ProductId { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public Nullable<double> SalePrice { get; set; }
        public Nullable<double> PurchasePrice { get; set; }
        public string Type { get; set; }
        public Nullable<int> OpeningQuantity { get; set; }
        public Nullable<int> AddedBy { get; set; }
        public Nullable<int> Company_ID { get; set; }
        public string StreetNumber { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }

        public string telephone { get; set; }
        public string Mobile { get; set; }

        public Nullable<System.DateTime> AddedDate { get; set; }
    }
}