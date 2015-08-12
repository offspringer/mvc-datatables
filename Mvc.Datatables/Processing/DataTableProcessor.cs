using System;
using System.Linq;

namespace Mvc.Datatables.Processing
{
    public class DataTableProcessor : IDataTableProcessor
    {
        private readonly IDataTableFilterProcessor _filterProcessor;

        public DataTableProcessor(IDataTableFilterProcessor filterprocessor)
        {
            _filterProcessor = filterprocessor;
        }

        public virtual IPageResponse<TSource> Process<TSource>(IQueryable<TSource> query, IFilterRequest request, Func<IQueryable<TSource>, IQueryable<TSource>> appendQuery = null)
        {
            // Causes an extra evaluation
            int totalRecords = query.Count();

            IQueryable<TSource> filteredData = query;

            if (_filterProcessor != null)
                filteredData = _filterProcessor.ApplyFiltersAndSort(request, query);

            if (appendQuery != null)
                filteredData = appendQuery(filteredData);

            int totalDisplayRecords = filteredData.Count();

            IQueryable<TSource> skipped = filteredData.Skip(request.Start);
            IQueryable<TSource> page = request.Length <= 0 ? skipped : skipped.Take(request.Length);

            TSource[] dataSource = page.ToArray();
            return new PageResponse<TSource>(request.Draw, totalRecords, totalDisplayRecords, dataSource);
        }
    }
}
