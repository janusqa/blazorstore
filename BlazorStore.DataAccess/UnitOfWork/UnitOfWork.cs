using Microsoft.EntityFrameworkCore.Storage;
using BlazorStore.DataAccess.Data;
using BlazorStore.DataAccess.UnitOfWork.IUnitOfWork;

namespace BlazorStore.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;

        public ICategoryRepository Categories { get; init; }

        public IProductRepository Products { get; init; }

        public IProductPriceRepository ProductPrices { get; init; }

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;

            Categories = new CategoryRepository(_db);
            Products = new ProductRepository(_db);
            ProductPrices = new ProductPriceRepository(_db);
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