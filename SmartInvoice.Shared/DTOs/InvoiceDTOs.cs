using System;
using System.Collections.Generic;

namespace SmartInvoice.Shared.DTOs
{
    // Request DTOs
    public class CreateInvoiceRequest
    {
        public Guid CustomerId { get; set; }
        public List<InvoiceItemRequest> Items { get; set; } = new();
        public DateTime DueDate { get; set; }
        public string? Notes { get; set; }
    }

    public class InvoiceItemRequest
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string Description { get; set; } = string.Empty;
    }

    // Response DTOs
    public class InvoiceResponse
    {
        public Guid Id { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime DueDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public List<InvoiceItemResponse> Items { get; set; } = new();
    }

    public class InvoiceItemResponse
    {
        public Guid Id { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice => Quantity * UnitPrice;
    }
}