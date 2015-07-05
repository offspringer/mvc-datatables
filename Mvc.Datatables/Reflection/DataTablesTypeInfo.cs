using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Mvc.Datatables.Reflection
{
	public static class DataTablesTypeInfo
	{
		private static ConcurrentDictionary<Type, DataTablesPropertyInfo[]> _propertiesCache = new ConcurrentDictionary<Type, DataTablesPropertyInfo[]>();

		public static DataTablesPropertyInfo[] Properties(Type type)
		{
			return _propertiesCache.GetOrAdd(type, t =>
			{
				var infos = from pi in t.GetProperties()
							where pi.GetCustomAttribute<JsonIgnoreAttribute>() == null
							let jsonProperty = (pi.GetCustomAttributes()).OfType<JsonProperty>().SingleOrDefault()
							select new DataTablesPropertyInfo(pi, jsonProperty);
				return infos.ToArray();
			});
		}
	}

	public static class DataTablesTypeInfo<T>
	{
		public static DataTablesPropertyInfo[] Properties { get; private set; }

		static DataTablesTypeInfo()
		{
			Properties = DataTablesTypeInfo.Properties(typeof(T));
		}

		public static Dictionary<string, object> ToDictionary(T row)
		{
			var dictionary = new Dictionary<string, object>();

			foreach (var pi in Properties)
				dictionary[pi.Name] = pi.PropertyInfo.GetValue(row, null);

			return dictionary;
		}

		public static OrderedDictionary ToOrderedDictionary(T row)
		{
			var dictionary = new OrderedDictionary();

			foreach (var pi in Properties)
				dictionary[pi.Name] = pi.PropertyInfo.GetValue(row, null);

			return dictionary;
		}
	}
}