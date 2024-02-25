using Asp.Versioning;
using BlazorStore.DataAccess.UnitOfWork;
using BlazorStore.Dto;
using BlazorStore.Models.Domain;
using BlazorStore.Models.Extensions;
using BlazorStore.Models.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;

namespace BlazorStore.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/products")]  // hard coded route name
    [ApiVersion("1.0")]
    public class ProductController : ControllerBase
    {
        private readonly IUnitOfWork _uow;

        public ProductController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> GetAll()
        {
            try
            {
                var products = (await _uow.Products.SqlQueryAsync<ProductWithPrice>(@"
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
                        ProductPrices pp ON (pp.ProductId = p.Id);"
                , []))
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
                    (k, g) => new Product
                    {
                        Id = k,
                        Name = g.First().Name,
                        Description = g.First().Description,
                        ShopFavorites = g.First().ShopFavorites,
                        CustomerFavorites = g.First().CustomerFavorites,
                        Color = g.First().Color,
                        CategoryId = g.First().CategoryId,
                        ImageUrl = g.First().ImageUrl,
                        Category = new Category { Id = g.First().CategoryId, Name = g.First().CategoryName },
                        ProductPrices = g
                        .Where(p => p.ProductPriceId is not null)
                        .Select(p => new ProductPrice { Id = p.ProductPriceId ?? 0, Size = p.Size ?? string.Empty, Price = p.Price ?? 0, ProductId = k }).ToList()
                    }).Select(p => p.ToDto()).ToList();

                return Ok(new ApiResponse { IsSuccess = true, Result = products, StatusCode = System.Net.HttpStatusCode.OK });
            }
            catch (Exception ex)
            {
                return new ObjectResult(new ApiResponse { IsSuccess = false, ErrorMessages = [ex.Message], StatusCode = System.Net.HttpStatusCode.InternalServerError }) { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }

        [HttpGet("{entityId:int}")] // indicates that this endpoint expects an entityId
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> Get(int entityId)
        {
            // lets do some simple validation
            if (entityId < 1) return BadRequest(new ApiResponse { IsSuccess = false, StatusCode = System.Net.HttpStatusCode.BadRequest });

            try
            {
                var product = (await _uow.Products.SqlQueryAsync<ProductWithPrice>(@"
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
                    (k, g) => new Product
                    {
                        Id = k,
                        Name = g.First().Name,
                        Description = g.First().Description,
                        ShopFavorites = g.First().ShopFavorites,
                        CustomerFavorites = g.First().CustomerFavorites,
                        Color = g.First().Color,
                        CategoryId = g.First().CategoryId,
                        ImageUrl = g.First().ImageUrl,
                        Category = new Category { Id = g.First().CategoryId, Name = g.First().CategoryName },
                        ProductPrices = g
                        .Where(p => p.ProductPriceId is not null)
                        .Select(p => new ProductPrice { Id = p.ProductPriceId ?? 0, Size = p.Size ?? string.Empty, Price = p.Price ?? 0, ProductId = k }).ToList()
                    }
                ).FirstOrDefault()?.ToDto();

                if (product is null) return NotFound(new ApiResponse { IsSuccess = false, StatusCode = System.Net.HttpStatusCode.NotFound });

                return Ok(new ApiResponse { IsSuccess = true, Result = product, StatusCode = System.Net.HttpStatusCode.OK });
            }
            catch (Exception ex)
            {
                return new ObjectResult(new ApiResponse { IsSuccess = false, ErrorMessages = [ex.Message], StatusCode = System.Net.HttpStatusCode.InternalServerError }) { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }
    }
}