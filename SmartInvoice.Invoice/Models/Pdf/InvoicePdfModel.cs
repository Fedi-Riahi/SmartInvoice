using System.Text;

namespace SmartInvoice.Invoice.Models.Pdf
{
    public class InvoicePdfModel
    {
        public string InvoiceNumber { get; set; } = string.Empty;
        public DateTime IssueDate { get; set; }
        public DateTime DueDate { get; set; }
        public string Status { get; set; } = string.Empty;

        // Company Information
        public CompanyInfo Company { get; set; } = new();

        // Customer Information
        public CustomerInfo Customer { get; set; } = new();

        // Invoice Items
        public List<InvoiceItemPdf> Items { get; set; } = new();

        // Totals
        public decimal SubTotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Balance { get; set; }

        // Additional Info
        public string? Notes { get; set; }
        public List<PaymentInfo> Payments { get; set; } = new();

        // Helper properties
        public bool HasPayments => Payments.Any();
        public bool HasNotes => !string.IsNullOrWhiteSpace(Notes);
        public bool IsOverdue => DueDate < DateTime.UtcNow && Status != "Paid" && Status != "Cancelled";
        public bool IsFullyPaid => Balance <= 0;

        public class CompanyInfo
        {
            public string Name { get; set; } = "SmartInvoice Inc.";
            public string Address { get; set; } = "123 Business Street";
            public string CityStateZip { get; set; } = "New York, NY 10001";
            public string Phone { get; set; } = "(123) 456-7890";
            public string Email { get; set; } = "invoicing@smartinvoice.com";
            public string Website { get; set; } = "www.smartinvoice.com";
        }

        public class CustomerInfo
        {
            public string Name { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string? Company { get; set; }
            public string? Address { get; set; }
            public string? Phone { get; set; }
        }

        public class InvoiceItemPdf
        {
            public string Description { get; set; } = string.Empty;
            public int Quantity { get; set; }
            public string UnitType { get; set; } = "unit";
            public decimal UnitPrice { get; set; }
            public decimal DiscountPercentage { get; set; }
            public decimal Total { get; set; }

            public decimal DiscountAmount => UnitPrice * Quantity * (DiscountPercentage / 100m);
            public decimal ItemTotal => (UnitPrice * Quantity) - DiscountAmount;
        }

        public class PaymentInfo
        {
            public DateTime PaymentDate { get; set; }
            public string Method { get; set; } = string.Empty;
            public decimal Amount { get; set; }
            public string? TransactionId { get; set; }
            public string Status { get; set; } = string.Empty;
        }

        // Helper methods
        public string GetFormattedDate(DateTime date) => date.ToString("dd MMMM yyyy");
        public string FormatCurrency(decimal amount) => amount.ToString("C2");
        public string GetStatusColor()
        {
            return Status switch
            {
                "Paid" => "#10B981", // Green
                "Sent" => "#3B82F6", // Blue
                "Draft" => "#6B7280", // Gray
                "Overdue" => "#EF4444", // Red
                "Cancelled" => "#F59E0B", // Yellow
                _ => "#6B7280"
            };
        }
    }
}