namespace CityInfo.API.Models;

public class PointOfInterest
{
    public PointOfInterest() { }
    public PointOfInterest(int id, string? name, string? category, string? description, double latitude, double longitude, int cityId)
    {
        Id = id;
        Name = name;
        Category = category;
        Description = description;
        Latitude = latitude;
        Longitude = longitude;
        CityId = cityId;
    }

    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Category { get; set; }
    public string? Description { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }

    public City City { get; set; }
    public int CityId { get; set; }
}
