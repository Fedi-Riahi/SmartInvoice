using Microsoft.EntityFrameworkCore;
using SmartInvoice.Invoice.Data;
using SmartInvoice.Invoice.DTOs;
// Use aliases to avoid namespace conflicts
using InvoiceEntity = SmartInvoice.Invoice.Models.Invoice;
using InvoiceItemEntity = SmartInvoice.Invoice.Models.InvoiceItem;
using PaymentEntity = SmartInvoice.Invoice.Models.Payment;
using SmartInvoice.Invoice.Models; // Keep for enums
using SmartInvoice.Shared.DTOs;

namespace SmartInvoice.Invoice.Services
{
    public class InvoiceService(InvoiceDbContext context, ILogger<InvoiceService> logger) : IInvoiceService
    {
        private readonly InvoiceDbContext _context = context;
        private readonly ILogger<InvoiceService> _logger = logger;

        public async Task<InvoiceResponse> CreateInvoiceAsync(CreateInvoiceRequest request)
        {
            try
            {
                // Generate invoice number
                var invoiceNumber = await GenerateInvoiceNumberAsync();

                // Create items with calculated totals
                var items = new List<InvoiceItemEntity>();
                foreach (var itemRequest in request.Items)
                {
                    var invoiceItem = new InvoiceItemEntity
                    {
                        Description = itemRequest.Description,
                        Quantity = itemRequest.Quantity,
                        UnitPrice = itemRequest.UnitPrice,
                        UnitType = itemRequest.UnitType,
                        DiscountPercentage = itemRequest.DiscountPercentage
                    };

                    // Calculate and set the totals
                    invoiceItem.CalculateTotals();
                    items.Add(invoiceItem);
                }

                var subTotal = items.Sum(item => item.Total);
                var taxAmount = subTotal * (request.TaxRate / 100m);
                var totalAmount = subTotal + taxAmount;

                // Create invoice
                var invoice = new InvoiceEntity
                {
                    InvoiceNumber = invoiceNumber,
                    CustomerId = request.CustomerId,
                    CustomerName = request.CustomerName,
                    CustomerEmail = request.CustomerEmail,
                    DueDate = request.DueDate,
                    Notes = request.Notes,
                    SubTotal = subTotal,
                    TaxAmount = taxAmount,
                    TotalAmount = totalAmount,
                    Status = InvoiceStatus.Draft,
                    Items = items
                };

                await _context.Invoices.AddAsync(invoice);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Invoice created: {InvoiceNumber}", invoiceNumber);

                return MapToInvoiceResponse(invoice);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating invoice");
                throw;
            }
        }

        public async Task<InvoiceResponse> GetInvoiceAsync(Guid invoiceId)
        {
            var invoice = await _context.Invoices
                .Include(i => i.Items)
                .Include(i => i.Payments)
                .FirstOrDefaultAsync(i => i.Id == invoiceId);

            if (invoice == null)
                throw new KeyNotFoundException($"Invoice {invoiceId} not found");

            return MapToInvoiceResponse(invoice);
        }

