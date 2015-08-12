using System;
using System.ComponentModel;

namespace Mvc.Datatables.Util
{
	public static class TypeExtensions
	{
		public static bool CanAssignValue(this PropertyDescriptor p, object value)
		{
			return value == null ? p.IsNullable() : p.PropertyType.IsInstanceOfType(value);
		}

		public static bool IsNullable(this PropertyDescriptor p)
		{
			return p.PropertyType.IsNullable();
		}

		public static bool IsNullable(this Type t)
		{
			return !t.IsValueType || Nullable.GetUnderlyingType(t) != null;
		}
	}
}
