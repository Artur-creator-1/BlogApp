using BlogApp.DTOs;
using BlogApp.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostsController : ControllerBase
    {
        private readonly IPostService _postService;
        private readonly ILogger<PostsController> _logger;

        public PostsController(IPostService postService, ILogger<PostsController> logger)
        {
            _postService = postService;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPostById(long id)
        {
            _logger.LogInformation($"[PostsController] GET /api/posts/{id}");

            var result = await _postService.GetPostByIdAsync(id);

            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPosts()
        {
            _logger.LogInformation("[PostsController] GET /api/posts");

            var result = await _postService.GetAllPostsAsync();

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePost([FromQuery] long userId, [FromBody] CreatePostDto request)
        {
            _logger.LogInformation($"[PostsController] POST /api/posts - User ID: {userId}");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("[PostsController] Invalid model state for post creation");
                return BadRequest(ApiResponse<PostDto>.Error("Invalid request",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
            }

            var result = await _postService.CreatePostAsync(userId, request);

            if (!result.Success)
            {
                _logger.LogWarning($"[PostsController] Post creation failed - {result.Message}");
                return BadRequest(result);
            }

            _logger.LogInformation($"[PostsController] Post created successfully - ID: {result.Data.Id}");
            return CreatedAtAction(nameof(GetPostById), new { id = result.Data.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePost(long id, [FromBody] UpdatePostDto request)
        {
            _logger.LogInformation($"[PostsController] PUT /api/posts/{id}");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning($"[PostsController] Invalid model state for post update - ID: {id}");
                return BadRequest(ApiResponse<PostDto>.Error("Invalid request",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
            }

            var result = await _postService.UpdatePostAsync(id, request);

            if (!result.Success)
            {
                _logger.LogWarning($"[PostsController] Post update failed - ID: {id}");
                return NotFound(result);
            }

            _logger.LogInformation($"[PostsController] Post updated successfully - ID: {id}");
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(long id)
        {
            _logger.LogInformation($"[PostsController] DELETE /api/posts/{id}");

            var result = await _postService.DeletePostAsync(id);

            if (!result.Success)
            {
                _logger.LogWarning($"[PostsController] Post deletion failed - ID: {id}");
                return NotFound(result);
            }

            _logger.LogInformation($"[PostsController] Post deleted successfully - ID: {id}");
            return Ok(result);
        }
    }
}
