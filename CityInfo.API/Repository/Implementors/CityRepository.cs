using CityInfo.API.Contracts;
using CityInfo.API.Data;
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

    public PagedList<City> GetCities(CityRequestParameters cityRequestParameters)
    {
        ArgumentNullException.ThrowIfNull(nameof(cityRequestParameters));
        var cities = GetCitiesAsync(true).Result.ToList();
        if (string.IsNullOrEmpty(cityRequestParameters.SearchTerm)
            && string.IsNullOrEmpty(cityRequestParameters.FilterTerm))
            return PagedList<City>.ToPagedList(cities,
                cityRequestParameters.PageNumber, cityRequestParameters.PageSize);

        if (!string.IsNullOrWhiteSpace(cityRequestParameters.FilterTerm) ||
          !string.IsNullOrWhiteSpace(cityRequestParameters.SearchTerm))
        {
            var filterTerm = cityRequestParameters.FilterTerm?.Trim();
            var searchTerm = cityRequestParameters.SearchTerm?.Trim();

            cities = cities
                .Where(c => c.Name!.Equals(filterTerm, StringComparison.CurrentCultureIgnoreCase) ||
                (
                    c.Name!.Contains(searchTerm!, StringComparison.CurrentCultureIgnoreCase) ||
                    c.Country!.Contains(searchTerm!, StringComparison.CurrentCultureIgnoreCase)
                )).ToList();
        }

        return PagedList<City>.ToPagedList(cities,
            cityRequestParameters.PageNumber,
            cityRequestParameters.PageSize);
    }

    public void CreateCityCollection(IEnumerable<City> cityCollection)
    {
        var collection = cityCollection.ToList();
        collection.ForEach(city =>
        {
            ArgumentNullException.ThrowIfNull(city);
            Create(city);
        });

    }

    public async Task<IEnumerable<City>> GetCityCollection(IEnumerable<int> ids)
    {
        var cityCollection = new List<City>();
        foreach (var id in ids)
        {
            var city = await GetCityAsync(id, true);
            cityCollection.Add(city);
        }
        return cityCollection;
    }
}
