using System.Text.Json;
using BlazorStore.ApiAccess.Service;
using BlazorStore.Dto;
using Fluxor;

namespace BlazorStore.Client.Store.Auth
{
    // ********************
    // State
    // ********************
    [FeatureState]
    public record AuthState
    {
        public string? AccessToken { get; init; } = null;
    }

    // ********************
    // Reducers
    // ********************
    public static class Reducers
    {
        [ReducerMethod]
        public static AuthState AccessTokenRetrievedReducer(AuthState state, AccessTokenRetrieved action)
        {
            return state with { AccessToken = action.AccessToken };
        }
    }

    // ********************
    // Effects
    // ********************
    public class Effects
    {
        private readonly IApiService _api;

        public Effects(IApiService api)
        {
            _api = api;
        }

        [EffectMethod(typeof(AccessTokenFetched))]
        public async Task HandleAccessTokenFetched(IDispatcher dispatcher)
        {
            var response = await _api.Auth.RefreshAsync();
            if (response is not null && response.IsSuccess)
            {
                var tokenDto = JsonSerializer.Deserialize<TokenDto>(JsonSerializer.Serialize(response.Result));
                dispatcher.Dispatch(new AccessTokenRetrieved(tokenDto?.AccessToken));
            }
            else
            {
                dispatcher.Dispatch(new AccessTokenRetrieved(null));
            }
        }
    }

    // ********************
    // Actions
    // ********************
    public record AccessTokenFetched();
    public record AccessTokenRetrieved(string? AccessToken);

}