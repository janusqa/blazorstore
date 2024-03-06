
using BlazorStore.Common;
using BlazorStore.Dto;

namespace BlazorStore.ApiAccess.Service
{
    public class ApplicationUserService : Repository<ApiResponse>, IApplicationUserService
    {
        private readonly string _url;

        public ApplicationUserService(
            IHttpClientFactory httpClient,
            IHttpRequestMessageBuilder messageBuilder,
            ICookieService cookieService,
            string url
        ) : base(httpClient, messageBuilder, cookieService, url)
        {
            _url = url;
        }

        public async Task<ApiResponse?> GetUserInfoAsync()
        {
            return await RequestAsync(
                new ApiRequest { ApiMethod = SD.ApiMethod.GET, Url = $"{_url}/userinfo" }
            );
        }
    }
}