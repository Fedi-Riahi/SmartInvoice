using System;
using System.Threading.Tasks;
using SmartInvoice.Shared.DTOs;

namespace SmartInvoice.Shared.Contracts
{
    public interface IInvoiceService
    {
        Task<InvoiceResponse> CreateInvoiceAsync(CreateInvoiceRequest request);
        Task<InvoiceResponse> GetInvoiceAsync(Guid invoiceId);
        Task<List<InvoiceResponse>> GetInvoicesByCustomerAsync(Guid customerId);
        Task<bool> DeleteInvoiceAsync(Guid invoiceId);
    }
}