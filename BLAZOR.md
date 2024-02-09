
Tooling for ASP.NET Core Blazor
https://learn.microsoft.com/en-us/aspnet/core/blazor/tooling?view=aspnetcore-8.0&pivots=windows
   
1. Create Project
   1. NB THIS WILL CREATE THE ACTUAL ROOT PROJECT FOLDER FOR YOU, SO DO NOT START FROM WITHING A ROOT FOLDER PER SAY OR YOU WILL GET ERRORS AND WARNINGS!!!
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
