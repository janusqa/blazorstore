
namespace BlazorStore.ApiAccess.Service
{
    public interface IApiService : IDisposable
    {
        IAuthService Auth { get; init; }
    }
}