﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StackOverflowTags.DbContexts;
using StackOverflowTags.Services;
using System.ComponentModel.DataAnnotations;

namespace StackOverflowTags.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StackOverflowController : ControllerBase
    {
        private readonly IStackOverflowService _stackOverflowService;
        private readonly IHttpService _httpService;
        public StackOverflowController(IStackOverflowService stackOverflowService, IHttpService httpService)
        {
            _stackOverflowService = stackOverflowService;
            _httpService = httpService;
        }

        /// <summary>
        /// Get any tags
        /// </summary>
        [HttpGet("tags")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GetTagsAsync()
        {
            return Ok(await _stackOverflowService.GetStackOverflowTagsAsync());
        }
    }
}