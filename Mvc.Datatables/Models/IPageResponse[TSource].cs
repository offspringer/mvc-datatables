using System;

namespace Mvc.Datatables
{
	/// <summary>
	/// Type-safe page response
	/// </summary>
	/// <typeparam name="TSource">The type of the data array</typeparam>
	public interface IPageResponse<TSource> : IPageResponse
	{
		TSource[] DataSource { get; set; }

		IPageResponse<TSource> Transform(Func<TSource, TSource> transformRow);

		IPageResponse<object> Transform<TMutableSource, TTransform>(Func<TMutableSource, TTransform> transformRow);
	}
}
