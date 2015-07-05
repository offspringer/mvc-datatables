using System;
using System.Reflection;
using Newtonsoft.Json.Serialization;

namespace Mvc.Datatables.Reflection
{
	public class DataTablesPropertyInfo
	{
		public PropertyInfo PropertyInfo { get; private set; }
		public JsonProperty JsonProperty { get; private set; }

		public Type Type
		{
			get { return this.PropertyInfo.PropertyType; }
		}

		public string Name
		{
			get { return this.JsonProperty != null ? this.JsonProperty.PropertyName : this.PropertyInfo.Name; }
		}

		public DataTablesPropertyInfo(PropertyInfo propertyInfo, JsonProperty jsonProperty)
		{
			this.PropertyInfo = propertyInfo;
		}
	}
}