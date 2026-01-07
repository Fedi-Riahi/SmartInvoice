// Models/InvoiceStatus.cs
namespace SmartInvoice.Invoice.Models;

public enum InvoiceStatus
{
    Draft = 0,
    Sent = 1,
    Paid = 2,
    Overdue = 3,
    Cancelled = 4,
    PartiallyPaid = 5
}