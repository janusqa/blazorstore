using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using BlazorStore.Common;
using BlazorStore.DataAccess.Data;
using BlazorStore.Dto;
using BlazorStore.Models.Domain;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Data.Sqlite;

namespace BlazorStore.DataAccess.Repository
{
    public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
    {

        private readonly UserManager<ApplicationUser> _um;
        private readonly IUserStore<ApplicationUser> _us;
        private readonly IUserEmailStore<ApplicationUser> _es;

        private readonly string _jwtAccessSecret;
        private readonly string _jwtRefreshSecret;

        public ApplicationUserRepository(
            ApplicationDbContext db,
            IConfiguration config,
            UserManager<ApplicationUser> um,
            IUserStore<ApplicationUser> us
        ) : base(db)
        {
            _um = um;
            _us = us;
            _es = GetEmailStore();
            _jwtAccessSecret = config.GetSection("ApiSettings:JwtAccessSecret").Value ?? "";
            _jwtRefreshSecret = config.GetSection("ApiSettings:JwtRefreshSecret").Value ?? "";
        }

        public async Task<bool> IsUinqueUser(string UserName)
        {
            var user = await _um.FindByNameAsync(UserName);

            return user is null;
        }

        public async Task<TokenDto?> Login(ApplicationUserLoginRequestDto loginRequestDto)
        {
            // Note this is for demo only so no password hassing applied
            // in realworld scenario, please use somthing like bcrypt
            // to seed and hash the password. NEVER store plaintext
            // passwords in the database. Later we will use .net Identity
            // to take care of all the heavy lifting here.

            var user = await _um.FindByNameAsync(loginRequestDto.UserName);

            if (user is null || user.UserName is null) return null;

            bool isValidCredentials = await _um.CheckPasswordAsync(user, loginRequestDto.Password);

            if (!isValidCredentials) return null;

            var xsrf = Guid.NewGuid().ToString();
            var jwtAccessToken = await CreateJwtToken(user, xsrf: xsrf);
            var jwtRefreshToken = await CreateJwtToken(user, isRefresh: true);

            return jwtAccessToken is not null && jwtRefreshToken is not null
                ? new TokenDto(
                    AccessToken: jwtAccessToken,
                    XsrfToken: xsrf,
                    RefreshToken: jwtRefreshToken)
                : null;
        }

        public async Task<TokenDto?> LoginLite(string userName)
        {
            // Note this is for demo only so no password hassing applied
            // in realworld scenario, please use somthing like bcrypt
            // to seed and hash the password. NEVER store plaintext
            // passwords in the database. Later we will use .net Identity
            // to take care of all the heavy lifting here.

            var user = await _um.FindByNameAsync(userName);

            if (user is null) return null;

            var xsrf = Guid.NewGuid().ToString();
            var jwtAccessToken = await CreateJwtToken(user, xsrf: xsrf);
            var jwtRefreshToken = await CreateJwtToken(user, isRefresh: true);

            return jwtAccessToken is not null && jwtRefreshToken is not null
                ? new TokenDto(
                    AccessToken: jwtAccessToken,
                    XsrfToken: xsrf,
                    RefreshToken: jwtRefreshToken)
                : null;
        }

        public async Task<TokenDto?> Register(CreateApplicationUserDto userDto)
        {
            if (await IsUinqueUser(userDto.UserName))
            {
                var user = CreateUser();
                await _us.SetUserNameAsync(user, userDto.UserName, CancellationToken.None);
                await _es.SetEmailAsync(user, userDto.UserName, CancellationToken.None);
                user.Name = userDto.Name;
                user.UserSecret = BcryptUtils.CreateSalt();
                var result = await _um.CreateAsync(user, userDto.Password);

                if (result.Succeeded)
                {
                    if (userDto.Role is not null && userDto.Role != "")
                    {
                        await _um.AddToRoleAsync(user, userDto.Role);
                    }
                    else
                    {
                        await _um.AddToRoleAsync(user, SD.Role_Customer);
                    }
                    return await Login(new ApplicationUserLoginRequestDto(
                        userDto.UserName,
                        userDto.Password
                    ));
                }
            }

            return null;
        }

        public async Task<TokenDto?> RefreshToken(ApplicationUser user)
        {
            if (user is null) return null;

            var xsrf = Guid.NewGuid().ToString();
            var jwtAccessToken = await CreateJwtToken(user, xsrf: xsrf);

            return jwtAccessToken is not null
                ? new TokenDto(AccessToken: jwtAccessToken, XsrfToken: xsrf)
                : null;
        }

        public async Task RevokeToken(ApplicationUser user)
        {
            if (user is null) return;

            await ExecuteSqlAsync($@"
                    UPDATE AspNetUsers SET UserSecret = @UserSecret WHERE (UserName = @UserName)
                ", [
                    new SqliteParameter("UserName", user.UserName),
                        new SqliteParameter("UserSecret", BcryptUtils.CreateSalt())
            ]);

            // Response.Cookies.Delete(SD.JwtRrefreshTokenCookie);
            // Response.Cookies.Delete(SD.ApiXsrfCookie);
        }

        private async Task<string?> CreateJwtToken(ApplicationUser user, string? xsrf = null, bool isRefresh = false)
        {
            if (user.UserName is null || user.UserSecret is null) return null;

            var roles = await _um.GetRolesAsync(user);
            if (roles.Count < 1) return null;

            var jwtTokenHandler = new JsonWebTokenHandler();
            var serverSecret = isRefresh ? _jwtRefreshSecret : _jwtAccessSecret;
            var userSecret = user.UserSecret;
            var jwtKey = Encoding.ASCII.GetBytes($"{serverSecret}{userSecret}");
            // var jwtKey = Encoding.ASCII.GetBytes(serverSecret);

            var jwtTokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[] {
                    new Claim(ClaimTypes.Name, user.UserName.ToString()),
                    new Claim(ClaimTypes.Role, roles.First())
                }),
                Expires = isRefresh ? DateTime.UtcNow.AddMinutes(SD.JwtRefreshTokenExpiry) : DateTime.UtcNow.AddMinutes(SD.JwtAccessTokenExpiry),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(jwtKey), SecurityAlgorithms.HmacSha256Signature)
            };

            // add xsrf if an access token is being generated
            if (!isRefresh && xsrf is not null) jwtTokenDescriptor.Subject.AddClaim(new Claim("xsrf", xsrf));

            var jwtToken = jwtTokenHandler.CreateToken(jwtTokenDescriptor);

            return jwtToken;
        }

        private ApplicationUser CreateUser()
        {
            try
            {
                // return Activator.CreateInstance<ApplicationUser>();
                return Activator.CreateInstance<ApplicationUser>(); // updated so we can customize info we collected on a user 
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                    $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!_um.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<ApplicationUser>)_us;
        }

    }
}