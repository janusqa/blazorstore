using BlazorStore.Dto;

namespace BlazorStore.Service.IService
{
    public interface IOrderService
    {
        public Task<OrderDto> Get(int entityId);
        public Task<IEnumerable<OrderDto>> GetAll(string? userId = null, string? status = null);
        public Task<OrderDto?> Create(OrderDto orderDto);
        public Task<int> Cancel(int entityId);
        public Task<OrderHeaderDto> UpdateOrderDetails(OrderHeaderDto orderHeader);
        public Task<OrderHeaderDto> PaymentConfirmation(int entityId);
        public Task<bool> UpdateOrderStatus(int orderId, string status);
    }

}