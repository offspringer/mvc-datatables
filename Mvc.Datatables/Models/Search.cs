
namespace Mvc.Datatables
{
    /// <summary>
    /// Search configuration for columns of a filter request
    /// </summary>
    public class Search : ISearch
    {
        /// <summary>
        /// Search value to apply to this specific column.
        /// </summary>
        public virtual string Value { get; set; }

        /// <summary>
        /// Flag to indicate if the search term for this column should be treated as regular expression (true) or not (false). As with global search, normally server-side processing scripts will not perform regular expression searching for performance reasons on large data sets, but it is technically possible and at the discretion of your script.
        /// </summary>
        public virtual bool IsRegex { get; set; }

        public Search() { }

        public Search(string value, bool isRegex)
        {
            this.Value = value;
            this.IsRegex = isRegex;
        }
    }
}
