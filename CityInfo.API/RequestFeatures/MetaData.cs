namespace CityInfo.API.RequestFeatures;

public sealed class MetaData
{
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int TotalCount { get; set; }
    public int PageSize { get; set; }
    public bool HasNext => CurrentPage < TotalPages;
    public bool HasPrevious => CurrentPage > 1;
    public string? PreviousPageLink { get; set; }
    public string? NextPageLink { get; set; }
}
