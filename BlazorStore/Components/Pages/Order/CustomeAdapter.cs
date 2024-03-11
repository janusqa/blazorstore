// using BlazorStore.Common;
// using BlazorStore.DataAccess.UnitOfWork;
// using BlazorStore.Dto;
// using BlazorStore.Models.Domain;
// using BlazorStore.Models.Extensions;
// using Microsoft.Data.Sqlite;
// using Syncfusion.Blazor;

// namespace BlazorStore.Components.Pages.Order
// {

//     public class CustomAdaptor : DataAdaptor
//     {
//         private readonly IUnitOfWork _uow;
//         public CustomAdaptor(IUnitOfWork uow)
//         {
//             _uow = uow;
//         }

//         public List<OrderHeaderDto> OrderHeaders { get; set; } = OrderDetails.GetAllRecords();
//         public override async Task<Object> ReadAsync(DataManagerRequest dataManagerRequest, string key = null)
//         {
//             IEnumerable GridData = Orders;
//             IEnumerable DataSource = Orders;
//             await Task.Delay(100); //To mimic asynchronous operation, we delayed this operation using Task.Delay
//             if (dataManagerRequest.Sorted?.Count > 0) // perform Sorting
//             {
//                 GridData = DataOperations.PerformSorting(GridData, dataManagerRequest.Sorted);
//             }
//             if (dataManagerRequest.Skip != 0)
//             {
//                 GridData = DataOperations.PerformSkip(GridData, dataManagerRequest.Skip); //Paging
//             }
//             if (dataManagerRequest.Take != 0)
//             {
//                 GridData = DataOperations.PerformTake(GridData, dataManagerRequest.Take);
//             }
//             IDictionary<string, object> aggregates = new Dictionary<string, object>();
//             if (dataManagerRequest.Aggregates != null) // Aggregation
//             {
//                 aggregates = DataUtil.PerformAggregation(DataSource, dataManagerRequest.Aggregates);
//             }
//             if (dataManagerRequest.Group != null && dataManagerRequest.Group.Any()) //Grouping
//             {
//                 foreach (var group in dataManagerRequest.Group)
//                 {
//                     GridData = DataUtil.Group<OrderDetails>(GridData, group, dataManagerRequest.Aggregates, 0, dataManagerRequest.GroupByFormatter);
//                 }
//             }
//             return dataManagerRequest.RequiresCounts ? new DataResult() { Result = GridData, Count = Orders.Count(), Aggregates = aggregates } : (object)GridData;
//         }

//         private async Task<(IEnumerable<OrderHeaderDto> Data, int Count)> GetAll(
//             int offset = SD.paginationDefaultPage,
//             int? limit = SD.paginationDefaultSize
//         )
//         {
//             var numRecords = (await _uow.Products.SqlQueryAsync<int>($@"SELECT COUNT(Id) FROM OrderHeaders;", [])).FirstOrDefault();

//             var data = (await _uow.OrderHeaders.FromSqlAsync($@"
//                 SELECT oh.*
//                 FROM
//                 OrderHeaders oh
//                 INNER JOIN
//                 (SELECT Id FROM OrderHeaders LIMIT @Limit OFFSET @Offset) AS fast
//                 USING (Id);",
//             [
//             new SqliteParameter("Offset", offset), new SqliteParameter("Limit", limit)
//             ])).Select(oh => oh.ToDto()) ?? [];

//             return (data, numRecords);
//         }

//         public override async Task<object> InsertAsync(DataManager dataManager, object value, string key)
//         {
//             var orderHeader = value as OrderHeaderDto;
//             return (await _uow.OrderHeaders.SqlQueryAsync<OrderHeader>(@"
//                 INSERT INTO OrderHeaders
//                 (Id, UserId)
//                 VALUES (@Id, @UserId)
//                 ON CONFLICT(Id) DO UPDATE SET
//                 UserId = EXCLUDED.UserId returning *;"
//             , [
//                 new SqliteParameter("Id", orderHeader?.Id > 0 ? orderHeader?.Id : (object)DBNull.Value),
//                 new SqliteParameter("UserId", orderHeader?.Id)
//             ])).FirstOrDefault()?.ToDto() ?? value;
//         }


//         public override async Task<object> UpdateAsync(DataManager dataManager, object value, string keyField, string key)
//         {

//             return await InsertAsync(dataManager, value, key);
//         }

//         public override async Task<object> RemoveAsync(DataManager dataManager, object value, string keyField, string key)
//         {
//             var transaction = _uow.Transaction();

//             try
//             {
//                 int orderHeaderId = (int)value;
//                 await _uow.OrderHeaders.ExecuteSqlAsync(@"DELETE FROM OrderHeaders WHERE Id = @Id;"
//                 , [new SqliteParameter("Id", orderHeaderId)]);

//                 await _uow.OrderDetails.ExecuteSqlAsync(@"DELETE FROM OrderDetails WHERE OrderHeaderId = @OrderHeaderId;"
//                 , [new SqliteParameter("OrderheaderId", orderHeaderId)]);

//                 transaction.Commit();
//             }
//             catch
//             {
//                 transaction.Rollback();
//             }

//             return value;
//         }
//     }
// }