using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NineNineQuotes.Services;
using NineNineQuotes.Data;
using NineNineQuotes.Wrappers;
using NineNineQuotes.Filter;
using Microsoft.AspNetCore.Http;

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

        [HttpGet("random")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRandomQuoteAsync()
        {
            Quote quote = await _quoteService.GetRandomQuoteAsync();
            return Ok(new SingleResponse<Quote>(quote));
        }

        // GET api/<QuotesController>/random/from?character=Amy
        // GET api/<QuotesController>/random/from?episode=AC/DC
        [HttpGet("random/from")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetRandomQuoteFromAsync([FromQuery] string character, string episode)
        {
            Quote quote = await _quoteService.GetRandomQuoteFromAsync(character, episode);
            return quote != null ? Ok(new SingleResponse<Quote>(quote)) : NotFound(new SingleResponse<Quote>(quote));
        }

        // GET api/<QuotesController>/all/
        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllQuotesAsync([FromQuery] PaginationFilter filter)
        {
            PaginationFilter inputFilter = new(filter.PageNumber, filter.PageSize);
            List<Quote> response = await _quoteService.GetAllQuotesAsync(filter.PageNumber, filter.PageSize);

            return response.Count != 0
                ? Ok(new PagedResponse<List<Quote>>(response, inputFilter.PageNumber, inputFilter.PageSize))
                : NotFound(new SingleResponse<List<Quote>>(response));
        }

        // GET api/<QuotesController>/all/from?character=Jake&pageNumber=2&pageSize=50
        // GET api/<QuotesController>/all/from?episode=AC/DC&pageNumber=2&pageSize=50
        [HttpGet("all/from")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllQuotesFromAsync([FromQuery] string character, string episode, [FromQuery] PaginationFilter filter)
        {
            PaginationFilter inputFilter = new(filter.PageNumber, filter.PageSize);
            List<Quote> response = await _quoteService.GetAllQuotesFromAsync(character, episode, filter.PageNumber, filter.PageSize);

            return response.Count != 0 
                ? Ok(new PagedResponse<List<Quote>>(response, inputFilter.PageNumber, inputFilter.PageSize))
                : NotFound(new SingleResponse<List<Quote>>(response));
        }

        // GET api/<QuotesController>/find?character=Amy&searchTerm=pet&pageNumber=1&pageSize=50
        // GET api/<QuotesController>/find?&searchTerm=pet&pageNumber=1&pageSize=50
        [HttpGet("find")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> FindQuoteFromAsync([FromQuery] PaginationFilter filter, string character, string searchTerm)
        {
            PaginationFilter inputFilter = new(filter.PageNumber, filter.PageSize);
            List<Quote> response = await _quoteService.FindQuoteAsync(character, searchTerm);

            return response.Count != 0
                ? Ok(new PagedResponse<List<Quote>>(response, inputFilter.PageNumber, inputFilter.PageSize))
                : NotFound(new SingleResponse<List<Quote>>(response));
        }
    }
}
