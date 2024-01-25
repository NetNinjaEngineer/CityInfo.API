using AutoMapper;
using CityInfo.API.ActionFilters;
using CityInfo.API.Contracts;
using CityInfo.API.DataTransferObjects.City;
using CityInfo.API.DataTransferObjects.Link;
using CityInfo.API.Helpers;
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
    private readonly IPropertyCheckerService _propertyCheckerService;

    public CitiesController(IUnitOfWork unitOfWork, ILogger<CitiesController> logger, IMapper mapper, IPropertyCheckerService propertyCheckerService)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
        _propertyCheckerService = propertyCheckerService;
    }

    [HttpPost]
    [Route("CreateCityCollection")]
    public async Task<IActionResult> CreateCityCollectionAsync([FromBody] IEnumerable<CityForCreationDto> cityForCreationCollection)
    {
        var cityCollectionEntities = _mapper.Map<IEnumerable<City>>(cityForCreationCollection);
        foreach (var city in cityCollectionEntities)
            _unitOfWork.CityRepository.CreateCity(city);

        await _unitOfWork.SaveAsync();
        return Ok(cityCollectionEntities);
    }

    [HttpGet(Name = "GetCitiesAsync")]
    [HttpHead]
    public async Task<IActionResult> GetCitiesAsync()
    {
        var cities = await _unitOfWork.CityRepository.GetCitiesAsync(trackChanges: true);
        return Ok(cities);
    }

    [HttpPost(Name = "CreateCity")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> CreateCityAsync([FromBody] CityForCreationDto requestModel)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var cityForCreationRequest = _mapper.Map<City>(requestModel);
        _unitOfWork.CityRepository.CreateCity(cityForCreationRequest);
        await _unitOfWork.SaveAsync();
        var cityToReturn = _mapper.Map<City>(cityForCreationRequest);
        var links = CreateLinksForCity(cityToReturn.Id, null!);
        var linkedResourceToReturn = cityToReturn.ShapeObject(null!)
            as IDictionary<string, object>;
        linkedResourceToReturn.Add("links", links);

        return CreatedAtRoute("GetCity", new { CityId = (int)linkedResourceToReturn["Id"] }, linkedResourceToReturn);
    }

    [HttpGet("{cityId}", Name = "GetCity")]
    public async Task<IActionResult> GetCityAsync(int cityId, string? fields)
    {
        if (fields != null)
            if (!_propertyCheckerService.TypeHasProperties<City>(fields))
                return BadRequest($"Type city doesn't have properties '{fields}' .");

        var city = await _unitOfWork.CityRepository.GetCityAsync(cityId, trackChanges: true);
        if (city == null)
        {
            _logger.LogInformation($"There is no city founded with id: {cityId}");
            return NotFound();
        }

        var cityLinks = CreateLinksForCity(cityId, fields!);
        var linkedResourceToReturn = city.ShapeObject(fields!) as IDictionary<string, object>;
        linkedResourceToReturn.Add("links", cityLinks);
        return Ok(linkedResourceToReturn);
    }


    [HttpPut("{cityId}", Name = "UpdateCity")]
    [ServiceFilter(typeof(CityExistsFilterAttribute), Order = 2)]
    [ServiceFilter(typeof(ValidationFilterAttribute), Order = 1)]
    public async Task<IActionResult> UpdateCityAsync([FromBody] CityForUpdateDto cityForUpdateDto, int cityId)
    {
        var cityForUpdate = await _unitOfWork.CityRepository
            .GetCityAsync(cityId, trackChanges: true);
        var cityForUpdateToReturn = _mapper.Map(cityForUpdateDto, cityForUpdate);
        _unitOfWork.CityRepository.UpdateCity(cityForUpdateToReturn);
        await _unitOfWork.SaveAsync();
        return NoContent();
    }



    [HttpDelete("{cityId}", Name = "DeleteCity")]
    [ServiceFilter(typeof(CityExistsFilterAttribute))]
    public async Task<IActionResult> DeleteCityAsync(int cityId)
    {
        var cityForDelete = await _unitOfWork.CityRepository.GetCityAsync(cityId,
            trackChanges: true);
        _unitOfWork.CityRepository.DeleteCity(cityForDelete);
        await _unitOfWork.SaveAsync();
        return NoContent();
    }

    [HttpGet]
    [Route("GetCitiesByCityParamaters")]
    public IActionResult GetCities([FromQuery] CityRequestParameters cityRequestParameters)
    {
        if (!_propertyCheckerService.TypeHasProperties<City>(cityRequestParameters.Fields))
            return BadRequest($"Type city doesn't have properties '{cityRequestParameters.Fields}' .");

        var pagedResult = _unitOfWork.CityRepository.GetCities(cityRequestParameters);
        pagedResult.MetaData.PreviousPageLink = (pagedResult.MetaData.HasPrevious) ?
            CreateCitiesResourceUri(cityRequestParameters, ResourceUriType.PreviousPage) : null;

        pagedResult.MetaData.NextPageLink = (pagedResult.MetaData.HasNext) ?
             CreateCitiesResourceUri(cityRequestParameters, ResourceUriType.NextPage) : null;

        pagedResult.MetaData.Fields = (string.IsNullOrEmpty(cityRequestParameters.Fields)) ? null : cityRequestParameters.Fields;

        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(pagedResult.MetaData));
        return Ok(pagedResult.ShapData(cityRequestParameters.Fields!));
    }

    [HttpOptions]
    public IActionResult GetCityOptions()
    {
        Response.Headers.Append("Allow", "GET,OPTIONS,POST,DELETE,PUT");
        return Ok();
    }


    private async Task<bool> CheckCityExists(int cityId)
      => await _unitOfWork.CityRepository
      .GetCityAsync(cityId, true) == null ? false : true;

    private string CreateCitiesResourceUri(CityRequestParameters cityRequestParameters,
        ResourceUriType resourceUriType)
    {
        switch (resourceUriType)
        {
            case ResourceUriType.PreviousPage:
                return Url.Link("GetCitiesAsync", new
                {
                    fields = cityRequestParameters.Fields,
                    pageNumber = cityRequestParameters.PageNumber - 1,
                    pageSize = cityRequestParameters.PageSize,
                    searchTerm = cityRequestParameters.SearchTerm,
                    filterTerm = cityRequestParameters.FilterTerm
                })!;

            case ResourceUriType.NextPage:
                return Url.Link("GetCitiesAsync", new
                {
                    fields = cityRequestParameters.Fields,
                    pageNumber = cityRequestParameters.PageNumber + 1,
                    pageSize = cityRequestParameters.PageSize,
                    searchTerm = cityRequestParameters.SearchTerm,
                    filterTerm = cityRequestParameters.FilterTerm
                })!;

            default:
                return Url.Link("GetCitiesAsync", new
                {
                    fields = cityRequestParameters.Fields,
                    pageNumber = cityRequestParameters.PageNumber - 1,
                    pageSize = cityRequestParameters.PageSize,
                    searchTerm = cityRequestParameters.SearchTerm,
                    filterTerm = cityRequestParameters.FilterTerm
                })!;
        }
    }

    private IEnumerable<LinkDto> CreateLinksForCity(int cityId, string fields)
    {
        var cityLinks = new List<LinkDto>();
        if (string.IsNullOrWhiteSpace(fields))
        {

            cityLinks.Add(new LinkDto
            {
                Href = Url.Link("GetCity", new { cityId }),
                Rel = "self",
                Method = "GET"
            });
        }

        else
        {
            cityLinks.Add(new LinkDto
            {
                Href = Url.Link("GetCity", new { cityId, fields }),
                Rel = "self",
                Method = "GET"
            });
        }

        cityLinks.AddRange([
            new()
            {
                Href = Url.Link("GetCity", new { cityId }),
                Rel = "get_city",
                Method = "GET"
            },
            new()
            {
                Href = Url.Link("DeleteCity", new { cityId }),
                Rel = "delete_city",
                Method = "DELETE"
            },
            new()
            {
                Href = Url.Link("UpdateCity", new { cityId }),
                Rel = "update_city",
                Method = "PUT"
            },
            new()
            {
                Href = Url.Link("CreateCity", new{ }),
                Rel = "create_city",
                Method = "POST"
            }
        ]);

        return cityLinks;

    }
}
