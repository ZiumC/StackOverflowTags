namespace StackOverflowTags.Services
{
    public interface IHttpService
    {
        public Task<string> DoGetAsync(string url);
    }
}
