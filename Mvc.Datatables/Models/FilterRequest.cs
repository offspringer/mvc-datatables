using System.Collections.Generic;
using System.Linq;

namespace Mvc.Datatables
{
    /// <summary>
    /// Filter request parameters
    /// </summary>
    public class FilterRequest : IDataTableMessage
    {
        /// <summary>
        /// Draw counter. This is used by DataTables to ensure that the Ajax returns from server-side processing requests are drawn in sequence by DataTables (Ajax requests are asynchronous and thus can return out of sequence). 
        /// </summary>
        public int Draw { get; set; }

        /// <summary>
        /// Paging first record indicator. This is the start point in the current data set (0 index based - i.e. 0 is the first record).
        /// </summary>
        public int Start { get; set; }

        /// <summary>
        /// Number of records that the table can display in the current draw. It is expected that the number of records returned will be equal to this number, unless the server has fewer records to return. Note that this can be -1 to indicate that all records should be returned (although that negates any benefits of server-side processing!)
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// Global search component to be applied to all columns which have searchable as true.
        /// </summary>
        private Search _search;
        public Search Search
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

        /// <summary>
        /// The columns' sorting configuration.
        /// </summary>
        private List<Sort> _sort;
        public List<Sort> Sort
        {
            get
            {
                if (_sort == null)
                    _sort = new List<Sort>();
                return _sort;
            }
            set
            {
                _sort = value ?? new List<Sort>();
            }
        }

        /// <summary>
        /// The columns collection.
        /// </summary>
        private Dictionary<int, Column> _columns;
        public Dictionary<int, Column> Columns
        {
            get
            {
                if (_columns == null)
                    _columns = new Dictionary<int, Column>();
                return _columns;
            }
            set
            {
                _columns = value ?? new Dictionary<int, Column>();
            }
        }

        public FilterRequest()
        {
            this.Length = 10;
        }

        public bool HasColumn(int index)
        {
            return this.Columns.ContainsKey(index);
        }

        public Column GetColumn(int index)
        {
            return this.Columns.ContainsKey(index) ? this.Columns[index] : null;
        }

        public bool HasSortForColumn(int index)
        {
            return this.Sort.Any(x => x.Column == index);
        }

        public Sort GetSortForColumn(int index)
        {
            return this.Sort.SingleOrDefault(x => x.Column == index);
        }
    }
}
