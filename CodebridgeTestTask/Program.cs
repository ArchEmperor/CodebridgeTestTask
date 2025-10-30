using CodebridgeTestTask.Data;
using CodebridgeTestTask.Data.Entities;
using CodebridgeTestTask.Middleware;
using CodebridgeTestTask.Repositories;
using CodebridgeTestTask.Services;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;

Env.Load();
var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(options=>
    options.UseNpgsql(builder.Configuration.
        GetConnectionString("Postgres")));
builder.Services.AddScoped<IDogsRepository, DogsRepository>();
builder.Services.AddScoped<DogsService>();
var requestsPerSecond = builder.Configuration.GetValue<int>(
    "RateLimiting:RequestsPerSecond", 10);
builder.Services.AddGlobalRateLimiting(requestsPerSecond);
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseRateLimiter();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
    if (!db.Dogs.Any())
    {
        var dogs = new List<Dog>
        {
            new Dog { Name = "Rex", Color = "Black", TailLength = 12, Weight = 25 },
            new Dog { Name = "Buddy", Color = "Brown", TailLength = 9, Weight = 20 },
            new Dog { Name = "Charlie", Color = "White", TailLength = 10, Weight = 18 },
            new Dog { Name = "Max", Color = "Golden", TailLength = 11, Weight = 22 },
            new Dog { Name = "Rocky", Color = "Gray", TailLength = 8, Weight = 19 },
            new Dog { Name = "Lucky", Color = "Beige", TailLength = 10, Weight = 23 },
            new Dog { Name = "Cooper", Color = "Black & White", TailLength = 13, Weight = 27 },
            new Dog { Name = "Jack", Color = "Brown & White", TailLength = 7, Weight = 17 },
            new Dog { Name = "Leo", Color = "Dark Brown", TailLength = 10, Weight = 24 },
            new Dog { Name = "Toby", Color = "Light Gray", TailLength = 9, Weight = 21 }
        };

        db.Dogs.AddRange(dogs);
        await db.SaveChangesAsync();
    }
}
app.Run();