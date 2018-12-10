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

        public List<FileNam> FileNameList { get; set; }
    }


    public class FileNam
    {
        public string FileName { get; set; }
    }
}