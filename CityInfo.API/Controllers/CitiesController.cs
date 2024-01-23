using CityInfo.API.Data;
using CityInfo.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class CitiesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public CitiesController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public ActionResult<IEnumerable<City>> GetCitiesAsync()
    {
        var cities = _context.Cities.Include(c => c.PointOfInterests).ToList();
        return Ok(cities);
    }

    [HttpGet("{id}")]
    public ActionResult<City> GetCity(int id)
    {
        var city = _context.Cities.Include(c => c.PointOfInterests).FirstOrDefault(c => c.Id == id);
        if (city == null)
            return NotFound();
        return Ok(city);
    }
}
