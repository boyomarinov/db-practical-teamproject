using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Supermarket.Model;
using SupermarketSQL.Data;
using SupermarketSQL.Data.Migrations;

namespace Application
{
    public static class Program
    {
        public static void SeedDatabase(SupermarketModel mysql, SupermarketContext sql)
        {
            using (mysql)
            {
                using (sql)
                {
                    //sql.Database.Delete();

                    foreach (var measure in mysql.Measures)
                    {
                        SupermarketSQL.Model.Measure measureToAdd = new SupermarketSQL.Model.Measure
                            {
                                MeasureId = measure.MeasureId,
                                MeasureName = measure.MeasureName
                            };

                        sql.Measures.Add(measureToAdd);
                        sql.SaveChanges();
                    }

                    foreach (var vendor in mysql.Vendors)
                    {
                        SupermarketSQL.Model.Vendor vendorToAdd = new SupermarketSQL.Model.Vendor
                        {
                            VendorId = vendor.VendorId,
                            VendorName = vendor.VendorName
                        };

                        sql.Vendors.Add(vendorToAdd);
                        sql.SaveChanges();
                    }

                    foreach (var product in mysql.Products)
                    {
                        SupermarketSQL.Model.Product productToAdd = new SupermarketSQL.Model.Product
                            {
                                VendorId = product.VendorId,
                                ProductName = product.ProductName,
                                MeasureId = product.MeasureId,
                                BasePrice = product.BasePrice
                            };

                        sql.Products.Add(productToAdd);
                        sql.SaveChanges();
                    }
                }
            }
        }

        public static void Main()
        {
            SupermarketModel mysqlDb = new SupermarketModel();
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<SupermarketContext, Configuration>());
            var sqlDb = new SupermarketContext();

            SeedDatabase(mysqlDb, sqlDb);
        }
    }
}
