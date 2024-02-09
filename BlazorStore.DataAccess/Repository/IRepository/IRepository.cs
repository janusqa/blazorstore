using System.Linq.Expressions;
using Microsoft.Data.Sqlite;

namespace BlazorStore.DataAccess.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        Task<T?> GetAsync(Expression<Func<T, bool>> predicate, bool tracked);
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? predicate);
        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        Task RemoveAsync(T entity);
        Task RemoveRangeAsync(IEnumerable<T> entities);

        Task<IEnumerable<T>> FromSqlAsync(string sql, List<SqliteParameter> sqlParameters);
        Task ExecuteSqlAsync(string sql, List<SqliteParameter> sqlParameters);
        Task<IEnumerable<U>> SqlQueryAsync<U>(string sql, List<SqliteParameter> sqlParameters);
    }
}