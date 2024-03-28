using Microsoft.EntityFrameworkCore;
using StackOverflowTags.DbContexts;
using StackOverflowTags.Models.DatabaseModels;

namespace StackOverflowTags.Services
{
    public class StackOverflowService : IStackOverflowService
    {
        private readonly InMemoryContext _inMemoryContext;

        public StackOverflowService(IHttpService httpService)
        {
            _inMemoryContext = new InMemoryContext().GetDatabaseContextAsync(httpService).Result;
        }

        public async Task<IEnumerable<TagModel>> GetStackOverflowTagsAsync()
        {
            var tags = new List<TagModel>();

            return tags;
        }
    }
}
