using Asp.Versioning;
using BlazorStore.DataAccess.UnitOfWork;
using BlazorStore.Dto;
using BlazorStore.Models.Domain;
using BlazorStore.Models.Extensions;
using BlazorStore.Models.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;

namespace BlazorStore.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/users")]  // hard coded route name
    [ApiVersion("1.0")]
    public class UserController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        private readonly UserManager<ApplicationUser> _um;

        public UserController(IUnitOfWork uow, UserManager<ApplicationUser> um)
        {
            _uow = uow;
            _um = um;
        }

        // [HttpGet("userinfo/{entityId:int}", Name = "Userinfo")]
        [HttpGet("userinfo", Name = "UserInfo")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> UserInfo()
        {
            try
            {
                var userId = User.Identity?.Name;
                if (userId is not null)
                {
                    var user = (await _um.FindByNameAsync(userId))?.ToDto();
                    if (user is not null)
                    {
                        return Ok(new ApiResponse { IsSuccess = true, Result = user, StatusCode = System.Net.HttpStatusCode.OK });
                    }
                    return NotFound(new ApiResponse { IsSuccess = false, ErrorMessages = ["User not found"], StatusCode = System.Net.HttpStatusCode.NotFound });
                }

                return BadRequest(new ApiResponse { IsSuccess = false, ErrorMessages = ["User claim not present"], StatusCode = System.Net.HttpStatusCode.BadRequest });
            }
            catch (Exception ex)
            {
                return new ObjectResult(new ApiResponse { IsSuccess = false, ErrorMessages = [ex.Message], StatusCode = System.Net.HttpStatusCode.InternalServerError }) { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }
    }
}