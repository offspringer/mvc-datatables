using System;

namespace Mvc.Datatables.Util
{
	public static class ModelExtensions
	{
		public static Type GetPageResponseArgument(this IPageResponse response)
		{
			Type genericType = typeof(object);

			if (response != null)
			{
				Type baseType = response.GetType();

				for (var current = baseType; current != null; current = current.BaseType)
				{
					if (current.IsGenericType && current.GetGenericTypeDefinition() == typeof(PageResponse<>))
					{
						if (response.Data != null && response.Data.Length > 0)
							genericType = response.Data[0].GetType();
						else
							genericType = current.GetGenericArguments()[0];
						break;
					}
				}
			}

			return genericType;
		}
	}
}
