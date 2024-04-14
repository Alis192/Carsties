using System.Text.Json;
using MongoDB.Driver;
using MongoDB.Entities;

namespace SearchService;
public class DbInitializer
{
    public static async Task InitDb(WebApplication app) 
    {
        //Initializes a MongoDB database named SearchDb using a connection string obtained from the application’s configuration.
        await DB.InitAsync("SearchDb", MongoClientSettings.FromConnectionString(app.Configuration.GetConnectionString("MongoDbConnection")));

        await DB.Index<Item>()
            .Key(x => x.Make, KeyType.Text)
            .Key(x => x.Model, KeyType.Text)
            .Key(x => x.Color, KeyType.Text)
            .CreateAsync();

        var count = await DB.CountAsync<Item>();

        if (count == 0) 
        {
            Console.WriteLine("No data - will attempt to seed");
            var itemData = await File.ReadAllTextAsync("Data/auctions.json");

            var options = new JsonSerializerOptions{PropertyNameCaseInsensitive = true};
            
            var items = JsonSerializer.Deserialize<List<Item>>(itemData, options);

            await DB.SaveAsync(items);
        }
    }
}

