using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using SupermarketSQL.Data;

namespace XmlCreator
{
    public static class Program
    {
        public static void Main()
        {
            SupermarketContext sqldb = new SupermarketContext();

            using (sqldb)
            {
                XElement sales = new XElement("sales");
                foreach (var vendor in sqldb.Vendors.ToList())
                {
                    XElement elSale = new XElement("sale");
                    elSale.SetAttributeValue("vendor", vendor.VendorName.ToString());
                    foreach (var product in vendor.Products.ToList())
                    {
                        foreach (var report in product.Reports.ToList())
                        {
                            XElement summary = new XElement("summary");
                            summary.SetAttributeValue("date", report.ReportDate.ToShortDateString());
                            summary.SetAttributeValue("total-sum", report.Sum.ToString());
                            elSale.Add(summary);
                        }
                    }

                    sales.Add(elSale);
                }

                sales.Save("../../sales.xml");
            }
        }
    }
}