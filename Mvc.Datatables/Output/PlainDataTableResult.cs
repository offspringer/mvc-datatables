using Mvc.Datatables.Processing;
using System.Linq;

namespace Mvc.Datatables.Output
{
    public class PlainDataTableResult<TSource> : DataTableResult<IPageResponse<TSource>>
    {
        public PlainDataTableResult(IPageResponse<TSource> data, OutputType? outputType = null)
            : base(outputType)
        {
            this.Data = data;
        }

        public PlainDataTableResult(IQueryable<TSource> query, IFilterRequest request, OutputType? outputType = null)
            : base(outputType)
        {
            IDataTableFilterProcessor filterProcessor = new DataTableFilterProcessor();
            IDataTableProcessor processor = new DataTableProcessor(filterProcessor);
            this.Data = processor.Process(query, request);
        }
    }
}
