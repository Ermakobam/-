using Shop.BLL.DTOs;

namespace Shop.Tests
{
    public class DtoTests
    {
        [Fact]
        public void OrderDtos_HaveSafeDefaults()
        {
            OrderItemRequest item = new OrderItemRequest();
            Assert.Equal(0, item.ProductId);
            Assert.Equal(0, item.Quantity);
            item.ProductId = 1;
            item.Quantity = 2;

            OrderCreateRequest order = new OrderCreateRequest();
            Assert.Equal(0, order.UserId);
            Assert.Equal("", order.ShippingAddress);
            Assert.Empty(order.Items);
            order.UserId = 5;
            order.ShippingAddress = "addr";
            order.Items.Add(item);
            Assert.Single(order.Items);
        }

        [Fact]
        public void UserRegisterRequest_HaveSafeDefaults()
        {
            UserRegisterRequest request = new UserRegisterRequest();
            Assert.Equal("", request.Email);
            Assert.Equal("", request.Password);
            Assert.Equal("", request.FirstName);
            Assert.Equal("", request.LastName);
            Assert.Equal("", request.Phone);
            request.Email = "e";
            request.Password = "p";
            request.FirstName = "f";
            request.LastName = "l";
            request.Phone = "t";
            Assert.Equal("e", request.Email);
        }
    }
}
