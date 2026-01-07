using System.ComponentModel.DataAnnotations;
using SmartInvoice.Invoice.Models;

namespace SmartInvoice.Invoice.DTOs
{
    // Request DTOs
    public class CreateInvoiceRequest
    {
        [Required]
        public Guid CustomerId { get; set; }

        [Required]
        [MaxLength(200)]
        public string CustomerName { get; set; } = string.Empty;

        [EmailAddress]
        [MaxLength(200)]
        public string CustomerEmail { get; set; } = string.Empty;

        [Required]
        public DateTime DueDate { get; set; }

        public List<InvoiceItemRequest> Items { get; set; } = [];

        [MaxLength(1000)]
        public string? Notes { get; set; }

        public decimal TaxRate { get; set; } = 0;
    }

    public class InvoiceItemRequest
    {
        [Required]
        [MaxLength(200)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; } = 1;

        [Required]
        [Range(0, double.MaxValue)]
        public decimal UnitPrice { get; set; }

        [MaxLength(50)]
        public string? UnitType { get; set; } = "unit";

        [Range(0, 100)]
        public decimal DiscountPercentage { get; set; } = 0;
    }

    public class UpdateInvoiceRequest
    {
        [MaxLength(1000)]
        public string? Notes { get; set; }

        public DateTime? DueDate { get; set; }
        public InvoiceStatus? Status { get; set; }

        // Add these properties to allow updating items
        public List<InvoiceItemRequest>? Items { get; set; }

        [Range(0, 100)]
        public decimal? TaxRate { get; set; }
    }

    public class AddPaymentRequest
    {
        [Required]
        [MaxLength(50)]
        public string PaymentMethod { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }

        [MaxLength(100)]
        public string? TransactionId { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }
    }

    // Response DTOs
    public class InvoiceResponse
    {
        public Guid Id { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public DateTime IssueDate { get; set; }
        public DateTime DueDate { get; set; }
        public string? Notes { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Balance { get; set; }
        public InvoiceStatus Status { get; set; }
        public bool IsOverdue { get; set; }
        public bool IsFullyPaid { get; set; }
        public List<InvoiceItemResponse> Items { get; set; } = [];
        public List<PaymentResponse> Payments { get; set; } = [];
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class InvoiceItemResponse
    {
        public Guid Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string UnitType { get; set; } = string.Empty;
        public decimal DiscountPercentage { get; set; }
        public decimal SubTotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal Total { get; set; }
    }

    public class PaymentResponse
    {
        public Guid Id { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string? TransactionId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string? Notes { get; set; }
        public PaymentStatus Status { get; set; }
    }

    public class InvoiceSummaryResponse
    {
        public int TotalInvoices { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal OutstandingAmount { get; set; }
        public int OverdueInvoices { get; set; }
        public Dictionary<string, int> StatusCount { get; set; } = [];
        public Dictionary<string, decimal> MonthlyRevenue { get; set; } = [];
    }
}