﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace InvoiceDiskLast.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class DBEntities : DbContext
    {
        public DBEntities()
            : base("name=DBEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<ComapnyInfo> ComapnyInfoes { get; set; }
        public virtual DbSet<QutationDetailsTable> QutationDetailsTables { get; set; }
        public virtual DbSet<QutationTable> QutationTables { get; set; }
        public virtual DbSet<PurchaseOrderTable> PurchaseOrderTables { get; set; }
        public virtual DbSet<UserTable> UserTables { get; set; }
        public virtual DbSet<ContactsTable> ContactsTables { get; set; }
        public virtual DbSet<ProductUnitTable> ProductUnitTables { get; set; }
        public virtual DbSet<AspNetUser> AspNetUsers { get; set; }
        public virtual DbSet<AccountTable> AccountTables { get; set; }
        public virtual DbSet<ControlAccountTable> ControlAccountTables { get; set; }
        public virtual DbSet<HeadAccountTable> HeadAccountTables { get; set; }
        public virtual DbSet<OrderStatusTable> OrderStatusTables { get; set; }
        public virtual DbSet<PaymentTermTable> PaymentTermTables { get; set; }
        public virtual DbSet<PendingTable> PendingTables { get; set; }
        public virtual DbSet<QutationOrderStatusTable> QutationOrderStatusTables { get; set; }
        public virtual DbSet<AccountTransictionTable> AccountTransictionTables { get; set; }
        public virtual DbSet<ProductTable> ProductTables { get; set; }
        public virtual DbSet<PurchaseOrderDetailsTable> PurchaseOrderDetailsTables { get; set; }
    
        [DbFunction("DBEntities", "GetStockItem")]
        public virtual IQueryable<GetStockItem_Result> GetStockItem(Nullable<int> comapniId)
        {
            var comapniIdParameter = comapniId.HasValue ?
                new ObjectParameter("ComapniId", comapniId) :
                new ObjectParameter("ComapniId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.CreateQuery<GetStockItem_Result>("[DBEntities].[GetStockItem](@ComapniId)", comapniIdParameter);
        }
    }
}
