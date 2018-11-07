using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InvoiceDiskLast.Models
{
    public class MVCAccountTableModel
    {
        public int? AccountId { get; set; }
        public string AccountCode { get; set; }
        public string AccountTitle { get; set; }
        public string AccountDescription { get; set; }
        public Nullable<int> FK_HeadAccountId { get; set; }
        public Nullable<int> AddedBy { get; set; }
        public Nullable<int> FK_CompanyId { get; set; }
    }
}