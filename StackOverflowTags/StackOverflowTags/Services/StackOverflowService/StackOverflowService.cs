using Microsoft.EntityFrameworkCore;
using StackOverflowTags.DbContexts;
using StackOverflowTags.Mappers;
using StackOverflowTags.Models.DatabaseModels;
using StackOverflowTags.Models.JsonModels;
using StackOverflowTags.Services.HttpService;
using System.Reflection.Emit;
using System;

namespace StackOverflowTags.Services.StackOverflowService
{
    public class StackOverflowService : IStackOverflowService
    {
        private readonly InMemoryContext _inMemoryContext;
        private readonly IConfiguration _config;
        private readonly IHttpService _httpService;
        private readonly int _times = 1;
        private readonly int _size = 100;

        public StackOverflowService(InMemoryContext inMemoryContext, IConfiguration config, IHttpService httpService)
        {
            _inMemoryContext = inMemoryContext;
            _inMemoryContext.Database.EnsureCreated();
            _config = config;
            _httpService = httpService;
        }

        public async Task<IEnumerable<TagModel>> GetStackOverflowTagsAsync()
        {
            return await _inMemoryContext.Tags
                .ToListAsync();
        }

        public async Task<bool> RefillDatabase()
        {
            foreach (var tag in _inMemoryContext.Tags)
            {
                _inMemoryContext.Tags.Remove(tag);
            }
            await _inMemoryContext.SaveChangesAsync();

            string? url = _config["EndpointHosts:StackOverflow:Tags"];
            string? keyNameTagsJson = _config["Application:TagsKeyJsonName"];
            int id = 1;

            if (string.IsNullOrEmpty(url))
            {
                throw new Exception("Unable to create in memory data due to url string is empty");
            }

            if (string.IsNullOrEmpty(keyNameTagsJson))
            {
                throw new Exception("Unable to create in memory data due to url string is empty");
            }

            for (int i = 1; i <= _times; i++)
            {
                url = string.Format(url, i, _size);
                //string stackOverflowTagsString = await _httpService.DoGetAsync(url);
                string stackOverflowTagsString = "{\"items\":[{\"has_synonyms\":true,\"is_moderator_only\":false,\"is_required\":false,\"count\":2528949,\"name\":\"javascript\"},{\"has_synonyms\":true,\"is_moderator_only\":false,\"is_required\":false,\"count\":2192345,\"name\":\"python\"},{\"has_synonyms\":true,\"is_moderator_only\":false,\"is_required\":false,\"count\":1917388,\"name\":\"java\"},{\"has_synonyms\":true,\"is_moderator_only\":false,\"is_required\":false,\"count\":1615031,\"name\":\"c#\"},{\"collectives\":[{\"tags\":[\"php\"],\"external_links\":[{\"type\":\"support\",\"link\":\"https://stackoverflow.com/contact?topic=15\"}],\"description\":\"A collective where developers working with PHP can learn and connect about the open source scripting language.\",\"link\":\"/collectives/php\",\"name\":\"PHP\",\"slug\":\"php\"}],\"has_synonyms\":true,\"is_moderator_only\":false,\"is_required\":false,\"count\":1464485,\"name\":\"php\"},{\"collectives\":[{\"tags\":[\"android\",\"ios\"],\"external_links\":[{\"type\":\"support\",\"link\":\"https://stackoverflow.com/contact?topic=15\"}],\"description\":\"A collective for developers who want to share their knowledge and learn more about mobile development practices and platforms\",\"link\":\"/collectives/mobile-dev\",\"name\":\"Mobile Development\",\"slug\":\"mobile-dev\"}],\"has_synonyms\":true,\"is_moderator_only\":false,\"is_required\":false,\"count\":1417280,\"name\":\"android\"}],\"has_more\":true,\"quota_max\":10000,\"quota_remaining\":9692}";
                try
                {
                    var tagData = new TagMapper().DeserializeResponse<IEnumerable<JsonTagModel>>(stackOverflowTagsString, keyNameTagsJson);
                    if (tagData == null)
                    {
                        throw new Exception("Unavle to receive C# objest from string");
                    }

                    var totalShare = tagData.Select(td => td.Count).Sum();

                    foreach (var tag in tagData)
                    {
                        _inMemoryContext.Tags.Add(new TagModel
                        {
                            Id = id,
                            Name = tag.Name,
                            Count = tag.Count,
                            HasSynonyms = tag.Has_synonyms,
                            IsModeratorOnly = tag.Is_moderator_only,
                            IsRequired = tag.Is_required,
                            Share = (double)tag.Count / (double)totalShare
                        });
                        ;
                        id++;
                    }

                    await _inMemoryContext.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return false;
                }
            }
            return true;
        }
    }
}
