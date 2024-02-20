namespace BlazorStore.Common
{
    public static class SD
    {
        // Database
        public const string modelsAssembly = "BlazorStore.Models";
        public const string modelsNamespace = "Domain";
        public const string dbName = "";
        public static readonly (string Model, string Table)[] dbEntity = [
            ("Category","Categories"),
            ("Product","Products"),
            ("ProductPrice", "ProductPrices")
        ];

        // Pagination
        public const int paginationSize = 20;
        public const int paginationDefaultPage = 0;
        public const int paginationDefaultSize = 10;

        // Application
        public const string appBaseUrl = "https://localhost:7036";

        // Api
        public const string ApiVersion = "v1";
        public const string ApiBaseUrl = "https://localhost:7036";
        public enum ContentType { Json, MultiPartFormData }
        public enum ApiMethod
        {
            GET,
            POST,
            PUT,
            DELETE
        }
        public const string JwtAccessTokenCookie = "JWT_ACCESS_TOKEN";
        public const int JwtAccessTokenExpiry = 1;
        public const string JwtRrefreshTokenCookie = "JWT_REFRESH_TOKEN";
        public const int JwtRefreshTokenExpiry = 5;
        public const string ApiXsrfCookie = "XSRF-TOKEN";
    }
}