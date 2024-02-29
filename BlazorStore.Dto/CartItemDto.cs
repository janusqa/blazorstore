using System.ComponentModel.DataAnnotations;

namespace BlazorStore.Dto
{
    public class CartItemDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a value greater than 0")]
        public int Count { get; set; }
        [Required]
        public int ProductPriceId { get; set; }
        public ProductPriceDto ProductPrice { get; set; }

        public CartItemDto()
        {
            Count = 1;
            ProductPriceId = 0;
            ProductPrice = new() { Size = string.Empty };
        }
    }
}