using AutoMapper;
using CityInfo.API.ActionFilters;
using CityInfo.API.Contracts;
using CityInfo.API.DataTransferObjects.City;
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

    //[HttpGet("({ids})")]
    //public async Task<IActionResult> GetCityCollectionAsync(
    //    [FromRoute]
    //    [ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<int> ids)
    //{
    //    if (ids == null)
    //        return BadRequest();

    //    var cityCollectionToReturn = await _unitOfWork.CityRepository.GetCityCollection(ids);

    //    if (ids.Count() != cityCollectionToReturn.Count())
    //        return NotFound();

    //    return Ok(cityCollectionToReturn);
    //}


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
        return CreatedAtRoute("GetCity", new { CityId = cityForCreationRequest.Id }, cityForCreationRequest);
    }

    [HttpGet("{cityId}", Name = "GetCity")]
    public async Task<IActionResult> GetCityAsync(int cityId, string fields)
    {
        if (string.IsNullOrWhiteSpace(fields))
            return BadRequest($"{nameof(fields)} is null");

        if (!_propertyCheckerService.TypeHasProperties<City>(fields))
            return BadRequest($"Type city doesn't have properties '{fields}' .");

        var city = await _unitOfWork.CityRepository.GetCityAsync(cityId, trackChanges: true);
        if (city == null)
        {
            _logger.LogInformation($"There is no city founded with id: {cityId}");
            return NotFound();
        }
        return Ok(city.ShapeObject(fields));
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



    [HttpDelete("{cityId}")]
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

}
