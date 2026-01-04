using Microsoft.AspNetCore.Mvc;
using SmartInvoice.Shared.DTOs;

namespace SmartInvoice.Customer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        [HttpGet("health")]
        public IActionResult HealthCheck()
        {
            return Ok(new ApiResponse<HealthResponse>
            {
                Success = true,
                Message = "Customer service is healthy",
                Data = new HealthResponse
                {
                    Service = "Customer Management Service",
                    Status = "Healthy",
                    Timestamp = DateTime.UtcNow,
                    Details = new Dictionary<string, string>
                    {
                        { "Port", "5003" },
                        { "Version", "1.0.0" },
                        { "Features", "Customer CRUD, Contact Management" }
                    }
                }
            });
        }

        [HttpGet]
        public IActionResult GetCustomers()
        {
            return Ok(new ApiResponse<List<CustomerResponse>>
            {
                Success = true,
                Message = "Get customers endpoint - to be implemented",
                Data = new List<CustomerResponse>
                {
                    new CustomerResponse
                    {
                        Id = Guid.NewGuid(),
                        Name = "Tech Solutions Inc.",
                        Email = "contact@techsolutions.com",
                        Phone = "+1 (555) 123-4567",
                        Address = "123 Tech Street, San Francisco, CA",
                        CreatedAt = DateTime.UtcNow.AddMonths(-3)
                    },
                    new CustomerResponse
                    {
                        Id = Guid.NewGuid(),
                        Name = "Global Retail Corp",
                        Email = "info@globalretail.com",
                        Phone = "+1 (555) 987-6543",
                        Address = "456 Business Ave, New York, NY",
                        CreatedAt = DateTime.UtcNow.AddMonths(-1)
                    }
                }
            });
        }

        [HttpGet("{id}")]
        public IActionResult GetCustomer(Guid id)
        {
            return Ok(new ApiResponse<CustomerResponse>
            {
                Success = true,
                Message = $"Get customer {id} - to be implemented",
                Data = new CustomerResponse
                {
                    Id = id,
                    Name = "Sample Customer",
                    Email = "customer@example.com",
                    Phone = "+1 (555) 000-0000",
                    Address = "789 Sample Street, City, State",
                    CreatedAt = DateTime.UtcNow.AddMonths(-2)
                }
            });
        }

        [HttpPost]
        public IActionResult CreateCustomer([FromBody] CreateCustomerRequest request)
        {
            return Ok(new ApiResponse<CustomerResponse>
            {
                Success = true,
                Message = "Customer created successfully (mock)",
                Data = new CustomerResponse
                {
                    Id = Guid.NewGuid(),
                    Name = request.Name,
                    Email = request.Email,
                    Phone = request.Phone,
                    Address = request.Address,
                    CreatedAt = DateTime.UtcNow
                }
            });
        }

        [HttpPut("{id}")]
        public IActionResult UpdateCustomer(Guid id, [FromBody] UpdateCustomerRequest request)
        {
            return Ok(new ApiResponse<CustomerResponse>
            {
                Success = true,
                Message = $"Customer {id} updated successfully (mock)",
                Data = new CustomerResponse
                {
                    Id = id,
                    Name = request.Name,
                    Email = request.Email,
                    Phone = request.Phone,
                    Address = request.Address,
                    CreatedAt = DateTime.UtcNow.AddMonths(-1)
                }
            });
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteCustomer(Guid id)
        {
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = $"Customer {id} deleted successfully (mock)",
                Data = new { DeletedCustomerId = id }
            });
        }

        [HttpGet("{id}/invoices")]
        public IActionResult GetCustomerInvoices(Guid id)
        {
            return Ok(new ApiResponse<CustomerInvoicesResponse>
            {
                Success = true,
                Message = $"Get invoices for customer {id} - to be implemented",
                Data = new CustomerInvoicesResponse
                {
                    CustomerId = id,
                    CustomerName = "Sample Customer",
                    TotalInvoices = 5,
                    TotalAmount = 12500.00m,
                    OutstandingAmount = 2500.00m,
                    Invoices = new List<CustomerInvoiceResponse>
                    {
                        new CustomerInvoiceResponse
                        {
                            InvoiceId = Guid.NewGuid(),
                            InvoiceNumber = "INV-2024-001",
                            Amount = 3000.00m,
                            Status = "Paid",
                            IssueDate = DateTime.UtcNow.AddDays(-45)
                        }
                    }
                }
            });
        }

        // DTO Classes
        public class CreateCustomerRequest
        {
            public string Name { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string Phone { get; set; } = string.Empty;
            public string Address { get; set; } = string.Empty;
            public string? TaxId { get; set; }
        }

        public class UpdateCustomerRequest
        {
            public string Name { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string Phone { get; set; } = string.Empty;
            public string Address { get; set; } = string.Empty;
        }

        public class CustomerResponse
        {
            public Guid Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string Phone { get; set; } = string.Empty;
            public string Address { get; set; } = string.Empty;
            public DateTime CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }

        public class CustomerInvoicesResponse
        {
            public Guid CustomerId { get; set; }
            public string CustomerName { get; set; } = string.Empty;
            public int TotalInvoices { get; set; }
            public decimal TotalAmount { get; set; }
            public decimal OutstandingAmount { get; set; }
            public List<CustomerInvoiceResponse> Invoices { get; set; } = new();
        }

        public class CustomerInvoiceResponse
        {
            public Guid InvoiceId { get; set; }
            public string InvoiceNumber { get; set; } = string.Empty;
            public decimal Amount { get; set; }
            public string Status { get; set; } = string.Empty;
            public DateTime IssueDate { get; set; }
            public DateTime? PaymentDate { get; set; }
        }
    }
}