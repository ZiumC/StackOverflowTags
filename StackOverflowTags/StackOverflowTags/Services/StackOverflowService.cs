using Microsoft.EntityFrameworkCore;
using StackOverflowTags.DbContexts;
using StackOverflowTags.Models.DatabaseModels;

namespace StackOverflowTags.Services
{
    public class StackOverflowService : IStackOverflowService
    {
        private readonly InMemoryContext _inMemoryContext;

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
