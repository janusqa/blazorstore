
using BlazorStore.Common;
using BlazorStore.Dto;

namespace BlazorStore.ApiAccess.Service
{
    public class OrderService : Repository<ApiResponse>, IOrderService
    {
        private readonly string _url;
        public OrderService(
            IHttpClientFactory httpClient,
            IHttpRequestMessageBuilder messageBuilder,
            ICookieService cookieService,
            string url
        ) : base(httpClient, messageBuilder, cookieService, url)
        {
            _url = url;
        }

        public async Task<ApiResponse?> FinalizeAsync(int entityId, ApiRequest request)
        {
            return await RequestAsync(request with { ApiMethod = SD.ApiMethod.GET, Url = $"{_url}/finalize/{entityId}" });
        }
    }
}