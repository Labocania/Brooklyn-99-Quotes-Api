using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace NineNineQuotes.Data
{
    public static class QuoteSeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new AppDbContext(serviceProvider.GetRequiredService<DbContextOptions<AppDbContext>>()))
            {
                if (context.Quotes.Any())
                {
                    return;
                }

                string fileName = "Data/nine-nine-quotes.json";

                using (System.IO.FileStream openStream = System.IO.File.OpenRead(fileName))
                {
                    JsonDocument document = JsonDocument.Parse(openStream);

                    using (document)
                    {
                        foreach (var element in document.RootElement.GetProperty("root").EnumerateArray())
                        {
                            context.Add(element.ToObject<Quote>());
                        }
                    }
                }

                context.SaveChanges();
            }
        }
    }

    // Credits: https://stackoverflow.com/a/59047063 User: https://stackoverflow.com/users/3744182/dbc
    // Converts JsonElement straight to C# class.
    public static partial class JsonExtensions
    {
        public static T ToObject<T>(this JsonElement element, JsonSerializerOptions options = null)
        {
            var bufferWriter = new System.Buffers.ArrayBufferWriter<byte>();
            using (var writer = new Utf8JsonWriter(bufferWriter))
                element.WriteTo(writer);
            return JsonSerializer.Deserialize<T>(bufferWriter.WrittenSpan, options);
        }

        public static T ToObject<T>(this JsonDocument document, JsonSerializerOptions options = null)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));
            return document.RootElement.ToObject<T>(options);
        }
    }
}
