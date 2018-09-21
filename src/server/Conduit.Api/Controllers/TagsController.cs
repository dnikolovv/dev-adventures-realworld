using Conduit.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Conduit.Api.Controllers
{
    public class TagsController : ApiController
    {
        private readonly ITagsService _tagsService;

        public TagsController(ITagsService tagsService)
        {
            _tagsService = tagsService;
        }

        /// <summary>
        /// Gets all tags.
        /// </summary>
        /// <returns>A list of tags.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(string[]), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get() =>
            Ok(new { tags = await _tagsService.GetAllAsync() });
    }
}
