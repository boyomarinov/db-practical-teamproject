using System;
using System.Data.Entity;
using SupermarketSQL.Model;

namespace SupermarketSQL.Data
{
    public class SupermarketContext : DbContext
    {
        public SupermarketContext()
            : base("Supermarket")
        {
        }

        public DbSet<Measure> Measures { get; set; }

        public DbSet<Vendor> Vendors { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<SalesReport> SalesReports { get; set; }

        public DbSet<VendorExpense> VendorExpenses { get; set; }
    }
}
