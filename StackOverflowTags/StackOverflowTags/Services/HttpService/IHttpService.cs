using StackOverflowTags.Models.DatabaseModels;

namespace StackOverflowTags.Services.HttpService
{
    public interface IHttpService
    {
        public Task<string> DoGetAsync(string url);
    }
}
