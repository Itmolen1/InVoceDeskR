using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InvoiceDiskLast.Models
{
    public class MVCQutationViewModel
    {
        public int? QutationID { get; set; }

        [Required]
        public string Qutation_ID { get; set; }
        public string RefNumber { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/mm/yyyy}")]
        [DataType(DataType.Date)]
        public System.DateTime QutationDate { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/mm/yyyy}")]
        [DataType(DataType.Date)]
        public System.DateTime DueDate { get; set; }
        public Nullable<double> SubTotal { get; set; }
        public Nullable<double> DiscountAmount { get; set; }
        public Nullable<double> Vat { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/mm/yyyy}")]
        public Nullable<System.DateTime> DueDate1 { get; set; }

        public bool Isdelete { get; set; }
        public Nullable<double> TotalAmount { get; set; }
        public string CustomerNote { get; set; }
        public string Status { get; set; }
        public Nullable<int> UserId { get; set; }
        public Nullable<int> CompanyId { get; set; }

        public Nullable<float> TotalVat6 { get; set; }
        public Nullable<float> TotalVat21 { get; set; }

        public Nullable<double> RowSubTotal { get; set; }

        public Nullable<DateTime> ServiceDate { get; set; }
        public int? QutationDetailId { get; set; }
        public Nullable<int> ItemId { get; set; }
        public string Description { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Nullable<double> Rate { get; set; }
        public Nullable<double> Total { get; set; }
        public int? ConatctId { get; set; }
        public string ItemName { get; set; }
        public int? QuantityRemaing { get; set; }

        public int ExeecdQuantity { get; set; }

        public int SaleQuantity { get; set; }

        public string Type { get; set; }
        public QutationDetailsTable[] QutationDetailslist1 { get; set; }


        public HttpPostedFileWrapper[] file23 { get; set; }
        public HttpPostedFileBase[] Files { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> sdate { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> sdate2 { get; set; }
        public List<QutationDetailsTable> QutationDetailslist { get; set; }

    }
}