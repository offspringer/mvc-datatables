
namespace Mvc.Datatables
{
    /// <summary>
    /// Columns' parameters for filter requests
    /// </summary>
    public class Column : IColumn
    {
        /// <summary>
        /// Column's data source.
        /// </summary>
        public virtual string Data { get; set; }

        /// <summary>
        /// Column's name.
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Flag to indicate if this column is searchable (true) or not (false). 
        /// </summary>
        public virtual bool Searchable { get; set; }

        /// <summary>
        /// Flag to indicate if this column is sortable (true) or not (false).
        /// </summary>
        public virtual bool Sortable { get; set; }

        /// <summary>
        /// The search component for the column.
        /// </summary>
        private ISearch _search;
        public virtual ISearch Search
        {
            get
            {
                if (_search == null)
                    _search = new Search();
                return _search;
            }
            set
            {
                _search = value ?? new Search();
            }
        }

        public Column() { }
    }
}
