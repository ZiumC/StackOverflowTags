using StackOverflowTags.Models.DatabaseModels;

namespace StackOverflowTags.Services
{
    public interface IStackOverflowService
    {
        public Task<IEnumerable<TagModel>> GetStackOverflowTagsAsync();
    }
}
