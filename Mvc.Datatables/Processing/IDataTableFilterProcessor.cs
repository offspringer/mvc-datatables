using Mvc.Datatables.Reflection;
using System.Linq;

namespace Mvc.Datatables.Processing
{
    public interface IDataTableFilterProcessor
    {
        IQueryable<T> ApplyFiltersAndSort<T>(IFilterRequest filter, IQueryable<T> data);

        IQueryable<T> ApplyFiltersAndSort<T>(IFilterRequest filter, IQueryable<T> data, DataTablesPropertyInfo[] columns);
    }
}
