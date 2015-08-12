using System;
using System.Linq;

namespace Mvc.Datatables.Processing
{
    public interface IDataTableProcessor
    {
        IPageResponse<TSource> Process<TSource>(IQueryable<TSource> query, IFilterRequest request, Func<IQueryable<TSource>, IQueryable<TSource>> appendQuery = null);
    }
}
