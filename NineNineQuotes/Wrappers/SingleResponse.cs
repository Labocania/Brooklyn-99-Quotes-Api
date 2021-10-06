
namespace NineNineQuotes.Wrappers
{
    // Credit: https://codewithmukesh.com/blog/pagination-in-aspnet-core-webapi/
    public class SingleResponse<T>
    {
        public bool Succeeded { get; set; }
        public string[] Errors { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }

        public SingleResponse() { }
        public SingleResponse(T data)
        {
            Succeeded = true;
            Message = string.Empty;
            Errors = null;
            Data = data;
        }
    }
}
