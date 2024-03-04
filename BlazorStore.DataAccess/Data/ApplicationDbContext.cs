using BlazorStore.Common;
using BlazorStore.Models.Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BlazorStore.DataAccess.Data
{
    // To configure Identity we changed DBContext to IdentityDbContext<ApplicationUser>
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductPrice> ProductPrices { get; set; }
        public DbSet<OrderHeader> OrderHeaders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            // we need this line when using identity or will get error
            // ERROR: InvalidOperationException: The entity type 'IdentityUserLogin<string>' requires a primary key to be defined. If you intended to use a keyless entity type, call 'HasNoKey' in 'OnModelCreating'. For more information on keyless entity types, see https://go.microsoft.com/fwlink/?linkid=2141943.
            base.OnModelCreating(modelBuilder);

            // set created date in tables
            foreach (var (Model, _) in SD.dbEntity)
            {
                var entityType = Type.GetType($"{SD.modelsAssembly}.{SD.modelsNamespace}.{Model}, {SD.modelsAssembly}");
                if (entityType is not null)
                {
                    var entity = modelBuilder.Entity(entityType);
                    entity.Property("CreatedDate").HasDefaultValueSql("DATETIME('now')");
                    entity.Property("UpdatedDate").HasDefaultValueSql("DATETIME('now')");
                }
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableDetailedErrors(); // Enable detailed error messages

        }
    }
}