using CityInfo.API.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class CitiesController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CitiesController> _logger;

    public CitiesController(IUnitOfWork unitOfWork, ILogger<CitiesController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetCitiesAsync()
    {
        var cities = await _unitOfWork.CityRepository.GetCitiesAsync(trackChanges: true);
        return Ok(cities);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCityAsync(int id)
    {
        var city = await _unitOfWork.CityRepository.GetCityAsync(id, trackChanges: true);
        if (city == null)
        {
            _logger.LogInformation($"There is no city founded with id: {id}");
            return NotFound();
        }
        return Ok(city);
    }
}
