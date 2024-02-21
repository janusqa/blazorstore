using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BlazorStore.Common;
using BlazorStore.DataAccess.UnitOfWork;
using BlazorStore.Dto;
using BlazorStore.Models.Extensions;


namespace RealEstate.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/auth")]
    [ApiVersionNeutral]
    public class AuthController : ControllerBase
    {
        private readonly IUnitOfWork _uow;

        public AuthController(IUnitOfWork uow)
        {
            _uow = uow;
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

                var result = await _uow.ApplicationUsers.Refresh(User);
                if (result?.AccessToken is not null && result?.XsrfToken is not null)
                {
                    var accessTokenDto = result.ToAccessTokenDto();

                    // xsrf
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

                    // usually we should just return access token in json, refresh token in a httpOnly cookie and xsrf in a regular cookie
                    // return Ok(new ApiResponse { IsSuccess = true, Result = accessTokenDto, StatusCode = System.Net.HttpStatusCode.OK });

                    // BUT our front-end is an .net core mvc app so we need to do things differently.  We must
                    // return access, refresh and xsrf all in the json and let the mvc app set and clear cookies for browser
                    return Ok(new ApiResponse { IsSuccess = true, Result = accessTokenDto, StatusCode = System.Net.HttpStatusCode.OK });
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