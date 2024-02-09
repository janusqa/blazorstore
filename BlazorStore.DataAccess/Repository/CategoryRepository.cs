using BlazorStore.DataAccess.Data;
using BlazorStore.Models.Domain;

namespace BlazorStore.DataAccess.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(ApplicationDbContext db) : base(db)
        {

        }
    }
}