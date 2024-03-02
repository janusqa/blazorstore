
using BlazorStore.Dto;

namespace BlazorStore.ApiAccess.Service
{
    public class OrderService : Repository<ApiResponse>, IOrderService
    {
        public OrderService(
            IHttpClientFactory httpClient,
            IHttpRequestMessageBuilder messageBuilder,
            ICookieService cookieService,
            string url
        ) : base(httpClient, messageBuilder, cookieService, url)
        {
        }
    }
}