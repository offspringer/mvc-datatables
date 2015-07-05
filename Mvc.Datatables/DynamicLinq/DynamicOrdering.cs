using System.Linq.Expressions;

namespace Mvc.Datatables.DynamicLinq
{
    internal class DynamicOrdering
    {
        public Expression Selector;
        public bool Ascending;
    }
}