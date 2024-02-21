

using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using BlazorStore.Common;
using BlazorStore.DataAccess.UnitOfWork;
using BlazorStore.Service.IService;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace BlazorStore.Service
{
    public partial class CustomJwtBearerHandler : JwtBearerHandler, ICustomJwtBearerHandler
    {
        private readonly IUnitOfWork _uow;
        private readonly IConfiguration _config;

        public CustomJwtBearerHandler(
            IOptionsMonitor<JwtBearerOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            IConfiguration config,
            IUnitOfWork uow
        )
            : base(options, logger, encoder)
        {
            _uow = uow;
            _config = config;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var refTokenReqRegex = RefTokenReqRegex();
            var accTokenReqRegex = AccTokenReqRegex();
            var authHeaderRegex = AuthHeaderRegex();

            string? clientJwtToken = null;
            bool isRefresh = false;

            string? xsrfClaim = null;
            string? userNameClaim = null;

            var url = Request.GetEncodedUrl();

            if (refTokenReqRegex.Matches(url).Count > 0)
            {
                // if we are accessing /users/refresh
                // we need to validate the JWT Refresh Token as it
                // will be used to fetch the new JWT Access Token

                isRefresh = true;

                if (!Request.Cookies.TryGetValue(SD.JwtRrefreshTokenCookie, out clientJwtToken))
                {
                    // Unauthorized
                    return AuthenticateResult.NoResult();
                }

            }
            else
            {
                // We are accessing a regular protected route
                // we need to vailidate the JWT Access Token which
                // will be used to authorize the request

                isRefresh = false;

                if (Request.Headers.TryGetValue("authorization", out StringValues authorizationValues))
                {
                    string? authorization = authorizationValues.FirstOrDefault();
                    if (authorization is not null && authHeaderRegex.Matches(authorization).Count > 0)
                    {
                        clientJwtToken = authorization.Replace("Bearer ", "");
                    }
                    else
                    {
                        // unauthorized
                        return AuthenticateResult.NoResult();
                    }
                }
                else
                {
                    // unauthorized
                    return AuthenticateResult.NoResult();
                }
            }

            // Check the validity of the token and return success or a failure
            // Note MUST set "MapInboundClaims" to true when creating JsonWebTokenHandler
            // so that claims are populated properly and that authorization will work
            // properly in the controllers. I.E the User object will be populated properly
            var jwtTokenHandler = new JsonWebTokenHandler { MapInboundClaims = true };

            try
            {
                var jwtToken = jwtTokenHandler.ReadJsonWebToken(clientJwtToken);
                userNameClaim = jwtToken.Claims.First(c => c.Type == "unique_name").Value;
                if (!isRefresh) xsrfClaim = jwtToken.Claims.First(c => c.Type == "xsrf").Value;
            }
            catch (Exception ex)
            {
                // Unauthorized
                return AuthenticateResult.Fail(ex.Message);
            }

            if (userNameClaim is not null)
            {
                string? userSecret = null;
                try
                {
                    userSecret = (await _uow.ApplicationUsers.SqlQueryAsync<string>($@"
                        SELECT UserSecret FROM dbo.AspNetUsers WHERE UserName = @UserName
                    ", [new SqliteParameter("UserName", userNameClaim)])).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    return AuthenticateResult.Fail(ex.Message);
                }

                var serverKey = isRefresh
                    ? _config.GetValue<string>("ApiSettings:JwtRefreshSecret")
                    : _config.GetValue<string>("ApiSettings:JwtAccessSecret");

                if (userSecret is not null && serverKey is not null)
                {
                    // validate the token here
                    var key = Encoding.ASCII.GetBytes($"{serverKey}{userSecret}");
                    try
                    {
                        var validationResult = await jwtTokenHandler.ValidateTokenAsync(
                            clientJwtToken,
                            new TokenValidationParameters
                            {
                                IssuerSigningKey = new SymmetricSecurityKey(key),
                                ValidateIssuerSigningKey = true,
                                ValidateLifetime = true,
                                ValidateIssuer = false,
                                ValidateAudience = false,
                                ClockSkew = TimeSpan.Zero // IMPORTANT: this must be set to expire token at exact expiry
                            });

                        if (validationResult.IsValid)
                        {
                            // check xsrf if we have a valid jwttoken
                            if (refTokenReqRegex.Matches(url).Count == 0 && accTokenReqRegex.Matches(url).Count == 0)
                            {
                                if (!Request.Headers.TryGetValue("x-xsrf-token", out StringValues xsrfnValues))
                                {
                                    return AuthenticateResult.NoResult();
                                }

                                string? xsrf = xsrfnValues.FirstOrDefault();
                                // if we have a valid token we should also have a valid xsrf
                                // fail authentication process if we do not
                                if (xsrf is not null && xsrf != xsrfClaim)
                                {
                                    // unauthorized
                                    return AuthenticateResult.NoResult();
                                }

                            }
                            // Success
                            var claimsPrincipal = new ClaimsPrincipal(validationResult.ClaimsIdentity);
                            var validatedToken = validationResult.SecurityToken;
                            // var tokenValidatedContext = new TokenValidatedContext(Context, Scheme, Options)
                            // {
                            //     Principal = claimsPrincipal,
                            //     SecurityToken = validatedToken,
                            // };
                            // tokenValidatedContext.Properties.ExpiresUtc = GetSafeDateTime(validatedToken.ValidTo);
                            // tokenValidatedContext.Properties.IssuedUtc = GetSafeDateTime(validatedToken.ValidFrom);

                            // await Events.TokenValidated(tokenValidatedContext);
                            // if (tokenValidatedContext.Result is not null)
                            // {
                            //     return tokenValidatedContext.Result;
                            // }

                            var ticket = new AuthenticationTicket(claimsPrincipal, Scheme.Name);

                            if (Options.SaveToken && !isRefresh)
                            {
                                ticket.Properties.StoreTokens(
                                [
                                    new AuthenticationToken { Name = "access_token", Value = clientJwtToken }
                                ]);
                            }

                            return AuthenticateResult.Success(ticket);
                        }
                        else
                        {
                            return AuthenticateResult.Fail(validationResult.Exception);
                        }
                    }
                    catch (Exception ex)
                    {
                        return AuthenticateResult.Fail(ex.Message);
                        // check for invalid signature
                        // check for expired jwt
                        // if isRefresh then unauthorized
                        // else forbidden since its only an access token (client needs to send refresh request)
                    }
                }
                else
                {
                    // Unauthorized
                    return AuthenticateResult.NoResult();
                }
            }
            else
            {
                // Unauthorized
                return AuthenticateResult.NoResult();
            }
        }

        // private static DateTime? GetSafeDateTime(DateTime dateTime)
        // {
        //     // Assigning DateTime.MinValue or default(DateTime) to a DateTimeOffset when in a UTC+X timezone will throw
        //     // Since we don't really care about DateTime.MinValue in this case let's just set the field to null
        //     if (dateTime == DateTime.MinValue) return null;
        //     return dateTime;
        // }

        [GeneratedRegex(@"/users/refresh$", RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-US")]
        private static partial Regex RefTokenReqRegex();
        [GeneratedRegex(@"/users/login$", RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-US")]
        private static partial Regex AccTokenReqRegex();
        [GeneratedRegex(@"^Bearer ", RegexOptions.Compiled, "en-US")]
        private static partial Regex AuthHeaderRegex();
    }
}