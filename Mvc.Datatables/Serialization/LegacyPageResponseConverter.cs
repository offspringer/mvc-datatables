using Mvc.Datatables.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Mvc.Datatables.Serialization
{
    public class LegacyPageResponseConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(PageResponse) || objectType.IsSubclassOf(typeof(PageResponse));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jsonObject = JObject.Load(reader);
            IEnumerable<JProperty> properties = jsonObject.Properties();
            Dictionary<string, JProperty> otherProperties = new Dictionary<string, JProperty>();

            PageResponse message = Activator.CreateInstance(objectType) as PageResponse;

            foreach (JProperty property in properties)
            {
                if (property.Name == "sEcho")
                    message.Draw = property.Value.ToObject<int>();

                else if (property.Name == "iTotalRecords")
                    message.TotalRecords = property.Value.ToObject<int>();

                else if (property.Name == "iTotalDisplayRecords")
                    message.TotalFilteredRecords = property.Value.ToObject<int>();

                else if (property.Name == "aaData")
                {
                    if (objectType.IsGenericType)
                    {
                        Type genericType = objectType.GetGenericArguments()[0].MakeArrayType();
                        object genericArray = property.Value.ToObject(genericType);
                        message.Data = (object[])genericArray;
                    }
                    else
                        message.Data = property.Value.ToObject<object[]>();
                }

                else
                    otherProperties.Add(property.Name, property);
            }

            JsonConvertHelper.ReadJson(ref message, otherProperties, serializer, type =>
            {
                return type == typeof(PageResponse)
                    || type.IsGenericType && type.GetGenericTypeDefinition() == typeof(PageResponse<>);
            });

            return message;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteStartObject();

            PageResponse message = value as PageResponse;

            if (message != null)
            {
                writer.WritePropertyName("sEcho");
                writer.WriteValue(message.Draw);

                writer.WritePropertyName("iTotalRecords");
                writer.WriteValue(message.TotalRecords);

                writer.WritePropertyName("iTotalDisplayRecords");
                writer.WriteValue(message.TotalFilteredRecords);

                writer.WritePropertyName("aaData");
                serializer.Serialize(writer, message.Data);
            }

            JsonConvertHelper.WriteJson(ref message, writer, serializer, type =>
            {
                return type == typeof(PageResponse)
                    || type.IsGenericType && type.GetGenericTypeDefinition() == typeof(PageResponse<>);
            });

            writer.WriteEndObject();
        }
    }
}
