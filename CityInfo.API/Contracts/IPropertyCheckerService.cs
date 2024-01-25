namespace CityInfo.API.Contracts;

public interface IPropertyCheckerService
{
    bool TypeHasProperties<T>(string fields);
}
