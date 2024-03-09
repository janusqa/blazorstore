using BlazorStore.Dto;

namespace BlazorStore.Service.IService
{
    public interface IPaymentService<T> where T : class
    {
        T Checkout(OrderDto Order);
        T GetSession(OrderHeaderDto OrderHeader);
    }
}