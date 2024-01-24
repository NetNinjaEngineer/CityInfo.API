namespace CityInfo.API.DataTransferObjects.PointOfInterest;

public class PointOfInterestDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Category { get; set; }
    public string? Description { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public int CityId { get; set; }
}
