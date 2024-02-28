using System.Net;
using System.Text.Json.Serialization;

namespace BlazorStore.Dto
{
    public record ApiResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        public bool IsSuccess { get; set; }
        public List<string>? ErrorMessages { get; set; }
        public object? Result { get; set; }
    }
}