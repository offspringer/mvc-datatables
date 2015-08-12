using Mvc.Datatables.Processing;
using System.Linq;

namespace Mvc.Datatables.Output
{
    public class MutableDataTableResult : DataTableResult<IPageResponse<object>>
    {
        public MutableDataTableResult(IPageResponse<object> data, OutputType? outputType = null)
            : base(outputType)
        {
            this.Data = data;
        }

        public MutableDataTableResult(IQueryable<object> query, IFilterRequest request, OutputType? outputType = null)
            : base(outputType)
        {
            IDataTableFilterProcessor filterProcessor = new DataTableFilterProcessor();
            IDataTableProcessor processor = new DataTableProcessor(filterProcessor);
            IPageResponse<object> response = processor.Process(query, request);
            this.Data = response;
        }
    }
}
