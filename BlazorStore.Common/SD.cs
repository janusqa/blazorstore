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
        ];

        // Pagination
        public const int paginationSize = 5;
        public const int paginationDefaultPage = 0;
        public const int paginationDefaultSize = 10;
    }
}