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

        private SfGrid<ProductPriceDto>? sfGridRef;

        private readonly IEnumerable<string> Sizes = [
            "Small", "Medium", "Large", "8oz", "24oz"
        ];

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
        }

        private async Task<ProductDto?> Get(int entityId)
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
                    .Select(p => new Models.Domain.ProductPrice { Id = p.ProductPriceId ?? 0, Size = p.Size ?? string.Empty, Price = p.Price ?? 0, ProductId = k })
                }).FirstOrDefault()?.ToDto();
        }

        private async Task SfGridActions(ActionEventArgs<ProductPriceDto> args)
        {
            if (args.RequestType.Equals(Syncfusion.Blazor.Grids.Action.Save))
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
                    new SqliteParameter("Id", args.Data.Id > 0 ? args.Data.Id : (object)DBNull.Value),
                    new SqliteParameter("ProductId", args.Data.ProductId > 0 ? args.Data.ProductId : EntityId),
                    new SqliteParameter("Size", args.Data.Size),
                    new SqliteParameter("Price", args.Data.Price),
                ]);

                if (sfGridRef is not null) await sfGridRef.Refresh();
                // if (args.Action.Equals("Add", StringComparison.CurrentCultureIgnoreCase)){}
                // else if (args.Action.Equals("Edit", StringComparison.CurrentCultureIgnoreCase)){}
            }
            else if (args.RequestType.Equals(Syncfusion.Blazor.Grids.Action.Delete))
            {
                await _uow.ProductPrices.ExecuteSqlAsync(@"
                    DELETE FROM  ProductPrices WHERE Id = @Id;"
                , [new SqliteParameter("Id", args.Data.Id)]);

                if (sfGridRef is not null) await sfGridRef.Refresh();
            }
        }

    }
}