using System;
using Shop.Core.Enums;

namespace Shop.Core.Entities
{
    public class Payment : BaseEntity
    {
        public int OrderId { get; set; }
        public Order? Order { get; set; }

        public DateTime PaymentDate { get; set; } = DateTime.Now;
        public decimal Amount { get; set; }
        public PaymentMethod Method { get; set; } = PaymentMethod.Card;
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
        public string TransactionId { get; set; } = "";
    }

    public class Delivery : BaseEntity
    {
        public int OrderId { get; set; }
        public Order? Order { get; set; }

        public string Carrier { get; set; } = "";
        public string TrackingNumber { get; set; } = "";
        public DeliveryStatus Status { get; set; } = DeliveryStatus.Created;
        public DateTime EstimatedDate { get; set; }
        public DateTime? ActualDeliveryDate { get; set; }
    }

    public class Review : BaseEntity
    {
        public int ProductId { get; set; }
        public Product? Product { get; set; }

        public int UserId { get; set; }
        public User? User { get; set; }

        public int Rating { get; set; }
        public string Comment { get; set; } = "";
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
