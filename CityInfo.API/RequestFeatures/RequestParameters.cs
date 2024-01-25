namespace CityInfo.API.RequestFeatures;

public abstract class RequestParameters
{
    private const int _maxPageSize = 50;
    private int _pageSize = 10;
    public int PageNumber { get; set; } = 1;
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = (value > _maxPageSize) ? _maxPageSize : value;
    }

    public string? Fields { get; set; }
}
