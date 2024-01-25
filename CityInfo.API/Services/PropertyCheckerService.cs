using CityInfo.API.Contracts;
using System.Reflection;

namespace CityInfo.API.Services;

public class PropertyCheckerService : IPropertyCheckerService
{
    public bool TypeHasProperties<T>(string fields)
    {
        ArgumentNullException.ThrowIfNull(fields);
        if (string.IsNullOrWhiteSpace(fields))
            return true;
        var fieldsAfterSplit = fields.Split(',');
        foreach (var field in fieldsAfterSplit)
        {
            var propertyName = field.Trim();
            var propertyInfo = typeof(T).GetProperty(propertyName,
                BindingFlags.Instance | BindingFlags.IgnoreCase | BindingFlags.Public);

            if (propertyInfo == null)
                return false;
        }

        return true;
    }
}
