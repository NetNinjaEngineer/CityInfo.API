namespace CityInfo.API.RequestFeatures;

public sealed class PagedList<T> : List<T>
{
    public MetaData MetaData { get; set; }

    public PagedList(List<T> pagedItems, int totalCount, int pageNumber, int pageSize)
    {
        MetaData = new MetaData()
        {
            CurrentPage = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
        };

        AddRange(pagedItems);
    }

    public static PagedList<T> ToPagedList(List<T> source, int pageNumber, int pageSize)
    {
        var totalItemsCount = source.Count();
        var pagedItems = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
        return new PagedList<T>(pagedItems, totalItemsCount, pageNumber, pageSize);
    }
}
