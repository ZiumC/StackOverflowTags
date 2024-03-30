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
using System.Globalization;
using System.Numerics;

namespace StackOverflowTags.DbContexts
{
    public class InMemoryContext : DbContext
    {

        private readonly int _times = 11;
        private readonly int _size = 100;
        private readonly IConfiguration _config;
        private readonly IHttpService _httpService;
        private readonly string? _url;
        private readonly string? _tagsJsonField;

        public InMemoryContext(DbContextOptions<InMemoryContext> options, IConfiguration config, IHttpService httpService) : base(options)
        {
            _config = config;
            _httpService = httpService;
            _url = _config["EndpointHosts:StackOverflow:Tags"];
            _tagsJsonField = _config["Application:TagsJsonField"];

            if (string.IsNullOrEmpty(_url))
            {
                throw new Exception("Unable to create in memory data due to url string is empty");
            }

            if (string.IsNullOrEmpty(_tagsJsonField))
            {
                throw new Exception("Unable to create in memory data due to url string is empty");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TagModel>(tm =>
            {
                tm.HasKey(tm => tm.Id);
                tm.Property(tm => tm.Id).ValueGeneratedOnAdd();
            });

            int id = 1;
            for (int i = 1; i <= _times; i++)
            {
                string url = string.Format(_url, i, _size);
                string stackOverflowTagsString = _httpService.DoGetAsync(url).Result;
                var tagData = new TagUtils(_httpService).DeserializeResponse<IEnumerable<JsonTagModel>>(stackOverflowTagsString, _tagsJsonField);
                if (tagData == null)
                {
                    throw new Exception("Unavle to receive C# objest from string");
                }

                var totalShare = tagData.Select(td => td.Count).Sum();

                foreach (var tag in tagData)
                {
                    modelBuilder.Entity<TagModel>(tm =>
                    {
                        tm.HasData(new TagModel
                        {

                            Id = id++,
                            Name = tag.Name,
                            Count = tag.Count,
                            HasSynonyms = tag.Has_synonyms,
                            IsModeratorOnly = tag.Is_moderator_only,
                            IsRequired = tag.Is_required,
                            Share = (double)tag.Count / (double)totalShare
                        });
                    });
                }
            }
        }
        public DbSet<TagModel> Tags { get; set; }

    }
}
