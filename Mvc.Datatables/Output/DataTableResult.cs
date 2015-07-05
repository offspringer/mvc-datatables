using System.Web.Mvc;

namespace Mvc.Datatables.Output
{
	public abstract class DataTableResult : ActionResult
	{
		public object Data { get; set; }
	}
}
