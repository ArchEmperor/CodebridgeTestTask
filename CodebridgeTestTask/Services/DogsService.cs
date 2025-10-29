using CodebridgeTestTask.Data.Entities;
using CodebridgeTestTask.Repositories;

namespace CodebridgeTestTask.Services;

public class DogsService(IDogsRepository repository)
{
    public async Task<List<Dog>> GetDogsAsync(string? attribute="name",string order="asc" ,int pageNumber=0, int pageSize=0)
    {
        var asc = order!="desc";
        if (string.IsNullOrEmpty(attribute))
            attribute = "name";
        return await repository.GetDogsAsync(attribute, asc, pageNumber, pageSize);
    }
    public async Task AddDogAsync(Dog dog)
    {
        await repository.AddDogAsync(dog);
    }
}