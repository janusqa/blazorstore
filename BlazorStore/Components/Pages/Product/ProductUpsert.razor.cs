using BlazorStore.Dto;
using BlazorStore.Models.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.Data.Sqlite;

namespace BlazorStore.Components.Pages.Product
{
    public partial class ProductUpsert
    {
        private string? message;

        [Parameter]
        public int EntityId { get; set; }

        [SupplyParameterFromForm(FormName = "product-upsert")]
        public ProductDto ProductDto { get; set; } = new() { Name = string.Empty, Description = string.Empty };

        private string Title { get; set; } = "Create";
        private IEnumerable<CategoryDto> Categories { get; set; } = [];

        protected override async Task OnInitializedAsync()
        {
            if (EntityId != 0)
            {
                Title = "Update";
            }
            else
            {
                Title = "Create";
            }
            Categories = await GetCategories();
        }
        protected override async Task OnParametersSetAsync()
        {
            if (EntityId != 0)
            {
                var product = await Get(EntityId);
                if (product is not null)
                {
                    ProductDto.Id = product.Id;
                    ProductDto.Name ??= product.Name;
                    ProductDto.Description ??= product.Description;
                    ProductDto.ShopFavorites = product.ShopFavorites;
                    ProductDto.CustomerFavorites = product.CustomerFavorites;
                    ProductDto.Color ??= product.Color;
                    ProductDto.ImageUrl ??= product.ImageUrl;
                    ProductDto.CategoryId = product.CategoryId;
                    ProductDto.CategoryDto ??= product.CategoryDto;
                }
            }
        }

        private async Task Upsert()
        {
            if (ProductDto is not null)
            {
                message = "Saving...";

                await _uow.Products.ExecuteSqlAsync(@"
                    INSERT INTO categories (Id, Name)
                    VALUES (@Id, @Name)
                    ON CONFLICT(Id) DO UPDATE SET
                    Name = EXCLUDED.Name;"
                , [
                    new SqliteParameter("Name", ProductDto.Name),
                    new SqliteParameter("Id",EntityId !=0 ? EntityId : (object)DBNull.Value),
                ]);

                message = "Saved!";

                _nm.NavigateTo("/category");
            }
            else
            {
                message = "Something went wrong!";
            }
        }

        private async Task<ProductDto?> Get(int entityId)
        {
            return (await _uow.Products.SqlQueryAsync<Models.Helper.ProductWithIncluded>(@"
                SELECT p.*, c.Name AS CategoryName 
                FROM Products p INNER JOIN Categories c ON (p.CategoryId = c.Id) WHERE p.Id = @Id;"
                , [new SqliteParameter("Id", entityId)]))
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
                )
                .FirstOrDefault();
        }

        private async Task<IEnumerable<CategoryDto>> GetCategories()
        {
            return (await _uow.Categories.FromSqlAsync($@"SELECT * FROM Categories;", [])).Select(c => c.ToDto());
        }
    }
}