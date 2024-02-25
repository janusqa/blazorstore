using System.Net;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using BlazorStore.ApiAccess.Exceptions;
using BlazorStore.Common;
using BlazorStore.Dto;
using Microsoft.IdentityModel.JsonWebTokens;

namespace BlazorStore.ApiAccess.Service
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly IHttpClientFactory _httpClient;
        private readonly ICookieService _cookieService;
        private readonly IHttpRequestMessageBuilder _messageBuilder;
        private readonly string _url;

        public Repository(
            IHttpClientFactory httpClient,
            ICookieService cookieService,
            IHttpRequestMessageBuilder messageBuilder,
            string url
        )
        {
            _httpClient = httpClient;
            _cookieService = cookieService;
            _messageBuilder = messageBuilder;
            _url = url;
        }

        public async Task<T?> AddAsync<U>(U dto, bool withBearer, SD.ContentType contentType)
        {
            return await RequestAsync(
                new ApiRequest
                {
                    ApiMethod = SD.ApiMethod.POST,
                    Data = dto,
                    Url = _url,
                    ContentType = contentType
                }
            );
        }

        public async Task<T?> UpdateAsync<U>(int entityId, U dto, bool withBearer, SD.ContentType contentType)
        {
            return await RequestAsync(
                new ApiRequest
                {
                    ApiMethod = SD.ApiMethod.PUT,
                    Data = dto,
                    Url = $"{_url}/{entityId}",
                    ContentType = contentType
                }
            );
        }

        public async Task<T?> RemoveAsync(int entityId, bool withBearer)
        {
            return await RequestAsync(
                new ApiRequest
                {
                    ApiMethod = SD.ApiMethod.DELETE,
                    Url = $"{_url}/{entityId}"
                }
            );
        }

        public async Task<T?> GetAllAsync(bool withBearer)
        {
            return await RequestAsync(
                new ApiRequest
                {
                    ApiMethod = SD.ApiMethod.GET,
                    Url = _url
                }
            );
        }

        public async Task<T?> GetAsync(int entityId, bool withBearer)
        {
            return await RequestAsync(
                new ApiRequest
                {
                    ApiMethod = SD.ApiMethod.GET,
                    Url = $"{_url}/{entityId}"
                }
            );
        }

        protected async Task<T?> RequestAsync(ApiRequest apiRequest, bool withBearer = true)
        {
            try
            {
                var client = _httpClient.CreateClient("BlazorStoreApi");

                var messageFactory = () => _messageBuilder.Build(apiRequest);

                if (withBearer)
                {
                    var accessToken = "GetAccessToken()";
                    var xsrfToken = await _cookieService.GetCookie(SD.ApiXsrfCookie);
                    if (accessToken is not null && xsrfToken is not null)
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                        client.DefaultRequestHeaders.Add("X-XSRF-TOKEN", xsrfToken);
                    }
                }

                HttpResponseMessage httpResponseMessage = await client.SendAsync(messageFactory());
                // 1. If statuscode is authorized continure processing
                // 2. If statuscode is unauthorized and access token is expired. Refresh access token (using refresh token).

                if (httpResponseMessage.StatusCode == HttpStatusCode.Unauthorized)
                {
                    var accessToken = "GetAccessToken()";

                    var jwtAuthStatus = httpResponseMessage.Headers.WwwAuthenticate.ToString();

                    if (!string.IsNullOrEmpty(accessToken) && jwtAuthStatus.Contains("token expired"))
                    {
                        /*
                            FLUXOR
                           ******** TODO: SET ACCESSTOKEN TO NULL HERE ********
                        */
                        var newAccessToken = await RefreshTokenAsync(client);
                        var newXsrfToken = await _cookieService.GetCookie(SD.ApiXsrfCookie);
                        if (newAccessToken is not null)
                        {
                            /*
                                FLUXOR
                                ******** TODO: SET ACCESSTOKEN HERE ********
                            */

                            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", newAccessToken);
                            client.DefaultRequestHeaders.Remove("X-XSRF-TOKEN");
                            client.DefaultRequestHeaders.Add("X-XSRF-TOKEN", newXsrfToken);

                            httpResponseMessage = await client.SendAsync(messageFactory());
                        }
                        else
                        {
                            throw new AuthenticationFailureException("You are not authorized to perform this action. Please login.");
                        }
                    }
                    else
                    {
                        throw new AuthenticationFailureException("You are not authorized to perform this action. Please login.");
                    }
                }

                var jsonData = await httpResponseMessage.Content.ReadAsStringAsync();
                if (jsonData is not null && !string.IsNullOrEmpty(jsonData))
                {
                    // Success (response could still be a bad request etc. though)
                    // this just indicates a valid response that is not a 401 Unauthorized
                    // Note we conrcretely deserialize to an ApIResponse. Seems to negate
                    // the benefits of having this class as a generic but se le vie.
                    var apiResponse = JsonSerializer.Deserialize<ApiResponse>(jsonData);

                    if (!httpResponseMessage.IsSuccessStatusCode && apiResponse is not null)
                    {
                        apiResponse.ErrorMessages = httpResponseMessage.StatusCode switch
                        {
                            HttpStatusCode.NotFound => [.. (apiResponse.ErrorMessages ?? []), "Not Found"],
                            HttpStatusCode.Forbidden => [.. (apiResponse.ErrorMessages ?? []), "Access Denied"],
                            HttpStatusCode.Unauthorized => [.. (apiResponse.ErrorMessages ?? []), "Unauthorized"],
                            HttpStatusCode.InternalServerError => [.. (apiResponse.ErrorMessages ?? []), "Internal Server Error"],
                            _ => ["Oops, something went wrong. Please try again later"]
                        };
                    }

                    if (apiResponse is not null)
                    {
                        apiResponse.StatusCode = httpResponseMessage.StatusCode;
                        jsonData = JsonSerializer.Serialize(apiResponse);
                        var response = JsonSerializer.Deserialize<T>(jsonData);
                        return response;
                    }
                    else
                    {
                        throw new Exception("Oops, something went wrong. Please try again later");
                    }
                }
                else
                {
                    throw new Exception("Oops, something went wrong. Please try again later");
                }
            }
            catch (AuthenticationException)
            {
                var client = _httpClient.CreateClient("BlazorStoreApi");

                await SignOutAsync(client);
                /*
                    FLUXOUR
                    ******** TODO: SET ACCESSTOKEN NULL HERE ********
                */
                throw;
            }
            catch (Exception ex)
            {
                var errorResponse = new ApiResponse
                {
                    ErrorMessages = [ex.Message],
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.InternalServerError
                };
                var jsonError = JsonSerializer.Serialize(errorResponse);
                var apiResponse = JsonSerializer.Deserialize<T>(jsonError);
                return apiResponse;
            }
        }

        private async Task<string?> RefreshTokenAsync(HttpClient client)
        {
            HttpRequestMessage message = new HttpRequestMessage();
            message.Headers.Add("Accept", "application/json");
            message.RequestUri = new Uri($"/api/{SD.ApiVersion}/auth/refresh");
            message.Method = HttpMethod.Get;

            HttpResponseMessage httpResponseMessage = await client.SendAsync(message);
            var jsonData = await httpResponseMessage.Content.ReadAsStringAsync();
            if (jsonData is not null && !string.IsNullOrEmpty(jsonData))
            {
                var response = JsonSerializer.Deserialize<ApiResponse>(jsonData);
                jsonData = JsonSerializer.Serialize(response?.Result);
                if (response is not null && response.IsSuccess && !string.IsNullOrEmpty(jsonData))
                {
                    var tokenDto = JsonSerializer.Deserialize<TokenDto>(jsonData);
                    if (tokenDto?.AccessToken is not null && tokenDto.XsrfToken is not null)
                    {
                        return tokenDto.AccessToken;
                    }
                }
            }

            return null;
        }

        private async Task SignOutAsync(HttpClient client)
        {
            HttpRequestMessage message = new HttpRequestMessage();
            message.Headers.Add("Accept", "*/*");
            message.Method = HttpMethod.Post;
            message.RequestUri = new Uri($"/Account/Logout");
            var content = new MultipartFormDataContent { { new StringContent("/"), "returnUrl" } };
            message.Content = content;

            await client.SendAsync(message);
        }
    }
}