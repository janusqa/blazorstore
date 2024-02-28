using Microsoft.JSInterop;

namespace BlazorStore.ApiAccess.Service
{
    public class CookieService : ICookieService
    {
        private readonly IJSRuntime _jsRuntime;

        public CookieService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task<string> GetCookie(string name)
        {
            return await _jsRuntime.InvokeAsync<string>("blazorInterop.GetCookie", name);
        }
    }
}