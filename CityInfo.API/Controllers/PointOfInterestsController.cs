using CityInfo.API.Data;
using CityInfo.API.DataTransferObjects;
using CityInfo.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers;
[Route("api/cities/{cityId}/pointsofinterest")]
[ApiController]
public class PointOfInterestsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public PointOfInterestsController(ApplicationDbContext context)
    {
        _context = context;
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
            return NotFound();
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
}
