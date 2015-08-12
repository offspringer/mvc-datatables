using Newtonsoft.Json;
using System;
using System.ComponentModel;

namespace Mvc.Datatables.Reflection
{
    public class DataTablesPropertyInfo
    {
        public PropertyDescriptor PropertyInfo { get; private set; }
        public JsonPropertyAttribute JsonProperty { get; private set; }

        public Type Type
        {
            get { return this.PropertyInfo.PropertyType; }
        }

        public string Name
        {
            get { return this.JsonProperty != null ? this.JsonProperty.PropertyName : this.PropertyInfo.Name; }
        }

        public DataTablesPropertyInfo(PropertyDescriptor propertyInfo, JsonPropertyAttribute jsonProperty)
        {
            this.PropertyInfo = propertyInfo;
        }
    }
}