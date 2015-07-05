using System.Linq;
using Mvc.Datatables.Processing;

namespace Mvc.Datatables.Output
{
	public class MutableDataTableResult : DataTableResult<PageResponse<object>>
	{
        public MutableDataTableResult(PageResponse<object> data, OutputType? outputType = null)
            : base(outputType)
		{
			this.Data = data;
		}

        public MutableDataTableResult(IQueryable<object> query, FilterRequest request, OutputType? outputType = null)
            : base(outputType)
		{
			IDataTableFilterProcessor filterProcessor = new DataTablesFilterProcessor();
			IDataTableProcessor processor = new DataTableProcessor(filterProcessor);
			PageResponse<object> response = processor.Process(query, request);
			this.Data = response;
		}
	}
}
