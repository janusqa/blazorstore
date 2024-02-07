1. Install repo and SDK
   1. sudo apt update
   2. sudo apt upgrade
   3. wget https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
   4. sudo dpkg -i packages-microsoft-prod.deb
   5. rm packages-microsoft-prod.deb
   6. sudo apt update
   7. sudo apt upgrade
   8. sudo apt-get install -y dotnet-sdk-7.0

2. Install C# plugin for vscode

3. Create a soluton
   1. dotnet new gitignore
   2. dotnet new tool-manifest
   3. dotnet tool install dotnet-ef (I think this may be optional if you are intending to use Entity Framework. It's a big topic)
   4. dotnet tool install dotnet-aspnet-codegenerator 
   5. dotnet tool update dotnet-ef
   6. dotnet tool update dotnet-aspnet-codegenerator
   
4. Scaffold the type of app you want
   1. dotnet new sln
   2. dotnet new console --output [<foldername/namespace>] --framework net7.0  //console app
   3. dotnet new classlib --output [<foldername/namespace>] --framework net7.0  //class library app
   4. dotnet new mstest --output [<foldername/namespace>] //unit test app
   5. after adding your various projects need to now add them to the solution you oringally created
      1. dotnet sln add [<foldername/namespace>]/[<foldername/namespace>].csproj
   6. to reference a classlib in a console/desktop app
      1. dotnet add [<namespace-folder-console-desktop>]/[<namespace-folder-console-desktop>].csproj reference [<namespace-folder-classlib>]/[<namespace-folder-classlib>].csproj
      2. in the case of test projects... 
         1. dotnet add [<testproject>]/[<testproject>].csproj reference [<mainproject>/<mainproject>].csproj
         2. Run test: dotnet test [<testproject>/<testproject>].csproj
   7. dotnet new webapi --use-controllers -o [<project-name>]  // for minimalAPI remove --use-controllers
      1. dotnet add package Microsoft.EntityFrameworkCore.InMemory
      2. dotnet add package Microsoft.AspNetCore.JsonPatch
      3. dotnet add package Microsoft.AspNetCore.Mvc.NewtonsoftJson
      4. dotnet add package Microsoft.EntityFrameworkCore
      5. dotnet add package Microsoft.EntityFrameworkCore.SqlServer
      6. dotnet add package Microsoft.EntityFrameworkCore.Tools
   8. dotnet new mvc -au Individual -uld --output [<foldername/namespace>] --framework net7.0  // ASP.Net core web app mvc
      1. The -au Individual paramater makes it use Individual User accounts. The -uld has it use SQL Server instead of SQLite. 
      2. change connecting string in appsettings.json
      3. dotnet ef database update
      4. dotnet add package Microsoft.VisualStudio.Web.CodeGeneration.Design
      5. If PROJECT IS ALREADY SET UP we can still set up -au (authentication) and -uld (use local database)
         1. run the following commands to set up -au (authentication)
            1. dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore // this should be added to the DataAccess Project
            2. In our DataAccess Project and class inherit from IdentityDbContext instead of just DbContext
            3. Now in OnModelCreating method add this line as the first line.
               "base OnModelCreating(ModelBuilder)"
            4. ADD THE BELOW TO THE MAIN PROJECT....
            5. dotnet add package Microsoft.Extensions.Identity.Stores --version 8.0.0
            6. dotnet add package Microsoft.AspNetCore.Identity.UI --version 8.0.0
            7. dotnet add package Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore
            8. dotnet add package Microsoft.EntityFrameworkCore.Design
            9.  dotnet add package Microsoft.VisualStudio.Web.CodeGeneration.Design
            10. dotnet add package Microsoft.EntityFrameworkCore.SqlServer
            11. dotnet add package Microsoft.EntityFrameworkCore.Tools
            12. run command "dotnet aspnet-codegenerator identity -h"  // see scaffolding options
            13. run "dotnet aspnet-codegenerator identity --useDefaultUI"  // implement basic setup. Also see scaffolding options for other scenarios  
            14. I use "dotnet aspnet-codegenerator identity"  to install everything
            15. Note the generator will try to put in program.cs its own DBContext. Delete it and adjust this line to use our own existing context which we adjusted above to be IdentityDbContext.  This is the line to adjust in program.cs (builder.Services.AddDefaultIdentity.....)  
            16. We can optionally add <ApplicationUser> to our  public class ApplicationDbContext : IdentityDbContext<ApplicationUser> like that. TOTALLY OPTIONAL
            17. Add app.UseAuthentication() to program.cs. It must be added right before app.UseAuthorization()
            18. In appSettings.json Identity scaffolding tried to add a new connection.  We dont need it! Delete it!!!
            19. Now back in program.cs add two things
                1.  builder.Services.AddRazorPages();
                2.  app.MapRazorPages();
         2. Now update migrations after adding these packages
            1. dotnet ef migration add MyMigration --project YourClassLibraryProjectName --startup-project YourWebAppProjectName
            2. dotnet ef database update --startup-project path\to\your\startup\project.csproj
         3. We may want to extend the ApplicationUser and what fields it has. Example we may want to  give this user an Address.  
            1. In Models project add the "dotnet add package Microsoft.Extensions.Identity.Stores --version 8.0.0" 
            2. Create a new class eg. ApplicationUser and inherit it from IdentityUser. Add your customizations to this file. 
            3. Now we need to update the use of ApplicationUser in some places
               1. Register.cshtml.cs. Find "private ApplicationUser CreateUser()" method and change that and "return Activator.CreateInstance<ApplicationUser>()" to have "ApplicationUser" instead.
            4. Add appropriate DbSet to ApplicationDBContext for this class so it maps to the existing users table in the DB and run migrations to update DB
      6. run the following commands to set up -uld (use local database)
         1. dotnet add package Microsoft.EntityFrameworkCore.SqlServer
         2. dotnet add package Microsoft.EntityFrameworkCore.Tools
         3. Update your appsettings.json file to use SQL Server connection string. Modify the DefaultConnection to point to your SQL Server instance.
         4. Create a "Data" dir in project and add a "ApplicationDBcontex.cs" to it in wich you will define an ApplicationDbContext class. See project to see what this class is like.
         5. Register "ApplicationDbContext" in services area of "Program.cs"
            1. Add ...
            ```
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
               options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
            );
            ```
         6. EF/ConnectionString now configure so now run "dotnet ef database update" to create database
         7. To add a table, in "ApplicationDBContext" class create as many "Dbset" properties as needed.
            ex: Dbset<[<ModelName>]> [<TableName>] {get; set;}
            now perform the apply migrations below
         8. Apply migrations
            1. dotnet ef migrations add [<NameOfMigrationCanBeAnythingYouWantSoMakeItDescriptive>]
            2. dotnet ef database update
            3. IMPORTANT: IF YOU HAVE MOVED Data AND Migrations to their own start libray the comand to use would be
            ```
            dotnet ef migrations add MyMigration --project YourClassLibraryProjectName --startup-project YourWebAppProjectName 
            
            dotnet ef database update --startup-project path\to\your\startup\project.csproj
            ```
            The startup project is where the DbContext is.

5. CRTL+SHIFT+P.  In command palette type ">.NET: Generate Assets for Build and Debug".
   
6. dotnet run // run source code 
7. dotnet run --project [<project-name>] eg. dotnet run --project day01

8.  dotnet publish -c release -r ubuntu.16.04-x64 --self-contained  // compile console app as "executable"

# Linting
dotnet new editorconfig

add the below to .vscode/settings.json
{
  "omnisharp.enableRoslynAnalyzers": true,
  "omnisharp.enableEditorConfigSupport": true
}

change .editorconfig to contain only 
```
root = true

[*.cs]
dotnet_analyzer_diagnostic.category-Style.severity = warning

dotnet_diagnostic.IDE0003.severity = none
dotnet_diagnostic.IDE0008.severity = none
dotnet_diagnostic.IDE0058.severity = none
dotnet_diagnostic.IDE0160.severity = none
```

add "<EnforceCodeStyleInBuild>true</EnforceCodeStyleInBuild>"  to .csproj file.

# Additional scaffolding for ASP.NET Core Web Application (WHEN DONE MANUALLLY)
*** MUST BE IN PROJECT DIRECTORY NOT SOLUTION/ROOT DIRECTORY ***
dotnet add package Microsoft.VisualStudio.Web.CodeGeneration.Design
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package Microsoft.AspNetCore.Mvc.ViewFeatures // for things like SelectListItem 
```
replace "Server=(localdb)\\mssqllocaldb;Database=jokes;Trusted_Connection=True;MultipleActiveResultSets=true
with     "Server=localhost,1433;Database=jokes;User Id=sa;Password=P@ssw0rd;TrustServerCertificate=True;MultipleActiveResultSets=true"
```
# Uninstall tools
dotnet tool uninstall [<PACKAGE_NAME>] --global
dotnet tool uninstall [<PACKAGE_NAME>]   // package installed to the project itself

# Scaffold a controller based on a model, and create associated views too
*** MUST BE IN PROJECT DIRECTORY NOT SOLUTION/ROOT DIRECTORY ***
dotnet aspnet-codegenerator controller --controllerName JokesController --model Joke --dataContext ApplicationDbContext --relativeFolderPath Controllers --referenceScriptLibraries --useDefaultLayout --layout "/Views/Shared/_Layout.cshtml" --force

# Scaffold a view 
*** MUST BE IN PROJECT DIRECTORY NOT SOLUTION/ROOT DIRECTORY ***
dotnet aspnet-codegenerator view Search Create --model Joke --dataContext ApplicationDbContext --relativeFolderPath Views/Jokes --referenceScriptLibraries --useDefaultLayout --partialView

# Scaffold an area
*** MUST BE IN PROJECT DIRECTORY NOT SOLUTION/ROOT DIRECTORY ***
dotnet aspnet-codegenerator area [<AreaNameToGenerate>]
A suggest will be made in recofiguring (adding a change in code) to the routing
in Program.cs. It is straight forward and it is just now to include the concept of areas
in the routing. Now we have areas, controller, action that specifies how a request is routed.
Change
```
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
```
to
```
app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");
```
Where "Customer" is the area that will be used by default. It could be any other one of our scaffoled areas.
- Move Controllers and Views to their areas, remembering to update the namespace as neccessary.
eg. [<project>].Controllers now becomes [<project>].Areas.[<AreaNameYouChose>].Controllers
Now annotate your controllers etc. to indicate which Area a controller for example belongs too.
eg. above the say CategoryController class in the Admin area annote it with [Area("Admin)]
 - NOW move the views that correspond to each area inside their respective view area.
Additionally must COPY "_ViewImports.cshtml" and "_ViewStart.cshtml" to the "Views" folder of each Area
- Now go back to your views and update the links where you have asp-controller, asp-action, and now also add asp-area and set this to appropriate area. Even the shared views like "_Layout.cshtml" must be updated

# Updataing the DB via migrations
dotnet ef migrations add [<NameOfMigrationGoesHere>]  // Add a migration
dotnet ef migrations remove [<NameOfMigrationGoesHere>]  // remove a migration
dotnet ef database update


QuickStart
----
dotnet new gitignore
dotnet new tool-manifest
dotnet new sln --name mySolution
dotnet new mvc --output myProject --name myProject --framework net8.0 (create an ASP.net Core mvc project) 
dotnet sln add myProject/myProject.csproj
dotnet run --project myProject
dotnet run --project myProject --launch-profile https  // starts app with a profile in myProject/Properties/launchSettings.json
dotnet watch --project myProject --launch-profile https // this hot reloads changes to views
// open browser to http://localhost:5162.  This infromation is in myProject/Properties/launchSettings.json
// use  myProject/Properties/launchSettings.json to change which port app is accessible from if you like

Optional
--------
// Add a package from nuget to a project
dotnet add [<PROJECT>] package [<PACKAGE>]
dotnet add myProject package Microsoft.Z3 --version 4.11.2

// Remove a package installed with nuget to a project
dotnet remove [<PROJECT>] package [<PACKAGE>]

// List packages in a project
dotnet list [<PROJECT>] package 

SECRETS
-------
dotnet user-secrets --project [<projectname>] init
dotnet user-secrets --project [<projectname>] set "ApiKey" "your-api-key"
var apiKey = builder.Configuration.GetValue<string>("ApiKey");
OR
var jwtSecret = config["ApiSettings:JwtAccessSecret"]


IDENTITY ROLES
Files: ApplicationUser.cs, ApplicationDbContext.cs, programs.cs, Register.cshtml.cs
---
- in program.cs we updated the AddDefalutIdentity and changed it to AddIdentity
- in register.cshtml.cs we injected the RoleManager
- In the utilities SD.cs we added some constants for use with roles
- Back to register.cshtml.cs OnGetAsync to add some custom code there to add Admin role if it does not exists
- To avoid email sender error create an EmailSender in Utilites. It will be a mock so we can just get past this error for now.
- Now add it to program.cs services
- in order for users to be redirected to proper access denied page or login page update program.cs with "builder.Services.ConfigureApplicationCookie"
- Adding extra fields to register form should be done in register.cshtml and register.cshtml.cs

Protecting Controllers and Routes
in controller file can protect entire controller or just an action with..
 - Authorize[Roles = "Role1,Role2,..."]
 - Set current _layout.chtml in /[<project>]/Areas/Identity/manage/_layout.cshtml.  Set the layout there to point to the shared views "/Views/Shared/_Layout.cshtml"
 - now copy a _viewStart.cshtml to the manage folder and ajust it's layout line to just point to "_Layout.cshtml"

Configure Sessions
----
1.
in program.cs below AddIdentity and ConfigureAplicationCookie
```
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options => {
   options.IdleTimeout = TimeSpan.FromMinutes(100);
   options.Cookie.HttpOnly = true;
   options.Cookie.IsEssential = true;
});
```
2.
Still in program.cs need to add to the request pipeline.
i.e add "app.UseSession()" after "app.UseAuthorization()"

3.
To use set up a constant holding a string value of your choice. 
Sessions are key/value pair, and this sting is the key by which to access the session by 

For instance in SD.cs 
```
public const string SessionCart = "SessionShoppingCart";

```
Now use it eg.
```
HttpContext.Session.SetInt32(SD.SessionCart, itemCount ?? 0);
```

4.
Now to use it in cshtml pages at top of page put
```
@using Microsoft.AspNetCore.Http
@inject IHttpContextAccessor HttpContextAccessor
```
and use it for example
```
@(HttpContextAccessor.HttpContext?.Session.GetInt32(SD.SessionCart)
                                is not null ? HttpContextAccessor.HttpContext.Session.GetInt32(SD.SessionCart) : 0)
```

5.
If your application requires it clear session when logging out
Open logout.cshtml.cs in Areas/Identity/Account and below  "_signInManager.SignOutAsync" 
clear the session with "HttpContext.Session.Clear();"

You may also need to intilize a session with info when logging in for example you may want to set 
the number of items in the logged in users cart so it can be displayed in some header.

social logins
---
Set up developer accounts as usual and create an app
In facebook cases in usecases set the oauth url.
The oauth url in confirguring settings for face book is https://yoursite/signin-facebook
Get the keys for your .net app from facebook app settings -> basic setup
Download appropriate nuget package for facebook is Microsoft.AspNetCore.Authentication.Facebook
Now in programs.cs add the appropriate code. facebook example below
```
builder.Services.AddAuthentication().AddFacebook(option =>
   option.AppId = builder.Configuration.GetValue<string>("FacebookAppId");
   option.AppSecret = builder.Configuration.GetValue<string>("FacebookAppSecret");
);
```
Add Facebook AppId and AppSecret to secrets
eg. dotnet user-secrets --project[<PROJECT>] set "[<KEY>] [<VALUE>]

Remember to update the ExternalLogin.cshtml.cs and ExternalLogin.cshtml to handle custom fields files in /Area/Identity/Account (NOT THE ONES IN MANAGE THOUGH!!!)

Seeding the initial users and roles using custom DBInitilizer class
---
See DBInitilizer.
After setting this class up then utilize it in programs.cs
To test if it works change connecting string to point to a fresh database. We can delete it after testing.
Dont forget to change connecting string back.

create a function after app.Run() and call it before  app.MapControllers/Razorpages
```
async Task SeedDatabase()
{
    using (var scope = app.Services.CreateScope())
    {
        var dbInitilizer = scope.ServiceProvider.GetRequiredService<IDBInitilizer>();
        await dbInitilizer.Initilize();
    }
}
```


Error fixing
---
```
Only the invariant culture is supported in globalization-invariant mode. See https://aka.ms/GlobalizationInvariantMode for more information. (Parameter 'name')
en-us is an invalid culture identifier.
```
change in any of the projects csproj files. Mostly found in the main project though
<InvariantGlobalization>true</InvariantGlobalization>
to
<InvariantGlobalization>false</InvariantGlobalization>


WebApi - JWt - Auth configuration
---
1. Add "app.UseAuthentication()" above "app.UseAuthoriztion()" in program.cs
2. dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
3. configure the auth service by adding the below in the services section of program.cs
   ```
   var JwtAccessSecret = builder.Configuration.GetValue<string>("ApiSettings:JwtAccessSecret") ?? "";
   builder.Services.AddAuthentication(auth =>
   {
      auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
      auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
   }).AddJwtBearer(auth =>
   {
      auth.RequireHttpsMetadata = false;
      auth.SaveToken = true;
      auth.TokenValidationParameters = new TokenValidationParameters
      {
         IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(JwtAccessSecret)),
         ValidateIssuerSigningKey = true,
         ValidateLifetime = true,
         ValidateIssuer = false,
         ValidateAudience = false
      };
   });
   ```
4. Note it is assumed you have alreay set up  secrets
   1. dotnet user-secrets --project RealEstate init
   2. dotnet user-secrets --project RealEstate.csproj set "ApiSettings:JwtAccessSecret" "blahblahblah"
5. Modify swagger in program.cs to support Auth. See Program.cs
6. In the UI part of the solution/project (NOT API PART) add session management
   see earlier writeup on how to enable sessions
   Also add [Authorize] annotation where neccessary in the UI part.  Note
   we had also annoted the WebApi enpoints already. Yes this is double the work!
   
UI - jwt- auth configuration
---
   Now its time to configure Authentication inside the UI project. We have 
   already  previously done it for API project. So here we go again.
   1. dotnet add package Microsoft.IdentityModel.JsonWebTokens (note this replaces
      System.IdentityModel.Tokens.Jwt)
   2. add "app.UseAuthentication()" to Program.cs above "app.UseAuthorization()"
   3.  add to program.cs
      ```
      // add authentication
      builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
      .AddCookie(options =>
      {
         options.Cookie.HttpOnly = true;
         options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
         // we needed to set LoginPath as it was goint to the razor page for
         // identity which we ar not using yet. Our login page is "Auth/Login"
         // "not /Identity/Login" OR "/Account/Login" 
         options.LoginPath = "/[<Area>]/Auth/Login";
         options.AccessDeniedPath = "/[<Area>]/Auth/AccessDenied";
         options.SlidingExpiration = true;
      });
      ```

Versioning an Api
1. dotnet add [<API_PROJECT>] package Asp.Versioning.Mvc
2. dotnet add [<API_PROJECT>] package Asp.Versioning.Mvc.ApiExplorer 
3. Configure versioning in program.cs
   1. 
   ```
      builder.Services.AddApiVersioning(options =>
   {
      options.AssumeDefaultVersionWhenUnspecified = true;
      options.DefaultApiVersion = new ApiVersion(1, 0);
      options.ReportApiVersion = true;
   }).AddApiExplorer(options =>
      {
         options.GroupNameFormat = "'v'VVV";
         options.SubstituteApiVersionInUrl = true;
         options.AddApiVersionParametersWhenVersionNeutral = true;
      });
   ```
   2. Now annote your controllers where necessary eg. [ApiVersion("1.0")]
   3. As your api evolves you just add additional versons to the existing annotation
      ```
      [ApiVersion("1.0")]
      [ApiVersion("2.0")]
      ```
      and on your actions defrienctiate which action maps to which version
      ```
      [MapToApiVersion(2.0)]
      ```
   4. now adjust the route for the controller to handle versioning
      change ...
      ```[Route("api/villanumbers")]```
      to
      ```[Route("api/v{version:apiVersion}/villanumbers")]```
   5. for futher indepth config review in program.cs 
      1. builder.Services.AddSwaggerGen
      2. app.UseSwaggerUI
   6. AFTER VERSIONING YOUR API DONT FORGET TO UPDATE YOUR CALLS TO API IN FRONT END
      TO HANDLE THE UPDATED API URLS /v1/  or /v1/ ect

Enabling Caching for API
----
1. In progarm.cs of API project in services section add 
   ```
   builder.Services.AddResponseCaching();
   ```
2. in pipeline section of program.cs add
   ```
   app.UseResponseCaching();
   ```
3. Go to the action in a controller you want to apply caching too and add 
   annotations.
   ```
   [ResponseCache(Duration = 30)] // cache the results for 30s
   ```
4. An example of disabling cacheing for an action
   ```
   [ResponseCache(Location =ResponseCacheLocation.None, NoStore =true)] 
   ```
5. Setting up general rules like most of your actions should be cached for 30s
   1. Rather than annoting each action with the ResponseCache we can create a 
      cache profile. It is set as options in the "builder.Services.Addcontrolles"
      eg.
      ```
      builder.Services.AddControllers(options =>
      {
         options.CacheProfiles.Add(
            "Default30",
            new CacheProfile
            {
                  Duration = 30
            }
         );
      }).AddNewtonsoftJson();
      ```
      Note other cacheing options can be set for "Default30". Here we only use
      "Duration" option.
      Now on your contorller actions [ResponseCache(CacheProfileName="Default30")]


Getting data from a request
---
1. Getting data from a request querystring
   https://localhost/api/resources?myval=1
   ```
   public IActionResult test([FromQuery] int myval)
   ```
   can also name the query param eg.
   public IActionResult test([FromQuery(Name = "ThisIsMyVal")] int myval)
2. Getting data from a request body
   Sent usually via form submission or by some sort of programatic fetch
   ```
   public IActionResult test([FromBody] int myval)
   ```
3. Getting data from path
   https://localhost/api/resources/1
   ```
   [HttpGet("{myval:int}")]
   public IActionResult test(int myval)
   ```   
   or
      ```
   [HttpGet("{myval:int}")]
   public IActionResult test([FromRoute] int myval)
   ``` 
4. Getting form data
5. ```
   public IActionResult test([FromForm] MyFormData myval)
   ``````

Replacing our roll your own auth with .Net Identity for a WEBAPI
---
1. Create a ApplicationUser model and inherit from IdentityUser
2. dotnet add XXX.DataAccess package Microsoft.AspNetCore.Identity.EntityFrameworkCore
3. dotnet add XXX.MainProject package Microsoft.AspNetCore.Identity.EntityFrameworkCore
4. Add to programs.cs in services
   ```
   builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();
   ```
5. In ApplicationDbContext.cs inherit from  "IdentityDbContext" instead of "DbContext"
6. In ApplicactionDbContext.cs add 
   ```
   public DbSet<ApplicationUser> ApplicationUsers { get; set; }
   ```
   This is so we can access the .Net Identity Users table via the db context, like
   the rest of our tables.
7. Add the below to the "OnModelCreating" method in "ApplicationDbContext.cs"
   ```
   base.OnModelCreating(modelBuilder);
   ```
8. . Add a migration, and update database to generate the users table

Image uploads
---
1. Enable static files in api project
   add "app.UseStaticFiles()" to program.cs

Exception Filters 
---
1. create "Filters" Direction
2. create your class, inherited from IExceptionFilter
3. In programs.cs set up in the services section by extending builder.Services.AddControllersWithViews
   ```
   builder.Services.AddControllersWithViews(f => f.Filters.Add(new RedirectOnUnauthorized()));

   ```

Custom Exception Handling in API We have 4 ways
1) Controller
2) Filters
3) Extensions
4) Middleware
---
1. Create a ErrorHandlerController
2. Configure Exception Handling in program.cs
   add to pipeline
   ```
   app.UseExceptionHandler("/api/v2/errorhandler/processerror");
   ```
   "/errorhandling/processerror" is the route to the controller
3. Overiding the the type field in the "Problem" to return a custom value
   Say you want to show a custom link for 500 error handled by the ErrorHandler
   Controller.  In program.cs you would chanin "ConfigureApiBehaviourOptions" unto
   "AddControllers" and define a client error mapping in there. Any 500 handled now
   will in their Probmlem Type filed have this url. Now user can click and find out what your custom error means.
   eg.
   ```
   builder.Services.AddControllers()
   .ConfigureApiBehaviorOptions(options =>
    options.ClientErrorMapping[StatusCodes.Status500InternalServerError] = new ClientErrorData
    {
        Link = "https://fakelink.com" // this can be any link you like
    }
   )
   ```

FILTERS as an alternative Method for Exception Handling 
---
1. As usual create your Filters top level folder
2. Create Exceptions Folder (as they are various types of filtes, eg. action filters)
3. Create your exception filter class eg. "CustomExceptionFilter.cs"
   Note this time we inherit from IActionFilter so we have access
   to methods that hook us into what happns after or during execution
   of action
4. configure in program.cs
   ```
   builder.Services.AddControllers(options =>
      option.Filters.Add<CustomeExceptionFilter>()
   );

   ```   
5. Note that if you want processing of error to end at the filter,
   by defult it does not, the error will also be handled by the 
   ErrorHandlerController. We may or may not want this, so to stop
   an error from being futher handled in the filter, after processing the 
   error we can use "context.ExceptionHandled = true". This will stop any 
   furuther handling of the error that may overwrite the work we already
   did in the filter.

EXTENSIONS as an alternative Method for Exception Handling 
NB: if you use this you do not need the Controller, this will replace it
unlike Filters which is an additon to it.
---
Create the CustomExceptionExtension class as a static class (as ususal for extensions) as it will be an extension on the app itself. Then use it in 
program.cs by adding it to pipeline
```
app.HandleError() // HandleError is what we chose to call our extension method.
```
see Errorhandling/Extensions/CustomExceptionExtension.cs

MIDDLEWARE.
Becareful with middle where. Always remember to pass control to the next middleware
when you are done. And middleware can slow down your app if it is too heavy with logic.
We will use Errorhandling as an example by creating it as a middle ware
and putting it as part of the pipeline in "program.cs"
We will use the same logic as used in when we created it above for
a) Controllers
b) Filters
c) Extensions
d) MiddleWare (this current section)
1. Create Middleware folder and create your middleware file and class in it
eg. "CustomExceptionMiddleWare" 
2. Now set it up in programs.cs
   ```
   app.UseMiddleWare<CustomExceptionMiddleWare>();
   ```

