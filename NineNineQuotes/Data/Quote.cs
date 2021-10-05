using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace NineNineQuotes.Data
{
    [DataContract]
    public class Quote
    {
        [JsonIgnore]
        public int Id { get; set; }
        public string Character { get; set; }
        public string Episode { get; set; }
        public string QuoteText { get; set; }
    }
}
