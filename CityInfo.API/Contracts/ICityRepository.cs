using CityInfo.API.Models;
using CityInfo.API.RequestFeatures;

namespace CityInfo.API.Contracts;

public interface ICityRepository
{
    Task<IEnumerable<City>> GetCitiesAsync(bool trackChanges);
    Task<PagedList<City>> GetCitiesAsync(CityRequestParameters cityRequestParameters);
    Task<City> GetCityAsync(int cityId, bool trackChanges);
    void DeleteCity(City city);
    void UpdateCity(City city);
    void CreateCity(City city);
}
