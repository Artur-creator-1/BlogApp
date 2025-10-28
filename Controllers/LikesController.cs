using BlogApp.DTOs;
using BlogApp.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LikesController : ControllerBase
    {
        private readonly ILikesService _likesService;
        private readonly ILogger<LikesController> _logger;

        public LikesController(ILikesService likesService, ILogger<LikesController> logger)
        {
            _likesService = likesService;
            _logger = logger;
        }

        [HttpPost("posts/{postId}/like")]
        public async Task<IActionResult> LikePost(long postId, [FromQuery] long userId)
        {
            _logger.LogInformation($"[LikesController] POST /api/likes/posts/{postId}/like");
            var result = await _likesService.LikePostAsync(postId, userId);
            return Ok(result);
        }

        [HttpDelete("posts/{postId}/unlike")]
        public async Task<IActionResult> UnlikePost(long postId, [FromQuery] long userId)
        {
            _logger.LogInformation($"[LikesController] DELETE /api/likes/posts/{postId}/unlike");
            var result = await _likesService.UnlikePostAsync(postId, userId);
            return Ok(result);
        }

        [HttpGet("posts/{postId}/is-liked")]
        public async Task<IActionResult> IsPostLiked(long postId, [FromQuery] long userId)
        {
            _logger.LogInformation($"[LikesController] GET /api/likes/posts/{postId}/is-liked");
            var result = await _likesService.IsPostLikedAsync(postId, userId);
            return Ok(result);
        }

        [HttpGet("posts/{postId}/count")]
        public async Task<IActionResult> GetPostLikesCount(long postId)
        {
            _logger.LogInformation($"[LikesController] GET /api/likes/posts/{postId}/count");
            var result = await _likesService.GetPostLikesCountAsync(postId);
            return Ok(result);
        }

        [HttpPost("comments/{commentId}/like")]
        public async Task<IActionResult> LikeComment(long commentId, [FromQuery] long userId)
        {
            _logger.LogInformation($"[LikesController] POST /api/likes/comments/{commentId}/like");
            var result = await _likesService.LikeCommentAsync(commentId, userId);
            return Ok(result);
        }

        [HttpDelete("comments/{commentId}/unlike")]
        public async Task<IActionResult> UnlikeComment(long commentId, [FromQuery] long userId)
        {
            _logger.LogInformation($"[LikesController] DELETE /api/likes/comments/{commentId}/unlike");
            var result = await _likesService.UnlikeCommentAsync(commentId, userId);
            return Ok(result);
        }

        [HttpGet("comments/{commentId}/is-liked")]
        public async Task<IActionResult> IsCommentLiked(long commentId, [FromQuery] long userId)
        {
            _logger.LogInformation($"[LikesController] GET /api/likes/comments/{commentId}/is-liked");
            var result = await _likesService.IsCommentLikedAsync(commentId, userId);
            return Ok(result);
        }

        [HttpGet("comments/{commentId}/count")]
        public async Task<IActionResult> GetCommentLikesCount(long commentId)
        {
            _logger.LogInformation($"[LikesController] GET /api/likes/comments/{commentId}/count");
            var result = await _likesService.GetCommentLikesCountAsync(commentId);
            return Ok(result);
        }
    }
}
