namespace StackOverflowTags.Services
{
    public class HttpService : IHttpService
    {
        private readonly IConfiguration _config;
        public HttpService(IConfiguration config)
        {
            _config = config;
        }
    }
}
