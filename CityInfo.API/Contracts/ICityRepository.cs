using CityInfo.API.Models;

namespace CityInfo.API.Contracts;

public interface ICityRepository
{
    Task<IEnumerable<City>> GetCitiesAsync(bool trackChanges);
    Task<IEnumerable<City>> GetCitiesAsync(string? name, string? searchQuery);
    Task<City> GetCityAsync(int cityId, bool trackChanges);
    void DeleteCity(City city);
    void UpdateCity(City city);
    void CreateCity(City city);

}
