using Microsoft.EntityFrameworkCore;
using StackOverflowTags.DbContexts;
using StackOverflowTags.Mappers;
using StackOverflowTags.Models.DatabaseModels;
using StackOverflowTags.Models.JsonModels;
using StackOverflowTags.Services.HttpService;

namespace StackOverflowTags.Services.StackOverflowService
{
    public class StackOverflowService : IStackOverflowService
    {
        private readonly InMemoryContext _inMemoryContext;

        public StackOverflowService(InMemoryContext inMemoryContext)
        {
            _inMemoryContext = inMemoryContext;
            _inMemoryContext.Database.EnsureCreated();
        }

        public async Task<IEnumerable<TagModel>> GetStackOverflowTagsAsync()
        {
            return await _inMemoryContext.Tags
                .ToListAsync();
        }

        public Task<bool> RefillDatabase()
        {
            throw new NotImplementedException();
        }
    }
}
