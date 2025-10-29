using System.Linq.Expressions;
using CodebridgeTestTask.Data;
using CodebridgeTestTask.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CodebridgeTestTask.Repositories;

public class DogsRepository(AppDbContext context) : IDogsRepository
{
    public async Task<List<Dog>>GetDogsAsync(string sortBy="name", bool asc=true,int pageNumber=0, int pageSize=0)
    {
        var query = context.Dogs.AsQueryable();
        query = sortBy switch
        {
            "name" => asc ? query.OrderBy(d => d.Name) : query.OrderByDescending(d => d.Name),
            "color" => asc ? query.OrderBy(d => d.Color) : query.OrderByDescending(d => d.Color),
            "tail_length" => asc ? query.OrderBy(d => d.TailLength) : query.OrderByDescending(d => d.TailLength),
            "weight" => asc ? query.OrderBy(d => d.Weight) : query.OrderByDescending(d => d.Weight),
            _ => query.OrderBy(d => d.Name)
        };
        if (pageNumber > 0 && pageSize > 0)
        {
            query = query.Skip((pageNumber-1) * pageSize).Take(pageSize);
        }
        return await query.ToListAsync();
    }

    public async Task<Dog> AddDogAsync(Dog dog)
    {
         var result = await context.Dogs.AddAsync(dog);
         await context.SaveChangesAsync().ConfigureAwait(false);
         return result.Entity;
    }
}