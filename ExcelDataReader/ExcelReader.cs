using System;
using System.Data;
using System.Data.OleDb;
using System.Globalization;
using System.IO;
using System.Linq;
using Ionic.Zip;
using SupermarketSQL.Data;
using SupermarketSQL.Model;
using System.Threading;

namespace ExcelDataReader
{
    public static class ExcelReader
    {
        public static void ExtractZipFIle(string zipfilePath, string unzippedDirectoryPath)
        {
            using (ZipFile zip = new ZipFile(zipfilePath))
            {
                foreach (ZipEntry entry in zip)
                {
                    entry.Extract(unzippedDirectoryPath, ExtractExistingFileAction.OverwriteSilently);
                }
            }
        }

        public static void ParseExcelFile(SupermarketContext sqlDb, DateTime folderDate, string file)
        {
            string connectStr = @"Provider=Microsoft.ACE.OLEDB.12.0; " +
                                @"Data Source=" + file + "; " +
                                @"Extended Properties='Excel 12.0 Xml; HDR=YES'";
            OleDbConnection dbCon = new OleDbConnection(connectStr);
            dbCon.Open();
            DataSet currentDataSet = new DataSet();

            using (dbCon)
            {
                //OleDbCommand cmd = dbCon.CreateCommand();
                //cmd.CommandText = createTableStmt;
                //cmd.ExecuteNonQuery();
                string sqlQuery =
                    "select * from [Sales$]";
                OleDbCommand cmd = new OleDbCommand(sqlQuery, dbCon);
                OleDbDataAdapter myDataAdapter = new OleDbDataAdapter(cmd);

                myDataAdapter.Fill(currentDataSet, "Sales");
            }
            dbCon.Close();

            DataRowCollection excelRows = currentDataSet.Tables["Sales"].Rows;
            string location = string.Empty;
            string saleDate = Path.GetFileName(Path.GetDirectoryName(file));
            int counter = 0;

            foreach (DataRow row in excelRows)
            {
                counter++;

                if (counter == 1)
                {
                    location = row[0].ToString();
                    continue;
                }
                else if (counter == 2 || counter == excelRows.Count)
                {
                    continue;
                }

                int productId = 0;
                int quantity = 0;
                decimal unitPrice = 0;
                decimal sum = 0;

                int.TryParse(row[0].ToString(), out productId);

                int.TryParse(row[1].ToString(), out quantity);

                decimal.TryParse(row[2].ToString(), out unitPrice);

                decimal.TryParse(row[3].ToString(), out sum);

                if (productId != 0)
                {
                    SalesReport currentReport = new SalesReport
                                            {
                                                Location = location,
                                                ProductId = productId,
                                                Quantity = quantity,
                                                Sum = sum,
                                                UnitPrice = unitPrice,
                                                ReportDate = folderDate
                                            };
                    
                    sqlDb.SalesReports.Add(currentReport);
                    sqlDb.SaveChanges();
                }

                //Console.WriteLine(supermarketName);

                //Console.WriteLine("{0}, {1}, {2}, {3}", dataRow[0], dataRow[1], dataRow[2], dataRow[3]);

                //decimal result = 0;

                //decimal.TryParse(dataRow[0].ToString(), out result);
                //Console.WriteLine("PARSE: {0}", result);
            }
            #region old
            //    using (reader)
            //    {
            //        int count = 0;
            //        int locationId = 0;
            //        while (reader.Read())
            //        {
            //            string productId = (string)reader[0].ToString();
            //            string quantity = (string)reader[1].ToString();
            //            string unitPrice = (string)reader[2].ToString();
            //            string sum = (string)reader[3].ToString();

            //            if (productId != "Total sum:")
            //            {
            //                if (count == 0)
            //                {
            //                    //Console.WriteLine("{0}", productId, quantity, unitPrice, sum);
            //                    var foundLocation =
            //                        sqlDb.Locations.Where(x => x.Name == productId).Select(x => x.LocationId);
            //                    if (!foundLocation.Any())
            //                    {
            //                        Location newLocation = new Location
            //                            {
            //                                Name = productId
            //                            };
            //                        locationId = sqlDb.Locations.Add(newLocation).LocationId;
            //                    }
            //                    else
            //                    {
            //                        locationId = foundLocation.First();
            //                    }
            //                }
            //                else if (count == 1)
            //                {
            //                }
            //                else
            //                {
            //                    int parsedProduct = int.Parse(productId);
            //                    //SalesReport currentReport = new SalesReport
            //                    //    {
            //                    //        LocationId = locationId,
            //                    //        ProductId = int.Parse(productId),
            //                    //        Quantity = int.Parse(quantity),
            //                    //        Sum = decimal.Parse(sum),
            //                    //        UnitPrice = decimal.Parse(unitPrice)
            //                    //    };

            //                    //sqlDb.SalesReports.Add(currentReport);
            //                    //sqlDb.SaveChanges();
            //                }

            //                count++;
            //                //Console.WriteLine("{0} {1} {2} {3}", productId, quantity, unitPrice, sum);
            //            }
            //            else
            //            {

            //            }
            //        }
            //    }
            //}
            #endregion
        }

        public static void ParseExcelDirectory(SupermarketContext sqldb, string directoryPath)
        {
            DirectoryInfo dir = new DirectoryInfo(directoryPath);
            foreach (var subDirectory in dir.GetDirectories())
            {
                FileInfo[] files = subDirectory.GetFiles();
                DateTime folderDate = DateTime.Parse(subDirectory.Name);

                foreach (var item in files)
                {
                    Console.WriteLine(item.FullName);
                    ParseExcelFile(sqldb, folderDate, item.FullName);
                    sqldb.SaveChanges();
                    Console.WriteLine();
                }
            }
        }

        public static void Main()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            SupermarketContext sqldb = new SupermarketContext();

            ExtractZipFIle(@"../../Sample-Sales-Reports.zip", @"../../Sample-Sales-Reports");
            ParseExcelDirectory(sqldb, @"../../Sample-Sales-Reports");
        }
    }
}
