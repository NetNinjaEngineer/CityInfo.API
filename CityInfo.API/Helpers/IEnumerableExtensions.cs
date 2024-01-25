using System.Dynamic;
using System.Reflection;

namespace CityInfo.API.Helpers;

public static class IEnumerableExtensions
{
    public static IEnumerable<ExpandoObject> ShapData<TSource>(this IEnumerable<TSource> source, string fields)
    {
        ArgumentNullException.ThrowIfNull(source, nameof(source));
        var expandoObjectList = new List<ExpandoObject>();
        var propertyInfoList = new List<PropertyInfo>();
        if (string.IsNullOrWhiteSpace(fields))
        {
            var propertyInfos = typeof(TSource).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            propertyInfoList.AddRange(propertyInfos);
        }
        else
        {
            var fieldsAfterSplit = fields.Split(',');
            foreach (var field in fieldsAfterSplit)
            {
                var propertyName = field.Trim();
                var propertyInfo = typeof(TSource).GetProperty(propertyName,
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                ArgumentNullException.ThrowIfNull(propertyInfo, nameof(propertyInfo));
                propertyInfoList.Add(propertyInfo);
            }
        }

        foreach (var item in source)
        {
            var dataShapedObject = new ExpandoObject();
            foreach (var propertyInfo in propertyInfoList)
            {
                var propertyValue = propertyInfo.GetValue(item);
                (dataShapedObject as IDictionary<string, object>).Add(propertyInfo.Name, propertyValue);
            }


            expandoObjectList.Add(dataShapedObject);

        }

        return expandoObjectList;
    }
}
