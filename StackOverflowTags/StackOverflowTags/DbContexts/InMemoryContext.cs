using Microsoft.EntityFrameworkCore;
using StackOverflowTags.Services;

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

        public async Task<InMemoryContext> GetDatabaseContextAsync(IHttpService httpService)
        {
            var options = new DbContextOptionsBuilder<InMemoryContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var prodDbContext = new InMemoryContext(options);
            prodDbContext.Database.EnsureDeleted();

            Console.WriteLine(await httpService.DoGetAsync("https://api.stackexchange.com/2.3/tags?page=1&pagesize=1&order=desc&sort=popular&site=stackoverflow"));

            return prodDbContext;
        }
    }
}
