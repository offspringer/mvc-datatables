
namespace Mvc.Datatables
{
    /// <summary>
    /// Sort configuration for filter requests
    /// </summary>
    public class Sort : ISort
    {
        /// <summary>
        /// Column to which ordering should be applied. This is an index reference to the columns array of information that is also submitted to the server.
        /// </summary>
        public virtual int Column { get; set; }

        /// <summary>
        /// Ordering direction for this column. It will be asc or desc to indicate ascending ordering or descending ordering, respectively.
        /// </summary>
        public virtual SortDirection Direction { get; set; }

        public Sort() { }

        public Sort(int column, SortDirection direction)
        {
            this.Column = column;
            this.Direction = direction;
        }
    }
}
