using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InvoiceDiskLast.Models
{
    public class ExpenseDetailModel
    {
        public int Id { get; set; }
        public int  EXPENSE_ACCOUNT_ID { get; set; }
        public string DESCRIPTION { get; set; }
        public decimal  AMOUNT { get; set; }
        public decimal  TAX_PERCENT { get; set; }
        public decimal  TAX_AMOUNT { get; set; }
        public decimal  SUBTOTAL { get; set; }
        public int expense_id { get; set; }
        public int user_id { get; set; }
        public int comapny_id { get; set; }
        public string AccountTitle { get; set; }

       


    }
}