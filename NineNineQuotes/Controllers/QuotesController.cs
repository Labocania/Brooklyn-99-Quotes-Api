using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public SingleResponse<Quote> GetRandomQuote()
        {
            Quote randomQuote = _quoteService.GetRandomQuote().Result;
            SingleResponse<Quote> response = new SingleResponse<Quote>(randomQuote);
            return response;
        }

        // GET api/<QuotesController>/all?character=Jake&pageNumber=1&pageSize=50
        [HttpGet("all/{character}")]
        public async Task<IActionResult> GetAllQuotesFromCharacter([FromQuery] PaginationFilter filter, string character)
        {
            PaginationFilter inputFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            List<Quote> response = await _quoteService.GetAllQuotesFromCharacter(character, filter.PageNumber, filter.PageSize);
            return Ok(new PagedResponse<List<Quote>>(response, inputFilter.PageNumber, inputFilter.PageSize));
        }

        // GET api/<QuotesController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }
    }
}
