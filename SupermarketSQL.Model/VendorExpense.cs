using System;
using System.Collections.Generic;
using System.Linq;


namespace SupermarketSQL.Model
{
    public class VendorExpense
    {
        public int Id { get; set; }

        public int VendorId { get; set; }

        public virtual Vendor Vendor { get; set; }

        public DateTime MonthYear { get; set; }

        public decimal Amount { get; set; }
    }
}
