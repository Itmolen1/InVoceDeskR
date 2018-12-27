using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InvoiceDiskLast.Models
{
    

  
    public class MVCCompanyInfoModel
    {
        public int? CompanyID { get; set; }
        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyPhone { get; set; }
        public string CompanyCell { get; set; }
        public string CompanyEmail { get; set; }
        public string CompanyLogo { get; set; }
        public string CompanyTRN { get; set; }
        public string CompanyCity { get; set; }
        public string CompanyState { get; set; }
        public string CompanyCountry { get; set; }
        public Nullable<int> AddedBy { get; set; }
        public Nullable<System.DateTime> AddedDate { get; set; }
        public string UserName { get; set; }
        public string StreetNumber { get; set; }
        public string PostalCode { get; set; }
        public string Website { get; set; }
        public string BankName { get; set; }
        public string IBANNumber { get; set; }
        public string BIC { get; set; }
        public string KVK { get; set; }
        public string BTW { get; set; }
        public HttpPostedFileBase File { get; set; }
    }
}