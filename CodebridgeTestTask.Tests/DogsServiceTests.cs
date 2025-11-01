using CodebridgeTestTask.Data.Entities;
using CodebridgeTestTask.Repositories;
using CodebridgeTestTask.Services;
using Moq;
using Xunit;

namespace CodebridgeTestTask.Tests;

public class DogsServiceTests
{
    private readonly Mock<IDogsRepository> _mockRepository;
    private readonly DogsService _service;

    public DogsServiceTests()
    {
        _mockRepository = new Mock<IDogsRepository>();
        _service = new DogsService(_mockRepository.Object);
    }

    [Fact]
    public async Task GetDogsAsync_CallsRepository_WithDefaultParameters()
    {
        // Arrange
        var expectedDogs = new List<Dog>
        {
            new() { Name = "Dog1", Color = "Brown", TailLength = 15, Weight = 25 }
        };
        _mockRepository.Setup(x => x.GetDogsAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(expectedDogs);

        // Act
        var result = await _service.GetDogsAsync();

        // Assert
        Assert.Single(result);
        Assert.Equal(expectedDogs, result);
        _mockRepository.Verify(x => x.GetDogsAsync("name", true, 0, 0), Times.Once);
    }

    [Theory]
    [InlineData("color", "asc", true)]
    [InlineData("weight", "desc", false)]
    public async Task GetDogsAsync_CallsRepository_WithCustomParameters(string attribute, string order, bool expectedAsc)
    {
        // Arrange
        var expectedDogs = new List<Dog>();
        _mockRepository.Setup(x => x.GetDogsAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(expectedDogs);

        // Act
        await _service.GetDogsAsync(attribute, order);

        // Assert
        _mockRepository.Verify(x => x.GetDogsAsync(attribute, expectedAsc, 0, 0), Times.Once);
    }

    [Fact]
    public async Task GetDogsAsync_UsesDefaultAttributeName_WhenAttributeIsNull()
    {
        // Act
        await _service.GetDogsAsync(null);

        // Assert
        _mockRepository.Verify(x => x.GetDogsAsync("name", true, 0, 0), Times.Once);
    }

    [Fact]
    public async Task AddDogAsync_CallsRepository()
    {
        // Arrange
        var dog = new Dog { Name = "NewDog", Color = "Black", TailLength = 10, Weight = 20 };

        // Act
        await _service.AddDogAsync(dog);

        // Assert
        _mockRepository.Verify(x => x.AddDogAsync(dog), Times.Once);
    }
}
