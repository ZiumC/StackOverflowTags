using StackOverflowTags.Models.DatabaseModels;

namespace StackOverflowTags.Services
{
    public interface IHttpService
    {
        public Task<string> DoGetAsync(string url);
    }
}
