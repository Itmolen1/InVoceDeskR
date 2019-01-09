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
    }
}