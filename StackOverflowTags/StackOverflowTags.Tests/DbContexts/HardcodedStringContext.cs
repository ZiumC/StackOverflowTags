using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackOverflowTags.Tests.DbContexts
{
    public class HardcodedStringContext
    {
        public string GetHardcoded_6_Tags() 
        {
            return "{\"items\":[" +
                "{\"has_synonyms\": true,\"is_moderator_only\": false,\"is_required\": false,\"count\": 2528949,\"name\": \"javascript\"}," +
                "{\"has_synonyms\": true,\"is_moderator_only\": false,\"is_required\": false,\"count\": 2192345,\"name\": \"python\"}," +
                "{\"has_synonyms\": true,\"is_moderator_only\": false,\"is_required\": false,\"count\": 1917388,\"name\": \"java\"}," +
                "{\"has_synonyms\": true,\"is_moderator_only\": false,\"is_required\": false,\"count\": 1615031,\"name\": \"c#\"}," +
                "{\"has_synonyms\": true,\"is_moderator_only\": false,\"is_required\": false,\"count\": 1464485,\"name\": \"php\"}," +
                "{\"has_synonyms\": true,\"is_moderator_only\": false,\"is_required\": false,\"count\": 1417280,\"name\": \"android\"}]," +
                "\"has_more\": true,\"quota_max\": 10000,\"quota_remaining\": 9692}";
        }
    }
}
