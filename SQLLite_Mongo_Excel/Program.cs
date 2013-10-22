using System;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using MongoDB.Driver;
using SupermarketSQL.Data;
using Microsoft.Office.Interop.Excel;


namespace SQLLite_Mongo_Excel
{
    public static class Program
    {
        public static void LoadSaleReports(MongoCollection<MongoSale> salesCollection)
        {
            int count = 0;
            using (SupermarketEntities sqliteDb = new SupermarketEntities())
            {
                foreach (MongoSale mongoSale in salesCollection.FindAll().ToList())
                {
                    Sale toAdd = new Sale
                        {
                            ProductName = mongoSale.ProductName,
                            TotalIncomes = mongoSale.TotalIncomes,
                            TotalQuantitySold = mongoSale.TotalQuantitySold,
                            VendorName = mongoSale.VendorName
                        };

                    count++;
                    sqliteDb.Sales.Add(toAdd);
                }

                sqliteDb.SaveChanges();
            }
            Console.WriteLine(count + " sale reports added!");
        }

        public static void LoadExpenses(MongoCollection<MongoVendorExpense> expenseCollection)
        {
            int count = 0;
            using (SupermarketEntities sqliteDb = new SupermarketEntities())
            {
                foreach (MongoVendorExpense mongoExpense in expenseCollection.FindAll())
                {
                    VendorExpens toAdd = new VendorExpens
                    {
                        VendorId = mongoExpense.VendorId,
                        Amount = mongoExpense.Amount,
                        MonthYear = mongoExpense.MonthYear
                    };

                    count++;
                    sqliteDb.VendorExpenses.Add(toAdd);
                }

                sqliteDb.SaveChanges();
            }
            Console.WriteLine(count + " expenses added!");
        }

        public static void ReadSqliteExpenses()
        {
            using (SupermarketEntities sqliteDb = new SupermarketEntities())
            {
                foreach (var vendorExpense in sqliteDb.VendorExpenses)
                {
                    Console.WriteLine("{0} {1} {2}", vendorExpense.VendorId, vendorExpense.MonthYear, vendorExpense.Amount);
                }
            }
        }

        public static void ReadSqliteReports()
        {
            using (SupermarketEntities sqliteDb = new SupermarketEntities())
            {
                foreach (var sales in sqliteDb.Sales)
                {
                    Console.WriteLine("{0} {1} {2} {3}", sales.ProductName, sales.VendorName, sales.TotalIncomes, sales.TotalQuantitySold);
                }
            }
        }

        static void AddRow(OleDbConnection dbCon, string vendor, string incomes, string expenses, string taxes, string result)
        {
            OleDbCommand cmdInsert = new OleDbCommand("insert into [Sheet1$](vendor, incomes, expenses, taxes,  [Financial Result]) " +
                "values(@vendor,  @incomes, @expenses, @taxes, @results)", dbCon);
            cmdInsert.Parameters.AddWithValue("@vendor", vendor);
            cmdInsert.Parameters.AddWithValue("@incomes", incomes);
            cmdInsert.Parameters.AddWithValue("@expenses", expenses);
            cmdInsert.Parameters.AddWithValue("@taxes", taxes);
            cmdInsert.Parameters.AddWithValue("@[Financial Result]", result);
            cmdInsert.ExecuteNonQuery();
        }

        public static void WriteFinalReport(string file)
        {
            if (File.Exists(file))
            {
                File.Delete(file);
            }

            string connectStr = @"Provider=Microsoft.ACE.OLEDB.12.0; " +
                                @"Data Source=" + file + "; " +
                                @"Extended Properties='Excel 12.0 Xml; HDR=YES'";
            OleDbConnection dbCon = new OleDbConnection(connectStr);
            dbCon.Open();
            using (dbCon)
            {
                using (SupermarketEntities sqliteDb = new SupermarketEntities())
                {
                    using (SupermarketContext sqlDb = new SupermarketContext())
                    {
                        OleDbCommand cmd = new OleDbCommand("CREATE TABLE [Sheet1] ([Vendor] string, [Incomes] string, [Expenses] string, [Taxes] string, [Financial Result] string)"
                    , dbCon);

                        cmd.ExecuteNonQuery();

                        var vendorsList = sqliteDb.VendorExpenses
                                .Where(x => (x.MonthYear.Year == DateTime.Now.Year) && (x.MonthYear.Month == DateTime.Now.Month))
                                .GroupBy(report => report.VendorId);

                        foreach (var vendors in vendorsList)
                        {
                            int vendorId = vendors.First().VendorId;

                            var vendorsWithId = sqlDb.Vendors.Where(x => x.VendorId == vendorId);

                            string name = vendorsWithId.First().VendorName;

                            decimal incomes = 0;
                            foreach (var incomingString in sqliteDb.Sales
                                    .Where(x => x.VendorName == name).Select(x => x.TotalIncomes))
                            {
                                if (incomingString != null)
                                {
                                    incomes += decimal.Parse(incomingString);
                                }
                            }

                            decimal expenses = vendors
                                .Where(x => x.VendorId == vendorId)
                                .Sum(x => x.Amount);
                            decimal taxes = 0;
                            foreach (var taxAmountString in sqliteDb.Taxes
                                    .Where(x => x.VendorId == vendorId).Select(x => x.TaxAmount))
                            {
                                if (taxAmountString != null)
                                {
                                    taxes += taxAmountString;
                                }
                            }

                            decimal financialResult = incomes - expenses - taxes;

                            AddRow(dbCon, name, incomes.ToString(), expenses.ToString(), taxes.ToString(), financialResult.ToString());
                        }
                    }
                }
            }
        }

        public static void FormatExcel(string oldPath, string newPath)
        {
            if (File.Exists(newPath))
            {
                File.Delete(newPath);
            }

            var application = new Application();
            var workbook = application.Workbooks.Open(oldPath);
            Worksheet sheet = workbook.ActiveSheet;
            sheet.Cells[1, 1].EntireRow.Font.Bold = true;
            sheet.Columns["A:A"].ColumnWidth = 25;
            sheet.Columns["B:B"].ColumnWidth = 13;
            sheet.Columns["C:C"].ColumnWidth = 13;
            sheet.Columns["D:D"].ColumnWidth = 13;
            sheet.Columns["E:E"].ColumnWidth = 17;
            sheet.Cells[1, 5].EntireColumn.Font.Bold = true;

            workbook.SaveAs(newPath);
            workbook.Close();
        }

        public static void Main()
        {
            SupermarketEntities sqliteDb = new SupermarketEntities();
            var mongoClient = new MongoClient("mongodb://localhost//");
            var mongoServer = mongoClient.GetServer();
            var supermarketDB = mongoServer.GetDatabase("supermarket");
            MongoCollection<MongoVendorExpense> expenses = supermarketDB.GetCollection<MongoVendorExpense>("expenses");
            MongoCollection<MongoSale> sales = supermarketDB.GetCollection<MongoSale>("reports");

            //LoadExpenses(expenses);
            //LoadSaleReports(sales);

            //ReadSqliteExpenses();
            //Console.WriteLine();
            //ReadSqliteReports();
            //WriteFinalReport(@"../../Products-Total-Report.xlsx");
            //FormatExcel(@"E:\Visual projects\Databases_TeamProject_GinFizz\SQLLite_Mongo_Excel\Products-Total-Report.xlsx",
                        @"E:\Visual projects\Databases_TeamProject_GinFizz\SQLLite_Mongo_Excel\formatted.xlsx");
            
        }
    }
}
