using Mvc.Datatables.Util;
using System;
using System.Linq;
using System.Web.Mvc;

namespace Mvc.Datatables.Serialization
{
	/// <summary>
	/// Model binder for datatables.js parameters a la http://geeksprogramando.blogspot.com/2011/02/jquery-datatables-plug-in-with-asp-mvc.html
	/// </summary>
	public class LegacyFilterRequestModelBinder : IModelBinder
	{
		private readonly Type _concreteType;

		public LegacyFilterRequestModelBinder(Type concreteType = null)
		{
			if (concreteType != null)
			{
				if (!concreteType.IsClass)
					throw new NotSupportedException();

				if (!concreteType.GetInterfaces().Any(x => x == typeof(IFilterRequest)))
					throw new NotSupportedException();

				this._concreteType = concreteType;
			}
			else
				this._concreteType = typeof(FilterRequest);
		}

		public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
		{
			IValueProvider valueProvider = bindingContext.ValueProvider;

			IFilterRequest message = Activator.CreateInstance(_concreteType) as IFilterRequest;

			message.Draw = GetValue<int>(valueProvider, "sEcho");
			message.Start = GetValue<int>(valueProvider, "iDisplayStart");
			message.Length = GetValue<int>(valueProvider, "iDisplayLength");
			message.Search.Value = GetValue<string>(valueProvider, "sSearch");
			message.Search.IsRegex = GetValue<bool>(valueProvider, "bEscapeRegex");

			message.Sort.Capacity = GetValue<int>(valueProvider, "iSortingCols");
			for (int i = 0; i < message.Sort.Capacity; i++)
			{
				ISort sort = new Sort();

				sort.Column = GetValue<int>(valueProvider, "iSortCol_" + i);
				sort.Direction = GetValue<string>(valueProvider, "sSortDir_" + i).AsSortDirection();

				message.Sort.Add(sort);
			}

			int totalColumns = GetValue<int>(valueProvider, "iColumns");
			for (int i = 0; i < totalColumns; i++)
			{
				IColumn column = new Column();

				column.Data = GetValue<string>(valueProvider, "mDataProp_" + i);
				column.Searchable = GetValue<bool>(valueProvider, "bSearchable_" + i);
				column.Sortable = GetValue<bool>(valueProvider, "bSortable_" + i);
				column.Search.Value = GetValue<string>(valueProvider, "sSearch_" + i);
				column.Search.IsRegex = GetValue<bool>(valueProvider, "bRegex_" + i);

				message.Columns.Add(i, column);
			}

			FormConvertHelper.ReadForm(message, valueProvider,
				prop => FormConvertHelper.GetPropertiesFromType(typeof(IFilterRequest)).Select(x => x.Name).Contains(prop.Name));

			return message;
		}

		private static T GetValue<T>(IValueProvider valueProvider, string key)
		{
			ValueProviderResult valueResult = valueProvider.GetValue(key);
			return (valueResult == null) ? default(T) : (T)valueResult.ConvertTo(typeof(T));
		}
	}
}