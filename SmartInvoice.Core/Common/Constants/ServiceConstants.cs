namespace SmartInvoice.Core.Common.Constants
{
    public static class ServiceConstants
    {
        public const string GatewayService = "Gateway";
        public const string AuthService = "Auth";
        public const string InvoiceService = "Invoice";
        public const string CustomerService = "Customer";
        public const string AIService = "AI";

        // Service URLs (Development)
        public static class Urls
        {
            public const string Gateway = "http://localhost:5000";
            public const string Auth = "http://localhost:5001";
            public const string Invoice = "http://localhost:5002";
            public const string Customer = "http://localhost:5003";
            public const string AI = "http://localhost:5004";
        }

        // API Routes
        public static class Routes
        {
            public const string Health = "health";
            public const string ApiBase = "api";

            // Service-specific routes
            public static class Auth
            {
                public const string Login = "auth/login";
                public const string Register = "auth/register";
                public const string ValidateToken = "auth/validate";
            }

            public static class Invoice
            {
                public const string Create = "invoice/create";
                public const string Get = "invoice/{id}";
                public const string GetAll = "invoice/all";
            }
        }
    }
}