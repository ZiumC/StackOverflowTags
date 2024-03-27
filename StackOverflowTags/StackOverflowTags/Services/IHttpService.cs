namespace StackOverflowTags.Services
{
    public interface IHttpService
    {
        public Task<string> DoGet(string url);
    }
}