        public async Task<List<InvoiceResponse>> GetInvoicesAsync(int page = 1, int pageSize = 20, InvoiceStatus? status = null)
        {
            var query = _context.Invoices
                .Include(i => i.Items)
                .Include(i => i.Payments)
                .AsQueryable();

            if (status.HasValue)
            {
                query = query.Where(i => i.Status == status.Value);
            }

            var invoices = await query
                .OrderByDescending(i => i.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return invoices.Select(inv => MapToInvoiceResponse(inv)).ToList();
        }

        public async Task<InvoiceResponse> UpdateInvoiceAsync(Guid invoiceId, UpdateInvoiceRequest request)
        {
            var invoice = await _context.Invoices
                .Include(i => i.Items)
                .FirstOrDefaultAsync(i => i.Id == invoiceId);

            if (invoice == null)
                throw new KeyNotFoundException($"Invoice {invoiceId} not found");

            if (invoice.Status != InvoiceStatus.Draft)
                throw new InvalidOperationException("Only draft invoices can be updated");

            // Update basic properties
            if (request.Notes != null)
                invoice.Notes = request.Notes;

            if (request.DueDate.HasValue)
                invoice.DueDate = request.DueDate.Value;

            if (request.Status.HasValue)
                invoice.Status = request.Status.Value;

            // Update items if provided
            if (request.Items != null && request.Items.Any())
            {
                // Remove existing items
                _context.InvoiceItems.RemoveRange(invoice.Items);

                // Add new items
                var newItems = new List<InvoiceItemEntity>();
                foreach (var itemRequest in request.Items)
                {
                    var invoiceItem = new InvoiceItemEntity
                    {
                        Description = itemRequest.Description,
                        Quantity = itemRequest.Quantity,
                        UnitPrice = itemRequest.UnitPrice,
                        UnitType = itemRequest.UnitType,
                        DiscountPercentage = itemRequest.DiscountPercentage
                    };
                    invoiceItem.CalculateTotals();
                    newItems.Add(invoiceItem);
                }

                invoice.Items = newItems;

                // Recalculate invoice totals - use existing tax rate if not provided
                var taxRate = request.TaxRate ?? 0m; // Use 0 if TaxRate is null
                invoice.SubTotal = newItems.Sum(item => item.Total);
                invoice.TaxAmount = invoice.SubTotal * (taxRate / 100m);
                invoice.TotalAmount = invoice.SubTotal + invoice.TaxAmount;
            }

            invoice.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Invoice updated: {InvoiceNumber}", invoice.InvoiceNumber);

            return await GetInvoiceAsync(invoiceId);
        }

        public async Task<bool> DeleteInvoiceAsync(Guid invoiceId)
        {
            var invoice = await _context.Invoices.FindAsync(invoiceId);

            if (invoice == null)
                return false;

            if (invoice.Status != InvoiceStatus.Draft)
                throw new InvalidOperationException("Only draft invoices can be deleted");

            _context.Invoices.Remove(invoice);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Invoice deleted: {InvoiceNumber}", invoice.InvoiceNumber);

            return true;
        }

        public async Task<InvoiceResponse> SendInvoiceAsync(Guid invoiceId)
        {
            var invoice = await _context.Invoices.FindAsync(invoiceId);

            if (invoice == null)
                throw new KeyNotFoundException($"Invoice {invoiceId} not found");

            if (invoice.Status != InvoiceStatus.Draft)
                throw new InvalidOperationException("Only draft invoices can be sent");

            invoice.Status = InvoiceStatus.Sent;
            invoice.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Invoice sent: {InvoiceNumber}", invoice.InvoiceNumber);

            return await GetInvoiceAsync(invoiceId);
        }

        public async Task<InvoiceResponse> CancelInvoiceAsync(Guid invoiceId, string reason)
        {
            var invoice = await _context.Invoices.FindAsync(invoiceId);

            if (invoice == null)
                throw new KeyNotFoundException($"Invoice {invoiceId} not found");

            if (invoice.Status == InvoiceStatus.Paid || invoice.Status == InvoiceStatus.Cancelled)
                throw new InvalidOperationException("Cannot cancel paid or already cancelled invoices");

            invoice.Status = InvoiceStatus.Cancelled;
            invoice.CancelledDate = DateTime.UtcNow;
            invoice.Notes += $"\nCancelled: {reason}";
            invoice.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Invoice cancelled: {InvoiceNumber}", invoice.InvoiceNumber);

            return await GetInvoiceAsync(invoiceId);
        }

        public async Task<PaymentResponse> AddPaymentAsync(Guid invoiceId, AddPaymentRequest request)
        {
            var invoice = await _context.Invoices
                .Include(i => i.Payments)
                .FirstOrDefaultAsync(i => i.Id == invoiceId);

            if (invoice == null)
                throw new KeyNotFoundException($"Invoice {invoiceId} not found");

            // Create payment
            var payment = new PaymentEntity
            {
                InvoiceId = invoiceId,
                PaymentMethod = request.PaymentMethod,
                TransactionId = request.TransactionId,
                Amount = request.Amount,
                Notes = request.Notes,
                Status = PaymentStatus.Completed
            };

            await _context.Payments.AddAsync(payment);

            // Update invoice status if fully paid
            var balance = invoice.CalculateBalance() - request.Amount;
            if (balance <= 0)
            {
                invoice.Status = InvoiceStatus.Paid;
                invoice.PaidDate = DateTime.UtcNow;
            }
            else if (balance < invoice.TotalAmount)
            {
                invoice.Status = InvoiceStatus.PartiallyPaid;
            }

            invoice.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Payment added to invoice {InvoiceNumber}: {Amount}",
                invoice.InvoiceNumber, request.Amount);

            return MapToPaymentResponse(payment);
        }

        public async Task<List<PaymentResponse>> GetInvoicePaymentsAsync(Guid invoiceId)
        {
            var payments = await _context.Payments
                .Where(p => p.InvoiceId == invoiceId)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();

            return payments.Select(p => MapToPaymentResponse(p)).ToList();
        }

        public async Task<List<InvoiceResponse>> GetCustomerInvoicesAsync(Guid customerId)
        {
            var invoices = await _context.Invoices
                .Include(i => i.Items)
                .Include(i => i.Payments)
                .Where(i => i.CustomerId == customerId)
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync();

            return invoices.Select(inv => MapToInvoiceResponse(inv)).ToList();
        }

