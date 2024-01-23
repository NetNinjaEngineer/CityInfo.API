using AutoMapper;
using CityInfo.API.Contracts;
using CityInfo.API.DataTransferObjects;
using CityInfo.API.Models;
using CityInfo.API.RequestFeatures;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

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

    [HttpPost(Name = "CreateCity")]
    public async Task<IActionResult> CreateCityAsync([FromBody] CityForCreationDto requestModel)
    {
        var cityForCreationRequest = _mapper.Map<City>(requestModel);
        _unitOfWork.CityRepository.CreateCity(cityForCreationRequest);
        await _unitOfWork.SaveAsync();
        return CreatedAtRoute("GetCity", new { cityForCreationRequest.Id }, cityForCreationRequest);
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

    [HttpPut("{cityId}", Name = "UpdateCity")]
    public async Task<IActionResult> UpdateCityAsync([FromBody] CityForUpdateDto cityForUpdateDto, int cityId)
    {
        var cityExists = await CheckCityExists(cityId);
        if (!cityExists)
            return NotFound();
        var cityForUpdate = await _unitOfWork.CityRepository
            .GetCityAsync(cityId, trackChanges: true);
        var cityForUpdateToReturn = _mapper.Map(cityForUpdateDto, cityForUpdate);
        _unitOfWork.CityRepository.UpdateCity(cityForUpdateToReturn);
        await _unitOfWork.SaveAsync();
        return NoContent();
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

    [HttpGet]
    [Route("GetCitiesByCityParamaters")]
    public async Task<IActionResult> GetCitiesAsync([FromQuery] CityRequestParameters cityRequestParameters)
    {
        var pagedResult = await _unitOfWork.CityRepository.GetCitiesAsync(cityRequestParameters);
        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(pagedResult.MetaData));
        return Ok(pagedResult);
    }

    private async Task<bool> CheckCityExists(int cityId)
      => await _unitOfWork.CityRepository
      .GetCityAsync(cityId, true) == null ? false : true;
}
