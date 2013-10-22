using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;


namespace SQLLite_Mongo_Excel
{
    public class MongoSale
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public string ProductId { get; set; }

        public string ProductName { get; set; }

        public string VendorName { get; set; }

        public string TotalQuantitySold { get; set; }

        public string TotalIncomes { get; set; }

        public MongoSale(string productId, string productName, string vendorName, string totalQuantitySold, string totalIncomes)
        {
            this.ProductId = productId;
            this.ProductName = productName;
            this.VendorName = vendorName;
            this.TotalQuantitySold = totalQuantitySold;
            this.TotalIncomes = totalIncomes;
        }

        public override string ToString()
        {
            return string.Format("Id: {0}, ProductId: {1}, ProductName: {2}, VendorName: {3}, TotalQuantitySold: {4}, TotalIncomes: {5}", this.Id, this.ProductId, this.ProductName, this.VendorName, this.TotalQuantitySold, this.TotalIncomes);
        }
    }
}
