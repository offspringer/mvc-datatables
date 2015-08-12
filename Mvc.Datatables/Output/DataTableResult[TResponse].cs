using Mvc.Datatables.Serialization;
using Newtonsoft.Json;
using System;
using System.Web;
using System.Web.Mvc;

namespace Mvc.Datatables.Output
{
    public abstract class DataTableResult<TResponse> : DataTableResult
    {
        public new TResponse Data
        {
            get
            {
                return base.Data != null ? (TResponse)base.Data : default(TResponse);
            }
            set
            {
                base.Data = value;
            }
        }

        public OutputType OutputType { get; set; }

        public DataTableResult(OutputType? outputType)
        {
            this.OutputType = outputType ?? OutputType.New;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            JsonConverter converter = null;

            switch (this.OutputType)
            {
                case OutputType.Legacy:
                    converter = new LegacyPageResponseConverter();
                    break;
                case OutputType.New:
                    converter = new PageResponseConverter();
                    break;
                default:
                    throw new NotSupportedException();
            }

            HttpResponseBase response = context.HttpContext.Response;
            string serialized = JsonConvert.SerializeObject(this.Data, Formatting.Indented, converter);
            response.Write(serialized);
        }
    }
}
