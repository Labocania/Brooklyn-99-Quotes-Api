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
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class QuotesController : ControllerBase
    {
        private readonly QuoteService _quoteService;

        public QuotesController(QuoteService quoteService)
        {
            _quoteService = quoteService;
        }

        /// <summary>
        /// Returns a random Quote.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET api/quotes/random
        ///     {
        ///        "Message": "Quote returned.",
        ///        "Data": {
        ///             "Character": "Charles",
        ///             "Episode": "Paranoia",
        ///             "QuoteText": "..."
        ///         }       
        ///     }
        ///
        /// </remarks>
        /// <returns>A random Quote.</returns>
        /// <response code="200">Returns a random Quote.</response>
        [HttpGet("random")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRandomQuoteAsync()
        {
            Quote quote = await _quoteService.GetRandomQuoteAsync();
            return Ok(new SingleResponse<Quote>(quote));
        }

        /// <summary>
        /// Returns random quote from a character or episode.
        /// </summary>
        /// <remarks>
        /// If both query params are provided only character quotes are returned.
        ///
        ///     GET api/quotes/random/from?character=Amy
        ///     {
        ///        "Message": "Quote returned.",
        ///        "Data": {
        ///             "Character": "Amy",
        ///             "Episode": "Paranoia",
        ///             "QuoteText": "..."
        ///         }       
        ///     }
        ///     
        ///     GET api/quotes/random/from?episode=AC/DC
        ///     {
        ///        "Message": "Quote returned.",
        ///        "Data": {
        ///             "Character": "Rosa",
        ///             "Episode": "AC/DC",
        ///             "QuoteText": "..."
        ///         }       
        ///     }
        ///
        /// </remarks>
        /// <returns>Returns random quote from a character or episode.</returns>
        /// <response code="200">Returns a random Quote.</response>
        /// <response code="404">Returns message "Quote not found".</response>
        [HttpGet("random/from")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetRandomQuoteFromAsync([FromQuery] string character, string episode)
        {
            Quote quote = await _quoteService.GetRandomQuoteFromAsync(character, episode);
            return quote != null ? Ok(new SingleResponse<Quote>(quote)) : NotFound(new SingleResponse<Quote>(quote));
        }

        /// <summary>
        /// Returns all available quotes.
        /// </summary>
        /// <remarks>
        /// Can navigate pages by setting param pageNumber.
        /// Page size limited by maximum of 50, can be set lower.
        /// 
        ///     GET api/quotes/all
        ///     {   
        ///         "PageNumber": 1,
        ///         "PageSize": 2,
        ///         "Message": "Quote returned.",
        ///         "Data": [
        ///             {
        ///                 "Character": "Captain Holt",
        ///                 "Episode": "Honeymoon",
        ///                 "QuoteText": "..."
        ///             },
        ///             {
        ///                 "Character": "Hitchcock",
        ///                 "Episode": "M.E. Time",
        ///                 "QuoteText": "..."
        ///             }
        ///         ]
        ///     }
        /// </remarks>
        /// <returns>Returns all available quotes.</returns>
        /// <response code="200">Returns all available quotes.</response>
        /// <response code="404">Returns message "Quote not found".</response>
        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllQuotesAsync([FromQuery] PaginationFilter filter)
        {
            PaginationFilter inputFilter = new(filter.PageNumber, filter.PageSize);
            List<Quote> response = await _quoteService.GetAllQuotesAsync(inputFilter.PageNumber, inputFilter.PageSize);

            return response.Count != 0
                ? Ok(new PagedResponse<List<Quote>>(response, inputFilter.PageNumber, inputFilter.PageSize))
                : NotFound(new SingleResponse<List<Quote>>(response));
        }

        /// <summary>
        /// Returns all available quotes from a character or episode.
        /// </summary>
        /// <remarks>
        /// Can navigate pages by setting param pageNumber.
        /// Page size limited by maximum of 50, can be set lower.
        /// If both query params are provided only character quotes are returned.
        /// 
        ///     GET api/quotes/all/from?character=Jake
        ///     {   
        ///         "PageNumber": 1,
        ///         "PageSize": 2,
        ///         "Message": "Quote returned.",
        ///         "Data": [
        ///             {
        ///                 "Character": "Jake",
        ///                 "Episode": "Honeymoon",
        ///                 "QuoteText": "..."
        ///             },
        ///             {
        ///                 "Character": "Jake",
        ///                 "Episode": "M.E. Time",
        ///                 "QuoteText": "..."
        ///             }
        ///         ]
        ///     }
        /// </remarks>
        /// <returns>Returns all available quotes from a character or episode.</returns>
        /// <response code="200">Returns all available quotes from a character or episode.</response>
        /// <response code="404">Returns message "Quote not found".</response>
        [HttpGet("all/from")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllQuotesFromAsync([FromQuery] string character, string episode, [FromQuery] PaginationFilter filter)
        {
            PaginationFilter inputFilter = new(filter.PageNumber, filter.PageSize);
            List<Quote> response = await _quoteService.GetAllQuotesFromAsync(character, episode, inputFilter.PageNumber, inputFilter.PageSize);

            return response != null && response.Count != 0  
                ? Ok(new PagedResponse<List<Quote>>(response, inputFilter.PageNumber, inputFilter.PageSize))
                : NotFound(new SingleResponse<List<Quote>>(response));
        }

        /// <summary>
        /// Returns quotes from search term and/or character.
        /// </summary>
        /// <remarks>
        /// Can navigate pages by setting param pageNumber.
        /// Page size limited by maximum of 50, can be set lower.
        /// Can search for term in isolation but also filter by character.
        /// Case insensitive.
        /// 
        ///     GET api/quotes/find?searchTerm=pet
        ///     {   
        ///         "PageNumber": 1,
        ///         "PageSize": 2,
        ///         "Message": "Quote returned.",
        ///         "Data": [
        ///             {
        ///                 "Character": "Amy",
        ///                 "Episode": "Greg and Larry",
        ///                 "QuoteText": "..."
        ///             },
        ///             {
        ///                 "Character": "Scully",
        ///                 "Episode": "The Honeypot",
        ///                 "QuoteText": "..."
        ///             }
        ///         ]
        ///     }
        ///     
        ///     GET api/quotes/find?character=Amy&amp;searchTerm=pet
        ///     {   
        ///         "PageNumber": 1,
        ///         "PageSize": 2,
        ///         "Message": "Quote returned.",
        ///         "Data": [
        ///             {
        ///                 "Character": "Amy",
        ///                 "Episode": "Greg and Larry",
        ///                 "QuoteText": "..."
        ///             },
        ///             {
        ///                 "Character": "Amy",
        ///                 "Episode": "Chocolate Milk",
        ///                 "QuoteText": "..."
        ///             }
        ///         ]
        ///     }
        /// </remarks>
        /// <returns>Returns quotes from search term and/or character..</returns>
        /// <response code="200">Returns quotes from search term and/or character.</response>
        /// <response code="404">Returns message "Quote not found".</response>
        [HttpGet("find")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> FindQuoteFromAsync([FromQuery] PaginationFilter filter, string character, string searchTerm)
        {
            PaginationFilter inputFilter = new(filter.PageNumber, filter.PageSize);
            List<Quote> response = await _quoteService.FindQuoteAsync(character, searchTerm);

            return response != null && response.Count != 0
                ? Ok(new PagedResponse<List<Quote>>(response, inputFilter.PageNumber, inputFilter.PageSize))
                : NotFound(new SingleResponse<List<Quote>>(response));
        }
    }
}
