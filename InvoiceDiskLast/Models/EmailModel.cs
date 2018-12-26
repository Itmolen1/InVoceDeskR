using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InvoiceDiskLast.Models
{
    public class EmailModel
    {
        public string From { get; set; }
        public string ToEmail { get; set; }
        public string EmailBody { get; set; }
        public string Subject { get; set; }
        public string Attachment { get; set; }
        public string EmailText { get; set; }
        public string Topic { get; set; }
        public int invoiceId { get; set; }


        public List<Selected> SelectList { get; set; }




    }




    public class AttakmentList
    {
        public string Attckment { get; set; }


    }

    public class Selected
    {
        public bool IsSelected { get; set; }
        public string FileName { get; set; }
        public string Directory { get; set; }

    }

}