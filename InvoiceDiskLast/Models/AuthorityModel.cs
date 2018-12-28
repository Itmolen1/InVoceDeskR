using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InvoiceDiskLast.Models
{
    public class AuthorityModel
    {
        public int UserAuthorityId { get; set; }
        public string Authority { get; set; }
        public Nullable<int> CompanyId { get; set; }
       
    }
}