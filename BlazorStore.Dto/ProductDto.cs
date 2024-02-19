// DTOs. Used to access our models. 
// We do not access models directly in our applications we
// access DTOS. This is so we can scope/filter the info
// accessed. Eg. if our model for DB purposes consist of 
// a CreatedDate field we may not want to send this back
// So in our controller we gaurd the info the model returns
// Via a DTO.
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace BlazorStore.Dto
{
    public record ProductDto
    {
        public int Id { get; set; }
        [Required]
        public required string Name { get; set; }
        [Required]
        public required string Description { get; set; }
        [DisplayName("Shop Favorites")]
        public bool ShopFavorites { get; set; }
        [DisplayName("Customer Favorites")]
        public bool CustomerFavorites { get; set; }
        public string? Color { get; set; }
        public string? ImageUrl { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a category")]
        public int CategoryId { get; set; }
        public CategoryDto? CategoryDto { get; set; }
        public IFormFile? Image { get; set; }
        // public IFormFileCollection? Images { get; set; } // for multi file uploads
        public ICollection<ProductPriceDto>? ProductPrices { get; set; }
    };
}