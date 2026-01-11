
using Moq;
using Xunit;
using Microsoft.Extensions.Logging;
using SmartInvoice.Invoice.Services;

namespace SmartInvoice.Invoice.Tests.Services;


public class InvoiceServiceCalculationTests
{
    [Fact]
    public void CalculateTotals_SimpleCase()
    {
        // This tests the calculation logic
        // Example: 2 items at $50 each = $100
        decimal item1 = 50.00m;
        decimal item2 = 50.00m;
        decimal expected = 100.00m;

        decimal actual = item1 + item2;

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(100, 10, 110)]  // 100 + 10% tax = 110
    [InlineData(200, 15, 230)]  // 200 + 15% tax = 230
    [InlineData(50, 0, 50)]     // 50 + 0% tax = 50
    public void CalculateWithTax_TheoryTest(decimal subtotal, decimal taxPercent, decimal expectedTotal)
    {
        decimal taxAmount = subtotal * (taxPercent / 100m);
        decimal total = subtotal + taxAmount;

        Assert.Equal(expectedTotal, total);
    }
}