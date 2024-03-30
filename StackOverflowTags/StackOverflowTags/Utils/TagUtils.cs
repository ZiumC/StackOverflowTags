using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StackOverflowTags.Models.DatabaseModels;
using StackOverflowTags.Models.JsonModels;
using StackOverflowTags.Services.HttpService;

namespace StackOverflowTags.Mappers
{
    public class TagUtils
    {
        private readonly IHttpService _httpService;

        public TagUtils(IHttpService httpService)
        {
            _httpService = httpService;
        }

        public T? DeserializeResponse<T>(string httpMessage, string? key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new Exception("Key value can't be empty in parsing json string to object");
            }

            var jsonData = JsonConvert.DeserializeObject(httpMessage) as JObject;

            if (jsonData == null)
            {
                throw new Exception("JObject is empty during parse operation");
            }

            return JsonConvert.DeserializeObject<T>(jsonData[key].ToString());
        }

        public List<JsonTagModel> DoTagRequestAsync(string url, string? jsonTagField, int times = 11, int maxPageSize = 100)
        {
            if (string.IsNullOrEmpty(jsonTagField))
            {
                throw new Exception("Json tag field is empty");
            }

            var tagsData = new List<JsonTagModel>();
            for (int i = 1; i <= times; i++)
            {
                url = string.Format(url, i, maxPageSize);
                var deserializedTags = DeserializeResponse<IEnumerable<JsonTagModel>>(
                    _httpService.DoGetAsync(url).Result,
                    jsonTagField
                );

                if (deserializedTags != null)
                {
                    foreach (var deserializedTag in deserializedTags)
                    {
                        tagsData.Add(deserializedTag);
                    }
                }
            }

            return tagsData;
        }
    }
}
