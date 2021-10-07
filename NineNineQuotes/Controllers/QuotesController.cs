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
        public async Task<Quote> GetRandomQuote()
        {
            return await _quoteService.GetRandomQuoteAsync();
        }

        // GET api/<QuotesController>/random/Amy
        [HttpGet("random/{character:maxlength(30)}")]
        public async Task<Quote> GetRandomQuoteFromCharacterAsync(string character)
        {
            return await _quoteService.GetRandomQuoteFromCharacterAsync(character);
        }

        // GET api/<QuotesController>/all/Jake?pageNumber=1&pageSize=50
        [HttpGet("all/{character:maxlength(30)}")]
        public async Task<IActionResult> GetAllQuotesFromCharacter([FromQuery] PaginationFilter filter, string character)
        {
            PaginationFilter inputFilter = new(filter.PageNumber, filter.PageSize);
            List<Quote> response = await _quoteService.GetAllQuotesFromCharacter(character, filter.PageNumber, filter.PageSize);

            if (response.Count != 0)
            {
                return Ok(new PagedResponse<List<Quote>>(response, inputFilter.PageNumber, inputFilter.PageSize));
            }
            else
            {
                return NoContent();
            }
        }

        // GET api/<QuotesController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }
    }
}
