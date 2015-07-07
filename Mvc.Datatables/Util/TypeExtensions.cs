using System;
using System.Collections.Generic;

namespace Mvc.Datatables.Util
{
    public static class TypeExtensions
    {
        public static IEnumerable<Type> GetInheritancHierarchy(this Type type)
        {
            for (var current = type; current != null; current = current.BaseType)
                yield return current;
        }

        public static Type GetPageResponseArgument(this PageResponse response)
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
