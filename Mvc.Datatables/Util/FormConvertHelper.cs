using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.Mvc;

namespace Mvc.Datatables.Util
{
	public static class FormConvertHelper
	{
		public static void ReadForm(object message, Dictionary<string, object> otherValues, Func<PropertyDescriptor, bool> shouldBypass)
		{
			Type objectType = message.GetType();
			ICustomTypeDescriptor typeDescriptor = TypeDescriptor.GetProvider(objectType).GetTypeDescriptor(objectType);

			foreach (PropertyDescriptor propertyDescriptor in typeDescriptor.GetProperties())
			{
				if (shouldBypass != null && shouldBypass(propertyDescriptor))
					continue;

				if (propertyDescriptor.Attributes.Cast<Attribute>().OfType<JsonIgnoreAttribute>().Count() == 0)
				{
					JsonPropertyAttribute propAttr = propertyDescriptor.Attributes.Cast<Attribute>().OfType<JsonPropertyAttribute>().SingleOrDefault();
					string propertyName = propAttr != null ? propAttr.PropertyName : propertyDescriptor.Name;

					if (otherValues.ContainsKey(propertyName))
					{
						if (otherValues[propertyName] != null)
						{
							object convertedValue = TypeHelper.GetDefaultValue(propertyDescriptor.PropertyType);
							bool success = TypeHelper.TryCast(otherValues[propertyName], out convertedValue, propertyDescriptor.PropertyType);
							if (success)
								propertyDescriptor.SetValue(message, convertedValue);
						}
					}
				}
			}
		}

		public static void ReadForm(object message, IValueProvider valueProvider, Func<PropertyDescriptor, bool> shouldBypass)
		{
			Type objectType = message.GetType();
			ICustomTypeDescriptor typeDescriptor = TypeDescriptor.GetProvider(objectType).GetTypeDescriptor(objectType);

			foreach (PropertyDescriptor propertyDescriptor in typeDescriptor.GetProperties())
			{
				if (shouldBypass != null && shouldBypass(propertyDescriptor))
					continue;

				if (propertyDescriptor.Attributes.Cast<Attribute>().OfType<JsonIgnoreAttribute>().Count() == 0)
				{
					JsonPropertyAttribute propAttr = propertyDescriptor.Attributes.Cast<Attribute>().OfType<JsonPropertyAttribute>().SingleOrDefault();
					string propertyName = propAttr != null ? propAttr.PropertyName : propertyDescriptor.Name;

					ValueProviderResult valueResult = valueProvider.GetValue(propertyName);
					if (valueResult != null)
					{
						object convertedValue = valueResult.ConvertTo(propertyDescriptor.PropertyType);
						propertyDescriptor.SetValue(message, convertedValue);
					}
				}
			}
		}

		public static IEnumerable<PropertyDescriptor> GetPropertiesFromType(Type type)
		{
			ICustomTypeDescriptor typeDescriptor = TypeDescriptor.GetProvider(type).GetTypeDescriptor(type);
			foreach (PropertyDescriptor propertyDescriptor in typeDescriptor.GetProperties())
			{
				yield return propertyDescriptor;
			}
		}
	}
}
