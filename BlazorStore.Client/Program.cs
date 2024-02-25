using BlazorStore.ApiAccess.Service;
using BlazorStore.Client;
using Fluxor;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddSingleton<AuthenticationStateProvider, PersistentAuthenticationStateProvider>();

// HttpClient
builder.Services.AddHttpClient<IUnitOfWork, UnitOfWork>(
    "BlazorStoreApi",
    c => c.BaseAddress = new Uri(builder.Configuration.GetSection("AppUrls:BaseApiUrl").Value ?? "")
);
builder.Services.AddSingleton<IHttpRequestMessageBuilder, HttpRequestMessageBuilder>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ICookieService, CookieService>();

// Fluxor
builder.Services.AddFluxor(options =>
{
    options.ScanAssemblies(typeof(Program).Assembly);
    options.UseReduxDevTools(rdt => { rdt.Name = "BlazorStore"; });
});

await builder.Build().RunAsync();
