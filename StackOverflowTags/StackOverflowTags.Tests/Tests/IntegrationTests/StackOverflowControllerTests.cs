﻿using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using StackOverflowTags.Controllers;
using StackOverflowTags.Models.DatabaseModels;
using StackOverflowTags.Services.HttpService;
using StackOverflowTags.Services.StackOverflowService;
using StackOverflowTags.Tests.DbContexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackOverflowTags.Tests.Tests.IntegrationTests
{
    public class StackOverflowControllerTests
    {
        private readonly IConfiguration _config;

        public StackOverflowControllerTests()
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
        public async Task StackOverflowController_GetTagsAsync_ReturnsTags() 
        {
            //Arrange
            var inMemContext = await new InMemContext()
                .GetDatabaseContext();
            var httpService = new HttpService();
            var stackOverflowService = new StackOverflowService(inMemContext, _config, httpService);
            var stackOverflowController = new StackOverflowController(stackOverflowService, _config, httpService);

            //Act
            var okRequest_1 = await stackOverflowController.GetTagsAsync(); //with default request vals
            var okActionResult_1 = okRequest_1 as OkObjectResult;
            var tagsResult_1 = okActionResult_1?.Value as IEnumerable<TagModel>;

            var okRequest_2 = await stackOverflowController.GetTagsAsync(2, 2);
            var okActionResult_2 = okRequest_2 as OkObjectResult;
            var tagsResult_2 = okActionResult_2?.Value as IEnumerable<TagModel>;

            var okRequest_3 = await stackOverflowController.GetTagsAsync(1, 3, "desc");
            var okActionResult_3 = okRequest_3 as OkObjectResult;
            var tagsResult_3 = okActionResult_3?.Value as IEnumerable<TagModel>;

            //Assert
            okRequest_1.Should().BeOfType<OkObjectResult>();
            okActionResult_1.Should().NotBeNull();
            tagsResult_1.Should().NotBeNullOrEmpty();
            tagsResult_1?.Count().Should().Be(7);
            tagsResult_1?
                .Where(x => string.IsNullOrEmpty(x.Name))
                .Should().BeNullOrEmpty();
            tagsResult_1?.First().Name.Should().Be("android");
            tagsResult_1?.Last().Name.Should().Be("scala");

            okRequest_2.Should().BeOfType<OkObjectResult>();
            okActionResult_2.Should().NotBeNull();
            tagsResult_2.Should().NotBeNullOrEmpty();
            tagsResult_2?.Count().Should().Be(2);
            tagsResult_2?
                .Where(x => string.IsNullOrEmpty(x.Name))
                .Should().BeNullOrEmpty();
            tagsResult_2?.First().Name.Should().Be("android");
            tagsResult_2?.Last().Name.Should().Be("scala");

            okRequest_3.Should().BeOfType<OkObjectResult>();
            okActionResult_3.Should().NotBeNull();
            tagsResult_3.Should().NotBeNullOrEmpty();
            tagsResult_3?.Count().Should().Be(3);
            tagsResult_3?
                .Where(x => string.IsNullOrEmpty(x.Name))
                .Should().BeNullOrEmpty();
            tagsResult_3?.First().Name.Should().Be("java");
            tagsResult_3?.Last().Name.Should().Be("android");

        }
    }
}
