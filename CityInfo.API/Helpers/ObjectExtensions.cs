using System.Dynamic;
using System.Reflection;

namespace CityInfo.API.Helpers;

public static class ObjectExtensions
{
    public static ExpandoObject ShapeObject<T>(this T source, string fields) where T : class
    {
        ArgumentNullException.ThrowIfNull(source, "source is null");
        var dataShapedObject = new ExpandoObject();
        var propertyInfoList = new List<PropertyInfo>();
        if (string.IsNullOrWhiteSpace(fields))
        {
            var propertyInfos = source.GetType().GetProperties(BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance);
            propertyInfoList.AddRange(propertyInfos);
        }
        else
        {
            var fieldsAfterSplit = fields.Split(',');
            foreach (var field in fieldsAfterSplit)
            {
                var propertyName = field.Trim();
                var propertyInfo = source.GetType().GetProperty(propertyName, BindingFlags.Public |
                    BindingFlags.Instance | BindingFlags.IgnoreCase);
                ArgumentNullException.ThrowIfNull(propertyInfo, nameof(propertyInfo));
                propertyInfoList.Add(propertyInfo);
            }
        }

        foreach (var propertyInfo in propertyInfoList)
        {
            var propertyValue = propertyInfo.GetValue(source);
            var propertyName = propertyInfo.Name;
            (dataShapedObject as IDictionary<string, object>).Add(propertyName, propertyValue);
        }

        return dataShapedObject;

    }
}
