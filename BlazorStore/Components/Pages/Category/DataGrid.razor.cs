using BlazorStore.Common;
using BlazorStore.Dto;
using BlazorStore.Models.Extensions;
using Microsoft.AspNetCore.Components.QuickGrid;
using Microsoft.Data.Sqlite;
using Microsoft.JSInterop;

namespace BlazorStore.Components.Pages.Category
{
    public partial class DataGrid
    {
        private PaginationState pagination = new PaginationState { ItemsPerPage = SD.paginationSize };
        private GridItemsProvider<CategoryDto>? entityProvider;
        private QuickGrid<CategoryDto>? quickGridRef;

        protected override void OnInitialized()
        {
            entityProvider = async request =>
            {
                var response = await List(request.StartIndex, request.Count);
                return GridItemsProviderResult.From(items: response.Data.ToList(), totalItemCount: response.Count);
            };
        }

        private async Task<(IEnumerable<CategoryDto> Data, int Count)> List(int offset = SD.paginationDefaultPage, int? limit =
        SD.paginationDefaultSize)
        {
            var numRecords = (await _uow.Categories.SqlQueryAsync<int>($@"SELECT COUNT(Id) FROM Categories;", [])).FirstOrDefault();

            var data = (await _uow.Categories.FromSqlAsync($@"
                SELECT slow.* FROM Categories AS slow
                INNER JOIN (SELECT Id FROM Categories LIMIT @Limit OFFSET @Offset) AS fast
                USING (Id);",
            [
                new SqliteParameter("Offset", offset),
                new SqliteParameter("Limit", limit)
            ]))
            .Select(c => c.ToDto());

            return (data, numRecords);
        }

        private async Task Delete(int entityId)
        {
            var confirmed = await _ijsr.InvokeAsync<bool>("ConfirmationModal", "Confirm Delete", "Are you sure you want to delete",
            "Delete");
            if (confirmed)
            {
                await _ijsr.InvokeVoidAsync("Spinner", true);
                await _uow.Categories.ExecuteSqlAsync($@"DELETE FROM Categories WHERE Id = @Id;",
                [new SqliteParameter("Id", entityId)]);
                if (quickGridRef is not null) await quickGridRef.RefreshDataAsync();
                await _ijsr.InvokeVoidAsync("Spinner", false);
            }
        }
    }
}