using CityInfo.API.Contracts;
using CityInfo.API.Data;
using CityInfo.API.Models;
using CityInfo.API.RequestFeatures;
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

    public PagedList<PointOfInterest> GetPointsOfInterest(PointOfInterestRequestParameters pointOfInterestParameters)
    {
        ArgumentNullException.ThrowIfNull(pointOfInterestParameters);
        var pointsOfInterest = GetAllPointsOfInterestAsync(true).Result.ToList();
        if (string.IsNullOrEmpty(pointOfInterestParameters.SearchTerm)
            && string.IsNullOrEmpty(pointOfInterestParameters.FilterTerm))
            return PagedList<PointOfInterest>.ToPagedList(pointsOfInterest,
                pointOfInterestParameters.PageNumber, pointOfInterestParameters.PageSize);

        if (!string.IsNullOrWhiteSpace(pointOfInterestParameters.FilterTerm) ||
          !string.IsNullOrWhiteSpace(pointOfInterestParameters.SearchTerm))
        {
            var filterTerm = pointOfInterestParameters.FilterTerm?.Trim();
            var searchTerm = pointOfInterestParameters.SearchTerm?.Trim();

            pointsOfInterest = pointsOfInterest
                .Where(point => point.Name!.Equals(filterTerm, StringComparison.CurrentCultureIgnoreCase) ||
                (
                    point.Name!.Contains(searchTerm!, StringComparison.CurrentCultureIgnoreCase) ||
                    point.Category!.Contains(searchTerm!, StringComparison.CurrentCultureIgnoreCase) ||
                    point.Description!.Contains(searchTerm!, StringComparison.CurrentCultureIgnoreCase)
                 )).ToList();
        }

        return PagedList<PointOfInterest>.ToPagedList(pointsOfInterest,
            pointOfInterestParameters.PageNumber,
            pointOfInterestParameters.PageSize);
    }

    public async Task<IEnumerable<PointOfInterest>> GetPointsOfInterestForCityAsync(int cityId, bool trackChanges)
        => await FindByCondition(point =>
            point.CityId == cityId, trackChanges).ToListAsync();

    public void UpdatePointOfInterest(PointOfInterest pointOfInterest)
        => Update(pointOfInterest);
}
