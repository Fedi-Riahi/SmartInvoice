using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using SmartInvoice.Invoice.Models.Pdf;

namespace SmartInvoice.Invoice.Services.Pdf
{
    public static class PdfGenerator
    {
        public static byte[] GenerateInvoicePdf(InvoicePdfModel model)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var document = new InvoicePdfDocument(model);
            return document.GeneratePdf();
        }

        public static string GenerateInvoicePdfBase64(InvoicePdfModel model)
        {
            var pdfBytes = GenerateInvoicePdf(model);
            return Convert.ToBase64String(pdfBytes);
        }
    }
}