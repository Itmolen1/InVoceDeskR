﻿using System;
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
        public string BillingPersonName { get; set; }
        public string BillingCompanyName { get; set; }
        public string BillingAddress { get; set; }
        public string BillingCity { get; set; }
        public string BillingState { get; set; }
        public string BillingCountry { get; set; }
        public string BillingZibCode { get; set; }
        public string BillingEmail { get; set; }
        public string BillingVatTRN { get; set; }
        public string BillingPhone { get; set; }
        public string BillingMobile { get; set; }
        public string BillingFax { get; set; }
        public string ShippingPersonName { get; set; }
        public string ShippingAddress { get; set; }
        public string ShippingCompanyName { get; set; }
        public string ShippingCity { get; set; }
        public string ShippingState { get; set; }
        public string ShippingCountry { get; set; }
        public string ShippingZIP { get; set; }
        public string ShippingEmail { get; set; }
        public string ShippingVatTRN { get; set; }
        public string ShippingMobile { get; set; }
        public string ShippingPhone { get; set; }
        public string ShippingFax { get; set; }
        public string Remarks { get; set; }
        public Nullable<System.DateTime> Addeddate { get; set; }
        public Nullable<bool> Status { get; set; }
    }
}