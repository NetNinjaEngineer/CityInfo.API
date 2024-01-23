namespace CityInfo.API.Contracts;

public interface IUnitOfWork
{
    ICityRepository CityRepository { get; }
    IPointOfInterestRepository PointOfInterestRepository { get; }
    Task SaveAsync();
}
