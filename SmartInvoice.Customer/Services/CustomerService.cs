using Microsoft.EntityFrameworkCore;
using SmartInvoice.Customer.Data;
using SmartInvoice.Customer.Models;

namespace SmartInvoice.Customer.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly CustomerDbContext _context;

        public CustomerService(CustomerDbContext context)
        {
            _context = context;
        }

        public async Task<List<CustomerEntity>> GetAllAsync()
        {
            return await _context.Customers.ToListAsync();
        }

        public async Task<CustomerEntity?> GetByIdAsync(Guid id)
        {
            return await _context.Customers.FindAsync(id);
        }

        public async Task<CustomerEntity> CreateAsync(CustomerEntity customer)
        {
            customer.Id = Guid.NewGuid();
            customer.CreatedAt = DateTime.UtcNow;
            
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
            
            return customer;
        }

        public async Task<CustomerEntity?> UpdateAsync(Guid id, CustomerEntity customer)
        {
            var existing = await _context.Customers.FindAsync(id);
            if (existing == null) return null;

            existing.Name = customer.Name;
            existing.Email = customer.Email;
            existing.Phone = customer.Phone;
            existing.Address = customer.Address;
            existing.TaxId = customer.TaxId;
            existing.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null) return false;

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
