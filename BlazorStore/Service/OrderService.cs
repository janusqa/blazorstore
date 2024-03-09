using System.Text;
using BlazorStore.Common;
using BlazorStore.DataAccess.UnitOfWork;
using BlazorStore.Dto;
using BlazorStore.Models.Domain;
using BlazorStore.Models.Extensions;
using BlazorStore.Models.Helper;
using BlazorStore.Service.IService;
using Microsoft.Data.Sqlite;
using Stripe.Checkout;

namespace BlazorStore.Service
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _uow;
        private readonly IPaymentService<Session> _paymentService;

        public OrderService(IUnitOfWork uow, IPaymentService<Session> ps)
        {
            _uow = uow;
            _paymentService = ps;
        }

        public async Task<bool> Cancel(int entityId)
        {
            using var transaction = _uow.Transaction();
            try
            {
                await _uow.OrderDetails.ExecuteSqlAsync($@"
                    DELETE FROM OrderDetails WHERE OrderHeaderId = @Id;
                ", [new SqliteParameter("Id", entityId)]);

                await _uow.OrderHeaders.ExecuteSqlAsync($@"
                    DELETE FROM OrderHeaders WHERE Id = @Id;
                ", [new SqliteParameter("Id", entityId)]);

                transaction.Commit();

                return true;
            }
            catch (Exception)
            {
                transaction.Rollback();
                return false;
            }
        }

        public async Task<OrderDto?> Create(OrderDto order)
        {

            var session = _paymentService.Checkout(order);

            if (session is null) return null;

            using var transaction = _uow.Transaction();

            try
            {
                var orderHeaderId = (await _uow.OrderHeaders.SqlQueryAsync<int>($@"
                    INSERT INTO OrderHeaders
                    (
                        UserId,
                        Email,
                        OrderTotal,
                        OrderDate,
                        ShippingDate,
                        Status,
                        Name,
                        PhoneNumber,
                        StreetAddress,
                        State,
                        City,
                        PostalCode,
                        SessionId
                    ) VALUES (
                        @UserId,
                        @Email,
                        @OrderTotal,
                        @OrderDate,
                        @ShippingDate,
                        @Status,
                        @Name,
                        @PhoneNumber,
                        @StreetAddress,
                        @State,
                        @City ,
                        @PostalCode,
                        @SessionId
                    ) RETURNING Id;
                ", [
                        new SqliteParameter("UserId", order.OrderHeader.UserId),
                        new SqliteParameter("Email", order.OrderHeader.Email),
                        new SqliteParameter("OrderTotal",  order.OrderHeader.OrderTotal),
                        new SqliteParameter("OrderDate",  DateTime.UtcNow),
                        new SqliteParameter("Status",  order.OrderHeader.Status),
                        new SqliteParameter("ShippingDate",  order.OrderHeader.ShippingDate),
                        new SqliteParameter("Name",  order.OrderHeader.Name),
                        new SqliteParameter("PhoneNumber",  order.OrderHeader.PhoneNumber),
                        new SqliteParameter("StreetAddress",  order.OrderHeader.StreetAddress),
                        new SqliteParameter("State",  order.OrderHeader.State),
                        new SqliteParameter("City",  order.OrderHeader.City),
                        new SqliteParameter("PostalCode", order.OrderHeader.PostalCode),
                        new SqliteParameter("SessionId", session.Id),
                ])).FirstOrDefault();

                if (orderHeaderId != 0)
                {
                    List<string> inParams = [];
                    var sqlParams = new List<SqliteParameter>();

                    foreach (var (orderDetail, idx) in order.OrderDetails.Select((od, idx) => (od, idx)))
                    {
                        inParams.Add($@"
                            (@OrderHeaderId{idx}, 
                            @ProductId{idx}, 
                            @Price{idx}, 
                            @Size{idx},
                            @Count{idx},
                            @ProductName{idx})
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
                        VALUES {string.Join(", ", inParams)};
                    ", sqlParams);

                    transaction.Commit();

                    return order with { OrderHeader = order.OrderHeader with { Id = orderHeaderId }, PaymentUrl = session.Url };
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

        public async Task<OrderDto?> Get(int entityId)
        {
            try
            {
                var orderHeader = (await _uow.OrderHeaders.FromSqlAsync($@"
                    SELECT * FROM OrderHeaders WHERE Id = @Id;
                ", [new SqliteParameter("Id", entityId)])).FirstOrDefault();

                if (orderHeader is not null)
                {
                    var orderDetails = await _uow.OrderDetails.FromSqlAsync($@"
                        SELECT * FROM OrderDetails WHERE Id = @Id;
                    ", [new SqliteParameter("Id", entityId)]);

                    return new OrderDto
                    {
                        OrderHeader = orderHeader.ToDto(),
                        OrderDetails = orderDetails.Select(od => od.ToDto()).ToList()
                    };
                }

                return null;
            }
            catch (Exception)
            {
                return null;
            };
        }

        public async Task<IEnumerable<OrderDto>?> GetAll(string? userId = null, string? status = null)
        {
            try
            {
                var sqlParams = new List<SqliteParameter>();
                var inParams = new List<string>();
                if (userId is not null)
                {
                    sqlParams.Add(new SqliteParameter($"UserId", userId));
                    inParams.Add("oh.UserId = @UserId");
                }
                if (userId is not null)
                {
                    sqlParams.Add(new SqliteParameter($"Status", status));
                    inParams.Add("oh.Status = @Status");
                }

                var orderHeaders = await _uow.OrderHeaders.FromSqlAsync($@"
                    SELECT * FROM OrderHeaders WHERE {string.Join(" AND ", inParams)};
                ", sqlParams);

                var orderHeaderIds = orderHeaders.Select(oh => oh.Id).ToList();

                inParams.Clear();
                sqlParams.Clear();
                foreach (var (ohId, idx) in orderHeaderIds.Select((ohId, idx) => (ohId, idx)))
                {
                    inParams.Add($"@oh{idx}");
                    sqlParams.Add(new SqliteParameter($"oh{idx}", ohId));
                }

                var orderDetails = await _uow.OrderDetails.FromSqlAsync($@"
                    SELECT * FROM OrderDetails WHERE OrderHeaderId IN ({string.Join(", ", inParams)});
                ", sqlParams);

                List<OrderDto> orderDtos = [];
                foreach (var orderHeader in orderHeaders)
                {
                    orderDtos.Add(
                        new OrderDto
                        {
                            OrderHeader = orderHeader.ToDto(),
                            OrderDetails = orderDetails.Where(od => od.OrderHeaderId == orderHeader.Id).Select(od => od.ToDto()).ToList()
                        }
                    );
                }

                return orderDtos;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<OrderHeaderDto?> PaymentConfirmation(int entityId)
        {
            var orderHeaderDto = (await _uow.OrderHeaders.SqlQueryAsync<OrderHeader>($@"
                UPDATE OrderHeaders 
                SET 
                    Status = @NewStatus 
                WHERE Id = @Id AND Status = @OldStatus
                RETURNING *;
            ", [
                new SqliteParameter("Id", entityId),
                new SqliteParameter("OldStatus", SD.OrderStatusPending),
                new SqliteParameter("NewStatus", SD.OrderStatusApproved)
            ])).FirstOrDefault()?.ToDto();

            return orderHeaderDto;
        }

        public async Task<OrderHeaderDto?> UpdateOrderDetails(OrderHeaderDto orderHeader)
        {
            var orderHeaderDto = (await _uow.OrderHeaders.SqlQueryAsync<OrderHeader>($@"
                UPDATE OrderHeaders 
                SET 
                    Name = @Name, 
                    PhoneNumber = @PhoneNumber, 
                    StreetAddress = @StreetAddress, 
                    State = @State, 
                    City = @City, 
                    PostalCode = @PostalCode 
                WHERE Id = @Id
                RETURNING *;
            ", [
                new SqliteParameter("Id", orderHeader.Id),
                new SqliteParameter("Name", orderHeader.Name),
                new SqliteParameter("PhoneNumber", orderHeader.PhoneNumber),
                new SqliteParameter("StreetAddress", orderHeader.StreetAddress),
                new SqliteParameter("State", orderHeader.State),
                new SqliteParameter("City", orderHeader.City),
                new SqliteParameter("PostalCode", orderHeader.PostalCode)
            ])).FirstOrDefault()?.ToDto();

            return orderHeaderDto;
        }

        public async Task<bool> UpdateOrderStatus(int entityId, string status)
        {
            var sqlParams = new List<SqliteParameter> {
                new SqliteParameter("Id", entityId),
                new SqliteParameter("NewStatus", status)
            };
            var inParams = new List<string> { "Status = @NewStatus" };
            if (status == SD.OrderStatusShipped)
            {
                sqlParams.Add(new SqliteParameter("ShippingDate", DateTime.UtcNow));
                inParams.Add("ShippingDate = @ShippingDate");
            }

            var orderHeaderId = (await _uow.OrderHeaders.SqlQueryAsync<int>($@"
                UPDATE OrderHeaders 
                SET 
                    {string.Join(", ", inParams)} 
                WHERE Id = @Id
                RETURNING Id;
            ", sqlParams)).FirstOrDefault();

            return orderHeaderId != 0;
        }
    }
}