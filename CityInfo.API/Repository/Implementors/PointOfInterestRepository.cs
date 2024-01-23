using CityInfo.API.Contracts;
using CityInfo.API.Data;
using CityInfo.API.Models;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.API.Repository.Implementors;

public class PointOfInterestRepository(ApplicationDbContext context)
    : GenericRepository<PointOfInterest>(context),
    IPointOfInterestRepository
{
    public void CreatePointOfInterest(PointOfInterest pointOfInterest)
        => Create(pointOfInterest);

    public void DeletePointOfInterest(PointOfInterest pointOfInterest)
        => Delete(pointOfInterest);

    public async Task<IEnumerable<PointOfInterest>> GetAllPointsOfInterestAsync(bool trackChanges)
        => await FindAll(trackChanges).ToListAsync();

    public async Task<PointOfInterest> GetPointOfInterestAsync(int cityId, int pointOfInterestId, bool trackChanges)
        => await FindByCondition(p => p.CityId == cityId && p.Id ==
        pointOfInterestId, trackChanges).SingleOrDefaultAsync();

    public async Task<IEnumerable<PointOfInterest>> GetPointsOfInterestForCityAsync(int cityId, bool trackChanges)
        => await FindByCondition(point =>
            point.CityId == cityId, trackChanges).ToListAsync();

    public void UpdatePointOfInterest(PointOfInterest pointOfInterest)
        => Update(pointOfInterest);
}
