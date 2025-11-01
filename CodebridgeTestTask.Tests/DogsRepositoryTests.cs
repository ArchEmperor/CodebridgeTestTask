using CodebridgeTestTask.Data;
using CodebridgeTestTask.Data.Entities;
using CodebridgeTestTask.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CodebridgeTestTask.Tests;

public class DogsRepositoryTests
{
    private readonly AppDbContext _context;
    private readonly DogsRepository _repository;

    public DogsRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _repository = new DogsRepository(_context);
    }

    [Fact]
    public async Task GetDogsAsync_ReturnsAllDogs_OrderedByName()
    {
        // Arrange
        await SeedTestData();

        // Act
        var result = await _repository.GetDogsAsync();

        // Assert
        Assert.Equal(3, result.Count);
        Assert.Equal("Buddy", result[0].Name);
        Assert.Equal("Max", result[1].Name);
        Assert.Equal("Rex", result[2].Name);
    }

    [Theory]
    [InlineData("weight", true)]
    [InlineData("weight", false)]
    [InlineData("color", true)]
    [InlineData("tail_length", false)]
    public async Task GetDogsAsync_SortsCorrectly(string sortBy, bool asc)
    {
        // Arrange
        await SeedTestData();

        // Act
        var result = await _repository.GetDogsAsync(sortBy, asc);

        // Assert
        Assert.Equal(3, result.Count);
        if (sortBy == "weight" && asc)
        {
            Assert.Equal(20, result[0].Weight);
            Assert.Equal(25, result[1].Weight);
            Assert.Equal(30, result[2].Weight);
        }
        else if (sortBy == "weight" && !asc)
        {
            Assert.Equal(30, result[0].Weight);
            Assert.Equal(25, result[1].Weight);
            Assert.Equal(20, result[2].Weight);
        }
    }

    [Fact]
    public async Task GetDogsAsync_WithPagination_ReturnsCorrectPage()
    {
        // Arrange
        await SeedTestData();

        // Act
        var result = await _repository.GetDogsAsync(pageNumber: 1, pageSize: 2);

        // Assert
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task AddDogAsync_AddsDogToDatabase()
    {
        // Arrange
        var newDog = new Dog { Name = "Spot", Color = "White", TailLength = 12, Weight = 22 };

        // Act
        var result = await _repository.AddDogAsync(newDog);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Spot", result.Name);
        Assert.Equal(1, await _context.Dogs.CountAsync());
    }

    private async Task SeedTestData()
    {
        var dogs = new List<Dog>
        {
            new() { Name = "Max", Color = "Brown", TailLength = 15, Weight = 25 },
            new() { Name = "Rex", Color = "Black", TailLength = 20, Weight = 30 },
            new() { Name = "Buddy", Color = "White", TailLength = 10, Weight = 20 }
        };

        await _context.Dogs.AddRangeAsync(dogs);
        await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
