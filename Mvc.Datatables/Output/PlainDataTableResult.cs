using System.Linq;
using Mvc.Datatables.Processing;

namespace Mvc.Datatables.Output
{
	public class PlainDataTableResult<TSource> : DataTableResult<PageResponse<TSource>>
	{
        public PlainDataTableResult(PageResponse<TSource> data, OutputType? outputType = null)
            : base(outputType)
		{
			this.Data = data;
		}

        public PlainDataTableResult(IQueryable<TSource> query, FilterRequest request, OutputType? outputType = null)
            : base(outputType)
		{
			IDataTableFilterProcessor filterProcessor = new DataTablesFilterProcessor();
			IDataTableProcessor processor = new DataTableProcessor(filterProcessor);
			this.Data = processor.Process(query, request);
		}
	}
}
