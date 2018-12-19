using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InvoiceDiskLast.Models
{
    public class QutationIndexViewModel
    {
        public int? QutationID { get; set; }

        [Required]
        public string Qutation_ID { get; set; }
        public string RefNumber { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        [DataType(DataType.Date)]
        public Nullable<System.DateTime> QutationDate { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        [DataType(DataType.Date)]
        public Nullable<System.DateTime> DueDate { get; set; }   
      
        public Nullable<double> Vat { get; set; }
        public Nullable<double> TotalAmount { get; set; }

        public String UserName { get; set; }
        public String CustomerName { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }

    }
}