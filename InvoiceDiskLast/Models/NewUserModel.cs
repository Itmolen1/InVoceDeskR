using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InvoiceDiskLast.Models
{
    public class NewUserModel
    {
        public string email { get; set; }
        public string password { get; set; }
        public string confirmpassword { get; set; }
        public int Authority { get; set; }
    }
}