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
            ("ProductPrice", "ProductPrices"),
            ("OrderHeader", "OrderHeaders"),
            ("OrderDetail", "OrderDetails"),
        ];

        // Pagination
        public const int paginationSize = 20;
        public const int paginationDefaultPage = 0;
        public const int paginationDefaultSize = 10;

        // Application
        public const string appBaseUrl = "https://localhost:7036";
        public const string cartKey = "BlazorCartLocalStorage";

        // Api
        public const string ApiVersion = "v1";
        public const string ApiBaseUrl = "https://localhost:7036";
        public enum ContentType
        {
            Json,
            MultiPartFormData
        }
        public enum ApiMethod
        {
            GET,
            POST,
            PUT,
            DELETE
        }
        public const string JwtAccessTokenCookie = "JWT_ACCESS_TOKEN";
        public const int JwtAccessTokenExpiry = 1; //mins
        public const string JwtRrefreshTokenCookie = "JWT_REFRESH_TOKEN";
        public const int JwtRefreshTokenExpiry = 5; // mins
        public const string ApiXsrfCookie = "XSRF-TOKEN";

        // Role Constants
        public const string Role_Customer = "Customer";
        public const string Role_Admin = "Admin";
        public const string Role_Employee = "Employee";

        // Order Status Constants
        public const string OrderStatusPending = "Pending";
        public const string OrderStatusApproved = "Approved";
        public const string OrderStatusInProcess = "Processing";
        public const string OrderStatusShipped = "Shipped";
        public const string OrderStatusCancelled = "Cancelled";

        // Order Payment Status Constants
        public const string PaymentStatusPending = "Pending";
        public const string PaymentStatusApproved = "Approved";
        public const string PaymentStatusApprovedDelayedPayment = "ApprovedForDelayedPayment";
        public const string PaymentStatusRejected = "Rejected";
        public const string PaymentStatusRefunded = "Refunded";
        public const string PaymentStatusCancelled = "Cancelled";
    }
}