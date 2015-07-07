using Newtonsoft.Json;
using System;
using System.Linq;
using System.Xml.Serialization;

namespace Mvc.Datatables
{
	/// <summary>
	/// Type-safe page response
	/// </summary>
	/// <typeparam name="TSource">The type of the data array</typeparam>
	public class PageResponse<TSource> : PageResponse
	{
		[XmlIgnore]
		[JsonIgnore]
		public TSource[] DataSource
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

		public PageResponse(int echo, TSource[] dataSource)
		{
			this.Draw = echo;
			this.TotalRecords = dataSource.Length;
			this.TotalFilteredRecords = dataSource.Length;
			this.DataSource = dataSource;
		}

		public PageResponse(int echo, int totalRecords, int totalDisplayRecords, TSource[] dataSource)
		{
			this.Draw = echo;
			this.TotalRecords = totalRecords;
			this.TotalFilteredRecords = totalDisplayRecords;
			this.DataSource = dataSource;
		}

		public PageResponse<TSource> Transform(Func<TSource, TSource> transformRow)
		{
			TSource[] transformedData = this.DataSource.Select(transformRow).ToArray();
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