using Microsoft.AspNetCore.Http;
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
        /// <returns>SO tags</returns>
        [HttpGet("tags")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<TagModel>))]
        public async Task<IActionResult> GetTagsAsync(int page = 1, int pageSize = 20)
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

            return Ok(tagsPerPage);
        }
    }
}
