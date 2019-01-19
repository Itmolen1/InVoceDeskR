﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InvoiceDiskLast.Models
{
    public class QuotationReportModel
    {
        public int QutationID { get; set; }
        public string Qutation_ID { get; set; }
        public string RefNumber { get; set; }
    
        public String QutationDate { get; set; }
        public String DueDate { get; set; }
        public double SubTotal { get; set; }
        public double TotalVat6 { get; set; }
        public double TotalVat21 { get; set; }
        public double DiscountAmount { get; set; }
        public double TotalAmount { get; set; }
        public string CustomerNote { get; set; }
        public string Status { get; set; }



    }
}