using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InvoiceDiskLast.Models
{
    public class CommonModel
    {
        public string Name { get; set; }
        public System.DateTime FromDate { get; set; }
        public System.DateTime DueDate { get; set; }
        public string Number_Id { get; set; }
        public string ReferenceNumber { get; set; }

        public string Note { get; set; }

        public string SubTotal { get; set; }
        public string Vat6 { get; set; }
        public string Vat21 { get; set; }
        public string grandTotal { get; set; }
    }
}
