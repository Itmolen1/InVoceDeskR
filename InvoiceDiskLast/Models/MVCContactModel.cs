using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InvoiceDiskLast.Models
{
    public class MVCContactModel
    {
        public int? ContactsId { get; set; }
        public string ContactName { get; set; }
        public string ContactAddress { get; set; }
        public Nullable<int> Company_Id { get; set; }
        public Nullable<int> UserId { get; set; }
        public string Type { get; set; }
        public Nullable<System.DateTime> Addeddate { get; set; }
        public Nullable<bool> Status { get; set; }
        public string StreetNumber { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string LandLine { get; set; }
        public string telephone { get; set; }
        public string Mobile { get; set; }
        public string Website { get; set; }
        public string BillingEmail { get; set; }
        public string PersonCompany { get; set; }
        public string Remarks { get; set; }
        public int? PaymentTerm { get; set; }

    }
}