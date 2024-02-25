using Fluxor;

namespace BlazorStore.Client.Store
{
    [FeatureState]
    public record AuthStore(
        string AccessToken,
        string XsrfToken
    );
}