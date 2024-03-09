using Asp.Versioning;
using BlazorStore;
using BlazorStore.ApiAccess.Service;
using BlazorStore.Components;
using BlazorStore.Components.Account;
using BlazorStore.DataAccess.Data;
using BlazorStore.DataAccess.DBInitilizer;
using BlazorStore.DataAccess.UnitOfWork;
using BlazorStore.Models.Domain;
using BlazorStore.Service;
using BlazorStore.Service.IService;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Swashbuckle.AspNetCore.SwaggerGen;
using Syncfusion.Blazor;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddControllers().AddJsonOptions(opts => opts.JsonSerializerOptions.PropertyNamingPolicy = null); ;

// Blazor Authentitication with roles
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, PersistingRevalidatingAuthenticationStateProvider>();
// builder.Services.AddIdentity implements AddIdentityCookies by default
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();
builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

// Add Custmom Autentication
// add (jwt, could be other types of auth too) authentication
builder.Services.AddAuthentication().AddGoogle(options =>
{
    options.ClientId = builder.Configuration.GetValue<string>("Google:AppId") ?? "";
    options.ClientSecret = builder.Configuration.GetValue<string>("Google:AppSecret") ?? "";
});
//
// builder.Services.AddScoped<ICustomJwtBearerHandler, CustomJwtBearerHandler>();
// builder.Services.AddAuthentication(options =>
// {
//     options.DefaultAuthenticateScheme = "BlazorStore";
//     options.DefaultChallengeScheme = "BlazorStore";
// })
// .AddScheme<JwtBearerOptions, CustomJwtBearerHandler>(JwtBearerDefaults.AuthenticationScheme, options =>
// {
//     options.RequireHttpsMetadata = false;
//     options.SaveToken = true;
//     options.IncludeErrorDetails = true;
// }).AddGoogle(options =>
// {
//     options.ClientId = builder.Configuration.GetValue<string>("Google:AppId") ?? "";
//     options.ClientSecret = builder.Configuration.GetValue<string>("Google:AppSecret") ?? "";
// })
// .AddPolicyScheme("BlazorStore", "BlazorStore", options =>
// {
//     // runs on each request
//     options.ForwardDefaultSelector = context =>
//     {
//         // filter by auth type
//         string? authorization = context.Request.Headers[HeaderNames.Authorization];
//         if (!string.IsNullOrEmpty(authorization) && authorization.StartsWith("Bearer "))
//             return JwtBearerDefaults.AuthenticationScheme; ;

//         // otherwise always check for cookie auth
//         return IdentityConstants.ApplicationScheme;
//     };
// });
//
// builder.Services.AddAuthentication(options =>
// {
//     options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//     options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
// })
// .AddScheme<JwtBearerOptions, CustomJwtBearerHandler>(JwtBearerDefaults.AuthenticationScheme, options =>
// {
//     options.RequireHttpsMetadata = false;
//     options.SaveToken = true;
//     options.IncludeErrorDetails = true;
// });
//
// builder.Services.AddAuthentication(options =>
//     {
//         options.DefaultScheme = IdentityConstants.ApplicationScheme;
//         options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
//     })
//     .AddIdentityCookies();

// Add Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// add custom services
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IDBInitilizer, DBInitilizer>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<BlazorStore.Service.IService.IOrderService, BlazorStore.Service.OrderService>();
builder.Services.AddScoped<IPaymentService<Stripe.Checkout.Session>, StripeService>();


// Configure DPI for client services that will be neccessary on the server if pre-rendering is enabled 
BlazorStore.Client.CommonServices.ConfigureCommonServices(builder.Services, builder.Configuration);

// add custom components [syncfusion]
Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(builder.Configuration.GetValue<string>("SyncFusion:ApiKey"));

// Stripe Config
Stripe.StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe")["SecretKey"];
builder.Services.AddSyncfusionBlazor();

builder.Services.AddCors(options =>
{
    // We can have multiple policies one per expected client. 
    // In pipeline activate like
    //  app.UseCors("BlazorWasmClient");
    //  app.UseCors("BlazorWasmClient_2"); // if you have a second client policy.
    // This is the policy for the Blazor WASM Client
    options.AddPolicy("BlazorWasmClient", builder =>
        builder
        .WithOrigins("https://localhost:7036", "http://localhost:5199")
        .WithMethods("GET", "POST", "PUT", "PATCH", "DELETE", "HEAD", "OPTIONS")
        .WithHeaders("Origin", "X-Requested-With", "Content-Type", "Authorization", "X-Xsrf-Token", "X-Forwarded-For", "X-Real-IP")
        .AllowCredentials()
    );
});

// // HttpClient
// builder.Services.AddHttpClient<IApiService, ApiService>(
//     "BlazorStore",
//     http => http.BaseAddress = new Uri(builder.Configuration.GetSection("AppUrls:BaseApiUrl").Value ?? "")
// );

// Swagger Config
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
// enable API versioning
builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ReportApiVersions = true;
}).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
    options.AddApiVersionParametersWhenVersionNeutral = true;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, SwaggerConfiguration>();
builder.Services.AddSwaggerGen();

var app = builder.Build();

//seed the db
await SeedDatabase();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseMigrationsEndPoint();

    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "BlazorStoreApi_v1");
    });
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseCors("BlazorWasmClient");

app.UseStaticFiles();
app.UseAntiforgery();

app.MapControllers();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(BlazorStore.Client._Imports).Assembly);

app.MapAdditionalIdentityEndpoints();

app.Run();

async Task SeedDatabase()
{
    using (var scope = app.Services.CreateScope())
    {
        var dbInitilizer = scope.ServiceProvider.GetRequiredService<IDBInitilizer>();
        await dbInitilizer.Initilize();
    }
}