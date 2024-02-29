using System.ComponentModel.DataAnnotations;

namespace BlazorStore.Dto
{
    public class CartItemDto
    {
        [Required]
        public int ProductId { get; set; }
        [Required]
        public int ProductPriceId { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a value greater than 0")]
        public int Count { get; set; }
        public ProductPriceDto ProductPrice { get; set; }
        public ProductDto Product { get; set; }

        public CartItemDto()
        {
            ProductId = 0;
            ProductPriceId = 0;
            Count = 1;
            Product = new() { Name = string.Empty, Description = string.Empty };
            ProductPrice = new() { Size = string.Empty };
        }
    }
}