using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using StackOverflowTags.Models.DatabaseModels;
using StackOverflowTags.Services;
using StackOverflowTags.Mappers;
using StackOverflowTags.Models.JsonModels;

namespace StackOverflowTags.DbContexts
{
    public class InMemoryContext : DbContext
    {

        public InMemoryContext(DbContextOptions opt) : base(opt)
        {
        }

        public InMemoryContext()
        {

        }

        public async Task<InMemoryContext> GetDatabaseContextAsync(IConfiguration config, IHttpService httpService)
        {
            var options = new DbContextOptionsBuilder<InMemoryContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var prodDbContext = new InMemoryContext(options);
            prodDbContext.Database.EnsureDeleted();

            string? url = config["EndpointHosts:StackOverflow:Tags"];
            if (string.IsNullOrEmpty(url))
            {
                throw new Exception("Unable to create in memory data due to url string is empty");
            }

            url = string.Format(url, 1, 1);
            string stackOverflowTagsString = await httpService.DoGetAsync(url);

            var jsonData = new TagMapper().DeserializeResponse<IEnumerable<JsonTagModel>>(stackOverflowTagsString, "items");
            if (jsonData == null)
            {
                throw new Exception("Unavle to receive C# objest from string");
            }

            

            return prodDbContext;
        }

    }
}
