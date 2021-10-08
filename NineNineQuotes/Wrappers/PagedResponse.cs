using System;
namespace NineNineQuotes.Wrappers
{
    // Credit: https://codewithmukesh.com/blog/pagination-in-aspnet-core-webapi/
    public class PagedResponse<T> : SingleResponse<T>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        public PagedResponse(T data, int pageNumber, int pageSize, string message = "")
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            Message = message;
            Data = data;
        }
    }
}
