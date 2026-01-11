using Xunit;

namespace SmartInvoice.Invoice.Tests;

public class FailingTestExample
{
    [Fact]
    public void PassingTest_ShouldPass()
    {
        int a = 5;
        int b = 3;
        int result = a + b;
        Assert.Equal(8, result);
    }

    [Fact]
    public void FailingTest_ShouldFail()
    {
        int a = 5;
        int b = 3;
        int result = a + b;
        Assert.Equal(7, result);
    }

    [Fact]
    public void AnotherPassingTest()
    {
        string name = "Invoice";
        Assert.Equal("Invoice", name);
    }

    [Fact]
    public void BooleanTest_ShouldFail()
    {
        bool isTrue = true;
        Assert.False(isTrue);
    }

    [Fact]
    public void NullTest_ShouldPass()
    {
        object obj = null;
        Assert.Null(obj);
    }

    [Fact]
    public void NotNullTest_ShouldFail()
    {
        object obj = new object();
        Assert.Null(obj);
    }
}   