using BlazorStore.Dto;
using BlazorStore.Models.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.Data.Sqlite;

namespace BlazorStore.Components.Pages.Category
{
    public partial class CategoryUpsert
    {
        private string? message;

        [Parameter]
        public int EntityId { get; set; }

        [SupplyParameterFromForm(FormName = "category-upsert")]
        public CategoryDto? CategoryDto { get; set; }

        private string Title { get; set; } = "Create";

        protected override void OnInitialized()
        {
            if (EntityId > 0)
            {
                Title = "Update";
            }
            else
            {
                Title = "Create";
            }
        }
        protected override async Task OnParametersSetAsync()
        {
            if (EntityId > 0)
            {
                var category = await Get(EntityId);
                if (category is not null)
                {
                    CategoryDto ??= category;
                }
            }
            else
            {
                CategoryDto ??= new();
            }
        }

        private async Task Upsert()
        {
            if (CategoryDto is not null)
            {
                message = "Saving...";

                await _uow.Categories.ExecuteSqlAsync(@"
                    INSERT INTO categories (Id, Name)
                    VALUES (@Id, @Name)
                    ON CONFLICT(Id) DO UPDATE SET
                        Name = EXCLUDED.Name;"
                , [
                    new SqliteParameter("Id", EntityId > 0 ? EntityId : (object)DBNull.Value),
                    new SqliteParameter("Name", CategoryDto.Name),
                ]);

                message = "Saved!";

                _nm.NavigateTo("/category");
            }
            else
            {
                message = "Something went wrong!";
            }
        }

        private async Task<CategoryDto?> Get(int entityId)
        {
            return (await _uow.Categories
            .FromSqlAsync(@"SELECT * FROM Categories WHERE Id = @Id;", [new SqliteParameter("Id", entityId)]))
            .Select(c => c.ToDto()).FirstOrDefault();
        }
    }
}