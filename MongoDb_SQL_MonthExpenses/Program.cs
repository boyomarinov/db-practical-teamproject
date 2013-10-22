using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Xml;
using MongoDB.Driver;
using SupermarketSQL.Model;
using SupermarketSQL.Data;

namespace MongoDb_SQL_MonthExpenses
{
    public static class Program
    {
        public static List<Expense> LoadFromXML(SupermarketContext dbsql, string file)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(file);
            XmlNode rootNode = doc.DocumentElement;
            XmlNode salesNode = rootNode;
            List<Expense> result = new List<Expense>();

            using (dbsql)
            {
                foreach (XmlNode sale in salesNode.ChildNodes)
                {
                    string vendorAttribute = sale.Attributes["vendor"].Value;

                    int vendorId =
                        (from d in dbsql.Vendors
                         where d.VendorName == vendorAttribute
                         select d.VendorId).ToList().First();
                    foreach (XmlNode expenses in sale.ChildNodes)
                    {
                        string monthYear = expenses.Attributes["month"].Value;
                        decimal bla = decimal.Parse(expenses.InnerText);

                        Expense currentExpense = new Expense(vendorId,
                                                             DateTime.ParseExact(monthYear, "MMM-yyyy",
                                                                                 System.Globalization.CultureInfo
                                                                                       .InvariantCulture),
                                                             decimal.Parse(expenses.InnerText));

                        result.Add(currentExpense);
                    }
                }
            }

            return result;
        }

        public static void LoadExpensesToSQL(List<Expense> expenses)
        {
            using (SupermarketContext sql = new SupermarketContext())
            {
                foreach (Expense exp in expenses)
                {
                    VendorExpense toAdd = new VendorExpense
                        {
                            Amount = exp.Amount,
                            VendorId = exp.VendorId,
                            MonthYear = exp.MonthYear
                        };

                    sql.VendorExpenses.Add(toAdd);
                    sql.SaveChanges();
                }
            }
        }

        public static void LoadExpensesToMongo(MongoCollection<Expense> saleReports, List<Expense> expenses)
        {
            foreach (Expense exp in expenses)
            {
                saleReports.Insert(exp);
            }
        }

        public static void ListExpenses(MongoCollection<Expense> saleReports)
        {
            foreach (Expense exp in saleReports.FindAll())
            {
                Console.WriteLine(exp.ToString());
            }
        }

        public static void Main()
        {
            SupermarketContext dbsql = new SupermarketContext();
            Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

            var mongoClient = new MongoClient("mongodb://localhost//");
            var mongoServer = mongoClient.GetServer();
            var supermarketDB = mongoServer.GetDatabase("supermarket");
            MongoCollection<Expense> saleReports = supermarketDB.GetCollection<Expense>("expenses");

            List<Expense> expenses = LoadFromXML(dbsql, @"../../Vendors-Expenses.xml");

            //LoadExpensesToSQL(expenses);
            LoadExpensesToMongo(saleReports, expenses);
            //ListExpenses(saleReports);
        }
    }
}
