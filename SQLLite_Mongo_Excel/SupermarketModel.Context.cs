﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SQLLite_Mongo_Excel
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class SupermarketEntities : DbContext
    {
        public SupermarketEntities()
            : base("name=SupermarketEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<Sale> Sales { get; set; }
        public DbSet<Tax> Taxes { get; set; }
        public DbSet<VendorExpens> VendorExpenses { get; set; }
    }
}