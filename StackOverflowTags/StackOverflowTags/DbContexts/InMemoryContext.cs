using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using StackOverflowTags.Models.DatabaseModels;
using StackOverflowTags.Mappers;
using StackOverflowTags.Models.JsonModels;
using StackOverflowTags.Services.HttpService;
using System;
using Microsoft.Extensions.Configuration;

namespace StackOverflowTags.DbContexts
{
    public class InMemoryContext : DbContext
    {

        private readonly int _times = 11;
        private readonly int _size = 100;
        private readonly IConfiguration _config;
        private readonly IHttpService _httpService;

        public InMemoryContext(DbContextOptions<InMemoryContext> options, IConfiguration config, IHttpService httpService) : base(options)
        {
            _config = config;
            _httpService = httpService;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            string? url = _config["EndpointHosts:StackOverflow:Tags"];
            string? keyNameTagsJson = _config["Application:TagsKeyJsonName"];
            int id = 1;
            long totalShare = 0;

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
                string stackOverflowTagsString = "{\"items\": [{\"has_synonyms\": true,\"is_moderator_only\": false,\"is_required\": false,\"count\": 2528942,\"name\": \"javascript\"}],\"has_more\": true,\"quota_max\": 10000,\"quota_remaining\": 9693}";
                try
                {
                    var tagData = new TagMapper().DeserializeResponse<IEnumerable<JsonTagModel>>(stackOverflowTagsString, keyNameTagsJson);
                    if (tagData == null)
                    {
                        throw new Exception("Unavle to receive C# objest from string");
                    }

                    foreach (var tag in tagData)
                    {
                        modelBuilder.Entity<TagModel>(tm =>
                        {
                            tm.HasData(new TagModel
                            {
                                Id = id,
                                Name = tag.Name,
                                Count = tag.Count,
                                HasSynonyms = tag.Has_synonyms,
                                IsModeratorOnly = tag.Is_moderator_only,
                                IsRequired = tag.Is_required
                            });
                        });
                        id++;
                        totalShare = totalShare + tag.Count;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        public DbSet<TagModel> Tags { get; set; }

    }
}
