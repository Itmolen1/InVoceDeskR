using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InvoiceDiskLast.Models
{
    public class ExpenseDetailModel
    {
        public int Id { get; set; }
        public Nullable<int> EXPENSE_ACCOUNT_ID { get; set; }
        public string DESCRIPTION { get; set; }
        public Nullable<decimal> AMOUNT { get; set; }
        public Nullable<decimal> TAX_PERCENT { get; set; }
        public Nullable<decimal> TAX_AMOUNT { get; set; }
        public Nullable<decimal> SUBTOTAL { get; set; }
        public Nullable<int> expense_id { get; set; }
        public Nullable<int> user_id { get; set; }
        public Nullable<int> comapny_id { get; set; }

        public string AccountTitle { get; set; }



    }
}