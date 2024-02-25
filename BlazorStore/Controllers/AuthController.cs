using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BlazorStore.Common;
using BlazorStore.DataAccess.UnitOfWork;
using BlazorStore.Dto;
using BlazorStore.Models.Extensions;
using Microsoft.AspNetCore.Identity;
using BlazorStore.Models.Domain;


namespace RealEstate.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/auth")]
    [ApiVersionNeutral]
    public class AuthController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        private readonly UserManager<ApplicationUser> _um;

        public AuthController(IUnitOfWork uow, UserManager<ApplicationUser> um)
        {
            _uow = uow;
            _um = um;
        }

        [Authorize]
        [HttpGet("refresh")]
        [ProducesResponseType(StatusCodes.Status200OK)] // these are the types of responses this action can produce
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // we use them so swagger does not show responses as undocumented
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApiResponse>> Refresh()
        {
            try
            {
                if (User.Identity?.Name is null)
                    return new ObjectResult(new ApiResponse { IsSuccess = false, ErrorMessages = ["Invalid credentials"], StatusCode = System.Net.HttpStatusCode.Unauthorized }) { StatusCode = StatusCodes.Status401Unauthorized };

                var user = await _um.FindByNameAsync(User.Identity.Name);
                var result = user is not null ? await _uow.ApplicationUsers.RefreshToken(user) : null;
                if (result?.AccessToken is not null && result?.XsrfToken is not null)
                {
                    // xsrf
                    Response.Cookies.Delete(SD.ApiXsrfCookie);
                    Response.Cookies.Append(
                        SD.ApiXsrfCookie,
                        result.XsrfToken,
                        new CookieOptions()
                        {
                            HttpOnly = false,
                            Secure = true,
                            SameSite = SameSiteMode.Lax,
                            Path = "/",
                            MaxAge = DateTime.UtcNow.AddMinutes(SD.JwtAccessTokenExpiry) - DateTime.UtcNow
                        });

                    return Ok(new ApiResponse { IsSuccess = true, Result = result, StatusCode = System.Net.HttpStatusCode.OK });
                }
                return new ObjectResult(new ApiResponse { IsSuccess = false, ErrorMessages = ["Invalid credentials"], StatusCode = System.Net.HttpStatusCode.Unauthorized }) { StatusCode = StatusCodes.Status401Unauthorized };
            }
            catch (Exception ex)
            {
                return new ObjectResult(new ApiResponse { IsSuccess = false, ErrorMessages = [ex.Message], StatusCode = System.Net.HttpStatusCode.InternalServerError }) { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }
    }
}