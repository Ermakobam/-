using System;
using System.Collections.Generic;

namespace Shop.Core.Entities
{
    public class Product : BaseEntity
    {
        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        public string Name { get; set; } = "";
        public string Sku { get; set; } = "";
        public string Description { get; set; } = "";
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public List<Review> Reviews { get; set; } = new List<Review>();
    }
}
