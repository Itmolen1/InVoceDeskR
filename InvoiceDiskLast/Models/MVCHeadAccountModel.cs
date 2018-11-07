using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InvoiceDiskLast.Models
{
    public class MVCHeadAccountModel
    {
        public int? HeadAccountId { get; set; }
        public string HeadAccountTitle { get; set; }
        public string HeadAccountDescription { get; set; }
        public Nullable<int> FK_ControlAccountID { get; set; }
        public Nullable<int> FK_CompanyId { get; set; }
        public Nullable<int> AddedBy { get; set; }

    }
}