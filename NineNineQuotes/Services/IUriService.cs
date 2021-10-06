using System;

namespace NineNineQuotes.Services
{
    // Credit: https://codewithmukesh.com/blog/pagination-in-aspnet-core-webapi/
    public interface IUriService
    {
        public Uri GetPageUri(Filter.PaginationFilter filter, string route);
    }
}
