using BlazorStore.DataAccess.Data;
using BlazorStore.Models.Domain;

namespace BlazorStore.DataAccess.Repository
{
    public class ProductPriceRepository : Repository<ProductPrice>, IProductPriceRepository
    {
        public ProductPriceRepository(ApplicationDbContext db) : base(db)
        {

        }
    }
}