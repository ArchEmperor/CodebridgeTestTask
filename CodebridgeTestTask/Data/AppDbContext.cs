using CodebridgeTestTask.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CodebridgeTestTask.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options):DbContext(options)
{
    public DbSet<Dog> Dogs { get; set; }

}