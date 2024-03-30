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

        private readonly IConfiguration _config;
        private readonly IHttpService _httpService;
        private readonly TagUtils _tagUtils;
        private readonly string? _url;
        private readonly string? _tagsJsonField;

        public InMemoryContext(DbContextOptions<InMemoryContext> options, IConfiguration config, IHttpService httpService) : base(options)
        {
            _config = config;
            _httpService = httpService;
            _tagUtils = new TagUtils(_httpService);

            _url = _config["EndpointHosts:StackOverflow:Tags"];
            _tagsJsonField = _config["Application:TagsJsonField"];
        }

        public InMemoryContext(DbContextOptions<InMemoryContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TagModel>(tm =>
            {
                tm.HasKey(tm => tm.Id);
                tm.Property(tm => tm.Id).ValueGeneratedOnAdd();
            });

            if (string.IsNullOrEmpty(_url))
            {
                return;
            }

            var newTags = _tagUtils.DoTagRequest(_url, _tagsJsonField);
            if (newTags == null || newTags.Count() == 0)
            {
                throw new Exception("Response of tags request is empty");
            }

            var totalShare = newTags.Select(td => td.Count).Sum();
            int id = 1;
            foreach (var tag in newTags) 
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
                        IsRequired = tag.Is_required,
                        Share = (double)tag.Count / (double)totalShare
                    });
                });
                id++;
            }
        }

        public DbSet<TagModel> Tags { get; set; }

    }
}
