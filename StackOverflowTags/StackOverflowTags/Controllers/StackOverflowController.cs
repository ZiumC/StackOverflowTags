using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet("tags")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<TagModel>))]
        public async Task<IActionResult> GetTagsAsync()
        {

            return Ok(await _stackOverflowService.GetStackOverflowTagsAsync());
        }
    }
}
