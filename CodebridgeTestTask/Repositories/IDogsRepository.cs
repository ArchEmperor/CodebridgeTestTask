using CodebridgeTestTask.Data.Entities;

namespace CodebridgeTestTask.Repositories;

public interface IDogsRepository
{
    Task<List<Dog>> GetDogsAsync(string sortBy="name", bool asc=true,int pageNumber=0, int pageSize=0);
    Task<Dog> AddDogAsync(Dog dog);
}