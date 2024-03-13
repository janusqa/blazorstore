using BlazorStore.Dto;

namespace BlazorStore.Service.IService
{
    public interface IOrderService
    {
        public Task<OrderDto?> Get(int entityId);
        public Task<IEnumerable<OrderDto>?> GetAll(string? userId = null, string? status = null);
        public Task<OrderDto?> Create(OrderDto orderDto);
        public Task<OrderHeaderDto?> Cancel(int entityId);
        public Task<OrderHeaderDto?> UpdateOrderDetails(OrderHeaderDto orderHeader);
        public Task<OrderHeaderDto?> PaymentConfirmation(int entityId, string userName);
        public Task<bool> UpdateOrderStatus(int entityId, string status);
    }

}