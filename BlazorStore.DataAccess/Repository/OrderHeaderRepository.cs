using BlazorStore.DataAccess.Data;
using BlazorStore.Models.Domain;

namespace BlazorStore.DataAccess.Repository
{
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {
        public OrderHeaderRepository(ApplicationDbContext db) : base(db)
        {

        }
    }
}