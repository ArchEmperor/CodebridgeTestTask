using CodebridgeTestTask.Data.Entities;
using CodebridgeTestTask.Repositories;

namespace CodebridgeTestTask.Services;

public interface IDogsService
{
    public Task<List<Dog>> GetDogsAsync(string? attribute = "name", string order = "asc", int pageNumber = 0,
        int pageSize = 0);
    public Task AddDogAsync(Dog dog);
}