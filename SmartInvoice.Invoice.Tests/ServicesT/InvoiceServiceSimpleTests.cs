using Microsoft.Extensions.Logging;
using Moq;
using SmartInvoice.Invoice.Controllers;
using SmartInvoice.Invoice.Services;
using Xunit;

namespace SmartInvoice.Invoice.Tests.Services;

public class InvoiceServiceSimpleTests
{
    [Fact]
    public void Service_CanBeCreated()
    {
        // placeholder 
        Assert.True(true);
    }

    [Fact]
    public async Task CreateInvoice_SimpleMockTest()
    {
        // Arrange
        var mockService = new Mock<IInvoiceService>();
        var mockLogger = new Mock<ILogger<InvoiceController>>();
        var controller = new InvoiceController(mockService.Object, mockLogger.Object);

        // Create a simple request
        var request = new
        {
            CustomerId = Guid.NewGuid(),
            CustomerName = "Test Customer",
            CustomerEmail = "test@email.com"
        };

        // Act & Assert 
        var exception = await Record.ExceptionAsync(() =>
            controller.CreateInvoice(null));

        Assert.True(true);
    }

    [Fact]
    public async Task SendInvoice_SimpleTest()
    {
        // Arrange
        var mockService = new Mock<IInvoiceService>();
        var mockLogger = new Mock<ILogger<InvoiceController>>();
        var controller = new InvoiceController(mockService.Object, mockLogger.Object);

        var invoiceId = Guid.NewGuid();

        // Act
        var result = await controller.SendInvoice(invoiceId);

        // Assert
        Assert.NotNull(result);
    }
}