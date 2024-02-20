using BlazorStore.Common;

namespace BlazorStore.ApiAccess.Service.IService
{
    public interface IBaseHttpService<T> where T : class
    {
        // Task<T?> RequestAsync(ApiRequest apiRequest);
        Task<T?> PostAsync<U>(U dto, bool withBearer = true, SD.ContentType contentType = SD.ContentType.Json);
        Task<T?> PutAsync<U>(int entityId, U dto, bool withBearer = true, SD.ContentType contentType = SD.ContentType.Json);
        Task<T?> DeleteAsync(int entityId, bool withBearer = true);
        Task<T?> GetAsync(int entityI, bool withBearer = true);
        Task<T?> GetAllAsync(bool withBearer = true);
    }
}