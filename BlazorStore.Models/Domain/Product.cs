using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorStore.Models.Domain
{
    public class Product : BaseEntity
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public required string Name { get; set; }
        [Required]
        public required string Description { get; set; }
        public bool ShopFavorites { get; set; }
        public bool CustomerFavorites { get; set; }
        public string? Color { get; set; }
        public string? ImageUrl { get; set; }
        [Range(1, int.MaxValue)]
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }
        [NotMapped]
        public ICollection<ProductPrice>? ProductPrices { get; set; }
    }
}