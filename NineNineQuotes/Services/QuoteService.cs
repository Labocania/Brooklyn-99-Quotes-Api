using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NineNineQuotes.Data;

namespace NineNineQuotes.Services
{
    public class QuoteService
    {
        private readonly AppDbContext _context;
        private Random _random;
        public QuoteService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Quote> GetRandomQuote()
        {
            _random = new Random();
            int maxId = _context.Quotes.OrderBy(e => e.Id).LastOrDefault().Id;
            return await _context.Quotes.FindAsync(_random.Next(1, maxId));
        }
    }
}
