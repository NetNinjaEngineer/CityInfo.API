namespace CityInfo.API.Models;

public class City
{
    public City() { }
    public City(int id, string? name, string? country, int population, double latitude, double longitude)
    {
        Id = id;
        Name = name;
        Country = country;
        Population = population;
        Latitude = latitude;
        Longitude = longitude;
    }

    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Country { get; set; }
    public int Population { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public ICollection<PointOfInterest> PointOfInterests { get; set; } = new List<PointOfInterest>();
}
