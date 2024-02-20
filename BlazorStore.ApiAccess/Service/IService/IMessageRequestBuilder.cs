using BlazorStore.Dto;

namespace BlazorStore.ApiAccess.Service.IService
{
    public interface IMessageRequestBuilder
    {
        HttpRequestMessage Build(ApiRequest apiRequest);
    }
}