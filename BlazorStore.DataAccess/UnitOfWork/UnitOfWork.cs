using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using BlazorStore.DataAccess.Data;
using BlazorStore.DataAccess.UnitOfWork.IUnitOfWork;

namespace BlazorStore.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;

        public ICategoryRepository Categories { get; init; }


        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;

            Categories = new CategoryRepository(_db);
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