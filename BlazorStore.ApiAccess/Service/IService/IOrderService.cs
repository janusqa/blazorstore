
using BlazorStore.Dto;

namespace BlazorStore.ApiAccess.Service
{
    public interface IOrderService : IRepository<ApiResponse>
    {
        Task<ApiResponse?> FinalizeAsync(int entityId, ApiRequest request);
    }
}