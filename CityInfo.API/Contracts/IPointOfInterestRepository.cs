using CityInfo.API.Models;

namespace CityInfo.API.Contracts;

public interface IPointOfInterestRepository
{
    Task<IEnumerable<PointOfInterest>> GetAllPointsOfInterestAsync(bool trackChanges);
    Task<IEnumerable<PointOfInterest>> GetPointsOfInterestForCityAsync(int cityId, bool trackChanges);
    Task<PointOfInterest> GetPointOfInterestAsync(int cityId, int pointOfInterestId, bool trackChanges);
    void DeletePointOfInterest(PointOfInterest pointOfInterest);
    void UpdatePointOfInterest(PointOfInterest pointOfInterest);
    void CreatePointOfInterest(PointOfInterest pointOfInterest);
}
