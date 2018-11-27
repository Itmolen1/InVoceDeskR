using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InvoiceDiskLast.Models
{
    public class Control_Head_Account_tran_ViewModel
    {
        public int? ControlAccountId { get; set; }
        public string ControleAccountTitile { get; set; }
        public int? HeadAccountId { get; set; }
        public string HeadAccountTitle { get; set; }
        public int? AccountId { get; set; }
        public string AccountTitle { get; set; }
        public int Id { get; set; }
        public double? AmountCredit { get; set; }
        public double? AmountDebit { get; set; }
    }
}