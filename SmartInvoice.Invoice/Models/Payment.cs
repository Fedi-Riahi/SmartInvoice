using System.ComponentModel.DataAnnotations;

namespace SmartInvoice.Invoice.Models
{
    public class Payment
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid InvoiceId { get; set; }

        [Required]
        [MaxLength(50)]
        public string PaymentMethod { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? TransactionId { get; set; }

        [Required]
        public decimal Amount { get; set; }

        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

        [MaxLength(500)]
        public string? Notes { get; set; }

        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

        // Navigation property
        public Invoice Invoice { get; set; } = null!;
    }

    // Define enum ONLY here
    public enum PaymentStatus
    {
        Pending = 0,
        Completed = 1,
        Failed = 2,
        Refunded = 3
    }
}