using System.ComponentModel.DataAnnotations;

namespace BlazorStore.Dto
{
    public record ProductPriceDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int ProductId { get; set; }
        [Required]
        public required string Size { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Price must be greater than 1")]
        public double Price { get; set; }
    }
}