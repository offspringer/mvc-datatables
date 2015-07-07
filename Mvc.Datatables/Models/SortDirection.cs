using System;

namespace Mvc.Datatables
{
	/// <summary>
	/// Defines sort directions.
	/// </summary>
	public enum SortDirection
	{
		/// <summary>
		/// Represents an ascendant (A-Z) sorting.
		/// </summary>
		Ascendant = 0,

		/// <summary>
		/// Represents a descendant (Z-A) sorting.
		/// </summary>
		Descendant = 1
	}

	public static class SortDirectionExtensions
	{
		public static string AsString(this SortDirection val)
		{
			if (val == SortDirection.Descendant)
				return "desc";
			else
				return "asc";
		}

		public static SortDirection AsSortDirection(this string val)
		{
			if (val == "desc")
				return SortDirection.Descendant;
			else
				return SortDirection.Ascendant;
		}
	}
}
