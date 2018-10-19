using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InvoiceDiskLast.Models
{
    public class MVCVatDetailsModel
    {
        public int VatID { get; set; }
        [Required]
        public Nullable<double> VatPercentage { get; set; }
        public Nullable<double> Amount { get; set; }
        public Nullable<int> QutationId { get; set; }
        public Nullable<int> CompanyId { get; set; }
        public Nullable<int> AddedBy { get; set; }
        public Nullable<System.DateTime> AddedDate { get; set; }
        public Nullable<int> ClientId { get; set; }
    }
}