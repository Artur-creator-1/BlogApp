using BlogApp.DTOs;
using BlogApp.Repository.Interfaces;
using BlogApp.Services.Interfaces;

namespace BlogApp.Services.Implementations
{
    public class LikesService : ILikesService
    {
        private readonly IPostLikeRepository _postLikeRepository;
        private readonly ICommentLikeRepository _commentLikeRepository;
        private readonly IPostRepository _postRepository;
        private readonly ICommentRepository _commentRepository;
        private readonly ILogger<LikesService> _logger;

        public LikesService(
            IPostLikeRepository postLikeRepository,
            ICommentLikeRepository commentLikeRepository,
            IPostRepository postRepository,
            ICommentRepository commentRepository,
            ILogger<LikesService> logger)
        {
            _postLikeRepository = postLikeRepository;
            _commentLikeRepository = commentLikeRepository;
            _postRepository = postRepository;
            _commentRepository = commentRepository;
            _logger = logger;
        }

        public async Task<ApiResponse<bool>> LikePostAsync(long postId, long userId)
        {
            try
            {
                _logger.LogInformation($"[LikesService] Liking post. Post ID: {postId}, User ID: {userId}");

                if (postId <= 0 || userId <= 0)
                    return ApiResponse<bool>.Error("Invalid post ID or user ID");

                var post = await _postRepository.GetByIdAsync(postId);
                if (post == null)
                    return ApiResponse<bool>.Error("Post not found");

                var liked = await _postLikeRepository.LikeAsync(postId, userId);
                if (liked) _logger.LogInformation($"[LikesService] Post liked. Post ID: {postId}");

                return ApiResponse<bool>.Ok(liked, "Post liked");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[LikesService] Error liking post");
                return ApiResponse<bool>.Error("An error occurred");
            }
        }

        public async Task<ApiResponse<bool>> UnlikePostAsync(long postId, long userId)
        {
            try
            {
                _logger.LogInformation($"[LikesService] Unliking post. Post ID: {postId}, User ID: {userId}");

                if (postId <= 0 || userId <= 0)
                    return ApiResponse<bool>.Error("Invalid post ID or user ID");

                var unliked = await _postLikeRepository.UnlikeAsync(postId, userId);
                if (unliked) _logger.LogInformation($"[LikesService] Post unliked. Post ID: {postId}");

                return ApiResponse<bool>.Ok(unliked, "Post unliked");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[LikesService] Error unliking post");
                return ApiResponse<bool>.Error("An error occurred");
            }
        }

        public async Task<ApiResponse<bool>> IsPostLikedAsync(long postId, long userId)
        {
            try
            {
                var isLiked = await _postLikeRepository.IsLikedAsync(postId, userId);
                return ApiResponse<bool>.Ok(isLiked);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[LikesService] Error checking if post is liked");
                return ApiResponse<bool>.Error("An error occurred");
            }
        }

        public async Task<ApiResponse<int>> GetPostLikesCountAsync(long postId)
        {
            try
            {
                var count = await _postLikeRepository.GetCountAsync(postId);
                return ApiResponse<int>.Ok(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[LikesService] Error getting post likes count");
                return ApiResponse<int>.Error("An error occurred");
            }
        }

        public async Task<ApiResponse<bool>> LikeCommentAsync(long commentId, long userId)
        {
            try
            {
                _logger.LogInformation($"[LikesService] Liking comment. Comment ID: {commentId}, User ID: {userId}");

                if (commentId <= 0 || userId <= 0)
                    return ApiResponse<bool>.Error("Invalid comment ID or user ID");

                var comment = await _commentRepository.GetByIdAsync(commentId);
                if (comment == null)
                    return ApiResponse<bool>.Error("Comment not found");

                var liked = await _commentLikeRepository.LikeAsync(commentId, userId);
                if (liked) _logger.LogInformation($"[LikesService] Comment liked. Comment ID: {commentId}");

                return ApiResponse<bool>.Ok(liked, "Comment liked");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[LikesService] Error liking comment");
                return ApiResponse<bool>.Error("An error occurred");
            }
        }

        public async Task<ApiResponse<bool>> UnlikeCommentAsync(long commentId, long userId)
        {
            try
            {
                _logger.LogInformation($"[LikesService] Unliking comment. Comment ID: {commentId}, User ID: {userId}");

                if (commentId <= 0 || userId <= 0)
                    return ApiResponse<bool>.Error("Invalid comment ID or user ID");

                var unliked = await _commentLikeRepository.UnlikeAsync(commentId, userId);
                if (unliked) _logger.LogInformation($"[LikesService] Comment unliked. Comment ID: {commentId}");

                return ApiResponse<bool>.Ok(unliked, "Comment unliked");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[LikesService] Error unliking comment");
                return ApiResponse<bool>.Error("An error occurred");
            }
        }

        public async Task<ApiResponse<bool>> IsCommentLikedAsync(long commentId, long userId)
        {
            try
            {
                var isLiked = await _commentLikeRepository.IsLikedAsync(commentId, userId);
                return ApiResponse<bool>.Ok(isLiked);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[LikesService] Error checking if comment is liked");
                return ApiResponse<bool>.Error("An error occurred");
            }
        }

        public async Task<ApiResponse<int>> GetCommentLikesCountAsync(long commentId)
        {
            try
            {
                var count = await _commentLikeRepository.GetCountAsync(commentId);
                return ApiResponse<int>.Ok(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[LikesService] Error getting comment likes count");
                return ApiResponse<int>.Error("An error occurred");
            }
        }
    }
}
