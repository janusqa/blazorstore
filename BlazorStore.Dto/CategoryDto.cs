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
    public record CategoryDto(
        int Id,
        [Required][MaxLength(30)] string Name
    );

    public record CreateCategoryDto(
        [Required][MaxLength(30)] string Name
    );

    public record UpdateCategoryDto(
        [Required] int Id,
        [Required][MaxLength(30)] string Name
    );
}