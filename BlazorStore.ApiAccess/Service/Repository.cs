using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using BlazorStore.ApiAccess.Exceptions;
using BlazorStore.Common;
using BlazorStore.Dto;

namespace BlazorStore.ApiAccess.Service
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly IHttpClientFactory _httpClient;
        private readonly IHttpRequestMessageBuilder _messageBuilder;
        private readonly ICookieService _cookieService;
        private readonly string _url;

        public Repository(
            IHttpClientFactory httpClient,
            IHttpRequestMessageBuilder messageBuilder,
            ICookieService cookieService,
            string url
        )
        {
            _httpClient = httpClient;
            _cookieService = cookieService;
            _messageBuilder = messageBuilder;
            _url = url;
        }

        public async Task<T?> AddAsync(ApiRequest request)
        {
            return await RequestAsync(request with { ApiMethod = SD.ApiMethod.POST, Url = _url });
        }

        public async Task<T?> UpdateAsync(int entityId, ApiRequest request)
        {
            return await RequestAsync(request with { ApiMethod = SD.ApiMethod.PUT, Url = $"{_url}/{entityId}" });
        }

        public async Task<T?> RemoveAsync(int entityId, ApiRequest request)
        {
            return await RequestAsync(request with { ApiMethod = SD.ApiMethod.DELETE, Url = $"{_url}/{entityId}" });
        }

        public async Task<T?> GetAllAsync(ApiRequest request)
        {
            return await RequestAsync(request with { ApiMethod = SD.ApiMethod.GET, Url = _url });
        }

        public async Task<T?> GetAsync(int entityId, ApiRequest request)
        {
            return await RequestAsync(request with { ApiMethod = SD.ApiMethod.GET, Url = $"{_url}/{entityId}" });
        }

        protected async Task<T?> RequestAsync(ApiRequest apiRequest)
        {
            var client = _httpClient.CreateClient("BlazorStore");

            try
            {
                var messageFactory = () => _messageBuilder.Build(apiRequest, client.BaseAddress!.ToString());

                if (apiRequest.WithCredentials)
                {
                    var accessToken = apiRequest.AccessToken;
                    var xsrfToken = await _cookieService.GetCookie(SD.ApiXsrfCookie);
                    if (accessToken is not null && string.IsNullOrEmpty(xsrfToken))
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
                    var jwtAuthStatus = httpResponseMessage.Headers.WwwAuthenticate.ToString();

                    if (jwtAuthStatus.Contains("token expired"))
                    {
                        var newAccessToken = await RefreshTokenAsync(client);
                        var newXsrfToken = await _cookieService.GetCookie(SD.ApiXsrfCookie);
                        if (newAccessToken is not null && string.IsNullOrEmpty(newXsrfToken))
                        {
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

                var json = await httpResponseMessage.Content.ReadAsStringAsync();
                if (json is not null && !string.IsNullOrEmpty(json))
                {
                    // Success (response could still be a bad request etc. though)
                    // this just indicates a valid response that is not a 401 Unauthorized
                    // Note we conrcretely deserialize to an ApIResponse. Seems to negate
                    // the benefits of having this class as a generic but se le vie.
                    var apiResponse = JsonSerializer.Deserialize<ApiResponse>(json);

                    if (!httpResponseMessage.IsSuccessStatusCode && apiResponse is not null)
                    {
                        apiResponse.ErrorMessages = httpResponseMessage.StatusCode switch
                        {
                            HttpStatusCode.NotFound => [.. (apiResponse.ErrorMessages ?? []), "Not Found"],
                            HttpStatusCode.Forbidden => [.. (apiResponse.ErrorMessages ?? []), "Access Denied"],
                            HttpStatusCode.Unauthorized => [.. (apiResponse.ErrorMessages ?? []), "Unauthorized"],
                            HttpStatusCode.InternalServerError => [.. (apiResponse.ErrorMessages ?? []), "Internal Server Error"],
                            _ => [.. (apiResponse.ErrorMessages ?? []), "Oops, something went wrong. Please try again later"]
                        };
                    }

                    if (apiResponse is not null)
                    {
                        apiResponse.StatusCode = httpResponseMessage.StatusCode;
                        var response = JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(apiResponse));
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
            catch (AuthenticationFailureException)
            {
                await SignOutAsync(client);
                throw;
            }
            catch (Exception ex)
            {
                var errorResponse = new ApiResponse
                {
                    ErrorMessages = ex.InnerException?.Message is not null ? [ex.Message, ex.InnerException.Message] : [ex.Message],
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.InternalServerError
                };
                var apiResponse = JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(errorResponse));
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
            var json = await httpResponseMessage.Content.ReadAsStringAsync();
            if (json is not null && !string.IsNullOrEmpty(json))
            {
                var apiResponse = JsonSerializer.Deserialize<ApiResponse>(json);
                var tokenDto = JsonSerializer.Deserialize<TokenDto>(JsonSerializer.Serialize(apiResponse?.Result));
                if (tokenDto?.AccessToken is not null && tokenDto.XsrfToken is not null)
                {
                    return tokenDto.AccessToken;
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