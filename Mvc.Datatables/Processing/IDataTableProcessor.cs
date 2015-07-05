using System;
using System.Linq;

namespace Mvc.Datatables.Processing
{
	public interface IDataTableProcessor
	{
		PageResponse<TSource> Process<TSource>(IQueryable<TSource> query, FilterRequest request, Func<IQueryable<TSource>, IQueryable<TSource>> appendQuery = null);
	}
}
