namespace Shop.Core.Enums
{
    public enum OrderStatus
    {
        New = 1,
        Paid = 2,
        Processing = 3,
        Shipped = 4,
        Delivered = 5,
        Cancelled = 6
    }

    public enum PaymentMethod
    {
        Card = 1,
        SBP = 2,
        Cash = 3
    }

    public enum PaymentStatus
    {
        Pending = 1,
        Success = 2,
        Failed = 3
    }

    public enum DeliveryStatus
    {
        Created = 1,
        InTransit = 2,
        Delivered = 3,
        Failed = 4
    }
}
