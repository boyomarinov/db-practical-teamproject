using System;
using System.Collections.Generic;
using System.Linq;


namespace SupermarketSQL.Model
{
    public class Product
    {
        public int ProductId { get; set; }

        public int VendorId { get; set; }

        public virtual Vendor Vendor { get; set; }

        public string ProductName { get; set; }

        public int MeasureId { get; set; }

        public virtual Measure Measure { get; set; }

        public decimal BasePrice { get; set; }

        private ICollection<SalesReport> reports; 
        public virtual ICollection<SalesReport> Reports
        {
            get { return this.reports; }
            set { this.reports = value; }
        } 
    }
}
