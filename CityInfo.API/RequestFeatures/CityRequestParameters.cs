namespace CityInfo.API.RequestFeatures;

public sealed class CityRequestParameters : RequestParameters
{
    public string? SearchTerm { get; set; }
    public string? FilterTerm { get; set; }
}