using System.Text;
using BlazorStore.DataAccess.UnitOfWork;
using BlazorStore.Dto;
using BlazorStore.Models.Domain;
using BlazorStore.Service.IService;
using Microsoft.Data.Sqlite;

namespace BlazorStore.Service
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _uow;

        public OrderService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public Task<int> Cancel(int entityId)
        {
            throw new NotImplementedException();
        }

        public async Task<OrderDto?> Create(OrderDto orderDto)
        {
            using var transaction = _uow.Transaction();

            try
            {
                var orderHeaderId = (await _uow.OrderHeaders.SqlQueryAsync<int>($@"
                    INSERT INTO OrderHeaders
                    (
                        UserId,
                        OrderTotal,
                        OrderDate,
                        ShippingDate
                        Status,
                        Name,
                        PhoneNumber,
                        StreetAddress,
                        State,
                        City ,
                        PostalCode
                    ) VALUES (
                        @UserId,
                        @OrderTotal,
                        @OrderDate,
                        @ShippingDate
                        @Status,
                        @Name,
                        @PhoneNumber,
                        @StreetAddress,
                        @State,
                        @City ,
                        @PostalCode
                    ) RETURNING Id
                ", [
                        new SqliteParameter("UserId", orderDto.OrderHeader.UserId),
                        new SqliteParameter("OrderTotal",  orderDto.OrderHeader.OrderTotal),
                        new SqliteParameter("OrderDate",  orderDto.OrderHeader.OrderDate),
                        new SqliteParameter("ShippingDate",  orderDto.OrderHeader.ShippingDate),
                        new SqliteParameter("Status",  orderDto.OrderHeader.Status),
                        new SqliteParameter("Name",  orderDto.OrderHeader.Name),
                        new SqliteParameter("PhoneNumber",  orderDto.OrderHeader.PhoneNumber),
                        new SqliteParameter("StreetAddress",  orderDto.OrderHeader.StreetAddress),
                        new SqliteParameter("State",  orderDto.OrderHeader.State),
                        new SqliteParameter("City",  orderDto.OrderHeader.City),
                        new SqliteParameter("PostalCode", orderDto.OrderHeader.PostalCode),
                ])).FirstOrDefault();

                if (orderHeaderId != 0)
                {
                    var inParams = new StringBuilder();
                    var sqlParams = new List<SqliteParameter>();

                    foreach (var (orderDetail, idx) in orderDto.OderDetails.Select((od, idx) => (od, idx)))
                    {
                        inParams.Append($@"
                            (@OrderHeaderId{idx}, 
                            @ProductId{idx}, 
                            @Price{idx}, 
                            @Size{idx},
                            @Count{idx},
                            @ProductName{idx}),
                        ");
                        sqlParams.Add(new SqliteParameter($"OrderHeaderId{idx}", orderHeaderId));
                        sqlParams.Add(new SqliteParameter($"ProductId{idx}", orderDetail.ProductId));
                        sqlParams.Add(new SqliteParameter($"Price{idx}", orderDetail.Price));
                        sqlParams.Add(new SqliteParameter($"Size{idx}", orderDetail.Size));
                        sqlParams.Add(new SqliteParameter($"Count{idx}", orderDetail.Count));
                        sqlParams.Add(new SqliteParameter($"ProductName{idx}", orderDetail.ProductName));
                    }

                    await _uow.OrderDetails.ExecuteSqlAsync($@"
                        INSERT INTO OrderDetails
                        (OrderHeaderId, ProductId, Price, Size, Count, ProductName)
                        VALUES {inParams.ToString()[..^1]}
                    ", sqlParams);

                    transaction.Commit();

                    var orderHeader = orderDto.OrderHeader with { Id = orderHeaderId };
                    return orderDto with { OrderHeader = orderHeader };
                }
                else
                {
                    transaction.Rollback();
                    return null;
                }
            }
            catch (Exception)
            {
                transaction.Rollback();
                return null;
            }
        }

        public Task<OrderDto> Get(int entityId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<OrderDto>> GetAll(string? userId = null, string? status = null)
        {
            throw new NotImplementedException();
        }

        public Task<OrderHeaderDto> PaymentConfirmation(int entityId)
        {
            throw new NotImplementedException();
        }

        public Task<OrderHeaderDto> UpdateOrderDetails(OrderHeaderDto orderHeader)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateOrderStatus(int orderId, string status)
        {
            throw new NotImplementedException();
        }
    }
}