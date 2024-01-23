using AutoMapper;
using CityInfo.API.Data;
using CityInfo.API.DataTransferObjects;
using CityInfo.API.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers;
[Route("api/cities/{cityId}/pointsofinterest")]
[ApiController]
public class PointOfInterestsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<PointOfInterestsController> _logger;
    private readonly IMapper _mapper;

    public PointOfInterestsController(ApplicationDbContext context, IMapper mapper, ILogger<PointOfInterestsController> logger)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }

    [HttpGet(Name = "GetPointsOfInterest")]
    public ActionResult<IEnumerable<PointOfInterestDto>> GetPointsOfInterest(int cityId)
    {
        var validCity = _context.Cities.Any(c => c.Id == cityId);
        if (!validCity)
            return NotFound();
        var pointsOfInterest = _context.PointOfInterests.Where(c => c.CityId == cityId);

        return Ok(pointsOfInterest.Select(p => new PointOfInterestDto
        {
            Id = p.Id,
            CityId = p.CityId,
            Name = p.Name,
            Longitude = p.Longitude,
            Latitude = p.Latitude,
            Description = p.Description,
            Category = p.Category
        }));
    }

    [HttpGet("{pointOfInterestId}", Name = "GetPointOfInterest")]
    public ActionResult<PointOfInterestDto> GetPointOfInterest(int cityId, int pointOfInterestId)
    {
        var city = _context.Cities.FirstOrDefault(c => c.Id == cityId);
        if (city == null)
        {
            _logger.LogInformation($"City with id {cityId} wasn't founded when accessing point of interest");
            return NotFound();
        }
        var pointOfInterest = _context.PointOfInterests.FirstOrDefault(
            p => p.Id == pointOfInterestId);
        if (pointOfInterest == null)
            return NotFound();

        var pointOfInterestToReturn = new PointOfInterestDto
        {
            Id = pointOfInterestId,
            Category = pointOfInterest.Category,
            Description = pointOfInterest.Description,
            Latitude = pointOfInterest.Latitude,
            Longitude = pointOfInterest.Longitude,
            Name = pointOfInterest.Name,
            CityId = pointOfInterest.CityId
        };

        return Ok(pointOfInterestToReturn);
    }


    [HttpPost(Name = nameof(CreatePointOfInterest))]
    public ActionResult<PointOfInterestDto> CreatePointOfInterest(int cityId,
        [FromBody] PointOfInterestForCreationDto pointOfInterest)
    {
        if (!ModelState.IsValid)
            return BadRequest();

        var city = _context.Cities.FirstOrDefault(c => c.Id == cityId);
        if (city == null)
            return NotFound();

        var point = new PointOfInterest
        {
            Name = pointOfInterest.Name,
            Latitude = pointOfInterest.Latitude,
            Longitude = pointOfInterest.Longitude,
            Category = pointOfInterest.Category,
            Description = pointOfInterest.Description,
            CityId = cityId,
        };

        _context.PointOfInterests.Add(point);
        _context.SaveChanges();

        var pointOfInterestToReturn = new PointOfInterestDto
        {
            Id = point.Id,
            CityId = point.CityId,
            Name = point.Name,
            Longitude = point.Longitude,
            Latitude = point.Latitude,
            Description = point.Description,
            Category = point.Category
        };

        return CreatedAtRoute("GetPointOfInterest", new
        {
            CityId = cityId,
            PointOfInterestId = pointOfInterestToReturn.Id
        }, pointOfInterestToReturn);

    }


    [HttpPut("{pointOfInterestId}")]
    public ActionResult UpdatePointOfInterest(int cityId,
        int pointOfInterestId, PointOfInterestForUpdateDto dto)
    {
        var city = _context.Cities.FirstOrDefault(c => c.Id == cityId);
        if (city == null) return NotFound();

        var pointOfInterest = _context.PointOfInterests.FirstOrDefault(p =>
        p.Id == pointOfInterestId);
        if (pointOfInterest is null)
            return NotFound();

        pointOfInterest.Longitude = dto.Longitude;
        pointOfInterest.Latitude = dto.Latitude;
        pointOfInterest.Description = dto.Description;
        pointOfInterest.Category = dto.Category;
        pointOfInterest.Name = dto.Name;


        _context.SaveChanges();
        return NoContent();
    }

    [HttpPatch("{pointOfInterestId}")]
    public ActionResult PartiallyUpdatePointOfInterest(int cityId, int pointOfInterestId,
        JsonPatchDocument<PointOfInterestForUpdateDto> patchDocument)
    {
        var validCityId = _context.Cities.Any(c => c.Id == cityId);
        if (!validCityId)
            return NotFound();

        var pointOfInterest = _context.PointOfInterests.FirstOrDefault(p =>
            p.Id == pointOfInterestId);

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

        _context.SaveChanges();

        return NoContent();

    }

    [HttpDelete("{pointOfInterestId}")]
    public ActionResult DeletePointOfInterest(int cityId, int pointOfInterestId)
    {
        var validCity = _context.Cities.Any(c => c.Id == cityId);
        if (!validCity) return NotFound();
        var pointOfInterest = _context.PointOfInterests.FirstOrDefault(p =>
            p.Id == pointOfInterestId);
        if (pointOfInterest == null)
            return NotFound();
        _context.PointOfInterests.Remove(pointOfInterest);
        _context.SaveChanges();
        return NoContent();
    }
}
