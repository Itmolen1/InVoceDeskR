using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InvoiceDiskLast.Models
{
    public class UserModels
    {
        public int? UserId { get; set; }
        public string UserFname { get; set; }
        public string Insertion { get; set; }
        public string UserLname { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
       
        public Nullable<int> Gender { get; set; }
        public string DOB { get; set; }
        public Nullable<System.DateTime> AddedDate { get; set; }
        public Nullable<int> UserAuthorityID { get; set; }

        public bool Rememberme { get; set; }

        public string GuidToken { get; set; }
    }
}