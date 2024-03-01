using Blazored.LocalStorage;
using BlazorStore.ApiAccess.Service;
using BlazorStore.Client.AppState.FluxorMiddleware;
using Fluxor;

namespace BlazorStore.Client
{
    public static class CommonServices
    {
        public static void ConfigureCommonServices(IServiceCollection services, IConfiguration configuration)
        {
            // HttpClient
            services.AddHttpClient(
                "BlazorStore",
                http => http.BaseAddress = new Uri(configuration.GetSection("AppUrls:BaseApiUrl").Value!)
            );
            services.AddScoped<IApiService, ApiService>();
            services.AddScoped<ICookieService, CookieService>();
            services.AddSingleton<IHttpRequestMessageBuilder, HttpRequestMessageBuilder>();

            // Fluxor
            services.AddFluxor(options =>
            {
                options.ScanAssemblies(typeof(Program).Assembly)
                    .AddMiddleware<Cart>()
                    .UseReduxDevTools(rdt => { rdt.Name = "BlazorStore"; });
            });

            // LocalStorage
            services.AddBlazoredLocalStorage();
        }
    }
}