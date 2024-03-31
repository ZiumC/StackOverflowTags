using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using StackOverflowTags.Controllers;
using StackOverflowTags.Mappers;
using StackOverflowTags.Models.JsonModels;
using StackOverflowTags.Services.HttpService;
using StackOverflowTags.Services.StackOverflowService;
using StackOverflowTags.Tests.DbContexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackOverflowTags.Tests.Tests.UnitTests.UtilsTests
{
    public class TagUtilityTest
    {

        public readonly IConfiguration _config;


        public TagUtilityTest()
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
        public void TagUtils_DeserializeResponse_ReturnsDeserializedTags()
        {
            //Arrange
            var httpServiceLogger = Mock.Of<ILogger<HttpService>>();
            var tagUtils = new TagUtils(new HttpService(httpServiceLogger));
            var hardcodedTagsStrings = new HardcodedStringContext().GetHardcoded_6_Tags();
            var tagsJsonField = _config["Application:TagsJsonField"];

            //Act
            var desarielizedObjects = tagUtils
                .DeserializeResponse<IEnumerable<JsonTagModel>>(hardcodedTagsStrings, tagsJsonField);

            //Assert
            desarielizedObjects.Should().NotBeNullOrEmpty();
            desarielizedObjects?.Should().BeOfType<List<JsonTagModel>>();
            desarielizedObjects?.Count().Should().Be(6);
            desarielizedObjects?.Select(x => x.Count).Sum().Should().Be(11135478);
            desarielizedObjects?
                .Where(x => string.IsNullOrEmpty(x.Name))
                .Should().BeNullOrEmpty();
        }

        [Fact]
        public void TagUtils_DoTagRequestAsync_ReturnsDeserializedTags()
        {
            //Arrange
            var httpServiceLogger = Mock.Of<ILogger<HttpService>>();
            var tagUtils = new TagUtils(new HttpService(httpServiceLogger));
            var tagsJsonField = _config["Application:TagsJsonField"];
            var url = _config["EndpointHosts:StackOverflow:Tags"];

            //Act
            var desarielizedObjects = tagUtils
                .DoTagRequest(url, tagsJsonField, 1, 100);

            //Assert
            desarielizedObjects.Should().NotBeNullOrEmpty();
            desarielizedObjects.Should().BeOfType<List<JsonTagModel>>();
            desarielizedObjects.Count().Should().Be(100);
            desarielizedObjects
                .Where(x => string.IsNullOrEmpty(x.Name))
                .Should().BeNullOrEmpty();
        }

        [Fact]
        public void TagUtils_DoTagRequestAsync_ReturnsEmptyTags()
        {
            //Arrange
            var httpServiceLogger = Mock.Of<ILogger<HttpService>>();
            var tagUtils = new TagUtils(new HttpService(httpServiceLogger));
            var tagsJsonField = _config["Application:TagsJsonField"];
            var url = _config["EndpointHosts:StackOverflow:Tags"];

            //Act
            var desarielizedObjects = tagUtils
                .DoTagRequest(url, tagsJsonField, 0, 0);

            //Assert
            desarielizedObjects.Should().BeNullOrEmpty();
            desarielizedObjects.Should().BeOfType<List<JsonTagModel>>();
        }
    }
}
