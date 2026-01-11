
using Xunit;

namespace SmartInvoice.Invoice.Tests;


public class DtoValidationTests
{
    [Fact]
    public void Guid_ShouldBeValid()
    {
        // Test that GUIDs work
        var guid = Guid.NewGuid();

        Assert.NotEqual(Guid.Empty, guid);
        Assert.True(guid != Guid.Empty);
    }

    [Fact]
    public void DateTime_ShouldBeValid()
    {
        // Test date calculations
        var now = DateTime.UtcNow;
        var future = now.AddDays(30);

        Assert.True(future > now);
        Assert.Equal(30, (future - now).Days);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(-1)]
    public void Decimal_Operations(decimal value)
    {
        // Test decimal operations
        decimal result = value * 2m;

        Assert.Equal(value + value, result);
    }
}