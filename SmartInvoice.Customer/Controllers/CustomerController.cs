using Microsoft.AspNetCore.Mvc;
using SmartInvoice.Shared.DTOs;
using SmartInvoice.Customer.Services;
using SmartInvoice.Customer.Models;

namespace SmartInvoice.Customer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

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
                        { "Features", "Customer CRUD, Contact Management" },
                        { "Storage", "In-Memory" }
                    }
                }
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetCustomers()
        {
            var customers = await _customerService.GetAllAsync();
            var response = customers.Select(c => new CustomerResponse
            {
                Id = c.Id,
                Name = c.Name,
                Email = c.Email,
                Phone = c.Phone,
                Address = c.Address,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            }).ToList();

            return Ok(new ApiResponse<List<CustomerResponse>>
            {
                Success = true,
                Message = "Customers retrieved successfully",
                Data = response
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomer(Guid id)
        {
            var customer = await _customerService.GetByIdAsync(id);
            if (customer == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Customer not found",
                    Errors = new List<string> { $"Customer with ID {id} not found" }
                });
            }

            return Ok(new ApiResponse<CustomerResponse>
            {
                Success = true,
                Message = "Customer retrieved successfully",
                Data = new CustomerResponse
                {
                    Id = customer.Id,
                    Name = customer.Name,
                    Email = customer.Email,
                    Phone = customer.Phone,
                    Address = customer.Address,
                    CreatedAt = customer.CreatedAt,
                    UpdatedAt = customer.UpdatedAt
                }
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateCustomer([FromBody] CreateCustomerRequest request)
        {
            var newCustomer = new CustomerEntity
            {
                Name = request.Name,
                Email = request.Email,
                Phone = request.Phone,
                Address = request.Address,
                TaxId = request.TaxId
            };

            var created = await _customerService.CreateAsync(newCustomer);

            return CreatedAtAction(nameof(GetCustomer), new { id = created.Id }, new ApiResponse<CustomerResponse>
            {
                Success = true,
                Message = "Customer created successfully",
                Data = new CustomerResponse
                {
                    Id = created.Id,
                    Name = created.Name,
                    Email = created.Email,
                    Phone = created.Phone,
                    Address = created.Address,
                    CreatedAt = created.CreatedAt,
                    UpdatedAt = created.UpdatedAt
                }
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomer(Guid id, [FromBody] UpdateCustomerRequest request)
        {
            var updateModel = new CustomerEntity
            {
                Name = request.Name,
                Email = request.Email,
                Phone = request.Phone,
                Address = request.Address
            };

            var updated = await _customerService.UpdateAsync(id, updateModel);
            if (updated == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Customer not found",
                    Errors = new List<string> { $"Customer with ID {id} not found" }
                });
            }

            return Ok(new ApiResponse<CustomerResponse>
            {
                Success = true,
                Message = "Customer updated successfully",
                Data = new CustomerResponse
                {
                    Id = updated.Id,
                    Name = updated.Name,
                    Email = updated.Email,
                    Phone = updated.Phone,
                    Address = updated.Address,
                    CreatedAt = updated.CreatedAt,
                    UpdatedAt = updated.UpdatedAt
                }
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(Guid id)
        {
            var deleted = await _customerService.DeleteAsync(id);
            if (!deleted)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Customer not found",
                    Errors = new List<string> { $"Customer with ID {id} not found" }
                });
            }

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Customer deleted successfully",
                Data = new { DeletedCustomerId = id }
            });
        }

        [HttpGet("{id}/invoices")]
        public IActionResult GetCustomerInvoices(Guid id)
        {
            return Ok(new ApiResponse<CustomerInvoicesResponse>
            {
                Success = true,
                Message = $"Get invoices for customer {id} - Pending Invoice Service Integration",
                Data = new CustomerInvoicesResponse
                {
                    CustomerId = id,
                    CustomerName = "Integration Pending",
                    TotalInvoices = 0,
                    TotalAmount = 0m,
                    OutstandingAmount = 0m,
                    Invoices = new List<CustomerInvoiceResponse>()
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
