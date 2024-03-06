
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

        public static OrderHeaderDto ToDto(this OrderHeader OrderHeader)
        {
            return new OrderHeaderDto
            {
                Id = OrderHeader.Id,
                UserId = OrderHeader.UserId,
                Email = OrderHeader.Email,
                OrderTotal = OrderHeader.OrderTotal,
                OrderDate = OrderHeader.OrderDate,
                ShippingDate = OrderHeader.ShippingDate,
                Status = OrderHeader.Status,
                SessionId = OrderHeader.SessionId,
                PaymentIntentId = OrderHeader.PaymentIntentId,
                Name = OrderHeader.Name,
                PhoneNumber = OrderHeader.PhoneNumber,
                StreetAddress = OrderHeader.StreetAddress,
                State = OrderHeader.State,
                City = OrderHeader.City,
                PostalCode = OrderHeader.PostalCode
            };
        }

        public static OrderDetailDto ToDto(this OrderDetail OrderDetail)
        {
            return new OrderDetailDto
            {
                Id = OrderDetail.Id,
                OrderHeaderId = OrderDetail.OrderHeaderId,
                ProductId = OrderDetail.ProductId,
                Product = OrderDetail.Product?.ToDto(),
                Count = OrderDetail.Count,
                Price = OrderDetail.Price,
                Size = OrderDetail.Size,
                ProductName = OrderDetail.ProductName
            };
        }

        public static ApplicationUserDto ToDto(this ApplicationUser ApplicationUser)
        {
            return new ApplicationUserDto
            {
                Id = ApplicationUser.Id,
                UserName = ApplicationUser.UserName ?? string.Empty,
                Email = ApplicationUser.Email ?? string.Empty,
                PhoneNumber = ApplicationUser.PhoneNumber,
                Name = ApplicationUser.Name,
            };
        }
    }
}