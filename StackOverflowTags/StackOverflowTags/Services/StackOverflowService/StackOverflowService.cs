using Microsoft.EntityFrameworkCore;
using StackOverflowTags.DbContexts;
using StackOverflowTags.Models.DatabaseModels;
using StackOverflowTags.Services.HttpService;

namespace StackOverflowTags.Services.StackOverflowService
{
    public class StackOverflowService : IStackOverflowService
    {
        private InMemoryContext _inMemoryContext;

        public StackOverflowService(IConfiguration config, IHttpService httpService)
        {
            _inMemoryContext = new InMemoryContext().GetDatabaseContextAsync(config, httpService).Result;
        }

        public async Task<IEnumerable<TagModel>> GetStackOverflowTagsAsync()
        {
            return await _inMemoryContext.Tags.ToListAsync();
        }
    }
}
