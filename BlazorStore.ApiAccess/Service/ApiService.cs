
using BlazorStore.Common;

namespace BlazorStore.ApiAccess.Service
{
    public class ApiService : IApiService
    {
        public IAuthService Auth { get; init; }
        public IProductService Products { get; init; }
        public IOrderService Orders { get; init; }

        public IApplicationUserService ApplicationUsers { get; init; }

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
                 $@"api/{SD.ApiVersion}/auth"
            );

            Products = new ProductService(
                httpClient,
                messageBuilder,
                cookieService,
                $@"api/{SD.ApiVersion}/products"
            );

            Orders = new OrderService(
                httpClient,
                messageBuilder,
                cookieService,
                $@"api/{SD.ApiVersion}/orders"
            );

            ApplicationUsers = new ApplicationUserService(
                httpClient,
                messageBuilder,
                cookieService,
                $@"api/{SD.ApiVersion}/users"
            );
        }

        public void Dispose()
        {
        }

    }
}