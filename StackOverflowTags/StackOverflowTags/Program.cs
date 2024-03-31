using Microsoft.EntityFrameworkCore;
using NReco.Logging.File;
using Microsoft.OpenApi.Models;
using StackOverflowTags.DbContexts;
using StackOverflowTags.Services.HttpService;
using StackOverflowTags.Services.StackOverflowService;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IHttpService, HttpService>();
builder.Services.AddScoped<IStackOverflowService, StackOverflowService>();
builder.Services.AddDbContext<InMemoryContext>(opt => opt.UseInMemoryDatabase("InMemoryDb"));

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Pinionszek API",
        Version = "v1",
        Description = "GET endpoints"
    });

    c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());

    // Set the comments path for the Swagger JSON and UI.
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});


builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.AddFile("app.log", append: true);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint($"/swagger/v1/swagger.json", "StackOverflow Api v1");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
