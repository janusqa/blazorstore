
using BlazorStore.Common;

namespace BlazorStore.ApiAccess.Service
{
    public class ApiService : IApiService
    {
        public IAuthService Auth { get; init; }

        public ApiService(
            IHttpClientFactory httpClient,
            IHttpRequestMessageBuilder messageBuilder,
            ICookieService cookieService
        )
        {
            Auth = new AuthService(
                httpClient,
                messageBuilder,
                cookieService,
                 $@"/api/{SD.ApiVersion}/auth"
            );
        }

        public void Dispose()
        {
        }

    }
}