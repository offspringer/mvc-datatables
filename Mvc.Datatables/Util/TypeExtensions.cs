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
    }
}
