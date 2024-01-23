using AutoMapper;
using CityInfo.API.Contracts;
using CityInfo.API.DataTransferObjects;
using CityInfo.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class CitiesController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CitiesController> _logger;
    private readonly IMapper _mapper;

    public CitiesController(IUnitOfWork unitOfWork, ILogger<CitiesController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetCitiesAsync()
    {
        var cities = await _unitOfWork.CityRepository.GetCitiesAsync(trackChanges: true);
        return Ok(cities);
    }

    [HttpGet("{cityId}", Name = "GetCity")]
    public async Task<IActionResult> GetCityAsync(int cityId)
    {
        var city = await _unitOfWork.CityRepository.GetCityAsync(cityId, trackChanges: true);
        if (city == null)
        {
            _logger.LogInformation($"There is no city founded with id: {cityId}");
            return NotFound();
        }
        return Ok(city);
    }

    [HttpPost(Name = "CreateCity")]
    public async Task<IActionResult> CreateCityAsync([FromBody] CityForCreationDto requestModel)
    {
        var cityForCreationRequest = _mapper.Map<City>(requestModel);
        _unitOfWork.CityRepository.CreateCity(cityForCreationRequest);
        await _unitOfWork.SaveAsync();
        return CreatedAtRoute("GetCity", new { cityForCreationRequest.Id }, cityForCreationRequest);
    }

    [Route("SearchFilterCities")]
    [HttpGet]
    public async Task<IActionResult> GetCitiesAsync([FromQuery] string? name, string? searchQuery)
    {
        var filteredCities = await _unitOfWork.CityRepository.GetCitiesAsync(name, searchQuery);
        return Ok(filteredCities);
    }

    [HttpDelete("{cityId}")]
    public async Task<IActionResult> DeleteCityAsync(int cityId)
    {
        var existCity = await CheckCityExists(cityId);
        if (!existCity)
            return NotFound();
        var cityForDelete = await _unitOfWork.CityRepository.GetCityAsync(cityId,
            trackChanges: true);
        _unitOfWork.CityRepository.DeleteCity(cityForDelete);
        await _unitOfWork.SaveAsync();
        return NoContent();
    }

    private async Task<bool> CheckCityExists(int cityId)
      => await _unitOfWork.CityRepository
      .GetCityAsync(cityId, true) == null ? false : true;
}
