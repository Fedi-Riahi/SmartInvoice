// Constants/CorsConstants.cs
namespace SmartInvoice.Invoice.Constants
{
    public static class CorsConstants
    {
        public const string AllowFrontendPolicy = "AllowFrontend";

        // Development URLs
        public static readonly string[] AllowedOrigins =
        {
            "http://localhost:3000",    // React dev server
            "http://localhost:5173",    // Vite dev server
            "http://localhost:4200"     // Angular dev server
        };
    }
}