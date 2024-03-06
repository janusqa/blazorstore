
Tooling for ASP.NET Core Blazor
https://learn.microsoft.com/en-us/aspnet/core/blazor/tooling?view=aspnetcore-8.0&pivots=windows
   
1. Create Project
   1. NB THIS WILL CREATE THE ACTUAL ROOT PROJECT FOLDER FOR YOU, SO DO NOT START FROM WITHIN A ROOT FOLDER PER SAY OR YOU WILL GET ERRORS AND WARNINGS!!!
   2. dotnet new blazor -o [<PROJECTNAME>] (for interactive server-side rendering aka blazor server) ... OR ...
   3. dotnet new blazor -int WebAssembly -o [<PROJECTNAME>] (for client-side rendering ONLY aka blazor wasm) ... OR ...
   4. dotnet new blazor -int Auto -o [<PROJECTNAME>] (for client-side and server-side rendering crated at same time)
2. dotnet new gitignore
3. dotnet new tool-manifest
4. dotnet tool install dotnet-ef 
5. dotnet tool update dotnet-ef
6. dotnet add BlazorStore.DataAccess package Microsoft.EntityFrameworkCore
7. dotnet add BlazorStore.DataAccess package Microsoft.EntityFrameworkCore.SqlServer 
   OR dotnet add BlazorStore.DataAccess package Microsoft.EntityFrameworkCore.Sqlite
8. dotnet add BlazorStore.DataAccess package Microsoft.EntityFrameworkCore.Relational
9. dotnet add BlazorStore.DataAccess package Microsoft.EntityFrameworkCore.Tools
10. dotnet add BlazorStore.DataAccess package Microsoft.EntityFrameworkCore.Design
11. dotnet add BlazorStore.DataAccess package Microsoft.Extensions.Configuration
12. dotnet add BlazorStore.DataAccess package Microsoft.AspNetCore.Identity.EntityFrameworkCore
13. dotnet add BlazorStore.Models package Microsoft.Extensions.Identity.Stores
14. dotnet add BlazorStore package Microsoft.EntityFrameworkCore.Design
15. dotnet run --project [<PROJECTNAME>] --launch-profile https  

Razor Class Library
---
dotnet new razorclasslib -o BlazorStore.PageScript

SyncFusion
---
dotnet add BlazorStore package Syncfusion.Blazor.Themes
dotnet add BlazorStore package Syncfusion.Blazor.Grid 

Place in App.razor
---
<link href="_content/Syncfusion.Blazor.Themes/bookstrap5.css" rel="stylesheet" /> or <link href="https://cdn.syncfusion.com/blazor/19.3.43/styles/bootstrap5.css" rel="stylesheet" /> 

Place in program.cs
Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("YOUR LICENSE KEY");
builder.Services.AddSyncfusionBlazor();

<SfRichTextEditor @bind-Value="ProductDto.Description"></SfRichTextEditor>

Add API to Blazer Server Project (needed if you have web assembly client)
Note API can be in it's own project but unless there is a good reason, why not have it in the same Blazor server project to cut down on projects needed to be managed. Blazor server is well capable of hosting the API part also.
---
1. dotnet add BlazorStore package Asp.Versioning.Mvc
2. dotnet add BlazorStore package Asp.Versioning.Mvc.ApiExplorer
3. dotnet add BlazorStore package Swashbuckle.AspNetCore
4. In programs.cs
   1. Add "builder.Services.AddControllers();" to services section
   2. Add "app.MapControllers();" to pipeline section
   3. Add the swagger configuration into program cs. See program.cs
   4. Add "SwaggerConfiguration.cs" to root of project


Adding Authentication AFTER starting a project
===
ON CLIENT
---
1.
dotnet add <[YOUR_PROJECT]>.Client package Microsoft.AspNetCore.Components.WebAssembly.Authentication

2.
Copy the following files to <[YOUR_PROJECT]>.Client main folder
   a. PersistentAuthenticationStateProvider.cs
   b. RedirectToLogin.razor
   c. UserInfo.cs

3. 
Add the following to program.cs above "await builder.Build().RunAsync()
```
builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddSingleton<AuthenticationStateProvider, PersistentAuthenticationStateProvider>();
```

ON SERVER
---
3.
dotnet add <[YOUR_PROJECT]> package Microsoft.AspNetCore.Identity.EntityFrameworkCore
dotnet add <[YOUR_PROJECT]> package Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore

4.
update Routes.razor in server to 
```
<Router AppAssembly="typeof(Program).Assembly" AdditionalAssemblies="new[] { typeof(Client._Imports).Assembly }">
    <Found Context="routeData">
        <AuthorizeRouteView RouteData="routeData" DefaultLayout="typeof(Layout.MainLayout)">
            <NotAuthorized>
                <RedirectToLogin />
            </NotAuthorized>
        </AuthorizeRouteView>
        <FocusOnNavigate RouteData="routeData" Selector="h1" />
    </Found>
</Router>
```

5.
From a fresh project copy /Components/Account folder to  <[YOUR_PROJECT]>/Components/Account folder

6. Update program.cs
   1. In services section add 
	```
	builder.Services.AddCascadingAuthenticationState();
	builder.Services.AddScoped<IdentityUserAccessor>();
	builder.Services.AddScoped<IdentityRedirectManager>();
	builder.Services.AddScoped<AuthenticationStateProvider, PersistingRevalidatingAuthenticationStateProvider>();
	builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
	    .AddEntityFrameworkStores<ApplicationDbContext>()
	    .AddSignInManager()
	    .AddDefaultTokenProviders();
	builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();	
	```
	2. In pipeline sections
   	```
	app.MapAdditionalIdentityEndpoints();
	```
7. Add AddAuthentication to services section (an example of cookie auth is used below but we actually use jwt, see programs.cs)
   ```
   builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();
    ```
8. Add "@using Microsoft.AspNetCore.Components.Authorization"  to _imports.razor in webassembly and server project

9. Files to adjust
   1.  WebAssembly project : UserInfo.cs   
       add Role "public required string Email { get; set; }"
   2.  Main Project : PersistingRevalidatingAuthenticationStateProvider.cs
       In "OnPersistingAsync" method add...
       ```
       var role = principal.FindFirst(options.ClaimsIdentity.RoleClaimType)?.Value;
       .
       .
       .
       Role = role ?? SD.Role_Customer,
       ```
   3.  WebAssembly project : PersistentAuthenticationStateProvider.cs
       add
       ```
       new Claim(ClaimTypes.Role, userInfo.Role)
       ```
   4.  

10. REMEBER TO UPDATE ANY NAME SPACES AS THEY ARE QUITE A FEW OF THEM!!!
11.  Other files touched to adjust for jwt and roles were register.razor, login.razor, applicationuserrepository.cs, an api was created to retireve a refresh token.  Manage/Index must also be adjusted if you want to add custom user fields

Fluxor
---
1.
Install the following to webassembly client project
    1. dotnet add BlazorStore.Client package Fluxor
    2. dotnet add BlazorStore.Client package Fluxor.Blazor.Web
    3. dotnet add BlazorStore.Client package Fluxor.Blazor.Web.ReduxDevTools

2.
Update Program.cs of webassembly client project by adding to services section the below snippet
```
// Fluxor
builder.Services.AddFluxor(options =>
{
    options.ScanAssemblies(typeof(Program).Assembly);
    options.UseReduxDevTools(rdt => { rdt.Name = "BlazorStore"; });
});
```


