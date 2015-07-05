using System.Linq;
using Mvc.Datatables.Reflection;

namespace Mvc.Datatables.Processing
{
	public interface IDataTableFilterProcessor
	{
		IQueryable<T> ApplyFiltersAndSort<T>(FilterRequest filter, IQueryable<T> data);

		IQueryable<T> ApplyFiltersAndSort<T>(FilterRequest filter, IQueryable<T> data, DataTablesPropertyInfo[] columns);
	}
}
