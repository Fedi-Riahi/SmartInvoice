using Microsoft.AspNetCore.Mvc;
using SmartInvoice.Shared.DTOs;

namespace SmartInvoice.Invoice.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InvoiceController : ControllerBase
    {
        [HttpGet("health")]
        public IActionResult HealthCheck()
        {
            return Ok(new ApiResponse<HealthResponse>
            {
                Success = true,
                Message = "Invoice service is healthy",
                Data = new HealthResponse
                {
                    Service = "Invoice Management Service",
                    Status = "Healthy",
                    Timestamp = DateTime.UtcNow,
                    Details = new Dictionary<string, string>
                    {
                        { "Port", "5002" },
                        { "Version", "1.0.0" },
                        { "Features", "Invoice CRUD, PDF Generation" }
                    }
                }
            });
        }

        [HttpGet]
        public IActionResult GetInvoices()
        {
            return Ok(new ApiResponse<List<InvoiceResponse>>
            {
                Success = true,
                Message = "Get invoices endpoint - to be implemented",
                Data = new List<InvoiceResponse>
                {
                    new InvoiceResponse
                    {
                        Id = Guid.NewGuid(),
                        InvoiceNumber = "INV-001",
                        CustomerName = "John Doe",
                        TotalAmount = 1500.00m,
                        Status = "Paid",
                        IssueDate = DateTime.UtcNow.AddDays(-10),
                        DueDate = DateTime.UtcNow.AddDays(20)
                    }
                }
            });
        }

        [HttpGet("{id}")]
        public IActionResult GetInvoice(Guid id)
        {
            return Ok(new ApiResponse<InvoiceResponse>
            {
                Success = true,
                Message = $"Get invoice {id} - to be implemented",
                Data = new InvoiceResponse
                {
                    Id = id,
                    InvoiceNumber = "INV-001",
                    CustomerName = "John Doe",
                    TotalAmount = 1500.00m,
                    Status = "Pending",
                    IssueDate = DateTime.UtcNow,
                    DueDate = DateTime.UtcNow.AddDays(30)
                }
            });
        }

        [HttpPost]
        public IActionResult CreateInvoice([FromBody] CreateInvoiceRequest request)
        {
            return Ok(new ApiResponse<InvoiceResponse>
            {
                Success = true,
                Message = "Invoice created successfully (mock)",
                Data = new InvoiceResponse
                {
                    Id = Guid.NewGuid(),
                    InvoiceNumber = $"INV-{DateTime.Now:yyyyMMdd}-001",
                    CustomerName = request.CustomerName,
                    TotalAmount = request.Items.Sum(i => i.Quantity * i.UnitPrice),
                    Status = "Draft",
                    IssueDate = DateTime.UtcNow,
                    DueDate = request.DueDate,
                    Items = request.Items.Select(i => new InvoiceItemResponse
                    {
                        ProductName = i.ProductName,
                        Quantity = i.Quantity,
                        UnitPrice = i.UnitPrice,
                        TotalPrice = i.Quantity * i.UnitPrice
                    }).ToList()
                }
            });
        }

        [HttpPut("{id}")]
        public IActionResult UpdateInvoice(Guid id, [FromBody] UpdateInvoiceRequest request)
        {
            return Ok(new ApiResponse<InvoiceResponse>
            {
                Success = true,
                Message = $"Invoice {id} updated successfully (mock)",
                Data = new InvoiceResponse
                {
                    Id = id,
                    InvoiceNumber = request.InvoiceNumber,
                    CustomerName = request.CustomerName,
                    TotalAmount = request.TotalAmount,
                    Status = request.Status,
                    IssueDate = DateTime.UtcNow.AddDays(-5),
                    DueDate = request.DueDate
                }
            });
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteInvoice(Guid id)
        {
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = $"Invoice {id} deleted successfully (mock)",
                Data = new { DeletedInvoiceId = id }
            });
        }

        // DTO Classes (will move to Shared later)
        public class CreateInvoiceRequest
        {
            public string CustomerName { get; set; } = string.Empty;
            public string CustomerEmail { get; set; } = string.Empty;
            public List<InvoiceItemRequest> Items { get; set; } = new();
            public DateTime DueDate { get; set; }
            public string? Notes { get; set; }
        }

        public class UpdateInvoiceRequest
        {
            public string InvoiceNumber { get; set; } = string.Empty;
            public string CustomerName { get; set; } = string.Empty;
            public decimal TotalAmount { get; set; }
            public string Status { get; set; } = string.Empty;
            public DateTime DueDate { get; set; }
        }

        public class InvoiceResponse
        {
            public Guid Id { get; set; }
            public string InvoiceNumber { get; set; } = string.Empty;
            public string CustomerName { get; set; } = string.Empty;
            public decimal TotalAmount { get; set; }
            public string Status { get; set; } = string.Empty;
            public DateTime IssueDate { get; set; }
            public DateTime DueDate { get; set; }
            public List<InvoiceItemResponse> Items { get; set; } = new();
        }

        public class InvoiceItemRequest
        {
            public string ProductName { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public int Quantity { get; set; }
            public decimal UnitPrice { get; set; }
        }

        public class InvoiceItemResponse
        {
            public string ProductName { get; set; } = string.Empty;
            public int Quantity { get; set; }
            public decimal UnitPrice { get; set; }
            public decimal TotalPrice { get; set; }
        }
    }
}