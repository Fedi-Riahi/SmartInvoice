namespace SmartInvoice.Core.Common.Constants
{
    public static class ApiConstants
    {
        public const string ApiVersion = "v1";
        public const string GatewayRoute = "api/[controller]";

        // Service endpoints (update ports based on your setup)
        public const string AuthServiceUrl = "http://localhost:5001";
        public const string InvoiceServiceUrl = "http://localhost:5002";
        public const string CustomerServiceUrl = "http://localhost:5003";
        public const string AIServiceUrl = "http://localhost:5004";
    }
}