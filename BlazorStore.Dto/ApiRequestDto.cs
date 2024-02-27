using BlazorStore.Common;

namespace BlazorStore.Dto
{
    public record ApiRequest
    {
        public SD.ApiMethod ApiMethod { get; init; } = SD.ApiMethod.GET;
        public required string Url { get; init; }
        public object? Data { get; init; }
        public SD.ContentType ContentType { get; init; } = SD.ContentType.Json;
        public virtual bool WithCredentials { get; init; } = false;
        public string? AccessToken { get; init; } = null;
    }
}