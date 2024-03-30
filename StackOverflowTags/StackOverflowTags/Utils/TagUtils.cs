using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StackOverflowTags.Models.DatabaseModels;

namespace StackOverflowTags.Mappers
{
    public class TagUtils
    {
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
    }
}
