using Mvc.Datatables.Util;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace Mvc.Datatables.Serialization
{
    /// <summary>
    /// Model binder for datatables.js parameters a la http://geeksprogramando.blogspot.com/2011/02/jquery-datatables-plug-in-with-asp-mvc.html
    /// </summary>
    public class FilterRequestModelBinder : IModelBinder
    {
        private readonly Type _concreteType;

        public FilterRequestModelBinder(Type concreteType = null)
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
            if (bindingContext == null)
                throw new ArgumentNullException("bindingContext");

            IFilterRequest message = Activator.CreateInstance(_concreteType) as IFilterRequest;

            ReadConfiguration(ref message, StripPrefix(bindingContext, controllerContext.HttpContext.Request.QueryString));
            ReadConfiguration(ref message, StripPrefix(bindingContext, controllerContext.HttpContext.Request.Form));

            return message;
        }

        private NameValueCollection StripPrefix(ModelBindingContext bindingContext, NameValueCollection collection)
        {
            if (bindingContext.FallbackToEmptyPrefix || String.IsNullOrEmpty(bindingContext.ModelName))
                return collection;
            else if (collection.Count == 0)
                return collection;
            else
            {
                NameValueCollection filteredCollection = new NameValueCollection();

                foreach (var key in collection.AllKeys)
                {
                    if (key.StartsWith(bindingContext.ModelName))
                    {
                        String filteredKey;
                        filteredKey = key.Substring(bindingContext.ModelName.Length).TrimStart('.');
                        filteredCollection[filteredKey] = collection[key];
                    }
                }

                return filteredCollection;
            }
        }

        private void ReadConfiguration(ref IFilterRequest message, NameValueCollection collection)
        {
            Dictionary<string, object> otherValues = new Dictionary<string, object>();

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

                else
                    otherValues.Add(key, collection[key]);
            }

            FormConvertHelper.ReadForm(message, otherValues,
                prop => FormConvertHelper.GetPropertiesFromType(typeof(IFilterRequest)).Select(x => x.Name).Contains(prop.Name));
        }

        private void ReadSortConfiguration(ref IFilterRequest message, string key, object value)
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

        private void ReadColumnConfiguration(ref IFilterRequest message, string key, object value)
        {
            Match match = Regex.Match(key, @"columns\[([0-9]+)\](.+)");
            if (match.Success && match.Groups.Count == 3)
            {
                int index = Convert.ToInt32(match.Groups[1].Value);
                string propertyName = match.Groups[2].Value;

                IColumn currentColumn;

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
            return (value == null) ? default : (T)Convert.ChangeType(value, typeof(T));
        }
    }
}