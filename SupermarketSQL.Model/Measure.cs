using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;


namespace SupermarketSQL.Model
{
    public class Measure
    {
        private ICollection<Product> products; 

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int MeasureId { get; set; }

        public string MeasureName { get; set; }

        public virtual ICollection<Product> Products
        {
            get { return this.products; }
            set { this.products = value; }
        } 
    }
}
