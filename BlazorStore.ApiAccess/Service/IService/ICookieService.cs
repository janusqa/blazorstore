namespace BlazorStore.ApiAccess.Service
{
    public interface ICookieService
    {
        Task<string> GetCookie(string name);
    }
}