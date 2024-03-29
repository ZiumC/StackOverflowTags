﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json.Linq;
using StackOverflowTags.DbContexts;
using StackOverflowTags.Models.DatabaseModels;
using StackOverflowTags.Services.HttpService;
using StackOverflowTags.Services.StackOverflowService;
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
        /// Get StackOverflow tags
        /// </summary>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="order">Order by</param>
        /// <returns>SO tags</returns>
        [HttpGet("tags")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<TagModel>))]
        public async Task<IActionResult> GetTagsAsync(int page = 1, int pageSize = 20, string order = "asc")
        {
            if (page <= 0)
            {
                ModelState.AddModelError("error", "Page number is invalid");
                return BadRequest(ModelState);
            }

            if (pageSize <= 0)
            {
                ModelState.AddModelError("error", "Page size is invalid");
                return BadRequest(ModelState);
            }


            var tags = await _stackOverflowService.GetStackOverflowTagsAsync();
            if (tags == null || tags.Count() == 0)
            {
                return NotFound();
            }

            var tagsPerPage = tags
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            switch (order.ToLower())
            {
                case "asc":
                    tagsPerPage = tagsPerPage
                        .OrderBy(tpp => tpp.Name)
                        .ThenBy(tpp => tpp.Share)
                        .ToList();
                    break;
                case "desc":
                    tagsPerPage = tagsPerPage
                        .OrderByDescending(tpp => tpp.Name)
                        .ThenByDescending(tpp => tpp.Share)
                        .ToList();
                    break;
                default:
                    ModelState.AddModelError("error", "Order type is unknown");
                    return BadRequest(ModelState);
            }

            return Ok(tagsPerPage);
        }

        /// <summary>
        /// Refills database
        /// </summary>
        /// <returns></returns>
        [HttpGet("refill")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> RefillDatabaseAsync()
        {
            bool hasDbRefilled = await _stackOverflowService.RefillDatabase();
            return Ok(new { HasDbRefilled = hasDbRefilled });
        }
    }
}
