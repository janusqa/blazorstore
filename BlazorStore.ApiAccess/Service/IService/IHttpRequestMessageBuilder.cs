using BlazorStore.Dto;

namespace BlazorStore.ApiAccess.Service
{
    public interface IHttpRequestMessageBuilder
    {
        HttpRequestMessage Build(ApiRequest apiRequest, string baseAddress);
    }
}