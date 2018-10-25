using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InvoiceDiskLast.Models
{
    public class LoginModel
    {
        public string username { get; set; }
        public string password { get; set; }
        public string grant_type { get; set; }
    }
}