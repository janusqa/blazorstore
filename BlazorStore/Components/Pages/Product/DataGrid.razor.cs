using BlazorStore.Common;
using BlazorStore.Dto;
using BlazorStore.Models.Extensions;
using Microsoft.AspNetCore.Components.QuickGrid;
using Microsoft.Data.Sqlite;
using Microsoft.JSInterop;

namespace BlazorStore.Components.Pages.Product
{
    public partial class DataGrid
    {
        private PaginationState pagination = new PaginationState { ItemsPerPage = SD.paginationSize };
        private GridItemsProvider<ProductDto>? entityProvider;
        private QuickGrid<ProductDto>? quickGridRef;

        protected override void OnInitialized()
        {
            entityProvider = async request =>
            {
                var response = await List(request.StartIndex, request.Count);
                return GridItemsProviderResult.From(items: response.Data.ToList(), totalItemCount: response.Count);
            };
        }

        private async Task<(IEnumerable<ProductDto> Data, int Count)> List(int offset = SD.paginationDefaultPage, int? limit =
        SD.paginationDefaultSize)
        {
            var numRecords = (await _uow.Products.SqlQueryAsync<int>($@"SELECT COUNT(Id) FROM Products;", [])).FirstOrDefault();

            var data = (await _uow.Products.SqlQueryAsync<Models.Helper.ProductWithIncluded>($@"
                SELECT p.*, c.Name AS CategoryName
                FROM Products p INNER JOIN (SELECT Id FROM Products LIMIT @Limit OFFSET @Offset) AS fast USING (Id)
                INNER JOIN Categories c ON (p.categoryId = c.Id);",
            [
                new SqliteParameter("Offset", offset),
                new SqliteParameter("Limit", limit)
            ]))
            .Select(p =>
                new Models.Domain.Product
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    ShopFavorites = p.ShopFavorites,
                    CustomerFavorites = p.CustomerFavorites,
                    Color = p.Color,
                    ImageUrl = p.ImageUrl,
                    Category = new Models.Domain.Category
                    {
                        Id = p.CategoryId,
                        Name = p.CategoryName
                    }
                }.ToDto()
            );

            return (data, numRecords);
        }

        private async Task Delete(int entityId)
        {
            var confirmed = await _ijsr.InvokeAsync<bool>(
                "ConfirmationModal",
                "Confirm Delete",
                "Are you sure you want to delete",
                "Delete"
            );
            if (confirmed)
            {
                await _ijsr.InvokeVoidAsync("Spinner", true);
                await _uow.Products.ExecuteSqlAsync($@"DELETE FROM Products WHERE Id = @Id;",
                [new SqliteParameter("Id", entityId)]);
                if (quickGridRef is not null) await quickGridRef.RefreshDataAsync();
                await _ijsr.InvokeVoidAsync("Spinner", false);
            }
        }
    }
}