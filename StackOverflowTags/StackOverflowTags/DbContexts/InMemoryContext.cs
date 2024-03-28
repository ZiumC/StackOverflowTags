using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using StackOverflowTags.Models.DatabaseModels;
using StackOverflowTags.Mappers;
using StackOverflowTags.Models.JsonModels;
using StackOverflowTags.Services.HttpService;

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
            string? keyNameTagsJson = config["Application:TagsKeyJsonName"];
            if (string.IsNullOrEmpty(url))
            {
                throw new Exception("Unable to create in memory data due to url string is empty");
            }

            int id = 1;
            int times = 11;
            int pageSize = 100;
            for (int i = 1; i <= times; i++)
            {

                url = string.Format(url, i, pageSize);
                string stackOverflowTagsString = await httpService.DoGetAsync(url);

                try
                {
                    var tagData = new TagMapper().DeserializeResponse<IEnumerable<JsonTagModel>>(stackOverflowTagsString, keyNameTagsJson);
                    if (tagData == null)
                    {
                        throw new Exception("Unavle to receive C# objest from string");
                    }

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
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }

            await inMemDbContext.SaveChangesAsync();

            var totalShare = inMemDbContext.Tags.Select(t => t.Count).Sum();
            foreach (var tag in inMemDbContext.Tags)
            {
                double share = (double)tag.Count / (double)totalShare;
                tag.Share = share;
            }
            await inMemDbContext.SaveChangesAsync();

            return inMemDbContext;
        }

        public DbSet<TagModel> Tags { get; set; }

    }
}
