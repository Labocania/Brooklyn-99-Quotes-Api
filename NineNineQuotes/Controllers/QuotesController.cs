using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NineNineQuotes.Services;
using NineNineQuotes.Data;
using NineNineQuotes.Wrappers;
using NineNineQuotes.Filter;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NineNineQuotes.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuotesController : ControllerBase
    {
        private readonly QuoteService _quoteService;

        public QuotesController(QuoteService quoteService)
        {
            _quoteService = quoteService;
        }

        // GET: api/<QuotesController>/random
        [HttpGet("random")]
        public async Task<IActionResult> GetRandomQuoteAsync()
        {
            Quote quote = await _quoteService.GetRandomQuoteAsync();
            return Ok(new SingleResponse<Quote>(quote, "Quote returned."));
        }

        // GET api/<QuotesController>/random/Amy
        [HttpGet("random/{character:maxlength(30)}")]
        public async Task<IActionResult> GetRandomQuoteFromCharacterAsync(string character)
        {
            Quote quote = await _quoteService.GetRandomQuoteFromCharacterAsync(character);

            if (quote != null)
            {
                return Ok(new SingleResponse<Quote>(quote, "Quote returned."));
            }
            else
            {
                return NotFound(new SingleResponse<Quote>(quote, "Quote not found."));
            }
        }

        // GET api/<QuotesController>/all/Jake?pageNumber=1&pageSize=50
        [HttpGet("all/{character:maxlength(20)}")]
        public async Task<IActionResult> GetAllQuotesFromCharacter([FromQuery] PaginationFilter filter, string character)
        {
            PaginationFilter inputFilter = new(filter.PageNumber, filter.PageSize);
            List<Quote> response = await _quoteService.GetAllQuotesFromCharacter(character, filter.PageNumber, filter.PageSize);

            if (response.Count != 0)
            {
                return Ok(new PagedResponse<List<Quote>>(response, inputFilter.PageNumber, inputFilter.PageSize, "Quote returned."));
            }
            else
            {
                return NotFound(new SingleResponse<List<Quote>>(response, "Quote not found."));
            }
        }

        // GET api/<QuotesController>/find?character=Amy&searchTerm=pet&pageNumber=1&pageSize=50
        // LIMIT string length
        [HttpGet("find")]
        public async Task<IActionResult> FindQuoteFrom([FromQuery] PaginationFilter filter, string character, string searchTerm)
        {
            PaginationFilter inputFilter = new(filter.PageNumber, filter.PageSize);
            List<Quote> response = await _quoteService.FindQuote(character, searchTerm);

            if (response.Count != 0)
            {
                return Ok(new PagedResponse<List<Quote>>(response, inputFilter.PageNumber, inputFilter.PageSize));
            }
            else
            {
                return NotFound(new SingleResponse<List<Quote>>(response, "Quote not found."));
            }
        }
    }
}
