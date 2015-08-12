using Mvc.Datatables.Output;
using Mvc.Datatables.Processing;
using Mvc.Datatables.Reflection;
using Mvc.Datatables.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mvc.Datatables
{
    public sealed class DataTableResultFactory
    {
        #region Mutable results

        /// <typeparam name="TSource">The type of the source query</typeparam>
        /// <typeparam name="TTransform">The type of the result</typeparam>
        /// <param name="query">A queryable for the data</param>
        /// <param name="request">The request filters</param>
        /// <param name="transform">A transform for custom column rendering e.g. to do a custom date row => new { CreatedDate = row.CreatedDate.ToString("dd MM yy") }</param>
        /// <param name="outputType">The output type</param>
        /// <returns>The action result</returns>
        public static MutableDataTableResult CreateMutable<TSource, TTransform>(IQueryable<TSource> query, IFilterRequest request,
            Func<TSource, TTransform> transform, OutputType? outputType = null, ArrayOutputType? arrayOutputType = null)
        {
            var result = new MutableDataTableResult(query.Cast<object>(), request, outputType);

            result.Data = result.Data
                .Transform<TSource, Dictionary<string, object>>(row => TransformTypeInfo.MergeTransformValuesIntoDictionary(transform, row))
                .Transform<Dictionary<string, object>, Dictionary<string, object>>(StringTransformers.StringifyValues);

            result.Data = ApplyOutputRulesForMutable(result.Data, arrayOutputType);

            return result;
        }

        public static MutableDataTableResult CreateMutable<TSource>(IQueryable<TSource> query, IFilterRequest request,
            OutputType? outputType = null, ArrayOutputType? arrayOutputType = null)
        {
            var result = new MutableDataTableResult(query.Cast<object>(), request, outputType);

            result.Data = result.Data
                .Transform<TSource, Dictionary<string, object>>(DataTablesTypeInfo<TSource>.ToDictionary)
                .Transform<Dictionary<string, object>, Dictionary<string, object>>(StringTransformers.StringifyValues);

            result.Data = ApplyOutputRulesForMutable(result.Data, arrayOutputType);

            return result;
        }

        private static IPageResponse<object> ApplyOutputRulesForMutable(IPageResponse<object> sourceData, ArrayOutputType? arrayOutputType = null)
        {
            IPageResponse<object> outputData = sourceData;

            switch (arrayOutputType)
            {
                case ArrayOutputType.Index:
                    outputData = sourceData.Transform<Dictionary<string, object>, object[]>(d => d.Values.ToArray());
                    break;
                case ArrayOutputType.Name:
                default:
                    // Nothing is needed
                    break;
            }

            return outputData;
        }

        /// <param name="query">A queryable for the data</param>
        /// <param name="request">The request filters</param>
        /// <param name="transform">Should be a Func<TMutableSource, TTransform></param>
        /// <param name="outputType">The output type</param>
        /// <returns>The action result</returns>
        public static DataTableResult CreateMutable(IQueryable query, IFilterRequest request, object transform,
            OutputType? outputType = null, ArrayOutputType? arrayOutputType = null)
        {
            var s = "CreateMutable";
            var openCreateMethod = typeof(DataTableResultFactory).GetMethods().Single(x => x.Name == s && x.GetGenericArguments().Count() == 2);
            var queryableType = query.GetType().GetGenericArguments()[0];
            var transformType = transform.GetType().GetGenericArguments()[1];
            var closedCreateMethod = openCreateMethod.MakeGenericMethod(queryableType, transformType);
            return (DataTableResult)closedCreateMethod.Invoke(null, new object[] { query, request, transform, outputType, arrayOutputType });
        }

        public static DataTableResult CreateMutable(IQueryable query, IFilterRequest request,
            OutputType? outputType = null, ArrayOutputType? arrayOutputType = null)
        {
            var s = "CreateMutable";
            var openCreateMethod = typeof(DataTableResultFactory).GetMethods().Single(x => x.Name == s && x.GetGenericArguments().Count() == 1);
            var queryableType = query.GetType().GetGenericArguments()[0];
            var closedCreateMethod = openCreateMethod.MakeGenericMethod(queryableType);
            return (DataTableResult)closedCreateMethod.Invoke(null, new object[] { query, request, outputType, arrayOutputType });
        }

        public static MutableDataTableResult CreateMutableFromCollection<TSource>(ICollection<TSource> q, IFilterRequest request,
            OutputType? outputType = null, ArrayOutputType? arrayOutputType = null)
        {
            return CreateMutable(q.AsQueryable(), request, outputType, arrayOutputType);
        }

        public static MutableDataTableResult CreateMutableFromResponse<TSource, TTransform>(IPageResponse<TSource> response,
            Func<TSource, TTransform> transform, OutputType? outputType = null, ArrayOutputType? arrayOutputType = null)
        {
            IPageResponse<object> mutableResponse = PageResponse<TSource>.ToMutableResponse(response);
            var result = new MutableDataTableResult(mutableResponse, outputType);

            result.Data = result.Data
                .Transform<TSource, Dictionary<string, object>>(row => TransformTypeInfo.MergeTransformValuesIntoDictionary(transform, row))
                .Transform<Dictionary<string, object>, Dictionary<string, object>>(StringTransformers.StringifyValues);

            result.Data = ApplyOutputRulesForMutable(result.Data, arrayOutputType);

            return result;
        }

        public static MutableDataTableResult CreateMutableFromResponse<TSource>(IPageResponse<TSource> response,
            OutputType? outputType = null, ArrayOutputType? arrayOutputType = null)
        {
            IPageResponse<object> mutableResponse = PageResponse<TSource>.ToMutableResponse(response);
            var result = new MutableDataTableResult(mutableResponse, outputType);

            result.Data = result.Data
                .Transform<TSource, Dictionary<string, object>>(DataTablesTypeInfo<TSource>.ToDictionary)
                .Transform<Dictionary<string, object>, Dictionary<string, object>>(StringTransformers.StringifyValues);

            result.Data = ApplyOutputRulesForMutable(result.Data, arrayOutputType);

            return result;
        }

        #endregion

        #region Plain results

        /// <typeparam name="TSource">The type of the source query</typeparam>
        /// <param name="query">A queryable for the data.</param>
        /// <param name="request">The request filters</param>
        /// /// <param name="outputType">The output type</param>
        /// <returns>The action result</returns>
        public static PlainDataTableResult<TSource> CreatePlain<TSource>(IQueryable<TSource> query, IFilterRequest request, OutputType? outputType = null)
        {
            var result = new PlainDataTableResult<TSource>(query, request, outputType);
            return result;
        }

        public static DataTableResult CreatePlain(IQueryable query, IFilterRequest request, OutputType? outputType = null)
        {
            var s = "CreatePlain";
            var openCreateMethod = typeof(DataTableResultFactory).GetMethods().Single(x => x.Name == s && x.GetGenericArguments().Count() == 1);
            var queryableType = query.GetType().GetGenericArguments()[0];
            var closedCreateMethod = openCreateMethod.MakeGenericMethod(queryableType);
            return (DataTableResult)closedCreateMethod.Invoke(null, new object[] { query, request, outputType });
        }

        public static PlainDataTableResult<TSource> CreatePlainFromCollection<TSource>(ICollection<TSource> q, IFilterRequest request, OutputType? outputType = null)
        {
            return CreatePlain(q.AsQueryable(), request, outputType);
        }

        public static PlainDataTableResult<TSource> CreatePlainFromResponse<TSource>(IPageResponse<TSource> response,
            OutputType? outputType = null)
        {
            var result = new PlainDataTableResult<TSource>(response, outputType);
            return result;
        }

        #endregion
    }
}