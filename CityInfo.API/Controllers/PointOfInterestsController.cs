using AutoMapper;
using CityInfo.API.Contracts;
using CityInfo.API.DataTransferObjects;
using CityInfo.API.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers;
[Route("api/cities/{cityId}/pointsofinterest")]
[ApiController]
public class PointOfInterestsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<PointOfInterestsController> _logger;
    private readonly IMapper _mapper;

    public PointOfInterestsController(IUnitOfWork unitOfWork,
        ILogger<PointOfInterestsController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }

    [HttpGet(Name = "GetPointsOfInterest")]
    public async Task<IActionResult> GetPointsOfInterest(int cityId)
    {
        try
        {
            var city = await _unitOfWork.CityRepository.GetCityAsync(cityId, true);
            if (city == null)
                return NotFound();

            var pointsOfInterestForCity = await _unitOfWork
                .PointOfInterestRepository.GetPointsOfInterestForCityAsync(cityId, true);

            var pointsOfInterestToReturn = _mapper.Map<IEnumerable<PointOfInterestDto>>(pointsOfInterestForCity);
            return Ok(pointsOfInterestToReturn);
        }
        catch (Exception ex)
        {
            _logger.LogCritical($"Exception while getting points of interest for city with id {cityId}", ex);
            return StatusCode(500, "A problem happened while handing your request");
        }
    }

    [HttpGet("{pointOfInterestId}", Name = "GetPointOfInterest")]
    public async Task<IActionResult> GetPointOfInterest(int cityId, int pointOfInterestId)
    {
        var cityExists = await CheckCityExists(cityId);
        if (!cityExists)
        {
            _logger.LogInformation($"City with id {cityId} wasn't founded when accessing point of interest");
            return NotFound();
        }
        var pointOfInterest = await _unitOfWork.PointOfInterestRepository
            .GetPointOfInterestAsync(cityId, pointOfInterestId, true);

        if (pointOfInterest == null)
            return NotFound();

        var pointOfInterestToReturn = _mapper.Map<PointOfInterestDto>(pointOfInterest);

        return Ok(pointOfInterestToReturn);
    }


    [HttpPost(Name = nameof(CreatePointOfInterest))]
    public async Task<IActionResult> CreatePointOfInterest(int cityId,
        [FromBody] PointOfInterestForCreationDto pointOfInterest)
    {
        if (!ModelState.IsValid)
            return BadRequest();

        var cityExists = await CheckCityExists(cityId);
        if (!cityExists)
            return NotFound();

        var point = _mapper.Map<PointOfInterest>(pointOfInterest);
        point.CityId = cityId;

        _unitOfWork.PointOfInterestRepository.CreatePointOfInterest(point);
        await _unitOfWork.SaveAsync();

        var pointOfInterestToReturn = _mapper.Map<PointOfInterestDto>(point);

        return CreatedAtRoute("GetPointOfInterest", new
        {
            CityId = cityId,
            PointOfInterestId = pointOfInterestToReturn.Id
        }, pointOfInterestToReturn);

    }


    [HttpPut("{pointOfInterestId}")]
    public async Task<IActionResult> UpdatePointOfInterest(int cityId,
        int pointOfInterestId, PointOfInterestForUpdateDto dto)
    {
        var valid = await CheckCityExists(cityId);
        if (!valid) return NotFound();

        var pointOfInterest = await _unitOfWork
            .PointOfInterestRepository
            .GetPointOfInterestAsync(cityId, pointOfInterestId, true);

        if (pointOfInterest is null)
            return NotFound();

        pointOfInterest.Id = pointOfInterestId;
        pointOfInterest.CityId = cityId;
        pointOfInterest.Longitude = dto.Longitude;
        pointOfInterest.Latitude = dto.Latitude;
        pointOfInterest.Category = dto.Category;
        pointOfInterest.Description = dto.Description;
        pointOfInterest.Name = dto.Name;

        _unitOfWork.PointOfInterestRepository.UpdatePointOfInterest(pointOfInterest);

        await _unitOfWork.SaveAsync();

        return NoContent();
    }

    [HttpPatch("{pointOfInterestId}")]
    public async Task<IActionResult> PartiallyUpdatePointOfInterest(int cityId, int pointOfInterestId,
        JsonPatchDocument<PointOfInterestForUpdateDto> patchDocument)
    {
        var valid = await CheckCityExists(cityId);
        if (!valid)
            return NotFound();

        var pointOfInterest = await _unitOfWork
            .PointOfInterestRepository
            .GetPointOfInterestAsync(cityId, pointOfInterestId, true);

        if (pointOfInterest == null)
            return NotFound();

        var pointOfInterestToPatch = new PointOfInterestForUpdateDto
        {
            Category = pointOfInterest.Category,
            Description = pointOfInterest.Description,
            Latitude = pointOfInterest.Latitude,
            Longitude = pointOfInterest.Longitude
        };

        patchDocument.ApplyTo(pointOfInterestToPatch, ModelState);

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        _mapper.Map<PointOfInterest>(pointOfInterestToPatch);

        await _unitOfWork.SaveAsync();

        return NoContent();

    }

    [HttpDelete("{pointOfInterestId}")]
    public async Task<IActionResult> DeletePointOfInterest(int cityId, int pointOfInterestId)
    {
        var valid = await CheckCityExists(cityId);
        if (!valid) return NotFound();
        var pointOfInterest = await _unitOfWork.PointOfInterestRepository
            .GetPointOfInterestAsync(cityId, pointOfInterestId, trackChanges: true);
        if (pointOfInterest == null)
            return NotFound();
        _unitOfWork.PointOfInterestRepository.DeletePointOfInterest(pointOfInterest);
        await _unitOfWork.SaveAsync();
        return NoContent();
    }

    private async Task<bool> CheckCityExists(int cityId)
        => await _unitOfWork.CityRepository
        .GetCityAsync(cityId, true) == null ? false : true;
}
