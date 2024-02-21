
using BlazorStore.Dto;
using BlazorStore.Models.Domain;
using BlazorStore.Models.Helper;

namespace BlazorStore.Models.Extensions
{
    public static class DtoMapper
    {
        // NB this is an "extension method" for model
        // the "this" keyword allows this to appear as a member method
        // of the model. It allows us to call it like myModel.ToDto
        // which looks much better than DomainExtension.ToDto(myModel).
        // aka it is syntactic sugar over the static method.
        public static CategoryDto ToDto(this Category category)
        {
            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name
            };
        }

        public static ProductDto ToDto(this Product product)
        {
            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                ShopFavorites = product.ShopFavorites,
                CustomerFavorites = product.CustomerFavorites,
                Color = product.Color,
                ImageUrl = product.ImageUrl,
                CategoryId = product.CategoryId,
                CategoryDto = product.Category?.ToDto(),
                ProductPrices = product.ProductPrices?.Select(pp => pp.ToDto()).ToList()
            };
        }

        public static ProductPriceDto ToDto(this ProductPrice productPrice)
        {
            return new ProductPriceDto
            {
                Id = productPrice.Id,
                ProductId = productPrice.ProductId,
                Size = productPrice.Size,
                Price = productPrice.Price
            };
        }
        public static AccessTokenDto ToAccessTokenDto(this TokenDto tokenDto)
        {
            return new AccessTokenDto(
                AccessToken: tokenDto.AccessToken
            );
        }
    }
}