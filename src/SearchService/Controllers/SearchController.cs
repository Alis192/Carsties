using Microsoft.AspNetCore.Mvc;
using MongoDB.Entities;

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
    public async Task<ActionResult<List<Item>>> SearchItems(string searchTerm, int pageNumber = 1, int pageSize = 4)
    {
        //This line is starting a new query that will look for Item objects in the MongoDB database
        var query = DB.PagedSearch<Item>();

        // Sorts Items with Mark property in Ascending order
        query.Sort(x => x.Ascending(a => a.Make)); 



        if(!string.IsNullOrEmpty(searchTerm))
        {
            query.Match(Search.Full, searchTerm).SortByTextScore();
        }

        query.PageNumber(pageNumber);
        query.PageSize(pageSize);

        var result = await query.ExecuteAsync(); 

        return Ok(new 
        {
            results = result.Results,
            pageCount = result.PageCount,
            totalCount = result.TotalCount 
        });
    }
}
