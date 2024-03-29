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
        private readonly int _times = 11;
        private readonly int _size = 100;
        private readonly string? _url;
        private readonly string? _keyNameTagsJson;

        public StackOverflowService(InMemoryContext inMemoryContext, IConfiguration config, IHttpService httpService)
        {
            _inMemoryContext = inMemoryContext;
            _inMemoryContext.Database.EnsureCreated();
            _config = config;
            _httpService = httpService;

            _url = _config["EndpointHosts:StackOverflow:Tags"];
            _keyNameTagsJson = _config["Application:TagsKeyJsonName"];


            if (string.IsNullOrEmpty(_url))
            {
                throw new Exception("Unable to create in memory data due to url string is empty");
            }

            if (string.IsNullOrEmpty(_keyNameTagsJson))
            {
                throw new Exception("Unable to create in memory data due to url string is empty");
            }
        }

        public async Task<IEnumerable<TagModel>> GetStackOverflowTagsAsync()
        {
            return await _inMemoryContext.Tags
                .ToListAsync();
        }

        public async Task<bool> RefillDatabase()
        {
            //this wokraround is needed due to transactions aren't supported using DatabaseInMemory
            var dataCopy = _inMemoryContext.Tags.ToList();

            foreach (var tag in _inMemoryContext.Tags)
            {
                _inMemoryContext.Tags.Remove(tag);
            }
            await _inMemoryContext.SaveChangesAsync();

            for (int i = 1; i <= _times; i++)
            {
                string url = string.Format(_url, i, _size);
                string stackOverflowTagsString = await _httpService.DoGetAsync(url);
                var tagData = new TagMapper().DeserializeResponse<IEnumerable<JsonTagModel>>(stackOverflowTagsString, _keyNameTagsJson);
                if (tagData == null)
                {
                    await _inMemoryContext.Database.EnsureDeletedAsync();
                    foreach (var tag in dataCopy)
                    {
                        _inMemoryContext.Tags.Add(tag);
                    }
                    await _inMemoryContext.SaveChangesAsync();
                    return false;
                }

                var totalShare = tagData.Select(td => td.Count).Sum();

                foreach (var tag in tagData)
                {
                    _inMemoryContext.Tags.Add(new TagModel
                    {
                        Name = tag.Name,
                        Count = tag.Count,
                        HasSynonyms = tag.Has_synonyms,
                        IsModeratorOnly = tag.Is_moderator_only,
                        IsRequired = tag.Is_required,
                        Share = (double)tag.Count / (double)totalShare
                    });
                }

                await _inMemoryContext.SaveChangesAsync();
            }
            return true;
        }
    }
}
