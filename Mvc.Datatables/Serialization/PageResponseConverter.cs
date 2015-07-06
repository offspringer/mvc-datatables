using Mvc.Datatables.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Mvc.Datatables.Serialization
{
    public class PageResponseConverter : JsonConverter
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
                if (property.Name == "draw")
                    message.Draw = property.Value.ToObject<int>();

                else if (property.Name == "recordsTotal")
                    message.TotalRecords = property.Value.ToObject<int>();

                else if (property.Name == "recordsFiltered")
                    message.TotalFilteredRecords = property.Value.ToObject<int>();

                else if (property.Name == "data")
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

                else if (property.Name == "error")
                    message.Error = property.Value.ToObject<string>();

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
                writer.WritePropertyName("draw");
                writer.WriteValue(message.Draw);

                writer.WritePropertyName("recordsTotal");
                writer.WriteValue(message.TotalRecords);

                writer.WritePropertyName("recordsFiltered");
                writer.WriteValue(message.TotalFilteredRecords);

                writer.WritePropertyName("data");
                serializer.Serialize(writer, message.Data);

                if (!string.IsNullOrWhiteSpace(message.Error))
                {
                    writer.WritePropertyName("error");
                    writer.WriteValue(message.Error);
                }
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
