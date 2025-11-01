using System.Text.Json;
using CodebridgeTestTask.Controllers;
using CodebridgeTestTask.Data.Entities;
using CodebridgeTestTask.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace CodebridgeTestTask.Tests;

public class DogsControllerTests: IClassFixture<WebApplicationFactory<Program>>

{
    private readonly Mock<IDogsService> _mockDogsService;
    private readonly DogsController _controller;
    private readonly WebApplicationFactory<Program> _factory;

    public DogsControllerTests(WebApplicationFactory<Program> factory)
    {
        _mockDogsService = new Mock<IDogsService>();
        _controller = new DogsController(_mockDogsService.Object);
        _factory = factory;
    }

    [Fact]
    public void Ping_ReturnsVersion()
    {
        // Act
        var result = _controller.Ping() as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Dogshouseservice.Version1.0.1", result.Value);
    }
    

    [Fact]
    public async Task GetDogs_ReturnsOkResult_WithDogs()
    {
        // Arrange
        var dogs = new List<Dog>
        {
            new() { Name = "Dog1", Color = "Brown", TailLength = 15, Weight = 25 }
        };
        _mockDogsService.Setup(x => x.GetDogsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(dogs);

        // Act
        var result = await _controller.GetDogs() as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        var serializedDogs = JsonSerializer.Deserialize<List<Dog>>(result.Value.ToString());
        Assert.Single(serializedDogs);
        Assert.Equal("Dog1", serializedDogs[0].Name);
    }

    [Fact]
    public async Task GetDogs_ReturnsInternalServerError_WhenExceptionOccurs()
    {
        // Arrange
        _mockDogsService.Setup(x => x.GetDogsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
            .ThrowsAsync(new Exception());

        // Act
        var result = await _controller.GetDogs() as StatusCodeResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(500, result.StatusCode);
    }

    [Fact]
    public async Task AddDog_ReturnsOkResult_WhenDogIsValid()
    {
        // Arrange
        var dog = new Dog { Name = "NewDog", Color = "Black", TailLength = 10, Weight = 20 };

        // Act
        var result = await _controller.AddDog(dog) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Successfully added new dog", result.Value);
    }

    [Fact]
    public async Task AddDog_ReturnsBadRequest_WhenDogNameIsDuplicate()
    {
        // Arrange
        var dog = new Dog { Name = "ExistingDog", Color = "Black", TailLength = 10, Weight = 20 };
        _mockDogsService.Setup(x => x.AddDogAsync(It.IsAny<Dog>()))
            .ThrowsAsync(new DbUpdateException());

        // Act
        var result = await _controller.AddDog(dog) as BadRequestObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Dog name already taken", result.Value);
    }

    [Fact]
    public async Task Ping_ExceedsRateLimit_ReturnsTooManyRequests()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act - Send 11 requests (exceeding the 10 request limit)
        for (int i = 0; i < 10; i++)
        {
            await client.GetAsync("/ping");
        }
        var response = await client.GetAsync("/ping");

        // Assert
        Assert.Equal(429, (int)response.StatusCode); // 429 Too Many Requests
    }
}
