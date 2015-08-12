using System.Collections.Generic;

namespace Mvc.Datatables
{
	/// <summary>
	/// Filter request parameters
	/// </summary>
	public interface IFilterRequest : IDataTableMessage
	{
		/// <summary>
		/// Draw counter. This is used by DataTables to ensure that the Ajax returns from server-side processing requests are drawn in sequence by DataTables (Ajax requests are asynchronous and thus can return out of sequence). 
		/// </summary>
		int Draw { get; set; }

		/// <summary>
		/// Paging first record indicator. This is the start point in the current data set (0 index based - i.e. 0 is the first record).
		/// </summary>
		int Start { get; set; }

		/// <summary>
		/// Number of records that the table can display in the current draw. It is expected that the number of records returned will be equal to this number, unless the server has fewer records to return. Note that this can be -1 to indicate that all records should be returned (although that negates any benefits of server-side processing!)
		/// </summary>
		int Length { get; set; }

		/// <summary>
		/// Global search component to be applied to all columns which have searchable as true.
		/// </summary>
		ISearch Search { get; set; }

		/// <summary>
		/// The columns' sorting configuration.
		/// </summary>
		List<ISort> Sort { get; set; }

		/// <summary>
		/// The columns collection.
		/// </summary>
		Dictionary<int, IColumn> Columns { get; set; }

		bool HasColumn(int index);

		IColumn GetColumn(int index);

		bool HasSortForColumn(int index);

		ISort GetSortForColumn(int index);
	}
}
