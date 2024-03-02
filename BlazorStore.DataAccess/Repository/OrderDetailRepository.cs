using BlazorStore.DataAccess.Data;
using BlazorStore.Models.Domain;

namespace BlazorStore.DataAccess.Repository
{
    public class OrderDetailRepository : Repository<OrderDetail>, IOrderDetailRepository
    {
        public OrderDetailRepository(ApplicationDbContext db) : base(db)
        {

        }
    }
}