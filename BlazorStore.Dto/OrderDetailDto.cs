using System.ComponentModel.DataAnnotations;

namespace BlazorStore.Dto
{
    public record OrderDetailDto
    {
        public int Id { get; set; }
        [Required]
        public int OrderHeaderId { get; set; }
        [Required]
        public int ProductId { get; set; }
        public ProductDto? Product { get; set; }
        [Required]
        public double Price { get; set; }
        [Required]
        public required string Size { get; set; }
        [Required]
        public int Count { get; set; }
        [Required]
        public required string ProductName { get; set; }
    }
}