using Microsoft.EntityFrameworkCore;
using SmartInvoice.Customer.Models;

namespace SmartInvoice.Customer.Data
{
    public class CustomerDbContext : DbContext
    {
        public CustomerDbContext(DbContextOptions<CustomerDbContext> options) : base(options)
        {
        }

        public DbSet<CustomerEntity> Customers { get; set; }
    }
}
