using BlazorStore.ApiAccess.Service.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace BlazorStore.ApiAccess.AppHttpClient
{
    public class AppHttpClient : IAppHttpClient
    {
        // public IVillaService Villas { get; init; }
        // public IApplicationUsersService ApplicationUsers { get; init; }


        public AppHttpClient(
            IHttpClientFactory httpClient,
            IHttpContextAccessor httpAccessor,
            ITokenProvider tokenProvider,
            IConfiguration configuration,
            IMessageRequestBuilder messageBuilder
        )
        {
            var urlBase = configuration.GetValue<string>("ServiceUrls:VillaApi");

            // Villas = new VillaService(
            //     httpClient,
            //     httpAccessor,
            //     tokenProvider,
            //     messageBuilder,
            //     $@"{urlBase}/api/{SD.ApiVersion}/villas"
            // );

            // ApplicationUsers = new AuthService(
            //     httpClient,
            //     httpAccessor,
            //     tokenProvider,
            //     messageBuilder,
            //     $@"{urlBase}/api/{SD.ApiVersion}/users"
            // );

        }
    }
}