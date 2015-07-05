using System;
using System.Collections.Generic;
using System.Linq;
using Mvc.Datatables.DynamicLinq;
using Mvc.Datatables.Reflection;

namespace Mvc.Datatables.Processing
{
    public class DataTablesFilterProcessor : IDataTableFilterProcessor
    {
        private static readonly List<ReturnedFilteredQueryForType> _filters = new List<ReturnedFilteredQueryForType>()
		{
			Guard(IsBoolType, TypeFilters.BoolFilter),
			Guard(IsDateTimeType, TypeFilters.DateTimeFilter),
			Guard(IsDateTimeOffsetType, TypeFilters.DateTimeOffsetFilter),
			Guard(IsNumericType, TypeFilters.NumericFilter),
			Guard(IsEnumType, TypeFilters.EnumFilter),
			Guard(arg => arg.Type == typeof (string), TypeFilters.StringFilter),
		};

        public delegate string ReturnedFilteredQueryForType(string query, string columnName, DataTablesPropertyInfo columnType, List<object> parametersForLinqQuery);

        public delegate string GuardedFilter(string query, string columnName, DataTablesPropertyInfo columnType, List<object> parametersForLinqQuery);

        public virtual IQueryable<T> ApplyFiltersAndSort<T>(FilterRequest filter, IQueryable<T> data)
        {
            var outputProperties = DataTablesTypeInfo<T>.Properties;
            return ApplyFiltersAndSort<T>(filter, data, outputProperties);
        }

        public virtual IQueryable<T> ApplyFiltersAndSort<T>(FilterRequest filter, IQueryable<T> data, DataTablesPropertyInfo[] columns)
        {
            if (!string.IsNullOrEmpty(filter.Search.Value))
            {
                var parts = new List<string>();
                var parameters = new List<object>();
                for (var i = 0; i < filter.Columns.Count; i++)
                {
                    if (filter.Columns[i].Searchable && i < columns.Length)
                    {
                        try
                        {
                            parts.Add(GetFilterClause(filter.Search.Value, columns[i], parameters));
                        }
                        catch (Exception)
                        {
                            // If the clause doesn't work, skip it!
                        }
                    }
                }
                var values = parts.Where(p => p != null);
                data = data.Where(string.Join(" or ", values), parameters.ToArray());
            }

            foreach (KeyValuePair<int, Column> column in filter.Columns.OrderBy(x => x.Key))
            {
                if (column.Value.Searchable)
                {
                    var searchColumn = column.Value.Search;
                    if (!string.IsNullOrWhiteSpace(searchColumn.Value) && column.Key < columns.Length)
                    {
                        var parameters = new List<object>();
                        var filterClause = GetFilterClause(searchColumn.Value, columns[column.Key], parameters);
                        if (string.IsNullOrWhiteSpace(filterClause) == false)
                        {
                            data = data.Where(filterClause, parameters.ToArray());
                        }
                    }
                }
            }

            string sortString = "";
            for (int i = 0; i < filter.Sort.Count; i++)
            {
                int columnNumber = filter.Sort[i].Column;
                if (columnNumber < columns.Length)
                {
                    string columnName = columns[columnNumber].PropertyInfo.Name;
                    string sortDir = filter.Sort[i].Direction.AsString();

                    if (i != 0)
                        sortString += ", ";
                    sortString += columnName + " " + sortDir;
                }
            }

            if (string.IsNullOrWhiteSpace(sortString) && columns.Length > 0)
                sortString = columns[0].PropertyInfo.Name;

            if (!string.IsNullOrWhiteSpace(sortString))
                data = data.OrderBy(sortString);

            return data;
        }

        private static ReturnedFilteredQueryForType Guard(Func<DataTablesPropertyInfo, bool> guard, GuardedFilter filter)
        {
            return (q, c, t, p) =>
            {
                if (!guard(t))
                {
                    return null;
                }
                return filter(q, c, t, p);
            };
        }

        public static void RegisterFilter<T>(GuardedFilter filter)
        {
            _filters.Add(Guard(arg => arg is T, filter));
        }

        private static string GetFilterClause(string query, DataTablesPropertyInfo column, List<object> parametersForLinqQuery)
        {
            Func<string, string> filterClause = (queryPart) =>
                                                _filters.Select(
                                                    f => f(queryPart, column.PropertyInfo.Name, column, parametersForLinqQuery))
                                                       .FirstOrDefault(filterPart => filterPart != null) ?? "";

            var queryParts = query.Split('|').Select(filterClause).Where(fc => fc != "").ToArray();
            if (queryParts.Any())
            {
                return "(" + string.Join(") OR (", queryParts) + ")";
            }
            return null;
        }

        public static bool IsNumericType(DataTablesPropertyInfo propertyInfo)
        {
            var type = propertyInfo.Type;
            return IsNumericType(type);
        }

        private static bool IsNumericType(Type type)
        {
            if (type == null || type.IsEnum)
            {
                return false;
            }

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.Single:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return true;
                case TypeCode.Object:
                    if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        return IsNumericType(Nullable.GetUnderlyingType(type));
                    }
                    return false;
            }
            return false;
        }

        public static bool IsEnumType(DataTablesPropertyInfo propertyInfo)
        {
            return propertyInfo.Type.IsEnum;
        }

        public static bool IsBoolType(DataTablesPropertyInfo propertyInfo)
        {
            return propertyInfo.Type == typeof(bool) || propertyInfo.Type == typeof(bool?);
        }

        public static bool IsDateTimeType(DataTablesPropertyInfo propertyInfo)
        {
            return propertyInfo.Type == typeof(DateTime) || propertyInfo.Type == typeof(DateTime?);
        }

        public static bool IsDateTimeOffsetType(DataTablesPropertyInfo propertyInfo)
        {
            return propertyInfo.Type == typeof(DateTimeOffset) || propertyInfo.Type == typeof(DateTimeOffset?);
        }
    }
}