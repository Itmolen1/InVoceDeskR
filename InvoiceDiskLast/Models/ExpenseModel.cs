using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InvoiceDiskLast.Models
{
    public class ExpenseModel
    {
        public int Id { get; set; }
        public string REFERENCEno { get; set; }
        public int ACCOUNT_ID { get; set; }
        public int VENDOR_ID { get; set; }
        public string notes { get; set; }
        public double SUBTOTAL { get; set; }
        public double VAT_AMOUNT { get; set; }
        public double GRAND_TOTAL { get; set; }
        public int user_id { get; set; }
        public int comapny_id { get; set; }
        public string AddedDate { get; set; }
        public double Vat6 { get; set; }
        public double Vat21 { get; set; }
        public string AccountName { get; set; }
        public string VenderAccount { get; set; }
    }
}