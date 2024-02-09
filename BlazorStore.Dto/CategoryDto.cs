// DTOs. Used to access our models. 
// We do not access models directly in our applications we
// access DTOS. This is so we can scope/filter the info
// accessed. Eg. if our model for DB purposes consist of 
// a CreatedDate field we may not want to send this back
// So in our controller we gaurd the info the model returns
// Via a DTO.
using System.ComponentModel.DataAnnotations;

namespace BlazorStore.Dto
{
    public record CategoryDto
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(30)]
        public string? Name { get; set; }
    };

    public record CreateCategoryDto
    {
        [Required]
        [MaxLength(30)]
        public string? Name { get; set; }
    };

    public record UpdateCategoryDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [MaxLength(30)]
        public string? Name { get; set; }
    };
}