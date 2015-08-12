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
    public class PageResponse<TSource> : PageResponse, IPageResponse<TSource>
    {
        [XmlIgnore]
        [JsonIgnore]
        public virtual TSource[] DataSource
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

        public virtual IPageResponse<TSource> Transform(Func<TSource, TSource> transformRow)
        {
            TSource[] transformedData = this.DataSource.Select(transformRow).ToArray();
            IPageResponse<TSource> transformedResponse = new PageResponse<TSource>(this.Draw, this.TotalRecords, this.TotalFilteredRecords, transformedData);
            return transformedResponse;
        }

        public virtual IPageResponse<object> Transform<TMutableSource, TTransform>(Func<TMutableSource, TTransform> transformRow)
        {
            object[] transformedData = this.Data.Cast<TMutableSource>().Select(transformRow).Cast<object>().ToArray();
            IPageResponse<object> transformedResponse = new PageResponse<object>(this.Draw, this.TotalRecords, this.TotalFilteredRecords, transformedData);
            return transformedResponse;
        }

        public static IPageResponse<object> ToMutableResponse(IPageResponse<TSource> source)
        {
            IPageResponse<object> mutableResponse = new PageResponse<object>(source.Draw, source.TotalRecords, source.TotalFilteredRecords,
                source.Data != null ? source.Data.Cast<object>().ToArray() : null);
            return mutableResponse;
        }
    }
}