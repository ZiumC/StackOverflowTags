using FluentAssertions;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using StackOverflowTags.DbContexts;
using StackOverflowTags.Models.DatabaseModels;
using StackOverflowTags.Services.HttpService;
using StackOverflowTags.Services.StackOverflowService;
using StackOverflowTags.Tests.DbContexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackOverflowTags.Tests.Tests.UnitTests.ApiServiceTests
{
    public class StackOverflowServiceTest
    {
        private readonly IConfiguration _config;
        public StackOverflowServiceTest()
        {
            _config = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile(
                        path: "appsettings.json",
                        optional: false,
                        reloadOnChange: true)
                    .Build();
        }

        [Fact]
        public async void StackOverflowService_GetStackOverflowTagsAsync_ReturnsTags()
        {
            //Arrange
            var inMemContext = await new InMemContext()
                .GetDatabaseContext();
            var stackOverflowService = new StackOverflowService(inMemContext, _config, new HttpService());

            //Act
            var tags = await stackOverflowService.GetStackOverflowTagsAsync();

            //Assert
            tags.Should().NotBeNull();
            tags.Should().BeOfType<List<TagModel>>();
            tags.Count().Should().Be(7);
            tags
                .Where(t => string.IsNullOrEmpty(t.Name))
                .Should().BeNullOrEmpty();
        }
    }
}
