using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InvoiceDiskLast.Models
{
    public class QuatationReportViewModel
    {
        //CompanyModel     
        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyPhone { get; set; }
        public string CompanyCell { get; set; }
        public string CompanyEmail { get; set; }
        public string CompanyLogo { get; set; }
        public string CompanyCity { get; set; }
        public string CompanyCountry { get; set; }
        public string CompnayStreetNumber { get; set; }
        public string CompnayPostalCode { get; set; }
        public string IBANNumber { get; set; }
        public string BIC { get; set; }
        public string KVK { get; set; }
        public string BTW { get; set; }
        public string BankName { get; set; }
        //Contact Model      
        public string ContactName { get; set; }
        public string ContactAddress { get; set; }
        public string ConatctStreetNumber { get; set; }
         public string PostalCode { get; set; }
        public string City { get; set; }
        public string Land { get; set; }
        public string LandLine { get; set; }
        public string telephone { get; set; }
        public string Mobile { get; set; }
        public string BillingEmail { get; set; }
        public string PersonCompany { get; set; }

        // Quotation Model
        public int QutationID { get; set; }
        public string Qutation_ID { get; set; }
        public string RefNumber { get; set; }
        public System.DateTime QutationDate { get; set; }
        public System.DateTime DueDate { get; set; }
        public string CustomerNote { get; set; }


        public double SubTotal { get; set; }
        public double  TotalVat6 { get; set; }
        public double  TotalVat21 { get; set; }
        public double DiscountAmount { get; set; }
        public double  TotalAmount { get; set; }



        // Qutation Detail  Model       
        public string ItemName { get; set; }
        public string Description { get; set; }
        public int  Quantity { get; set; }
        public double Rate { get; set; }
        public double  Total { get; set; }
        public double  Vat { get; set; }
        public string Type { get; set; }
        //public Nullable<System.DateTime ServiceDate { get; set; }
        public double  RowSubTotal { get; set; }
    }
}