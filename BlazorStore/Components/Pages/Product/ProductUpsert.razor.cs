using BlazorStore.Dto;
using BlazorStore.Models.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Data.Sqlite;
using Microsoft.JSInterop;

namespace BlazorStore.Components.Pages.Product
{
    public partial class ProductUpsert
    {
        private string? message;

        [Parameter]
        public int EntityId { get; set; }

        [SupplyParameterFromForm(FormName = "product-upsert")]
        public ProductDto? ProductDto { get; set; }

        private string Title { get; set; } = "Create";
        private IEnumerable<CategoryDto> Categories { get; set; } = [];

        protected override async Task OnInitializedAsync()
        {
            if (EntityId > 0)
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
            if (EntityId > 0)
            {
                var product = await Get(EntityId);
                if (product is not null)
                {
                    ProductDto ??= product;
                }
            }
            else
            {
                ProductDto ??= new() { Name = string.Empty, Description = string.Empty, ImageUrl = "/images/product/default.png" };
            }
        }

        private async Task Upsert()
        {
            if (ProductDto is not null)
            {
                message = "Saving...";

                if (ProductDto.Image is not null)
                {
                    ProductDto.ImageUrl = await PostImageSSR(ProductDto.Image, ProductDto.ImageUrl);
                }

                await _uow.Products.ExecuteSqlAsync(@"
                    INSERT INTO products 
                    (Id, Name, Description, ShopFavorites, CustomerFavorites, Color, ImageUrl, CategoryId)
                    VALUES (@Id, @Name, @Description, @ShopFavorites, @CustomerFavorites, @Color, @ImageUrl, @CategoryId)
                    ON CONFLICT(Id) DO UPDATE SET
                        Name = EXCLUDED.Name,
                        Description = EXCLUDED.Description,
                        ShopFavorites = EXCLUDED.ShopFavorites,
                        CustomerFavorites = EXCLUDED.CustomerFavorites,
                        Color = EXCLUDED.Color,
                        ImageUrl = EXCLUDED.ImageUrl,
                        CategoryId = EXCLUDED.CategoryId;"
                , [
                    new SqliteParameter("Id", EntityId > 0 ? EntityId : (object)DBNull.Value),
                    new SqliteParameter("Name", ProductDto.Name),
                    new SqliteParameter("Description", ProductDto.Description),
                    new SqliteParameter("ShopFavorites", ProductDto.ShopFavorites),
                    new SqliteParameter("CustomerFavorites", ProductDto.CustomerFavorites),
                    new SqliteParameter("Color", ProductDto.Color),
                    new SqliteParameter("ImageUrl", ProductDto.ImageUrl ?? string.Empty),
                    new SqliteParameter("CategoryId", ProductDto.CategoryId),
                ]);

                message = "Saved!";

                _nm.NavigateTo("/product");
            }
            else
            {
                message = "Something went wrong!";
            }
        }

        private async Task<ProductDto?> Get(int entityId)
        {
            return (await _uow.Products.SqlQueryAsync<Models.Helper.ProductWithCategory>(@"
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
                        CategoryId = p.CategoryId,
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

        private async Task<string?> PostImageSSR(IFormFile Image, string? existingImageUrl)
        {
            try
            {
                if (Image is not null)
                {
                    if (
                        Path.GetExtension(Image.FileName) == ".jpg" ||
                        Path.GetExtension(Image.FileName) == ".png" ||
                        Path.GetExtension(Image.FileName) == ".jpeg"
                    )
                    {
                        return await _fu.PostFileSSR(Image, existingImageUrl);
                    }
                    else
                    {
                        await _ijsr.InvokeVoidAsync("ShowToastr", "error", "Please select .jpg, .jpeg or .png file only");
                    }

                }
            }
            catch (Exception ex)
            {
                await _ijsr.InvokeVoidAsync("ShowToastr", "error", ex.Message);
            }

            return null;
        }

        private async Task PostImage(InputFileChangeEventArgs e, string? existingImageUrl)
        {
            if (ProductDto is not null)
            {
                try
                {
                    if (e.GetMultipleFiles().Count > 0)
                    {
                        foreach (var file in e.GetMultipleFiles())
                        {
                            if (
                                Path.GetExtension(file.Name) == ".jpg" ||
                                Path.GetExtension(file.Name) == ".png" ||
                                Path.GetExtension(file.Name) == ".jpeg"
                            )
                            {
                                ProductDto.ImageUrl = await _fu.PostFile(file, existingImageUrl);
                            }
                            else
                            {
                                await _ijsr.InvokeVoidAsync("ShowToastr", "error", "Please select .jpg, .jpeg or .png file only");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    await _ijsr.InvokeVoidAsync("ShowToastr", "error", ex.Message);
                }
            }
        }
    }
}