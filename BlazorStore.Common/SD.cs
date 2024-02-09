namespace BlazorStore.Common
{
    public static class SD
    {

        public const string modelsAssembly = "BlazorStore.Models";
        public const string modelsNamespace = "Domain";
        public const string dbName = "";
        public static readonly (string Model, string Table)[] dbEntity = [
            ("Category","Categories")
        ];
    }
}