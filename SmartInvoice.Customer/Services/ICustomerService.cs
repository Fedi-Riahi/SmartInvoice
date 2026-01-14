using SmartInvoice.Customer.Models;
using SmartInvoice.Shared.DTOs;

namespace SmartInvoice.Customer.Services
{
    public interface ICustomerService
    {
        Task<List<CustomerEntity>> GetAllAsync();
        Task<CustomerEntity?> GetByIdAsync(Guid id);
        Task<CustomerEntity> CreateAsync(CustomerEntity customer);
        Task<CustomerEntity?> UpdateAsync(Guid id, CustomerEntity customer);
        Task<bool> DeleteAsync(Guid id);
    }
}
