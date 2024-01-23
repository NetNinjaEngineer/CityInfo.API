using CityInfo.API.Contracts;
using CityInfo.API.Data;
using CityInfo.API.Extensions;
using CityInfo.API.Models;
using CityInfo.API.RequestFeatures;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.API.Repository.Implementors;

public class CityRepository : GenericRepository<City>, ICityRepository
{
    public CityRepository(ApplicationDbContext context)
        : base(context) { }

    public void CreateCity(City city) => Create(city);

    public void DeleteCity(City city) => Delete(city);

    public async Task<IEnumerable<City>> GetCitiesAsync(bool trackChanges)
        => await FindAll(trackChanges, c => c.PointOfInterests).ToListAsync();
    public void UpdateCity(City city) => Update(city);
    public async Task<City> GetCityAsync(int cityId, bool trackChanges)
        => await FindByCondition(c => c.Id == cityId, trackChanges,
            c => c.PointOfInterests)
        .FirstOrDefaultAsync();

    public async Task<PagedList<City>> GetCitiesAsync(CityRequestParameters cityRequestParameters)
    {
        var cities = await GetCitiesAsync(true);
        if (string.IsNullOrEmpty(cityRequestParameters.SearchTerm) &&
             string.IsNullOrEmpty(cityRequestParameters.FilterTerm))
            return PagedList<City>.ToPagedList(cities.ToList(), cityRequestParameters.PageNumber, cityRequestParameters.PageSize);

        var collection = cities;

        if (!string.IsNullOrWhiteSpace(cityRequestParameters.FilterTerm) ||
            !string.IsNullOrWhiteSpace(cityRequestParameters.SearchTerm))
        {
            collection = collection.AsQueryable().Filter(cityRequestParameters.FilterTerm);
            collection = collection.AsQueryable().Search(cityRequestParameters.SearchTerm);
        }


        return PagedList<City>.ToPagedList(collection.ToList(), cityRequestParameters.PageNumber, cityRequestParameters.PageSize);
    }
}
