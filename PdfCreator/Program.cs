using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using iTextSharp.text;
using iTextSharp.text.pdf;
using SupermarketSQL.Data;

namespace PdfCreator
{
    public static class Program
    {
        public static void CreatePdf(string path)
        {
            Document myDoc = new Document(PageSize.A4.Rotate());
            PdfWriter.GetInstance(myDoc, new FileStream(path, FileMode.Create));
            myDoc.Open();
            PdfPTable table = new PdfPTable(5);

            table.TotalWidth = 800f;
            table.LockedWidth = true;
            Font fontBold = new Font(Font.FontFamily.HELVETICA, 14f);
            fontBold.SetStyle("bold");
            Font fontNormal = new Font(Font.FontFamily.HELVETICA, 12f);

            PdfPCell cell = new PdfPCell(new Phrase("Aggregated Sales Report", fontBold));
            cell.Colspan = 5;
            cell.Indent = 50;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.Padding = 10;
            table.AddCell(cell);

            SupermarketContext sqldb = new SupermarketContext();
            using (sqldb)
            {
                var dates =
                    (from d in sqldb.SalesReports
                     select d.ReportDate).Distinct().ToList();

                decimal totalTotalSum = 0;

                foreach (var currentDate in dates)
                {
                    PdfPCell cellDate = new PdfPCell(new Phrase("Date : " + currentDate.ToShortDateString()));
                    cellDate.Colspan = 5;
                    cellDate.Padding = 5;
                    cellDate.BackgroundColor = new BaseColor(229, 228, 226);
                    table.AddCell(cellDate);

                    PdfPCell cellProduct = new PdfPCell(new Phrase("Products", fontBold));
                    cellProduct.Padding = 5;
                    cellProduct.BackgroundColor = new BaseColor(229, 228, 226);
                    PdfPCell cellQuantity = new PdfPCell(new Phrase("Quantity", fontBold));
                    cellQuantity.Padding = 5;
                    cellQuantity.BackgroundColor = new BaseColor(229, 228, 226);
                    PdfPCell cellUnitPrice = new PdfPCell(new Phrase("Unit Price", fontBold));
                    cellUnitPrice.Padding = 5;
                    cellUnitPrice.BackgroundColor = new BaseColor(229, 228, 226);
                    PdfPCell cellLocation = new PdfPCell(new Phrase("Location", fontBold));
                    cellLocation.Padding = 5;
                    cellLocation.BackgroundColor = new BaseColor(229, 228, 226);
                    PdfPCell cellSum = new PdfPCell(new Phrase("Sum", fontBold));
                    cellSum.Padding = 5;
                    cellSum.BackgroundColor = new BaseColor(229, 228, 226);

                    table.AddCell(cellProduct);
                    table.AddCell(cellQuantity);
                    table.AddCell(cellUnitPrice);
                    table.AddCell(cellLocation);
                    table.AddCell(cellSum);

                    var sales =
                        (from s in sqldb.SalesReports
                         where s.ReportDate == currentDate
                         select s).Distinct().ToList();

                    decimal totalSum = 0;

                    foreach (var salesReport in sales)
                    {
                        if (salesReport.Product.ProductName != null)
                            {
                                table.AddCell(salesReport.Product.ProductName);
                                table.AddCell(salesReport.Quantity.ToString() + " " + sqldb.Measures
                                                                                    .Where(x => x.MeasureId == salesReport.Product.MeasureId)
                                                                                    .Select(x => x.MeasureName).First().ToString());
                                table.AddCell(salesReport.UnitPrice.ToString());
                                table.AddCell(salesReport.Location);
                                table.AddCell(salesReport.Sum.ToString());

                                totalSum += salesReport.Sum;
                            }
                    }

                    string text = string.Format("Total sum for {0} :", currentDate.ToShortDateString());
                    PdfPCell cellTotal = new PdfPCell(new Phrase(text, fontNormal));
                    cellTotal.Colspan = 4;
                    cellTotal.HorizontalAlignment = 2;
                    table.AddCell(cellTotal);
                    table.AddCell(totalSum.ToString());

                    totalTotalSum += totalSum;

                    #region old

                    //PdfPTable dateTable = new PdfPTable(5);

                    //PdfPCell dateCell = new PdfPCell(new Phrase(reports.First().ReportDate.ToString(), font));//reportsByDate.First().ReportDate.ToString(), font));
                    //dateCell.Colspan = 5;
                    //dateCell.Indent = 50;
                    //dateCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    //dateCell.Padding = 5;
                    //dateCell.BackgroundColor = new BaseColor(229, 228, 226);
                    //dateTable.AddCell(dateCell);

                    //foreach (var report in reports)
                    //{
                    //    //Console.WriteLine("Product={0} Quantity={1} UnitPrice={2} Location={3} Sum={4}",
                    //    //    report.ProductId,
                    //    //    report.Quantity,
                    //    //    report.UnitPrice,
                    //    //    report.Location,
                    //    //    report.Sum);
                    //    string productId = report.ProductId.ToString();
                    //    string quantity = report.Quantity.ToString();
                    //    string unitPrice = report.UnitPrice.ToString();
                    //    string location = report.Location.ToString();
                    //    string sum = report.Sum.ToString();

                    //    dateTable.AddCell(productId);
                    //    dateTable.AddCell(quantity);
                    //    dateTable.AddCell(unitPrice);
                    //    dateTable.AddCell(location);
                    //    dateTable.AddCell(sum);

                    //    myDoc.Add(dateTable);
                    //}

                    ////PdfPCell footer = new PdfPCell();
                    ////string sumTextLeft = "Total sum for " + date;
                    ////string sumText = sqldb.SalesReports.Where(x => x.ReportDate == date).Sum(x => x.Sum).ToString();
                    ////dateTable.AddCell(footer);

                    ////dateTable.AddCell(dateCell);

                    //myDoc.Add(table);

                    #endregion
                }

                string grand = string.Format("Grand Total: " );
                PdfPCell cellGrand = new PdfPCell(new Phrase(grand));
                cellGrand.Colspan = 4;
                cellGrand.HorizontalAlignment = 2;
                table.AddCell(cellGrand);
                table.AddCell(totalTotalSum.ToString());

                myDoc.Add(table);
                myDoc.Close();
            }

            myDoc.Close();
        }

        public static void Main()
        {
            CreatePdf("../../test.pdf");
        }
    }
}
