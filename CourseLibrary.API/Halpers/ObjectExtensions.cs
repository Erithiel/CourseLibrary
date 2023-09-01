using System.Dynamic;
using System.Reflection;

namespace CourseLibrary.API.Halpers
{
	public static class ObjectExtensions
	{
		public static ExpandoObject ShapeData<TSource>(this TSource source, string fields)
		{

			if (source == null)
			{
				throw new ArgumentNullException(nameof(source));
			}
			var expendoObject = new ExpandoObject();

			var propertyInfoList = new List<PropertyInfo>();

			if (string.IsNullOrWhiteSpace(fields))
			{
				// all public properties should be in the ExpandoObject
				var propertyInfos = typeof(TSource)
					.GetProperties(BindingFlags.IgnoreCase
								   | BindingFlags.Public | BindingFlags.Instance);

				propertyInfoList.AddRange(propertyInfos);

			}
			else
			{

				var fieldsAfterSplit = fields.Split(',');

				foreach (var field in fieldsAfterSplit)
				{
					var propertyName = field.Trim();

					var propertyInfo = typeof(TSource).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

					if (propertyInfo is null)
					{
						throw new Exception($"Property {propertyName} wasn't found on" +
											$" {typeof(TSource)}");
					}
					propertyInfoList.Add(propertyInfo);
				}


			}

			foreach (var property in propertyInfoList)
			{
				var propertyValue = property.GetValue(source);

				((IDictionary<string, object>)expendoObject)
					.Add(property.Name, propertyValue);

			}

			return expendoObject;
		}


	}
}
