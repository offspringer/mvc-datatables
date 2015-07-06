using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Mvc.Datatables.Util
{
    public class JsonConvertHelper
    {
        public static void ReadJson<T>(ref T message, Dictionary<string, JProperty> otherProperties, JsonSerializer serializer, Func<Type, bool> shouldBypass)
        {
            Type objectType = message.GetType();
            ICustomTypeDescriptor typeDescriptor = TypeDescriptor.GetProvider(objectType).GetTypeDescriptor(objectType);

            foreach (PropertyDescriptor propertyDescriptor in typeDescriptor.GetProperties())
            {
                if (shouldBypass != null && shouldBypass(propertyDescriptor.ComponentType))
                    continue;

                if (propertyDescriptor.Attributes.Cast<Attribute>().OfType<JsonIgnoreAttribute>().Count() == 0)
                {
                    JsonPropertyAttribute propAttr = propertyDescriptor.Attributes.Cast<Attribute>().OfType<JsonPropertyAttribute>().SingleOrDefault();
                    string propertyName = propAttr != null ? propAttr.PropertyName : propertyDescriptor.Name;

                    if (otherProperties.ContainsKey(propertyName))
                    {
                        object value = otherProperties[propertyName].Value.ToObject(propertyDescriptor.PropertyType,
                            serializer);
                        propertyDescriptor.SetValue(message, value);
                    }
                }
            }
        }

        public static void WriteJson<T>(ref T message, JsonWriter writer, JsonSerializer serializer, Func<Type, bool> shouldBypass)
        {
            Type objectType = message.GetType();
            ICustomTypeDescriptor typeDescriptor = TypeDescriptor.GetProvider(objectType).GetTypeDescriptor(objectType);

            foreach (PropertyDescriptor propertyDescriptor in typeDescriptor.GetProperties())
            {
                if (shouldBypass != null && shouldBypass(propertyDescriptor.ComponentType))
                    continue;

                if (propertyDescriptor.Attributes.Cast<Attribute>().OfType<JsonIgnoreAttribute>().Count() == 0)
                {
                    JsonPropertyAttribute propAttr = propertyDescriptor.Attributes.Cast<Attribute>().OfType<JsonPropertyAttribute>().SingleOrDefault();
                    string propertyName = propAttr != null ? propAttr.PropertyName : propertyDescriptor.Name;
                    object propertyValue = propertyDescriptor.GetValue(message);

                    writer.WritePropertyName(propertyName);
                    serializer.Serialize(writer, propertyValue);
                }
            }
        }
    }
}
