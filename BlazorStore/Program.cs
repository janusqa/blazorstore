using Asp.Versioning;
using BlazorStore;
using BlazorStore.Components;
using BlazorStore.DataAccess.Data;
using BlazorStore.DataAccess.DBInitilizer;
using BlazorStore.DataAccess.Repository;
using BlazorStore.DataAccess.UnitOfWork.IUnitOfWork;
using BlazorStore.Service;
using BlazorStore.Service.IService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Radzen;
using Swashbuckle.AspNetCore.SwaggerGen;
using Syncfusion.Blazor;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddControllers();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// add custom services
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IDBInitilizer, DBInitilizer>();
builder.Services.AddScoped<IFileService, FileService>();

// add custom components [syncfusion, radzen]
Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(builder.Configuration.GetValue<string>("SyncFusion:ApiKey"));
builder.Services.AddSyncfusionBlazor();
builder.Services.AddRadzenComponents();

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

app.UseStaticFiles();
app.UseAntiforgery();

app.MapControllers();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(BlazorStore.Client._Imports).Assembly);


app.Run();

async Task SeedDatabase()
{
    using (var scope = app.Services.CreateScope())
    {
        var dbInitilizer = scope.ServiceProvider.GetRequiredService<IDBInitilizer>();
        await dbInitilizer.Initilize();
    }
}