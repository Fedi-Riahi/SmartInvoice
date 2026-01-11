using Xunit;

namespace SmartInvoice.Invoice.Tests;

public class SimpleTests
{
    [Fact]
    public void Math_ShouldWork()
    {
        // Arrange
        int a = 5;
        int b = 3;

        // Act
        int result = a + b;

        // Assert
        Assert.Equal(8, result);
    }

    [Fact]
    public void Boolean_ShouldWork()
    {
        // Arrange
        bool isTrue = true;

        // Act & Assert
        Assert.True(isTrue);
    }
}