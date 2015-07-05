using System;
using System.Linq;
using Newtonsoft.Json;

namespace Mvc.Datatables
{
	/// <summary>
	/// Type-safe page response
	/// </summary>
	/// <typeparam name="TSource">The type of the data array</typeparam>
	public class PageResponse<TSource> : PageResponse
	{
		public new TSource[] Data
		{
			get
			{
				return base.Data != null ? base.Data.Cast<TSource>().ToArray() : null;
			}
			set
			{
				base.Data = value != null ? value.Cast<object>().ToArray() : null;
			}
		}

		public PageResponse() { }

		public PageResponse(int echo, TSource[] data)
		{
			this.Draw = echo;
			this.TotalRecords = data.Length;
			this.TotalFilteredRecords = data.Length;
			this.Data = data;
		}

		public PageResponse(int echo, int totalRecords, int totalDisplayRecords, TSource[] data)
		{
			this.Draw = echo;
			this.TotalRecords = totalRecords;
			this.TotalFilteredRecords = totalDisplayRecords;
			this.Data = data;
		}

		public PageResponse<TSource> Transform(Func<TSource, TSource> transformRow)
		{
			TSource[] transformedData = this.Data.Select(transformRow).ToArray();
			PageResponse<TSource> transformedResponse = new PageResponse<TSource>(this.Draw, this.TotalRecords, this.TotalFilteredRecords, transformedData);
			return transformedResponse;
		}

		public PageResponse<object> Transform<TMutableSource, TTransform>(Func<TMutableSource, TTransform> transformRow)
		{
			object[] transformedData = this.Data.Cast<TMutableSource>().Select(transformRow).Cast<object>().ToArray();
			PageResponse<object> transformedResponse = new PageResponse<object>(this.Draw, this.TotalRecords, this.TotalFilteredRecords, transformedData);
			return transformedResponse;
		}

		public static PageResponse<object> ToMutableResponse(PageResponse<TSource> source)
		{
			PageResponse<object> mutableResponse = new PageResponse<object>(source.Draw, source.TotalRecords, source.TotalFilteredRecords,
				source.Data != null ? source.Data.Cast<object>().ToArray() : null);
			return mutableResponse;
		}
	}
}