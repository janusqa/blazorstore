using BlazorStore.ApiAccess.Service;
using Fluxor;

namespace BlazorStore.Client
{
    public static class CommonServices
    {
        public static void ConfigureCommonServices(IServiceCollection services)
        {
            services.AddScoped<ICookieService, CookieService>();
            services.AddSingleton<IHttpRequestMessageBuilder, HttpRequestMessageBuilder>();

            // Fluxor
            services.AddFluxor(options =>
            {
                options.ScanAssemblies(typeof(Program).Assembly);
                options.UseReduxDevTools(rdt => { rdt.Name = "BlazorStore"; });
            });
        }
    }
}