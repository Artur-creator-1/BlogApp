using BlogApp.DTOs;
using BlogApp.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentService _commentService;
        private readonly ILogger<CommentsController> _logger;

        public CommentsController(ICommentService commentService, ILogger<CommentsController> logger)
        {
            _commentService = commentService;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCommentById(long id)
        {
            _logger.LogInformation($"[CommentsController] GET /api/comments/{id}");

            var result = await _commentService.GetCommentByIdAsync(id);

            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }

        [HttpGet("post/{postId}")]
        public async Task<IActionResult> GetCommentsByPostId(long postId)
        {
            _logger.LogInformation($"[CommentsController] GET /api/comments/post/{postId}");

            var result = await _commentService.GetCommentsByPostIdAsync(postId);

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateComment([FromQuery] long postId, [FromQuery] long userId, [FromBody] CreateCommentDto request)
        {
            _logger.LogInformation($"[CommentsController] POST /api/comments - Post ID: {postId}, User ID: {userId}");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("[CommentsController] Invalid model state for comment creation");
                return BadRequest(ApiResponse<CommentDto>.Error("Invalid request",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
            }

            var result = await _commentService.CreateCommentAsync(postId, userId, request);

            if (!result.Success)
            {
                _logger.LogWarning($"[CommentsController] Comment creation failed - {result.Message}");
                return BadRequest(result);
            }

            _logger.LogInformation($"[CommentsController] Comment created successfully - ID: {result.Data.Id}");
            return CreatedAtAction(nameof(GetCommentById), new { id = result.Data.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateComment(long id, [FromBody] UpdateCommentDto request)
        {
            _logger.LogInformation($"[CommentsController] PUT /api/comments/{id}");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning($"[CommentsController] Invalid model state for comment update - ID: {id}");
                return BadRequest(ApiResponse<CommentDto>.Error("Invalid request",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
            }

            var result = await _commentService.UpdateCommentAsync(id, request);

            if (!result.Success)
            {
                _logger.LogWarning($"[CommentsController] Comment update failed - ID: {id}");
                return NotFound(result);
            }

            _logger.LogInformation($"[CommentsController] Comment updated successfully - ID: {id}");
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(long id)
        {
            _logger.LogInformation($"[CommentsController] DELETE /api/comments/{id}");

            var result = await _commentService.DeleteCommentAsync(id);

            if (!result.Success)
            {
                _logger.LogWarning($"[CommentsController] Comment deletion failed - ID: {id}");
                return NotFound(result);
            }

            _logger.LogInformation($"[CommentsController] Comment deleted successfully - ID: {id}");
            return Ok(result);
        }
    }
}
