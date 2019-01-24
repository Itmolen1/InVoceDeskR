﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InvoiceDiskLast.Models
{
    public class ExpenseViewModel
    {
        public int Id { get; set; }
        public string REFERENCEno { get; set; }
        public Nullable<int> ACCOUNT_ID { get; set; }
        public Nullable<int> VENDOR_ID { get; set; }
        public string notes { get; set; }
        public Nullable<decimal> SUBTOTAL { get; set; }
        public Nullable<decimal> VAT_AMOUNT { get; set; }
        public Nullable<decimal> GRAND_TOTAL { get; set; }
        public Nullable<int> user_id { get; set; }

        public Nullable<decimal> Vat6 { get; set; }
        public Nullable<decimal> Vat21 { get; set; }

        public string PaidThrougAccount { get; set; }
        public string VenderName { get; set; }

        public string UserName { get; set; }

        public string CompanyName { get; set; }



        public int EXPENSE_ACCOUNT_ID { get; set; }
        public string DESCRIPTION { get; set; }
        public Nullable<decimal> AMOUNT { get; set; }
        public Nullable<decimal> TAX_PERCENT { get; set; }
        public Nullable<decimal> TAX_AMOUNT { get; set; }

        public Nullable<int> expense_id { get; set; }

        public Nullable<int> comapny_id { get; set; }
        public string AccountTitle { get; set; }


        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{dd/mm/yyyy}")]
        [DataType(DataType.Date)]
        public Nullable<DateTime> AddedDate { get; set; }


        public HttpPostedFileWrapper[] file23 { get; set; }
        public List<ExpenseDetail> ExpensenDetailList { get; set; }


        public int TotalRecord { get; set; }

    }
}