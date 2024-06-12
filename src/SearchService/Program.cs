using System.Net;
using MassTransit;
using Namespace;
using Polly;
using Polly.Extensions.Http;
using SearchService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddHttpClient<AuctionSvcHttpClient>().AddPolicyHandler(GetPolicy());


builder.Services.AddMassTransit( x => {   
   // This method scans the specified namespace for consumer classes and registers them
   x.AddConsumersFromNamespaceContaining<AuctionCreatedConsumer>();

   // This sets the naming convention for endpoints. KebabCaseEndpointNameFormatter converts endpoint names to kebab-case
   x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("search", false));

   // Configures RabbitMQ as the transport protocol for MassTransit. 
   // The cfg.ConfigureEndpoints(context) method automatically configures endpoints for the registered consumers.
    x.UsingRabbitMq((context, cfg) => {
        cfg.ConfigureEndpoints(context);
    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.


app.UseAuthorization();

app.MapControllers();


app.Lifetime.ApplicationStarted.Register(async () => 
{
   try 
   {
      await DbInitializer.InitDb(app);
   }
   catch (Exception e)
   {
      Console.WriteLine(e);
   }
});


app.Run();


static IAsyncPolicy<HttpResponseMessage> GetPolicy() 
   => HttpPolicyExtensions
      .HandleTransientHttpError()
      .OrResult(msg => msg.StatusCode == HttpStatusCode.NotFound)
      .WaitAndRetryForeverAsync(_ => TimeSpan.FromSeconds(3));

      
