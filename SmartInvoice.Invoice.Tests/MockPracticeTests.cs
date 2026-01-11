
using Moq;
using Xunit;

namespace SmartInvoice.Invoice.Tests;

public interface IMockPracticeService
{
    string GetName();
    int Calculate(int a, int b);
    Task<string> GetAsync();
}

public class MockPracticeTests
{
    [Fact]
    public void Practice_MockSetup()
    {
        // Arrange
        var mock = new Mock<IMockPracticeService>();

        // Setup mock behavior
        mock.Setup(m => m.GetName()).Returns("Test");
        mock.Setup(m => m.Calculate(It.IsAny<int>(), It.IsAny<int>()))
            .Returns((int a, int b) => a + b);

        // Act
        var service = mock.Object;
        var name = service.GetName();
        var result = service.Calculate(5, 3);

        // Assert
        Assert.Equal("Test", name);
        Assert.Equal(8, result);

        // Verify
        mock.Verify(m => m.GetName(), Times.Once);
        mock.Verify(m => m.Calculate(5, 3), Times.Once);
    }

    [Fact]
    public async Task Practice_AsyncMock()
    {
        // Arrange
        var mock = new Mock<IMockPracticeService>();

        mock.Setup(m => m.GetAsync())
            .ReturnsAsync("Async Result");

        // Act
        var service = mock.Object;
        var result = await service.GetAsync();

        // Assert
        Assert.Equal("Async Result", result);
    }
}