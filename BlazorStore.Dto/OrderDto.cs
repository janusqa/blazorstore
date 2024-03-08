using BlazorStore.Common;

namespace BlazorStore.Dto
{
    public record OrderDto
    {
        public OrderHeaderDto OrderHeader { get; set; }
        public List<OrderDetailDto> OrderDetails { get; set; } = [];

        public OrderDto()
        {
            OrderHeader = new OrderHeaderDto
            {
                UserId = string.Empty,
                Email = string.Empty,
                Name = string.Empty,
                PhoneNumber = string.Empty,
                StreetAddress = string.Empty,
                State = string.Empty,
                City = string.Empty,
                PostalCode = string.Empty,
                Status = SD.OrderStatusPending
            };
        }
    }
}