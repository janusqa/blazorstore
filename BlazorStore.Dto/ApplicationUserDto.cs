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
    public record ApplicationUserDto
    {
        [Required]
        public required string Id { get; set; }
        [Required]
        [EmailAddress(ErrorMessage = "Invalid UserName.")]
        public required string UserName { get; set; }
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public required string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Name { get; set; }
    };

    public record CreateApplicationUserDto(
    [Required][EmailAddress(ErrorMessage = "Invalid email address.")][MaxLength(30)] string UserName,
    [Required][MaxLength(30)] string Password,
    [MaxLength(36)] string? Role,
    [MaxLength(30)] string? Name
);

    public record ApplicationUserLoginRequestDto(
    [Required][EmailAddress(ErrorMessage = "Invalid email address.")][MaxLength(30)] string UserName,
    [Required][MaxLength(30)] string Password
);
}