
using BlazorStore.Common;
using BlazorStore.Dto;

namespace BlazorStore.ApiAccess.Service
{
    public class AuthService : Repository<ApiResponse>, IAuthService
    {
        private readonly string _url;

        public AuthService(
            IHttpClientFactory httpClient,
            IHttpRequestMessageBuilder messageBuilder,
            ICookieService cookieService,
            string url
        ) : base(httpClient, messageBuilder, cookieService, url)
        {
            _url = url;
        }

        public async Task<ApiResponse?> RefreshAsync()
        {
            return await RequestAsync(
                new ApiRequest
                {
                    ApiMethod = SD.ApiMethod.GET,
                    Url = $"{_url}/refresh"
                }, withBearer: false);
        }

    }
}