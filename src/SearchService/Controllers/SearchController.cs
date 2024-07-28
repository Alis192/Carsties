using Microsoft.AspNetCore.Mvc;
using MongoDB.Entities;
using ZstdSharp.Unsafe;

namespace SearchService;

[ApiController]
[Route("api/search")]
public class SearchController : ControllerBase
{
    /// <summary>
    /// Paginated Search
    /// </summary>
    /// <param name="searchTerm"></param>
    /// <param name="pageNumber">It is a parameter that represents the current page number that the user wants to access. The default value is set to 1</param>
    /// <param name="pageSize">It is another parameter that determines the number of items to display on each page</param>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult<List<Item>>> SearchItems([FromQuery]SearchParams searchParams)
    {
        // Initialize a paginated search query for Item objects. The first 'Item' specifies the entity type, and the second 'Item' specifies the result set type.
        var query = DB.PagedSearch<Item, Item>();

        // Check if a search term is provided. If so, perform a full-text search and sort the results by text score relevance.
        if(!string.IsNullOrEmpty(searchParams.SearchTerm))
        {
            query.Match(Search.Full, searchParams.SearchTerm).SortByTextScore();
        }

        // Use a switch expression to determine the sorting order based on the 'OrderBy' parameter in 'searchParams'.
        query = searchParams.OrderBy switch 
        {
            // If 'OrderBy' is "make", sort by the 'Make' property in ascending order.
            "make" => query.Sort(x => x.Ascending(a => a.Make))
                .Sort(x => x.Ascending(a => a.Model)),
            // If 'OrderBy' is "new", sort by the 'CreatedAt' property in descending order to show newest items first.
            "new" => query.Sort(x => x.Descending(a => a.CreatedAt)),
            // If 'OrderBy' is neither "make" nor "new", sort by the 'AuctionEnd' property in ascending order by default.
            _ => query.Sort(x => x.Ascending(a => a.AuctionEnd))
        };


        // Use a switch expression to determine the sorting order based on the 'OrderBy' parameter in 'searchParams'.
        query = searchParams.FilterBy switch 
        {
            "finished" => query.Match(x => x.AuctionEnd < DateTime.UtcNow),
            "endingSoon" => query.Match(x => x.AuctionEnd < DateTime.UtcNow.AddHours(6) && x.AuctionEnd > DateTime.UtcNow),
            _ => query.Match(x => x.AuctionEnd > DateTime.UtcNow)
        };


        if (!string.IsNullOrEmpty(searchParams.Seller))
        {
            query.Match(x => x.Seller == searchParams.Seller);
        }

        if (!string.IsNullOrEmpty(searchParams.Winner))
        {
            query.Match(x => x.Winner == searchParams.Winner);
        }



        // Set the page number for the query to the 'PageNumber' specified in 'searchParams'.
        query.PageNumber(searchParams.PageNumber);
        // Set the number of items per page for the query to the 'PageSize' specified in 'searchParams'.
        query.PageSize(searchParams.PageSize);

        // Execute the query asynchronously and store the result.
        var result = await query.ExecuteAsync(); 

        // Return an HTTP 200 OK response with the search results, including the list of items, page count, and total item count.
        return Ok(new 
        {
            results = result.Results,     // The list of items found in the search.
            pageCount = result.PageCount, // The total number of pages available.
            totalCount = result.TotalCount // The total number of items found.
        });

    }
}
