using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InvoiceDiskLast.Models
{
    public class TransactionModel
    {
        public int Id { get; set; }
        public int FromAccountId { get; set; }
        public int ToAccountId { get; set; }
        public double TotalAmount { get; set; }
              
        public string descrition { get; set; }
        public string TransactionType { get; set; }
        public int CompanyId { get; set; }
        public string Message { get; set; }

        public int paymentTermId { get; set; }

        public  DateTime? TranDate { get; set; }

        public int PurchaseOrderID { get; set; }
        public double? AmountCredit{ get; set;}
        public double? AmountDebit { get; set; }

        public string AccountCode { get; set; }

        public  string AccountTitle { get; set; }



    } 
}