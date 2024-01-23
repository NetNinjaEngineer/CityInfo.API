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
}
