using BlazorStore.Dto;
using BlazorStore.Models.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.Data.Sqlite;
using Syncfusion.Blazor.Grids;

namespace BlazorStore.Components.Pages.Product
{
    public partial class ProductPrice
    {
        [Parameter]
        public int EntityId { get; set; }

        private ProductDto? ProductDto { get; set; }
        private List<ProductPriceDto>? ProductPrices { get; set; }

        // syncfuscion
        private SfGrid<ProductPriceDto>? sfGridRef;

        private readonly IEnumerable<string> Sizes = [
            "Small", "Medium", "Large", "8oz", "24oz"
        ];

        protected override async Task OnParametersSetAsync()
        {
            if (EntityId > 0)
            {
                var product = await GetProduct(EntityId);
                if (product is not null)
                {
                    ProductDto ??= product;
                    ProductPrices ??= product.ProductPrices?.ToList();
                }
            }
        }

        private async Task<ProductDto?> GetProduct(int entityId)
        {
            return (await _uow.Products.SqlQueryAsync<Models.Helper.ProductWithPrice>(@"
                SELECT 
                    p.*, 
                    c.Name AS CategoryName, 
                    pp.Id AS ProductPriceId,
                    pp.Size,
                    pp.Price
                FROM 
                    Products p 
                INNER JOIN 
                    Categories c ON (p.CategoryId = c.Id) 
                LEFT JOIN 
                    ProductPrices pp ON (pp.ProductId = p.Id)
                WHERE p.Id = @Id;"
            , [new SqliteParameter("Id", entityId)]))
            .GroupBy(
                p => p.Id,
                p => new
                {
                    p.Id,
                    p.Name,
                    p.Description,
                    p.ShopFavorites,
                    p.CustomerFavorites,
                    p.Color,
                    p.CategoryId,
                    p.ImageUrl,
                    p.CategoryName,
                    p.ProductPriceId,
                    p.Price,
                    p.Size
                },
                (k, g) => new Models.Domain.Product
                {
                    Id = k,
                    Name = g.First().Name,
                    Description = g.First().Description,
                    ShopFavorites = g.First().ShopFavorites,
                    CustomerFavorites = g.First().CustomerFavorites,
                    Color = g.First().Color,
                    CategoryId = g.First().CategoryId,
                    ImageUrl = g.First().ImageUrl,
                    Category = new Models.Domain.Category { Id = g.First().CategoryId, Name = g.First().CategoryName },
                    ProductPrices = g
                    .Where(p => p.ProductPriceId is not null)
                    .Select(p => new Models.Domain.ProductPrice { Id = p.ProductPriceId ?? 0, Size = p.Size ?? string.Empty, Price = p.Price ?? 0, ProductId = k }).ToList()
                }).FirstOrDefault()?.ToDto();
        }

        private async Task<IEnumerable<ProductPriceDto>> GetAll(int ProductId)
        {
            return (await _uow.ProductPrices.FromSqlAsync(@"
                SELECT * FROM  ProductPrices WHERE ProductId = @ProductId;"
            , [new SqliteParameter("ProductId", ProductId)], tracked: false))
            .Select(pp => pp.ToDto()) ?? [];
        }

        private async Task Upsert(ProductPriceDto productPrice)
        {
            await _uow.ProductPrices.ExecuteSqlAsync(@"
                INSERT INTO ProductPrices 
                (Id, ProductId, Size, Price)
                VALUES (@Id, @ProductId, @Size, @Price)
                ON CONFLICT(Id) DO UPDATE SET
                    ProductId = EXCLUDED.ProductId,
                    Size = EXCLUDED.Size,
                    Price = EXCLUDED.Price;"
            , [
                new SqliteParameter("Id", productPrice.Id > 0 ? productPrice.Id : (object)DBNull.Value),
                new SqliteParameter("ProductId", productPrice.ProductId > 0 ? productPrice.ProductId : EntityId),
                new SqliteParameter("Size", productPrice.Size),
                new SqliteParameter("Price", productPrice.Price),
            ]);
        }

        private async Task Delete(ProductPriceDto productPrice)
        {
            await _uow.ProductPrices.ExecuteSqlAsync(@"
                 DELETE FROM ProductPrices WHERE Id = @Id;"
            , [new SqliteParameter("Id", productPrice.Id)]);
        }


        private async void OnGridActionComplete(ActionEventArgs<ProductPriceDto> args)
        {
            if (args.RequestType.Equals(Syncfusion.Blazor.Grids.Action.Save) || args.RequestType.Equals(Syncfusion.Blazor.Grids.Action.Delete))
            {
                ProductPrices = (await GetAll(EntityId)).ToList();

                foreach (var p in ProductPrices!)
                {
                    Console.WriteLine(p);
                }
            }
        }

        private async void OnGridActionBegin(ActionEventArgs<ProductPriceDto> args)
        {
            if (args.RequestType.Equals(Syncfusion.Blazor.Grids.Action.Save))
            {
                await Upsert(args.Data);
            }
            else if (args.RequestType.Equals(Syncfusion.Blazor.Grids.Action.Delete))
            {
                await Delete(args.Data);
            }
        }
    }
}