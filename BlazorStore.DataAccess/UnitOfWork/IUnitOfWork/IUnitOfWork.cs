using BlazorStore.DataAccess.Repository;
using Microsoft.EntityFrameworkCore.Storage;

namespace BlazorStore.DataAccess.UnitOfWork.IUnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        ICategoryRepository Categories { get; init; }
        IProductRepository Products { get; init; }

        Task<int> Complete();

        IDbContextTransaction Transaction();
    }
}