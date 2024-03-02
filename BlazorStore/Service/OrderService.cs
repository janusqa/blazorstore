using BlazorStore.Dto;
using BlazorStore.Service.IService;

namespace BlazorStore.Service
{
    public class OrderService : IOrderService
    {
        public Task<int> Cancel(int entityId)
        {
            throw new NotImplementedException();
        }

        public Task<OrderDto> Create(OrderDto orderDto)
        {
            throw new NotImplementedException();
        }

        public Task<OrderDto> Get(int entityId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<OrderDto>> GetAll(string? userId = null, string? status = null)
        {
            throw new NotImplementedException();
        }

        public Task<OrderHeaderDto> PaymentConfirmation(int entityId)
        {
            throw new NotImplementedException();
        }

        public Task<OrderHeaderDto> UpdateOrderDetails(OrderHeaderDto orderHeader)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateOrderStatus(int orderId, string status)
        {
            throw new NotImplementedException();
        }
    }
}