using Microsoft.AspNetCore.Mvc;
using SmartInvoice.Invoice.DTOs;
using SmartInvoice.Invoice.Models;
using SmartInvoice.Invoice.Services;
using SmartInvoice.Shared.DTOs;

namespace SmartInvoice.Invoice.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InvoiceController(IInvoiceService invoiceService, ILogger<InvoiceController> logger) : ControllerBase
    {
        private readonly IInvoiceService _invoiceService = invoiceService;
        private readonly ILogger<InvoiceController> _logger = logger;

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
                    Version = "1.0.0",
                    Details = new Dictionary<string, string>
                    {
                        { "Port", "5002" },
                        { "Features", "Invoice CRUD, PDF Generation, Payment Tracking" }
                    }
                }
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateInvoice([FromBody] CreateInvoiceRequest request)
        {
            try
            {
                var invoice = await _invoiceService.CreateInvoiceAsync(request);

                return Ok(new ApiResponse<InvoiceResponse>
                {
                    Success = true,
                    Message = "Invoice created successfully",
                    Data = invoice
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating invoice");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error creating invoice",
                    Errors = [ex.Message]
                });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetInvoice(Guid id)
        {
            try
            {
                var invoice = await _invoiceService.GetInvoiceAsync(id);

                return Ok(new ApiResponse<InvoiceResponse>
                {
                    Success = true,
                    Message = "Invoice retrieved successfully",
                    Data = invoice
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving invoice {InvoiceId}", id);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error retrieving invoice",
                    Errors = [ex.Message]
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetInvoices(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? status = null)
        {
            try
            {
                InvoiceStatus? invoiceStatus = null;
                if (!string.IsNullOrEmpty(status) && Enum.TryParse<InvoiceStatus>(status, true, out var parsedStatus))
                {
                    invoiceStatus = parsedStatus;
                }

                var invoices = await _invoiceService.GetInvoicesAsync(page, pageSize, invoiceStatus);

                return Ok(new ApiResponse<List<InvoiceResponse>>
                {
                    Success = true,
                    Message = "Invoices retrieved successfully",
                    Data = invoices
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving invoices");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error retrieving invoices",
                    Errors = [ex.Message]
                });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateInvoice(Guid id, [FromBody] UpdateInvoiceRequest request)
        {
            try
            {
                var invoice = await _invoiceService.UpdateInvoiceAsync(id, request);

                return Ok(new ApiResponse<InvoiceResponse>
                {
                    Success = true,
                    Message = "Invoice updated successfully",
                    Data = invoice
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating invoice {InvoiceId}", id);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error updating invoice",
                    Errors = [ex.Message]
                });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInvoice(Guid id)
        {
            try
            {
                var result = await _invoiceService.DeleteInvoiceAsync(id);

                if (!result)
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invoice not found"
                    });

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Invoice deleted successfully"
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting invoice {InvoiceId}", id);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error deleting invoice",
                    Errors = [ex.Message]
                });
            }
        }

        [HttpPost("{id}/send")]
        public async Task<IActionResult> SendInvoice(Guid id)
        {
            try
            {
                var invoice = await _invoiceService.SendInvoiceAsync(id);

                return Ok(new ApiResponse<InvoiceResponse>
                {
                    Success = true,
                    Message = "Invoice sent successfully",
                    Data = invoice
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending invoice {InvoiceId}", id);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error sending invoice",
                    Errors = [ex.Message]
                });
            }
        }

        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> CancelInvoice(Guid id, [FromBody] CancelRequest request)
        {
            try
            {
                var invoice = await _invoiceService.CancelInvoiceAsync(id, request.Reason);

                return Ok(new ApiResponse<InvoiceResponse>
                {
                    Success = true,
                    Message = "Invoice cancelled successfully",
                    Data = invoice
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling invoice {InvoiceId}", id);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error cancelling invoice",
                    Errors = [ex.Message]
                });
            }
        }

        [HttpPost("{id}/payments")]
        public async Task<IActionResult> AddPayment(Guid id, [FromBody] AddPaymentRequest request)
        {
            try
            {
                var payment = await _invoiceService.AddPaymentAsync(id, request);

                return Ok(new ApiResponse<PaymentResponse>
                {
                    Success = true,
                    Message = "Payment added successfully",
                    Data = payment
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding payment to invoice {InvoiceId}", id);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error adding payment",
                    Errors = [ex.Message]
                });
            }
        }

        [HttpGet("{id}/payments")]
        public async Task<IActionResult> GetInvoicePayments(Guid id)
        {
            try
            {
                var payments = await _invoiceService.GetInvoicePaymentsAsync(id);

                return Ok(new ApiResponse<List<PaymentResponse>>
                {
                    Success = true,
                    Message = "Payments retrieved successfully",
                    Data = payments
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving payments for invoice {InvoiceId}", id);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error retrieving payments",
                    Errors = [ex.Message]
                });
            }
        }

        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetCustomerInvoices(Guid customerId)
        {
            try
            {
                var invoices = await _invoiceService.GetCustomerInvoicesAsync(customerId);

                return Ok(new ApiResponse<List<InvoiceResponse>>
                {
                    Success = true,
                    Message = "Customer invoices retrieved successfully",
                    Data = invoices
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving invoices for customer {CustomerId}", customerId);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error retrieving customer invoices",
                    Errors = [ex.Message]
                });
            }
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            try
            {
                var summary = await _invoiceService.GetSummaryAsync(startDate, endDate);

                return Ok(new ApiResponse<InvoiceSummaryResponse>
                {
                    Success = true,
                    Message = "Invoice summary retrieved successfully",
                    Data = summary
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving invoice summary");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error retrieving summary",
                    Errors = [ex.Message]
                });
            }
        }

        [HttpGet("{id}/pdf")]
        public async Task<IActionResult> GeneratePdf(Guid id)
        {
            try
            {
                var pdfBytes = await _invoiceService.GeneratePdfAsync(id);

                if (pdfBytes.Length == 0)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "PDF generation not implemented yet"
                    });
                }

                return File(pdfBytes, "application/pdf", $"invoice-{id}.pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating PDF for invoice {InvoiceId}", id);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error generating PDF",
                    Errors = [ex.Message]
                });
            }
        }

        public class CancelRequest
        {
            public string Reason { get; set; } = string.Empty;
        }
    }
}