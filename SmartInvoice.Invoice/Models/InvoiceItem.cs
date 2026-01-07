using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartInvoice.Invoice.Models
{
    public class InvoiceItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid InvoiceId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public int Quantity { get; set; } = 1;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        [MaxLength(50)]
        public string? UnitType { get; set; } = "unit";

        [Column(TypeName = "decimal(5,2)")]
        public decimal DiscountPercentage { get; set; }

        // Store calculated values in the database
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal SubTotal { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountAmount { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }

        // Navigation property
        [ForeignKey("InvoiceId")]
        public Invoice? Invoice { get; set; }

        // Constructor
        public InvoiceItem() { }

        // Method to calculate totals
        public void CalculateTotals()
        {
            SubTotal = UnitPrice * Quantity;
            DiscountAmount = SubTotal * (DiscountPercentage / 100m);
            Total = SubTotal - DiscountAmount;
        }
    }
}