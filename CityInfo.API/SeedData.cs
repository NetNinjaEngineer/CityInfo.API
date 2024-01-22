using CityInfo.API.Models;

namespace CityInfo.API;

public static class SeedData
{
    public static IEnumerable<City> GetCities()
    {
        return new List<City>
        {
            new(1, "New York", "USA", 8398748, 40.7128, -74.0060),
            new(2, "London", "UK", 8982000, 51.5099, -0.1180),
            new(3, "Tokyo", "Japan", 13929286, 35.6895, 139.6917)
        };
    }

    public static IEnumerable<PointOfInterest> GetPointsOfInterests()
    {
        return new List<PointOfInterest>
        {
            new PointOfInterest(1, "Central Park", "Park", "A large urban park in Manhattan.", 40.7851, -73.9683, 1),
            new PointOfInterest(2, "Empire State Building", "Landmark", "Iconic skyscraper in Midtown Manhattan.", 40.7484, -73.9857, 1),
            new PointOfInterest(3, "Hyde Park", "Park", "One of the largest parks in London.", 51.5074, -0.1657, 2),
            new PointOfInterest(4, "British Museum", "Museum", "World-famous museum of art and antiquities.", 51.5194, -0.1270, 2),
            new PointOfInterest(5, "Ueno Park", "Park", "Famous public park in central Tokyo.", 35.7146, 139.7732, 3),
            new PointOfInterest(6, "Tokyo Tower", "Landmark", "Iconic communications and observation tower.", 35.6586, 139.7454, 3)
        };
    }
}
