using StackOverflowTags.Models.DatabaseModels;

namespace StackOverflowTags.Services.StackOverflowService
{
    public interface IStackOverflowService
    {
        public Task<IEnumerable<TagModel>> GetStackOverflowTagsAsync();
        public Task<bool> RefillDatabase();
    }
}
