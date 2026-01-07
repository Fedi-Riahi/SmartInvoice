// Models/Invoice.cs
using System.ComponentModel.DataAnnotations;

namespace SmartInvoice.Invoice.Models
{
    public class Invoice
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(50)]
        public string InvoiceNumber { get; set; } = string.Empty;

        [Required]
        public Guid CustomerId { get; set; }

        [MaxLength(200)]
        public string CustomerName { get; set; } = string.Empty;

        [MaxLength(200)]
        public string CustomerEmail { get; set; } = string.Empty;

        public DateTime IssueDate { get; set; } = DateTime.UtcNow;
        public DateTime DueDate { get; set; }

        [MaxLength(1000)]
        public string? Notes { get; set; }

        [Required]
        public decimal SubTotal { get; set; }

        [Required]
        public decimal TaxAmount { get; set; }

        [Required]
        public decimal TotalAmount { get; set; }

        [Required]
        public InvoiceStatus Status { get; set; } = InvoiceStatus.Draft;

        public DateTime? PaidDate { get; set; }
        public DateTime? CancelledDate { get; set; }

        public List<InvoiceItem> Items { get; set; } = [];
        public List<Payment> Payments { get; set; } = [];

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Helper methods
        public decimal CalculateBalance()
        {
            var totalPaid = Payments.Where(p => p.Status == PaymentStatus.Completed).Sum(p => p.Amount);
            return TotalAmount - totalPaid;
        }

        public bool IsOverdue => Status == InvoiceStatus.Sent && DueDate < DateTime.UtcNow;
        public bool IsFullyPaid => CalculateBalance() <= 0;
    }
}