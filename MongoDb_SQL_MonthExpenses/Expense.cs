﻿using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using SupermarketSQL.Model;

namespace MongoDb_SQL_MonthExpenses
{
    public class Expense
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public int VendorId { get; set; }

        public DateTime MonthYear { get; set; }

        public decimal Amount { get; set; }

        public Expense(int vendorId, DateTime monthYear, decimal amount)
        {
            this.VendorId = vendorId;
            this.MonthYear = monthYear;
            this.Amount = amount;
        }

        public override string ToString()
        {
            return string.Format("Id: {0}, VendorId: {1}, MonthYear: {2}, Amount: {3}", this.Id, this.VendorId, this.MonthYear, this.Amount);
        }
    }
}
