using BlazorStore.ApiAccess.Service;
using BlazorStore.Client;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddSingleton<AuthenticationStateProvider, PersistentAuthenticationStateProvider>();

// HttpClient
builder.Services.AddHttpClient<IApiService, ApiService>(
    "BlazorStore",
    http => http.BaseAddress = new Uri(builder.Configuration.GetSection("AppUrls:BaseApiUrl").Value ?? "")
);

CommonServices.ConfigureCommonServices(builder.Services);

await builder.Build().RunAsync();
