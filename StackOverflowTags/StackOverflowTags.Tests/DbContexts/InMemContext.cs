using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using StackOverflowTags.DbContexts;
using StackOverflowTags.Models.DatabaseModels;
using StackOverflowTags.Services.HttpService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackOverflowTags.Tests.DbContexts
{
    public class InMemContext 
    {
        public async Task<InMemoryContext> GetDatabaseContext()
        {
            var options = new DbContextOptionsBuilder<InMemoryContext>()
                .UseInMemoryDatabase(databaseName: "TEST_DB")
                .Options;

            var inMemContext = new InMemoryContext(options);
            await inMemContext.Database.EnsureCreatedAsync();

            if (inMemContext.Tags.Count() == 0)
            {
                inMemContext.Tags.Add(new TagModel
                {
                    Id = 1,
                    Name = "java",
                    IsModeratorOnly = false,
                    HasSynonyms = false,
                    IsRequired = true,
                    Count = 12
                });
                inMemContext.Tags.Add(new TagModel
                {
                    Id = 2,
                    Name = "c#",
                    IsModeratorOnly = false,
                    HasSynonyms = false,
                    IsRequired = true,
                    Count = 22
                });
                inMemContext.Tags.Add(new TagModel
                {
                    Id = 3,
                    Name = "android",
                    IsModeratorOnly = false,
                    HasSynonyms = false,
                    IsRequired = true,
                    Count = 9
                });
                inMemContext.Tags.Add(new TagModel
                {
                    Id = 4,
                    Name = "scala",
                    IsModeratorOnly = false,
                    HasSynonyms = false,
                    IsRequired = false,
                    Count = 3
                });
                inMemContext.Tags.Add(new TagModel
                {
                    Id = 5,
                    Name = "python",
                    IsModeratorOnly = false,
                    HasSynonyms = false,
                    IsRequired = true,
                    Count = 7
                });
                inMemContext.Tags.Add(new TagModel
                {
                    Id = 6,
                    Name = "php",
                    IsModeratorOnly = true,
                    HasSynonyms = false,
                    IsRequired = true,
                    Count = 21
                });
                inMemContext.Tags.Add(new TagModel
                {
                    Id = 7,
                    Name = "lua",
                    IsModeratorOnly = false,
                    HasSynonyms = false,
                    IsRequired = false,
                    Count = 4
                });

                await inMemContext.SaveChangesAsync();
            }

            return inMemContext;

        }

        public DbSet<TagModel> Tags { get; set; }   
    }
}
