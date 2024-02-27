
using BlazorStore.Common;
using BlazorStore.Dto;

namespace BlazorStore.ApiAccess.Service
{
    public interface IRepository<T> where T : class
    {
        // Task<T?> RequestAsync(ApiRequest apiRequest);
        Task<T?> AddAsync(ApiRequest request);
        Task<T?> UpdateAsync(int entityId, ApiRequest request);
        Task<T?> RemoveAsync(int entityId, ApiRequest request);
        Task<T?> GetAsync(int entityId, ApiRequest request);
        Task<T?> GetAllAsync(ApiRequest request);
    }
}