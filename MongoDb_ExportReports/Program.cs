using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MongoDB.Bson;
using MongoDB.Driver;
using SupermarketSQL.Model;
using SupermarketSQL.Data;

namespace MongoDb_ExportReports
{
    public static class Program
    {
        private static void AddProductsFromDb(MongoCollection<Sale> saleReports)
        {
            SupermarketContext sqldb = new SupermarketContext();
            using (sqldb)
            {
                var salesReportsFromSQL = sqldb.SalesReports.Include("Product").ToList();

                foreach (var product in sqldb.Products.ToList())
                {
                    string productId = product.ProductId.ToString();
                    string productName = product.ProductName;
                    string vendorName = product.Vendor.VendorName;
                    int totalQuantitySoldCount =
                        salesReportsFromSQL.Where(x => x.ProductId == product.ProductId)
                                           .GroupBy(x => x.ProductId)
                                           .Count();
                    string totalQuantitySold = totalQuantitySoldCount.ToString();
                    string totalIncomes = (product.BasePrice * totalQuantitySoldCount).ToString();

                    Sale currentSale = new Sale(productId, productName, vendorName, totalQuantitySold, totalIncomes);

                    saleReports.Insert(currentSale);
                }
            }
        }

        public static void SaveToFile(string path, MongoCollection<Sale> saleReports)
        {
            var directory = Directory.CreateDirectory(path);

            foreach (Sale sale in saleReports.FindAllAs<Sale>())
            {
                string currentPath = path + "/" + sale.ProductId + "_report.json";
                using (StreamWriter writer = new StreamWriter(currentPath))
                {
                    writer.Write(sale.ToJson());
                }
            }
        }

        public static void Main()
        {
            var mongoClient = new MongoClient("mongodb://localhost//");
            var mongoServer = mongoClient.GetServer();
            var supermarketDB = mongoServer.GetDatabase("supermarket");
            MongoCollection<Sale> saleReports = supermarketDB.GetCollection<Sale>("reports");

            AddProductsFromDb(saleReports);

            //saleReports.RemoveAll();

            //check integrity
            //foreach (var sale in saleReports.FindAll())
            //{
            //    Console.WriteLine(sale.ToString());
            //}

            //SaveToFile(@"../../Reports", saleReports);
        }
    }
}
