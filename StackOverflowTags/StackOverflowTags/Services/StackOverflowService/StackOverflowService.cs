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
        private readonly TagUtils _tagUtils;
        private readonly IConfiguration _config;
        private readonly IHttpService _httpService;
        private readonly string? _tagsJsonField;

        public StackOverflowService(InMemoryContext inMemoryContext, IConfiguration config, IHttpService httpService)
        {
            _inMemoryContext = inMemoryContext;
            _inMemoryContext.Database.EnsureCreated();
            _config = config;
            _httpService = httpService;
            _tagUtils = new TagUtils(_httpService);

            _tagsJsonField = _config["Application:TagsJsonField"];


        }

        public async Task<IEnumerable<TagModel>> GetStackOverflowTagsAsync()
        {
            return await _inMemoryContext.Tags
                .ToListAsync();
        }

        public async Task<bool> RefillDatabase(string? url)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new Exception("Unable to receive data due to url string is empty");
            }

            var newTags = _tagUtils.DoTagRequest(url, _tagsJsonField);
            if (newTags == null || newTags.Count() == 0)
            {
                return false;
            }
            long totalShare = newTags.Select(x => x.Count).Sum();

            //this wokraround is needed due to transactions aren't supported using DatabaseInMemory
            var tagsCopy = _inMemoryContext.Tags.ToList();
            foreach (var tag in _inMemoryContext.Tags)
            {
                _inMemoryContext.Tags.Remove(tag);
            }
            await _inMemoryContext.SaveChangesAsync();

            try
            {
                foreach (var tag in newTags)
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

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                await _inMemoryContext.Database.EnsureDeletedAsync();
                foreach (var tag in tagsCopy)
                {
                    _inMemoryContext.Tags.Add(tag);
                }
                await _inMemoryContext.SaveChangesAsync();
                return false;
            }
        }
    }
}
