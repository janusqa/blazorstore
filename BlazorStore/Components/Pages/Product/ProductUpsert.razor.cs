using BlazorStore.Models.ViewModel;
using BlazorStore.Models.Helper;
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
        public ProductVm? ProductVm { get; set; }

        private string Title { get; set; } = "Create";
        private IEnumerable<CategoryDto> Categories { get; set; } = [];

        private bool submissionError = false;

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
                    ProductVm ??= product;
                }
            }
            else
            {
                ProductVm ??= new() { Name = string.Empty, Description = string.Empty, ImageUrl = "/images/product/default.png" };
            }
        }

        private async Task Upsert()
        {
            if (ProductVm is not null)
            {
                message = "Saving...";

                var existingImageUrl = ProductVm.ImageUrl;
                var fileUploadResult = ProductVm.Image is not null
                    ? await PostImageSSR(ProductVm.Image, existingImageUrl)
                    : (null, null);

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
                    new SqliteParameter("Name", ProductVm.Name),
                    new SqliteParameter("Description", ProductVm.Description),
                    new SqliteParameter("ShopFavorites", ProductVm.ShopFavorites),
                    new SqliteParameter("CustomerFavorites", ProductVm.CustomerFavorites),
                    new SqliteParameter("Color", ProductVm.Color),
                    new SqliteParameter("ImageUrl", (fileUploadResult.ImageUrl is null ? existingImageUrl : fileUploadResult.ImageUrl) ?? string.Empty),
                    new SqliteParameter("CategoryId", ProductVm.CategoryId),
                ]);

                message = "Saved!";

                if (fileUploadResult.Error is null)
                    _nm.NavigateTo("/product");
                else
                {
                    submissionError = true;
                    message = fileUploadResult.Error;
                }
            }
            else
            {
                submissionError = true;
                message = "Something went wrong!";
            }
        }

        private async Task<ProductVm?> Get(int entityId)
        {
            return (await _uow.Products.SqlQueryAsync<ProductWithCategory>(@"
                SELECT p.*, c.Name AS CategoryName 
                FROM Products p INNER JOIN Categories c ON (p.CategoryId = c.Id) WHERE p.Id = @Id;"
                , [new SqliteParameter("Id", entityId)]))
                .Select(p =>
                    new ProductVm
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Description = p.Description,
                        ShopFavorites = p.ShopFavorites,
                        CustomerFavorites = p.CustomerFavorites,
                        Color = p.Color,
                        CategoryId = p.CategoryId,
                        ImageUrl = p.ImageUrl,
                        CategoryDto = new Models.Domain.Category
                        {
                            Id = p.CategoryId,
                            Name = p.CategoryName
                        }.ToDto()
                    }
                )
                .FirstOrDefault();
        }

        private async Task<IEnumerable<CategoryDto>> GetCategories()
        {
            return (await _uow.Categories.FromSqlAsync($@"SELECT * FROM Categories;", [])).Select(c => c.ToDto());
        }

        private async Task<(string? ImageUrl, string? Error)> PostImageSSR(IFormFile Image, string? existingImageUrl)
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
                        return (null, "Please select .jpg, .jpeg or .png file only");
                    }
                }
            }
            catch (Exception ex)
            {
                return (null, ex.Message);
            }

            return (null, null);
        }

        // private async Task PostImage(InputFileChangeEventArgs e, string? existingImageUrl)
        // {
        //     if (ProductVm is not null)
        //     {
        //         try
        //         {
        //             if (e.GetMultipleFiles().Count > 0)
        //             {
        //                 foreach (var file in e.GetMultipleFiles())
        //                 {
        //                     if (
        //                         Path.GetExtension(file.Name) == ".jpg" ||
        //                         Path.GetExtension(file.Name) == ".png" ||
        //                         Path.GetExtension(file.Name) == ".jpeg"
        //                     )
        //                     {
        //                         ProductVm.ImageUrl = await _fu.PostFile(file, existingImageUrl);
        //                     }
        //                     else
        //                     {
        //                         // await _ijsr.InvokeVoidAsync("ShowToastr", "error", "Please select .jpg, .jpeg or .png file only");
        //                     }
        //                 }
        //             }
        //         }
        //         catch (Exception ex)
        //         {
        //             // await _ijsr.InvokeVoidAsync("ShowToastr", "error", ex.Message);
        //         }
        //     }
        // }
    }
}