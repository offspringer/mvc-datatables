using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace Mvc.Datatables.Reflection
{
	public static class DataTablesTypeInfo
	{
		public static DataTablesPropertyInfo[] Properties(Type type)
		{
			var typeDescriptor = TypeDescriptor.GetProvider(type).GetTypeDescriptor(type);
			if (typeDescriptor == null)
				return new DataTablesPropertyInfo[0];

			var infos = from PropertyDescriptor pi in typeDescriptor.GetProperties()
						where pi.Attributes.Cast<Attribute>().OfType<JsonIgnoreAttribute>().Count() == 0
						let jsonProperty = pi.Attributes.Cast<Attribute>().OfType<JsonPropertyAttribute>().SingleOrDefault()
						select new DataTablesPropertyInfo(pi, jsonProperty);

			return infos.ToArray();
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
				dictionary[pi.Name] = pi.PropertyInfo.GetValue(row);

			return dictionary;
		}

		public static OrderedDictionary ToOrderedDictionary(T row)
		{
			var dictionary = new OrderedDictionary();

			foreach (var pi in Properties)
				dictionary[pi.Name] = pi.PropertyInfo.GetValue(row);

			return dictionary;
		}
	}
}