using BlazorStore.DataAccess.Data;
using BlazorStore.Models.Domain;

namespace BlazorStore.DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext db) : base(db)
        {

        }
    }
}