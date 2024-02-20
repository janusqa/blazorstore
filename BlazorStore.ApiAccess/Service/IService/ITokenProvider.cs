using BlazorStore.Dto;

namespace BlazorStore.ApiAccess.Service.IService
{
    public interface ITokenProvider
    {
        void SetToken(TokenDto TokenDto);
        TokenDto? GetToken();
        void ClearToken();
    }
}