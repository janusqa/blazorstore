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
    public record ApplicationUserDto(
        string Id,
        [Required][EmailAddress(ErrorMessage = "Invalid email address.")][MaxLength(30)] string UserName,
        [Required][MaxLength(36)] string Role,
        [MaxLength(30)] string? Name
    );

    public record CreateApplicationUserDto(
        [Required][EmailAddress(ErrorMessage = "Invalid email address.")][MaxLength(30)] string UserName,
        [Required][MaxLength(30)] string Password,
        [MaxLength(36)] string? Role,
        [MaxLength(30)] string? Name
    );

    public record UpdateApplicationUserDto(
        string Id,
        [Required][EmailAddress(ErrorMessage = "Invalid email address.")][MaxLength(30)] string UserName,
        [Required][MaxLength(30)] string Password,
        [Required][MaxLength(36)] string Role,
        [MaxLength(30)] string? Name
    );

    public record ApplicationUserLoginRequestDto(
        [Required][EmailAddress(ErrorMessage = "Invalid email address.")][MaxLength(30)] string UserName,
        [Required][MaxLength(30)] string Password
    );
}