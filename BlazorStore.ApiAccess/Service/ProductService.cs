
using BlazorStore.Dto;

namespace BlazorStore.ApiAccess.Service
{
    public class ProductService : Repository<ApiResponse>, IProductService
    {
        public ProductService(
            IHttpClientFactory httpClient,
            IHttpRequestMessageBuilder messageBuilder,
            ICookieService cookieService,
            string url
        ) : base(httpClient, messageBuilder, cookieService, url)
        {
        }
    }
}