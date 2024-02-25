

using System.Security.Claims;
using BlazorStore.Dto;
using BlazorStore.Models.Domain;

namespace BlazorStore.DataAccess.Repository
{
    public interface IApplicationUserRepository : IRepository<ApplicationUser>
    {
        Task<bool> IsUinqueUser(string UserName);
        Task<TokenDto?> Login(ApplicationUserLoginRequestDto loginRequestDto);
        Task<TokenDto?> LoginLite(string userName);
        Task<TokenDto?> Register(CreateApplicationUserDto userDto);
        Task<TokenDto?> RefreshToken(ApplicationUser user);
        Task RevokeToken(ApplicationUser user);
    }
}