using System;
using System.Linq;
using System.Linq.Expressions;

namespace Mvc.Datatables.Util
{
	public static class TypeHelper
	{
		public static T GetDefaultValue<T>()
		{
			// We want an Func<T> which returns the default.
			// Create that expression here.
			Expression<Func<T>> e = Expression.Lambda<Func<T>>(
				// The default value, always get what the *code* tells us.
				Expression.Default(typeof(T))
			);

			// Compile and return the value.
			return e.Compile()();
		}

		public static object GetDefaultValue(Type type)
		{
			// Validate parameters.
			if (type == null)
				throw new ArgumentNullException("type");

			// We want an Func<object> which returns the default.
			// Create that expression here.
			Expression<Func<object>> e = Expression.Lambda<Func<object>>(
				// Have to convert to object.
				Expression.Convert(
				// The default value, always get what the *code* tells us.
					Expression.Default(type), typeof(object)
				)
			);

			// Compile and return the value.
			return e.Compile()();
		}

		public static bool TryCast<T>(object value, out T result)
		{
			var type = typeof(T);

			// If the type is nullable and the result should be null, set a null value.
			if (type.IsNullable() && (value == null || value == DBNull.Value))
			{
				result = default(T);
				return true;
			}

			// Convert.ChangeType fails on Nullable<T> types.  We want to try to cast to the underlying type anyway.
			var underlyingType = Nullable.GetUnderlyingType(type) ?? type;

			try
			{
				if (underlyingType == typeof(Guid))
				{
					if (value is string)
					{
						value = new Guid(value as string);
					}
					if (value is byte[])
					{
						value = new Guid(value as byte[]);
					}
				}
				else if (underlyingType.IsEnum && value != null)
				{
					value = Enum.Parse(underlyingType, value.ToString(), true);
				}

				result = (T)Convert.ChangeType(value, underlyingType);
				return true;
			}
			catch
			{
				result = default(T);
				return false;
			}
		}

		public static bool TryCast(object value, out object result, Type type)
		{
			var s = "TryCast";
			var openTryCastMethod = typeof(TypeHelper).GetMethods().Single(x => x.Name == s && x.GetGenericArguments().Count() == 1);
			var closedTryCastMethod = openTryCastMethod.MakeGenericMethod(type);

			result = TypeHelper.GetDefaultValue(type);
			object[] args = new object[] { value, result };
			bool success = (bool)closedTryCastMethod.Invoke(null, args);
			result = args[1];

			return success;
		}
	}
}
