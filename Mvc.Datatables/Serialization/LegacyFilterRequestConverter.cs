using Mvc.Datatables.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mvc.Datatables.Serialization
{
	public class LegacyFilterRequestConverter : JsonConverter
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
				if (property.Name == "sEcho")
					message.Draw = property.Value.ToObject<int>();

				else if (property.Name == "iDisplayStart")
					message.Start = property.Value.ToObject<int>();

				else if (property.Name == "iDisplayLength")
					message.Length = property.Value.ToObject<int>();

				else if (property.Name == "sSearch")
					message.Search.Value = property.Value.ToObject<string>();

				else if (property.Name == "bEscapeRegex")
					message.Search.IsRegex = property.Value.ToObject<bool>();

				else if (property.Name == "iSortingCols")
					message.Sort.Capacity = property.Value.ToObject<int>();

				else if (property.Name == "iColumns")
					continue;

				else if (property.Name.StartsWith("iSortCol")
					|| property.Name.StartsWith("sSortDir"))
					ReadSortConfiguration(ref message, property);

				else if (property.Name.StartsWith("mDataProp")
					|| property.Name.StartsWith("bSearchable")
					|| property.Name.StartsWith("bSortable")
					|| property.Name.StartsWith("sSearch")
					|| property.Name.StartsWith("bRegex"))
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
			int separatorIndex = property.Name.LastIndexOf("_");
			int index = Convert.ToInt32(property.Name.Substring(separatorIndex + 1));
			string propertyName = property.Name.Substring(0, separatorIndex);

			while (index >= message.Sort.Count)
				message.Sort.Add(new Sort());

			if (propertyName == "iSortCol")
				message.Sort[index].Column = property.Value.ToObject<int>();

			else if (propertyName == "sSortDir")
				message.Sort[index].Direction = property.Value.ToObject<string>().AsSortDirection();
		}

		private void ReadColumnConfiguration(ref IFilterRequest message, JProperty property)
		{
			int separatorIndex = property.Name.LastIndexOf("_");
			int index = Convert.ToInt32(property.Name.Substring(separatorIndex + 1));
			string propertyName = property.Name.Substring(0, separatorIndex);

			IColumn currentColumn = null;

			if (!message.HasColumn(index))
			{
				currentColumn = new Column();
				message.Columns.Add(index, currentColumn);
			}
			else
				currentColumn = message.GetColumn(index);

			if (propertyName == "mDataProp")
				currentColumn.Data = property.Value.ToObject<string>();

			else if (propertyName == "bSearchable")
				currentColumn.Searchable = property.Value.ToObject<bool>();

			else if (propertyName == "bSortable")
				currentColumn.Sortable = property.Value.ToObject<bool>();

			else if (propertyName == "sSearch")
				currentColumn.Search.Value = property.Value.ToObject<string>();

			else if (propertyName == "bRegex")
				currentColumn.Search.IsRegex = property.Value.ToObject<bool>();
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			writer.WriteStartObject();

			IFilterRequest message = value as IFilterRequest;

			if (message != null)
			{
				writer.WritePropertyName("sEcho");
				writer.WriteValue(message.Draw);

				writer.WritePropertyName("iDisplayStart");
				writer.WriteValue(message.Start);

				writer.WritePropertyName("iDisplayLength");
				writer.WriteValue(message.Length);

				if (!string.IsNullOrWhiteSpace(message.Search.Value))
				{
					writer.WritePropertyName("sSearch");
					writer.WriteValue(message.Search.Value);

					writer.WritePropertyName("bEscapeRegex");
					writer.WriteValue(message.Search.IsRegex);
				}

				writer.WritePropertyName("iColumns");
				writer.WriteValue(message.Columns.Count);

				writer.WritePropertyName("iSortingCols");
				writer.WriteValue(message.Sort.Count);

				for (int i = 0; i < message.Sort.Count; i++)
				{
					writer.WritePropertyName("iSortCol_" + i);
					writer.WriteValue(message.Sort[i].Column);

					writer.WritePropertyName("sSortDir_" + i);
					writer.WriteValue(message.Sort[i].Direction.AsString());
				}

				foreach (KeyValuePair<int, IColumn> column in message.Columns.OrderBy(x => x.Key))
				{
					writer.WritePropertyName("mDataProp_" + column.Key);
					writer.WriteValue(column.Value.Data);

					writer.WritePropertyName("bSearchable_" + column.Key);
					writer.WriteValue(column.Value.Searchable);

					writer.WritePropertyName("bSortable_" + column.Key);
					writer.WriteValue(column.Value.Sortable);

					if (!string.IsNullOrWhiteSpace(column.Value.Search.Value))
					{
						writer.WritePropertyName("sSearch_" + column.Key);
						writer.WriteValue(column.Value.Search.Value);

						writer.WritePropertyName("bRegex_" + column.Key);
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
