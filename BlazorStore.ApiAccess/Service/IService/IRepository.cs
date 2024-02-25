
using BlazorStore.Common;

namespace BlazorStore.ApiAccess.Service
{
    public interface IRepository<T> where T : class
    {
        // Task<T?> RequestAsync(ApiRequest apiRequest);
        Task<T?> AddAsync<U>(U dto, bool withBearer = true, SD.ContentType contentType = SD.ContentType.Json);
        Task<T?> UpdateAsync<U>(int entityId, U dto, bool withBearer = true, SD.ContentType contentType = SD.ContentType.Json);
        Task<T?> RemoveAsync(int entityId, bool withBearer = true);
        Task<T?> GetAsync(int entityI, bool withBearer = true);
        Task<T?> GetAllAsync(bool withBearer = true);
    }
}