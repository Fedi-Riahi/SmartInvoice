using SmartInvoice.Invoice.DTOs;

namespace SmartInvoice.Invoice.Services
{
    public interface IInvoiceService
    {
        // CRUD Operations
        Task<InvoiceResponse> CreateInvoiceAsync(CreateInvoiceRequest request);
        Task<InvoiceResponse> GetInvoiceAsync(Guid invoiceId);
        Task<List<InvoiceResponse>> GetInvoicesAsync(int page = 1, int pageSize = 20, Models.InvoiceStatus? status = null);
        Task<InvoiceResponse> UpdateInvoiceAsync(Guid invoiceId, UpdateInvoiceRequest request);
        Task<bool> DeleteInvoiceAsync(Guid invoiceId);

        // Invoice Actions
        Task<InvoiceResponse> SendInvoiceAsync(Guid invoiceId);
        Task<InvoiceResponse> CancelInvoiceAsync(Guid invoiceId, string reason);

        // Payment Operations
        Task<PaymentResponse> AddPaymentAsync(Guid invoiceId, AddPaymentRequest request);
        Task<List<PaymentResponse>> GetInvoicePaymentsAsync(Guid invoiceId);

        // Customer Operations
        Task<List<InvoiceResponse>> GetCustomerInvoicesAsync(Guid customerId);

        // Reporting
        Task<InvoiceSummaryResponse> GetSummaryAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<byte[]> GeneratePdfAsync(Guid invoiceId);

        // Utility
        Task<string> GenerateInvoiceNumberAsync();
    }
}