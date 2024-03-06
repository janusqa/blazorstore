
namespace BlazorStore.ApiAccess.Service
{
    public interface IApiService : IDisposable
    {
        IAuthService Auth { get; init; }
        IProductService Products { get; init; }
        IOrderService Orders { get; init; }
        IApplicationUserService ApplicationUsers { get; init; }
    }
}