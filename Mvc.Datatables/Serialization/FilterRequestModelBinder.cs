using System;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace Mvc.Datatables.Serialization
{
	/// <summary>
	/// Model binder for datatables.js parameters a la http://geeksprogramando.blogspot.com/2011/02/jquery-datatables-plug-in-with-asp-mvc.html
	/// </summary>
	public class FilterRequestModelBinder : IModelBinder
	{
		public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
		{
			FilterRequest message = new FilterRequest();

            ReadConfiguration(ref message, controllerContext.HttpContext.Request.QueryString);
            ReadConfiguration(ref message, controllerContext.HttpContext.Request.Form);

			return message;
		}

        private void ReadConfiguration(ref FilterRequest message, NameValueCollection collection)
        {
            foreach (string key in collection.AllKeys)
		    {
                if (key == "draw")
                    message.Draw = GetValue<int>(collection[key]);

                else if (key == "start")
                    message.Start = GetValue<int>(collection[key]);

                else if (key == "length")
                    message.Length = GetValue<int>(collection[key]);

                else if (key == "search[value]")
                    message.Search.Value = GetValue<string>(collection[key]);

                else if (key == "search[regex]")
                    message.Search.IsRegex = GetValue<bool>(collection[key]);

                else if (key.StartsWith("order"))
                    ReadSortConfiguration(ref message, key, collection[key]);

                else if (key.StartsWith("columns"))
                    ReadColumnConfiguration(ref message, key, collection[key]);		        
		    }
        }

        private void ReadSortConfiguration(ref FilterRequest message, string key, object value)
        {
            Match match = Regex.Match(key, @"order\[([0-9]+)\](.+)");
            if (match.Success && match.Groups.Count == 3)
            {
                int index = Convert.ToInt32(match.Groups[1].Value);
                string propertyName = match.Groups[2].Value;

                while (index >= message.Sort.Count)
                    message.Sort.Add(new Sort());

                if (propertyName == "[column]")
                    message.Sort[index].Column = GetValue<int>(value);

                else if (propertyName == "[dir]")
                    message.Sort[index].Direction = GetValue<string>(value).AsSortDirection();
            }
        }

        private void ReadColumnConfiguration(ref FilterRequest message, string key, object value)
        {
            Match match = Regex.Match(key, @"columns\[([0-9]+)\](.+)");
            if (match.Success && match.Groups.Count == 3)
            {
                int index = Convert.ToInt32(match.Groups[1].Value);
                string propertyName = match.Groups[2].Value;

                Column currentColumn = null;

                if (!message.HasColumn(index))
                {
                    currentColumn = new Column();
                    message.Columns.Add(index, currentColumn);
                }
                else
                    currentColumn = message.GetColumn(index);

                if (propertyName == "[data]")
                    currentColumn.Data = GetValue<string>(value);

                else if (propertyName == "[name]")
                    currentColumn.Name = GetValue<string>(value);

                else if (propertyName == "[searchable]")
                    currentColumn.Searchable = GetValue<bool>(value);

                else if (propertyName == "[orderable]")
                    currentColumn.Sortable = GetValue<bool>(value);

                else if (propertyName == "[search][value]")
                    currentColumn.Search.Value = GetValue<string>(value);

                else if (propertyName == "[search][regex]")
                    currentColumn.Search.IsRegex = GetValue<bool>(value);
            }
        }

        private static T GetValue<T>(object value)
        {
            return (value == null) ? default(T) : (T)Convert.ChangeType(value, typeof(T));
        }
	}
}