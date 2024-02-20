namespace BlazorStore.Common
{
    public static class BcryptUtils
    {
        public static string CreateSalt(int workFactor = 6) =>
            BCrypt.Net.BCrypt.GenerateSalt(workFactor);
    }
}