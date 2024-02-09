using BlazorStore.Models.Domain;
using BlazorStore.DataAccess.Repository.IRepository;

namespace BlazorStore.DataAccess.Repository
{
    public interface ICategoryRepository : IRepository<Category>
    {
    }
}