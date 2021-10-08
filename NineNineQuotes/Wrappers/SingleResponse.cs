
namespace NineNineQuotes.Wrappers
{
    // Credit: https://codewithmukesh.com/blog/pagination-in-aspnet-core-webapi/
    public class SingleResponse<T>
    {
        public string Message { get; set; }
        public T Data { get; set; }

        public SingleResponse() { }
        public SingleResponse(T data, string message = "")
        {
            Data = data;
            Message = message;
        }
    }
}
