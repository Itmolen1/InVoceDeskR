using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace InvoiceDiskLast.Models
{
    public class Repositroy<T>
    {
        private DBEntities Entities = new DBEntities();

        //protected DbSet<T> DbSet
        //{
        //    get; set;
        //}

        public Repositroy()
        {
           // DbSet = Entities
        }

    }
}