using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InvoiceDiskLast.Models
{
    namespace MVCDemo.Models
    {
        [MetadataType(typeof(UserMetaData))]
        public partial class User
        {
        }

        public class UserMetaData
        {
            [Remote("IsUserNameAvailable", "Home", ErrorMessage = "UserName already in use.")]
            public string UserName { get; set; }
        }
    }

}