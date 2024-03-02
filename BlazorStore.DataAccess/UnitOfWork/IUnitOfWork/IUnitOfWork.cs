using BlazorStore.DataAccess.Repository;
using Microsoft.EntityFrameworkCore.Storage;

namespace BlazorStore.DataAccess.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        ICategoryRepository Categories { get; init; }
        IProductRepository Products { get; init; }
        IProductPriceRepository ProductPrices { get; init; }
        IApplicationUserRepository ApplicationUsers { get; init; }
        IOrderHeaderRepository OrderHeaders { get; init; }
        IOrderDetailRepository OrderDetails { get; init; }

        Task<int> Complete();

        IDbContextTransaction Transaction();
    }
}