
namespace Mvc.Datatables
{
	/// <summary>
	/// Columns' parameters for filter requests
	/// </summary>
	public interface IColumn
	{
		/// <summary>
		/// Column's data source.
		/// </summary>
		string Data { get; set; }

		/// <summary>
		/// Column's name.
		/// </summary>
		string Name { get; set; }

		/// <summary>
		/// Flag to indicate if this column is searchable (true) or not (false). 
		/// </summary>
		bool Searchable { get; set; }

		/// <summary>
		/// Flag to indicate if this column is sortable (true) or not (false).
		/// </summary>
		bool Sortable { get; set; }

		/// <summary>
		/// The search component for the column.
		/// </summary>
		ISearch Search { get; set; }
	}
}
