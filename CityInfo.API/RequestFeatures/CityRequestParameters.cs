namespace CityInfo.API.RequestFeatures;

public class CityRequestParameters : RequestParameters
{
    public string? SearchTerm { get; set; }
    public string? FilterTerm { get; set; }
}