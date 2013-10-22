using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;


namespace SupermarketSQL.Model
{
    public class Vendor
    {
        private ICollection<Product> products;

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int VendorId { get; set; }

        public string VendorName { get; set; }

        public virtual ICollection<Product> Products
        {
            get { return this.products; }
            set { this.products = value; }
        }
    }
}
