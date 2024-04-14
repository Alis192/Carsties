using Microsoft.AspNetCore.Mvc;
using MongoDB.Entities;

namespace SearchService;

[ApiController]
[Route("api/search")]
public class SearchController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<Item>>> SearchItems(string searchTerm)
    {
        //This line is starting a new query that will look for Item objects in the MongoDB database
        var query = DB.Find<Item>();

        query.Sort(x => x.Ascending(a => a.Make)); 

        if(!string.IsNullOrEmpty(searchTerm))
        {
            query.Match(Search.Full, searchTerm).SortByTextScore();
        }

        var result = await query.ExecuteAsync(); 

        return result;
    }
}
