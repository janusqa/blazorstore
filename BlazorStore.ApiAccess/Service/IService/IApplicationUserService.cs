
using BlazorStore.Dto;

namespace BlazorStore.ApiAccess.Service
{
    public interface IApplicationUserService : IRepository<ApiResponse>
    {
        Task<ApiResponse?> GetUserInfoAsync();
    }
}