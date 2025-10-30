using System.Text.Json;
using CodebridgeTestTask.Data.Entities;
using CodebridgeTestTask.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CodebridgeTestTask.Controllers;

[ApiController]
[Route("/")]
public class DogsController(IDogsService dogs) : ControllerBase
{
    Dictionary<string, Dog> orderBy = new Dictionary<string, Dog>();
    private const string Version = "Dogshouseservice.Version1.0.1";
    [HttpGet("ping")]
    public IActionResult Ping()
    {
        return Ok(Version);
    }

    [HttpGet("dogs")]
    public async Task<IActionResult> GetDogs([FromQuery]string attribute="name", [FromQuery]string order="asc", [FromQuery]int page=0, [FromQuery]int pageSize=0)
    {
        try
        {
            var result = await dogs.GetDogsAsync(attribute, order, page, pageSize);
            return Ok(JsonSerializer.Serialize(result));
        }
        catch (Exception)
        {
            return StatusCode(500);
        }
    }

    [HttpPost("dog")]
    public async Task<IActionResult> AddDog([FromBody]Dog dog)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        try
        {
            await dogs.AddDogAsync(dog);
            return Ok("Successfully added new dog");
        }
        catch (DbUpdateException e)
        {
            return BadRequest("Dog name already taken");
        }
    }
}