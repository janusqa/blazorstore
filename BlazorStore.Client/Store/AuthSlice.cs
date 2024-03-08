using System.Text.Json;
using BlazorStore.ApiAccess.Service;
using BlazorStore.Dto;
using Fluxor;

namespace BlazorStore.Client.AppState.Auth
{
    // ********************
    // State
    // ********************
    [FeatureState]
    public record AuthState
    {
        public string? AccessToken { get; init; } = null;
        public ApplicationUserDto? UserInfo { get; init; } = null;
        public bool IsLoading { get; init; } = false;
    }

    // ********************
    // Reducers
    // ********************
    public static class Reducers
    {
        [ReducerMethod]
        public static AuthState AccessTokenRetrievedReducer(AuthState state, AccessTokenRetrieved action)
        {
            return state with { AccessToken = action.AccessToken, IsLoading = false };
        }

        [ReducerMethod]
        public static AuthState UserInfoRetrievedReducer(AuthState state, UserInfoRetrieved action)
        {
            return state with { UserInfo = action.User, IsLoading = false };
        }

        [ReducerMethod]
        public static AuthState IsLoadedReducer(AuthState state, IsLoading action)
        {
            return state with { IsLoading = action.Loading };
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
        public async Task AccessTokenFetchedReducer(IDispatcher dispatcher)
        {
            try
            {
                TokenDto? token = null;
                dispatcher.Dispatch(new IsLoading(true));
                var response = await _api.Auth.RefreshAsync();
                if (response is not null && response.IsSuccess)
                {
                    token = JsonSerializer.Deserialize<TokenDto>(JsonSerializer.Serialize(response.Result));
                }
                dispatcher.Dispatch(new AccessTokenRetrieved(token?.AccessToken));
            }
            catch (Exception)
            {
                dispatcher.Dispatch(new IsLoading(false));
            }
        }

        [EffectMethod(typeof(UserInfoFetched))]
        public async Task UserInfoFetchedReducer(IDispatcher dispatcher)
        {
            try
            {
                ApplicationUserDto? user = null;
                dispatcher.Dispatch(new IsLoading(true));
                var response = await _api.ApplicationUsers.GetUserInfoAsync();
                if (response is not null && response.IsSuccess)
                {
                    user = JsonSerializer.Deserialize<ApplicationUserDto>(JsonSerializer.Serialize(response.Result));
                }
                dispatcher.Dispatch(new UserInfoRetrieved(user));
            }
            catch (Exception)
            {
                dispatcher.Dispatch(new IsLoading(false));
            }
        }
    }

    // ********************
    // Actions
    // ********************
    public record AccessTokenFetched();
    public record AccessTokenRetrieved(string? AccessToken);
    public record UserInfoFetched();
    public record UserInfoRetrieved(ApplicationUserDto? User);
    public record IsLoading(bool Loading);
}