﻿using FluentAssertions;
using Microsoft.Extensions.Configuration;
using StackOverflowTags.Mappers;
using StackOverflowTags.Models.JsonModels;
using StackOverflowTags.Tests.DbContexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackOverflowTags.Tests.Tests.UnitTests.MappersTest
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
        public void TagUtils_MapResponse_ReturnsTags()
        {
            //Arrange
            var tagUtils = new TagUtils();
            var hardcodedTagsStrings = new HardcodedStringContext().GetHardcoded_6_Tags();
            var tagsJsonField = _config["Application:TagsJsonField"];

            //Act
            var desarielizedObjects = tagUtils.DeserializeResponse<IEnumerable<JsonTagModel>>(hardcodedTagsStrings, tagsJsonField);

            //Assert
            desarielizedObjects.Should().NotBeNullOrEmpty();
            desarielizedObjects?.Count().Should().Be(6);
            desarielizedObjects?.Select(x => x.Count).Sum().Should().Be(11135478);
            desarielizedObjects?
                .Where(x => string.IsNullOrEmpty(x.Name))
                .Should().BeNullOrEmpty();
        }
    }
}