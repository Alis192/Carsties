using AuctionService.Data;
using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<AuctionDbContext>(opt => {
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// This method retrieves all assemblies loaded in the current application domain. 
// AutoMapper will scan each of these assemblies for classes that inherit from Profile.
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddMassTransit( x => {
    x.UsingRabbitMq((context, cfg) => {
        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

try 
{
    DbInitializer.InitDb(app);
}
catch (Exception e)
{
    Console.WriteLine(e);
}

app.Run();
