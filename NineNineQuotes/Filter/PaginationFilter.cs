
namespace NineNineQuotes.Filter
{
    // Credit: https://codewithmukesh.com/blog/pagination-in-aspnet-core-webapi/
    public class PaginationFilter
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public PaginationFilter()
        {
            PageNumber = 1;
            PageSize = 50;
        }
        public PaginationFilter(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber < 1 ? 1 : pageNumber;
            PageSize = pageSize <= 0 || pageSize > 50 ? 50 : pageSize;
        }
    }
}
