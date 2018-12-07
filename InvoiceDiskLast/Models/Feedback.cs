using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InvoiceDiskLast.Models
{
    public class Feedback
    {
        public int Id
        {
            get; set;
        }
        public string Title
        {
            get; set;
        }
        public string Comment
        {
            get; set;
        }


        public string Name
        {
            get; set;
        }
        public string Email
        {
            get; set;
        }
        public string Subject
        {
            get; set;
        }
        public string Message
        {
            get; set;
        }


    }
}