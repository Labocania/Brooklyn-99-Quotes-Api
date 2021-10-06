
namespace NineNineQuotes.Wrappers
{
    // Credit: https://codewithmukesh.com/blog/pagination-in-aspnet-core-webapi/
    public class SingleResponse<T>
    {
        public T Data { get; set; }

        public SingleResponse() { }
        public SingleResponse(T data)
        {
            Data = data;
        }
    }
}
