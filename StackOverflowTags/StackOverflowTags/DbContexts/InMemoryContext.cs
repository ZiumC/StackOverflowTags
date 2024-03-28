using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using StackOverflowTags.Models.DatabaseModels;
using StackOverflowTags.Services;
using StackOverflowTags.Mappers;
using StackOverflowTags.Models.JsonModels;

namespace StackOverflowTags.DbContexts
{
    public class InMemoryContext : DbContext
    {

        public InMemoryContext(DbContextOptions opt) : base(opt)
        {
        }

        public InMemoryContext()
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TagModel>(tm =>
            {
                tm.HasKey(tm => tm.Id);
                tm.Property(tm => tm.HasSynonyms).IsRequired();
                tm.Property(tm => tm.IsModeratorOnly).IsRequired();
                tm.Property(tm => tm.IsRequired).IsRequired();
                tm.Property(tm => tm.Count).IsRequired();
                tm.Property(tm => tm.Name).IsRequired().HasMaxLength(512);
            });
        }

        public async Task<InMemoryContext> GetDatabaseContextAsync(IConfiguration config, IHttpService httpService)
        {
            var options = new DbContextOptionsBuilder<InMemoryContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var inMemDbContext = new InMemoryContext(options);
            inMemDbContext.Database.EnsureDeleted();

            string? url = config["EndpointHosts:StackOverflow:Tags"];
            if (string.IsNullOrEmpty(url))
            {
                throw new Exception("Unable to create in memory data due to url string is empty");
            }

            url = string.Format(url, 1, 1);
            string stackOverflowTagsString = await httpService.DoGetAsync(url);

            var tagData = new TagMapper().DeserializeResponse<IEnumerable<JsonTagModel>>(stackOverflowTagsString, "items");
            if (tagData == null)
            {
                throw new Exception("Unavle to receive C# objest from string");
            }

            int id = 1;
            foreach (var tag in tagData)
            {
                inMemDbContext.Add(new TagModel
                {
                    Id = id,
                    Name = tag.Name,
                    Count = tag.Count,
                    HasSynonyms = tag.Has_synonyms,
                    IsModeratorOnly = tag.Is_moderator_only,
                    IsRequired = tag.Is_required
                });
                id++;
            }
            await inMemDbContext.SaveChangesAsync();

            return inMemDbContext;
        }

        public DbSet<TagModel> Tags { get; set; }

    }
}
