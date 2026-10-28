using System.Collections.Generic;

namespace Shop.BLL.DTOs
{
    public class OrderItemRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class OrderCreateRequest
    {
        public int UserId { get; set; }
        public string ShippingAddress { get; set; } = "";
        public List<OrderItemRequest> Items { get; set; } = new List<OrderItemRequest>();
    }

    public class UserRegisterRequest
    {
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Phone { get; set; } = "";
    }
}
