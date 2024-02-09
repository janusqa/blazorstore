// using Microsoft.AspNetCore.Identity;

using Microsoft.AspNetCore.Identity;

namespace BlazorStore.Models.Domain
{
    public class ApplicationUser : IdentityUser
    {
        public string? Name { get; set; }
        // public string? UserSecret { get; set; }
    }
}