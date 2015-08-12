using Mvc.Datatables.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Mvc.Datatables.Serialization
{
	public class FilterRequestConverter : JsonConverter
	{
		public override bool CanConvert(Type objectType)
		{
			return objectType.GetInterfaces().Any(x => x == typeof(IFilterRequest));
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			JObject jsonObject = JObject.Load(reader);
			IEnumerable<JProperty> properties = jsonObject.Properties();
			Dictionary<string, JProperty> otherProperties = new Dictionary<string, JProperty>();

			IFilterRequest message = Activator.CreateInstance(objectType) as IFilterRequest;

			foreach (JProperty property in properties)
			{
				if (property.Name == "draw")
					message.Draw = property.Value.ToObject<int>();

				else if (property.Name == "start")
					message.Start = property.Value.ToObject<int>();

				else if (property.Name == "length")
					message.Length = property.Value.ToObject<int>();

				else if (property.Name == "search[value]")
					message.Search.Value = property.Value.ToObject<string>();

				else if (property.Name == "search[regex]")
					message.Search.IsRegex = property.Value.ToObject<bool>();

				else if (property.Name.StartsWith("order"))
					ReadSortConfiguration(ref message, property);

				else if (property.Name.StartsWith("columns"))
					ReadColumnConfiguration(ref message, property);

				else
					otherProperties.Add(property.Name, property);
			}

			JsonConvertHelper.ReadJson(message, otherProperties, serializer,
				prop => JsonConvertHelper.GetPropertiesFromType(typeof(IFilterRequest)).Select(x => x.Name).Contains(prop.Name));

			return message;
		}

		private void ReadSortConfiguration(ref IFilterRequest message, JProperty property)
		{
			Match match = Regex.Match(property.Name, @"order\[([0-9]+)\](.+)");
			if (match.Success && match.Groups.Count == 3)
			{
				int index = Convert.ToInt32(match.Groups[1].Value);
				string propertyName = match.Groups[2].Value;

				while (index >= message.Sort.Count)
					message.Sort.Add(new Sort());

				if (propertyName == "[column]")
					message.Sort[index].Column = property.Value.ToObject<int>();

				else if (propertyName == "[dir]")
					message.Sort[index].Direction = property.Value.ToObject<string>().AsSortDirection();
			}
		}

		private void ReadColumnConfiguration(ref IFilterRequest message, JProperty property)
		{
			Match match = Regex.Match(property.Name, @"columns\[([0-9]+)\](.+)");
			if (match.Success && match.Groups.Count == 3)
			{
				int index = Convert.ToInt32(match.Groups[1].Value);
				string propertyName = match.Groups[2].Value;

				IColumn currentColumn = null;

				if (!message.HasColumn(index))
				{
					currentColumn = new Column();
					message.Columns.Add(index, currentColumn);
				}
				else
					currentColumn = message.GetColumn(index);

				if (propertyName == "[data]")
					currentColumn.Data = property.Value.ToObject<string>();

				else if (propertyName == "[name]")
					currentColumn.Name = property.Value.ToObject<string>();

				else if (propertyName == "[searchable]")
					currentColumn.Searchable = property.Value.ToObject<bool>();

				else if (propertyName == "[orderable]")
					currentColumn.Sortable = property.Value.ToObject<bool>();

				else if (propertyName == "[search][value]")
					currentColumn.Search.Value = property.Value.ToObject<string>();

				else if (propertyName == "[search][regex]")
					currentColumn.Search.IsRegex = property.Value.ToObject<bool>();
			}
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			writer.WriteStartObject();

			IFilterRequest message = value as IFilterRequest;

			if (message != null)
			{
				writer.WritePropertyName("draw");
				writer.WriteValue(message.Draw);

				writer.WritePropertyName("start");
				writer.WriteValue(message.Start);

				writer.WritePropertyName("length");
				writer.WriteValue(message.Length);

				if (!string.IsNullOrWhiteSpace(message.Search.Value))
				{
					writer.WritePropertyName("search[value]");
					writer.WriteValue(message.Search.Value);

					writer.WritePropertyName("search[regex]");
					writer.WriteValue(message.Search.IsRegex);
				}

				for (int i = 0; i < message.Sort.Count; i++)
				{
					writer.WritePropertyName(string.Format("order[{0}][column]", i));
					writer.WriteValue(message.Sort[i].Column);

					writer.WritePropertyName(string.Format("order[{0}][dir]", i));
					writer.WriteValue(message.Sort[i].Direction.AsString());
				}

				foreach (KeyValuePair<int, IColumn> column in message.Columns.OrderBy(x => x.Key))
				{
					writer.WritePropertyName(string.Format("columns[{0}][data]", column.Key));
					writer.WriteValue(column.Value.Data);

					writer.WritePropertyName(string.Format("columns[{0}][name]", column.Key));
					writer.WriteValue(column.Value.Name);

					writer.WritePropertyName(string.Format("columns[{0}][searchable]", column.Key));
					writer.WriteValue(column.Value.Searchable);

					writer.WritePropertyName(string.Format("columns[{0}][orderable]", column.Key));
					writer.WriteValue(column.Value.Sortable);

					if (!string.IsNullOrWhiteSpace(column.Value.Search.Value))
					{
						writer.WritePropertyName(string.Format("columns[{0}][search][value]", column.Key));
						writer.WriteValue(column.Value.Search.Value);

						writer.WritePropertyName(string.Format("columns[{0}][search][regex]", column.Key));
						writer.WriteValue(column.Value.Search.IsRegex);
					}
				}
			}

			JsonConvertHelper.WriteJson(message, writer, serializer,
				prop => JsonConvertHelper.GetPropertiesFromType(typeof(IFilterRequest)).Select(x => x.Name).Contains(prop.Name));

			writer.WriteEndObject();
		}
	}
}
