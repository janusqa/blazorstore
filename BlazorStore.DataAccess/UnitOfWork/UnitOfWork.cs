using Microsoft.EntityFrameworkCore.Storage;
using BlazorStore.DataAccess.Data;
using BlazorStore.DataAccess.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using BlazorStore.Models.Domain;

namespace BlazorStore.DataAccess.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;
        public ICategoryRepository Categories { get; init; }
        public IProductRepository Products { get; init; }
        public IProductPriceRepository ProductPrices { get; init; }
        public IApplicationUserRepository ApplicationUsers { get; init; }

        public UnitOfWork(
            ApplicationDbContext db,
            IConfiguration config,
            UserManager<ApplicationUser> um,
            IUserStore<ApplicationUser> us
        )
        {
            _db = db;

            Categories = new CategoryRepository(_db);
            Products = new ProductRepository(_db);
            ProductPrices = new ProductPriceRepository(_db);
            ApplicationUsers = new ApplicationUserRepository(_db, config, um, us);
        }

        public async Task<int> Complete()
        {
            return await _db.SaveChangesAsync();
        }

        public void Dispose()
        {
            _db.Dispose();
        }

        public IDbContextTransaction Transaction() => _db.Database.BeginTransaction();

    }
}