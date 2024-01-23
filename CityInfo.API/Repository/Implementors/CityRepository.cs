using CityInfo.API.Contracts;
using CityInfo.API.Data;
using CityInfo.API.Models;
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

    public async Task<IEnumerable<City>> GetCitiesAsync(string? name, string? searchQuery)
    {
        if (string.IsNullOrEmpty(name) && string.IsNullOrWhiteSpace(searchQuery))
            return await GetCitiesAsync(true);

        var cityCollection = await GetCitiesAsync(true);
        if (!string.IsNullOrWhiteSpace(name))
        {
            name = name.Trim();
            cityCollection = cityCollection!.Where(city =>
                city.Name!.ToLower() == name.ToLower());
        }

        if (!string.IsNullOrWhiteSpace(searchQuery))
        {
            searchQuery = searchQuery.Trim();
            cityCollection = cityCollection!.Where(city =>
             city.Name.ToLower().Contains(searchQuery.ToLower()) ||
             city.Country!.ToLower().Contains(searchQuery.ToLower()));
        }

        return cityCollection;

    }
}
