using System.ComponentModel.DataAnnotations;

namespace BlazorStore.Dto
{
    public record ProductPriceDto
    {
        public int Id { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid Product")]
        public int ProductId { get; set; }
        [Required]
        public required string Size { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Price must be greater than 1")]
        public double Price { get; set; }
    }
}