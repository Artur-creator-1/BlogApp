using BlogApp.DTOs;
using BlogApp.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TagsController : ControllerBase
    {
        private readonly ITagService _tagService;
        private readonly ILogger<TagsController> _logger;

        public TagsController(ITagService tagService, ILogger<TagsController> logger)
        {
            _tagService = tagService;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTagById(long id)
        {
            _logger.LogInformation($"[TagsController] GET /api/tags/{id}");
            var result = await _tagService.GetTagByIdAsync(id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        [HttpGet("slug/{slug}")]
        public async Task<IActionResult> GetTagBySlug(string slug)
        {
            _logger.LogInformation($"[TagsController] GET /api/tags/slug/{slug}");
            var result = await _tagService.GetTagBySlugAsync(slug);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTags()
        {
            _logger.LogInformation("[TagsController] GET /api/tags");
            var result = await _tagService.GetAllTagsAsync();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTag([FromBody] CreateTagDto request)
        {
            _logger.LogInformation($"[TagsController] POST /api/tags");
            var result = await _tagService.CreateTagAsync(request);
            if (!result.Success) return BadRequest(result);
            return CreatedAtAction(nameof(GetTagById), new { id = result.Data.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTag(long id, [FromBody] CreateTagDto request)
        {
            _logger.LogInformation($"[TagsController] PUT /api/tags/{id}");
            var result = await _tagService.UpdateTagAsync(id, request);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTag(long id)
        {
            _logger.LogInformation($"[TagsController] DELETE /api/tags/{id}");
            var result = await _tagService.DeleteTagAsync(id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }
    }
}
