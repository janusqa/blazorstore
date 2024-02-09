using System.Linq.Expressions;
using BlazorStore.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using BlazorStore.DataAccess.Data;

namespace BlazorStore.DataAccess.Repository
{

    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _db;
        internal DbSet<T> dbSet;

        public Repository(ApplicationDbContext db)
        {
            _db = db;
            dbSet = _db.Set<T>();
        }

        public async Task<T?> GetAsync(Expression<Func<T, bool>> predicate, bool tracked = true)
        {
            return tracked
                ? await dbSet.Where(predicate).FirstOrDefaultAsync()
                    : await dbSet.Where(predicate).AsNoTracking().FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? predicate)
        {
            return predicate is not null
                ? await dbSet.Where(predicate).ToListAsync()
                    : await dbSet.ToListAsync();
        }

        public async Task AddAsync(T entity)
        {
            await dbSet.AddAsync(entity);
        }

        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await dbSet.AddRangeAsync(entities);
        }

        public async Task RemoveAsync(T entity)
        {
            dbSet.Remove(entity);
            await Task.CompletedTask;
        }

        public async Task RemoveRangeAsync(IEnumerable<T> entities)
        {
            dbSet.RemoveRange(entities);
            await Task.CompletedTask;
        }

        public async Task<IEnumerable<T>> FromSqlAsync(string sql, List<SqliteParameter> sqlParameters)
        {
            return await dbSet.FromSqlRaw(sql, sqlParameters.ToArray()).ToListAsync();
        }

        public async Task ExecuteSqlAsync(string sql, List<SqliteParameter> sqlParameters)
        {
            await _db.Database.ExecuteSqlRawAsync(sql, sqlParameters.ToArray());
        }

        public async Task<IEnumerable<U>> SqlQueryAsync<U>(string sql, List<SqliteParameter> sqlParameters)
        {
            return await _db.Database.SqlQueryRaw<U>(sql, sqlParameters.ToArray()).ToListAsync();
        }
    }
}