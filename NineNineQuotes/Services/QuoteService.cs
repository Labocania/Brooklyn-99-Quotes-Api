using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NineNineQuotes.Data;

namespace NineNineQuotes.Services
{
    public class QuoteService
    {
        private readonly AppDbContext _context;
        private Random _random = new();
        public QuoteService(AppDbContext context)
        {
            _context = context;
        }

        private IQueryable<Quote> FindCharacter(string character)
        {
            return _context.Quotes.Where(quote => quote.Character == character).AsNoTracking();
        }

        private IQueryable<Quote> FindEpisode(string episode)
        {
            return _context.Quotes.Where(quote => quote.Episode == episode).AsNoTracking();
        }

        public async Task<Quote> GetRandomQuoteAsync()
        {
            int maxId = _context.Quotes.OrderBy(e => e.Id).AsNoTracking().LastOrDefault().Id;
            return await _context.Quotes.FindAsync(_random.Next(1, maxId));
        }

        public async Task<Quote> GetRandomQuoteFromAsync(string character, string episode)
        {
            IQueryable<Quote> query = character != null ? FindCharacter(character) : FindEpisode(episode);

            if (query.Any())
            {
                int maxSkip = query.Count();
                return await query.OrderBy(quote => quote.Id)
                    .Skip(_random.Next(1, maxSkip))
                    .Take(1)
                    .FirstOrDefaultAsync();
            }
            else
            {
                return null;
            }
        }

        public async Task<List<Quote>> GetAllQuotesAsync(int skipNumber, int takeNumber)
        {
            return await _context.Quotes
                .AsNoTracking()
                .OrderBy(quote => quote.Id)
                .Skip((skipNumber - 1) * takeNumber)
                .Take(takeNumber)
                .ToListAsync();
        }

        public async Task<List<Quote>> GetAllQuotesFromAsync(string character, string episode, int skipNumber, int takeNumber)
        {
            IQueryable<Quote> query = character != null ? FindCharacter(character) : FindEpisode(episode);
            return query.Any() ? await query.OrderBy(quote => quote.Id)
                                        .Skip((skipNumber - 1) * takeNumber)
                                        .Take(takeNumber)
                                        .ToListAsync()
                               : null;
        }

        public async Task<List<Quote>> FindQuoteAsync(string character, string searchTerm)
        {
            IQueryable<Quote> query = FindCharacter(character);
            if (query.Any())
            {
                return await query
                    .Where(quote => EF.Functions.ToTsVector(quote.QuoteText).Matches(EF.Functions.PhraseToTsQuery(searchTerm)))
                    .ToListAsync();
            }
            else
            {
                return await _context.Quotes
                    .Where(quote => EF.Functions.ToTsVector(quote.QuoteText).Matches(EF.Functions.PhraseToTsQuery(searchTerm)))
                    .ToListAsync();
            }

        }
    }
}