        public async Task<InvoiceSummaryResponse> GetSummaryAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            startDate ??= DateTime.UtcNow.AddMonths(-1);
            endDate ??= DateTime.UtcNow;

            var invoices = await _context.Invoices
                .Where(i => i.CreatedAt >= startDate && i.CreatedAt <= endDate)
                .ToListAsync();

            var summary = new InvoiceSummaryResponse
            {
                TotalInvoices = invoices.Count,
                TotalRevenue = invoices.Where(i => i.Status == InvoiceStatus.Paid || i.Status == InvoiceStatus.PartiallyPaid)
                                    .Sum(i => i.TotalAmount),
                OutstandingAmount = invoices.Where(i => i.Status == InvoiceStatus.Sent || i.Status == InvoiceStatus.Overdue)
                                          .Sum(i => i.CalculateBalance()),
                OverdueInvoices = invoices.Count(i => i.IsOverdue),
                StatusCount = invoices.GroupBy(i => i.Status.ToString())
                                    .ToDictionary(g => g.Key, g => g.Count()),
                MonthlyRevenue = invoices.Where(i => i.Status == InvoiceStatus.Paid || i.Status == InvoiceStatus.PartiallyPaid)
                                       .GroupBy(i => i.CreatedAt.ToString("yyyy-MM"))
                                       .ToDictionary(g => g.Key, g => g.Sum(i => i.TotalAmount))
            };

            return summary;
        }

        public async Task<string> GenerateInvoiceNumberAsync()
        {
            var today = DateTime.UtcNow;
            var yearMonth = today.ToString("yyyyMM");

            var lastInvoice = await _context.Invoices
                .Where(i => i.InvoiceNumber.StartsWith($"INV-{yearMonth}"))
                .OrderByDescending(i => i.InvoiceNumber)
                .FirstOrDefaultAsync();

            var sequence = 1;
            if (lastInvoice != null)
            {
                var parts = lastInvoice.InvoiceNumber.Split('-');
                if (parts.Length == 3 && int.TryParse(parts[2], out var lastSequence))
                {
                    sequence = lastSequence + 1;
                }
            }

            return $"INV-{yearMonth}-{sequence:D4}";
        }

        public async Task<byte[]> GeneratePdfAsync(Guid invoiceId)
        {
            // TODO: Implement PDF generation using QuestPDF
            // For now, return empty array asynchronously
            await Task.CompletedTask;
            return Array.Empty<byte>();
        }

        // Helper methods
        private InvoiceResponse MapToInvoiceResponse(InvoiceEntity invoice)
        {
            return new InvoiceResponse
            {
                Id = invoice.Id,
                InvoiceNumber = invoice.InvoiceNumber,
                CustomerId = invoice.CustomerId,
                CustomerName = invoice.CustomerName,
                CustomerEmail = invoice.CustomerEmail,
                IssueDate = invoice.IssueDate,
                DueDate = invoice.DueDate,
                Notes = invoice.Notes,
                SubTotal = invoice.SubTotal,
                TaxAmount = invoice.TaxAmount,
                TotalAmount = invoice.TotalAmount,
                Balance = invoice.CalculateBalance(),
                Status = invoice.Status,
                IsOverdue = invoice.IsOverdue,
                IsFullyPaid = invoice.IsFullyPaid,
                Items = invoice.Items.Select(item => MapToInvoiceItemResponse(item)).ToList(),
                Payments = invoice.Payments.Select(payment => MapToPaymentResponse(payment)).ToList(),
                CreatedAt = invoice.CreatedAt,
                UpdatedAt = invoice.UpdatedAt
            };
        }

        private InvoiceItemResponse MapToInvoiceItemResponse(InvoiceItemEntity item)
        {
            return new InvoiceItemResponse
            {
                Id = item.Id,
                Description = item.Description,
                Quantity = (int)item.Quantity,
                UnitPrice = item.UnitPrice,
                UnitType = item.UnitType ?? "unit",
                DiscountPercentage = item.DiscountPercentage,
                SubTotal = item.SubTotal,
                DiscountAmount = item.DiscountAmount,
                Total = item.Total
            };
        }

        private PaymentResponse MapToPaymentResponse(PaymentEntity payment)
        {
            return new PaymentResponse
            {
                Id = payment.Id,
                PaymentMethod = payment.PaymentMethod,
                TransactionId = payment.TransactionId,
                Amount = payment.Amount,
                PaymentDate = payment.PaymentDate,
                Notes = payment.Notes,
                Status = payment.Status
            };
        }
    }
}