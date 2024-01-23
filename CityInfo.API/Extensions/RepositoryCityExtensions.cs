using CityInfo.API.Models;

namespace CityInfo.API.Extensions;

public static class RepositoryCityExtensions
{
    public static IQueryable<City> Filter(this IQueryable<City> query, string? name) =>
        string.IsNullOrEmpty(name) ? query :
        query.Where(city => city.Name.ToLower() == name.ToLower());

    public static IQueryable<City> Search(this IQueryable<City> query, string? searchQuery)
        => string.IsNullOrEmpty(searchQuery) ? query :
        query.Where(c => c.Name.ToLower().Contains(searchQuery.Trim().ToLower()));

}
