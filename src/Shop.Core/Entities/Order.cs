using System;
using System.Collections.Generic;
using Shop.Core.Enums;

namespace Shop.Core.Entities
{
    public class Order : BaseEntity
    {
        public int UserId { get; set; }
        public User? User { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;
        public OrderStatus Status { get; set; } = OrderStatus.New;
        public decimal TotalAmount { get; set; }
        public string ShippingAddress { get; set; } = "";

        public List<OrderItem> Items { get; set; } = new List<OrderItem>();
        public Payment? Payment { get; set; }
        public Delivery? Delivery { get; set; }
    }
}
