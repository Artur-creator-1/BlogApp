using BlogApp.DTOs;
using BlogApp.Repository.Interfaces;
using BlogApp.Services.Interfaces;
using BlogTestTask.Models;

namespace BlogApp.Services.Implementations
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IPostRepository _postRepository;
        private readonly ILogger<CommentService> _logger;
        private const int MinCommentLength = 1;
        private const int MaxCommentLength = 5000;

        public CommentService(ICommentRepository commentRepository, IPostRepository postRepository, ILogger<CommentService> logger)
        {
            _commentRepository = commentRepository;
            _postRepository = postRepository;
            _logger = logger;
        }

        public async Task<ApiResponse<CommentDto>> GetCommentByIdAsync(long id)
        {
            try
            {
                _logger.LogInformation($"[CommentService] Getting comment by ID: {id}");

                if (id <= 0)
                {
                    _logger.LogWarning($"[CommentService] Invalid comment ID: {id}");
                    return ApiResponse<CommentDto>.Error("Invalid comment ID");
                }

                var comment = await _commentRepository.GetByIdAsync(id);

                if (comment == null)
                {
                    _logger.LogWarning($"[CommentService] Comment not found. ID: {id}");
                    return ApiResponse<CommentDto>.Error("Comment not found");
                }

                _logger.LogInformation($"[CommentService] Comment retrieved successfully. ID: {id}");
                return ApiResponse<CommentDto>.Ok(MapToDto(comment));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[CommentService] Error getting comment by ID: {id}");
                return ApiResponse<CommentDto>.Error("An error occurred");
            }
        }

        public async Task<ApiResponse<List<CommentDto>>> GetCommentsByPostIdAsync(long postId)
        {
            try
            {
                _logger.LogInformation($"[CommentService] Getting comments for post ID: {postId}");

                if (postId <= 0)
                {
                    _logger.LogWarning($"[CommentService] Invalid post ID: {postId}");
                    return ApiResponse<List<CommentDto>>.Error("Invalid post ID");
                }

                var post = await _postRepository.GetByIdAsync(postId);
                if (post == null)
                {
                    _logger.LogWarning($"[CommentService] Post not found. ID: {postId}");
                    return ApiResponse<List<CommentDto>>.Error("Post not found");
                }

                var comments = await _commentRepository.GetByPostIdAsync(postId);

                if (!comments.Any())
                {
                    _logger.LogInformation($"[CommentService] No comments found for post ID: {postId}");
                    return ApiResponse<List<CommentDto>>.Ok(new List<CommentDto>(), "No comments found");
                }

                var dtos = comments.Select(MapToDto).ToList();

                _logger.LogInformation($"[CommentService] Retrieved {dtos.Count} comments for post ID: {postId}");
                return ApiResponse<List<CommentDto>>.Ok(dtos, $"Retrieved {dtos.Count} comments");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[CommentService] Error getting comments for post: {postId}");
                return ApiResponse<List<CommentDto>>.Error("An error occurred");
            }
        }

        public async Task<ApiResponse<CommentDto>> CreateCommentAsync(long postId, long userId, CreateCommentDto request)
        {
            try
            {
                _logger.LogInformation($"[CommentService] Creating comment for post ID: {postId}, user ID: {userId}");

                if (postId <= 0 || userId <= 0)
                {
                    _logger.LogWarning($"[CommentService] Invalid post ID: {postId} or user ID: {userId}");
                    return ApiResponse<CommentDto>.Error("Invalid post ID or user ID");
                }

                if (request == null)
                {
                    _logger.LogWarning("[CommentService] Create comment request is null");
                    return ApiResponse<CommentDto>.Error("Invalid request");
                }

                var validationErrors = ValidateComment(request);
                if (validationErrors.Any())
                {
                    _logger.LogWarning($"[CommentService] Comment validation failed. Errors: {string.Join(", ", validationErrors)}");
                    return ApiResponse<CommentDto>.Error("Validation failed", validationErrors);
                }

                var post = await _postRepository.GetByIdAsync(postId);
                if (post == null)
                {
                    _logger.LogWarning($"[CommentService] Post not found. ID: {postId}");
                    return ApiResponse<CommentDto>.Error("Post not found");
                }

                var comment = new Comment
                {
                    PostId = postId,
                    UserId = userId,
                    ParentCommentId = request.ParentCommentId,
                    Content = request.Content.Trim()
                };

                var commentId = await _commentRepository.CreateAsync(comment);
                comment.Id = commentId;

                _logger.LogInformation($"[CommentService] Comment created successfully. ID: {commentId}, Post ID: {postId}, User ID: {userId}");

                return ApiResponse<CommentDto>.Ok(MapToDto(comment), "Comment created successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[CommentService] Error creating comment for post: {postId}");
                return ApiResponse<CommentDto>.Error("An error occurred");
            }
        }

        public async Task<ApiResponse<CommentDto>> UpdateCommentAsync(long commentId, UpdateCommentDto request)
        {
            try
            {
                _logger.LogInformation($"[CommentService] Updating comment. ID: {commentId}");

                if (commentId <= 0)
                {
                    _logger.LogWarning($"[CommentService] Invalid comment ID for update: {commentId}");
                    return ApiResponse<CommentDto>.Error("Invalid comment ID");
                }

                if (request == null)
                {
                    _logger.LogWarning("[CommentService] Update comment request is null");
                    return ApiResponse<CommentDto>.Error("Invalid request");
                }

                var validationErrors = ValidateCommentUpdate(request);
                if (validationErrors.Any())
                {
                    _logger.LogWarning($"[CommentService] Comment update validation failed. Errors: {string.Join(", ", validationErrors)}");
                    return ApiResponse<CommentDto>.Error("Validation failed", validationErrors);
                }

                var comment = await _commentRepository.GetByIdAsync(commentId);
                if (comment == null)
                {
                    _logger.LogWarning($"[CommentService] Comment not found for update. ID: {commentId}");
                    return ApiResponse<CommentDto>.Error("Comment not found");
                }

                var originalContent = comment.Content;
                comment.Content = request.Content.Trim();

                await _commentRepository.UpdateAsync(comment);

                _logger.LogInformation($"[CommentService] Comment updated successfully. ID: {commentId}. Content changed.");

                return ApiResponse<CommentDto>.Ok(MapToDto(comment), "Comment updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[CommentService] Error updating comment. ID: {commentId}");
                return ApiResponse<CommentDto>.Error("An error occurred");
            }
        }

        public async Task<ApiResponse<string>> DeleteCommentAsync(long commentId)
        {
            try
            {
                _logger.LogInformation($"[CommentService] Deleting comment. ID: {commentId}");

                if (commentId <= 0)
                {
                    _logger.LogWarning($"[CommentService] Invalid comment ID for deletion: {commentId}");
                    return ApiResponse<string>.Error("Invalid comment ID");
                }

                var comment = await _commentRepository.GetByIdAsync(commentId);
                if (comment == null)
                {
                    _logger.LogWarning($"[CommentService] Comment not found for deletion. ID: {commentId}");
                    return ApiResponse<string>.Error("Comment not found");
                }

                await _commentRepository.DeleteAsync(commentId);

                _logger.LogInformation($"[CommentService] Comment deleted successfully. ID: {commentId}");

                return ApiResponse<string>.Ok("Comment deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[CommentService] Error deleting comment. ID: {commentId}");
                return ApiResponse<string>.Error("An error occurred");
            }
        }

        private List<string> ValidateComment(CreateCommentDto request)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(request.Content))
                errors.Add("Content is required");
            else if (request.Content.Length < MinCommentLength)
                errors.Add($"Content must be at least {MinCommentLength} character");
            else if (request.Content.Length > MaxCommentLength)
                errors.Add($"Content must not exceed {MaxCommentLength} characters");

            return errors;
        }

        private List<string> ValidateCommentUpdate(UpdateCommentDto request)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(request.Content))
                errors.Add("Content is required");
            else if (request.Content.Length < MinCommentLength)
                errors.Add($"Content must be at least {MinCommentLength} character");
            else if (request.Content.Length > MaxCommentLength)
                errors.Add($"Content must not exceed {MaxCommentLength} characters");

            return errors;
        }

        private CommentDto MapToDto(Comment comment)
        {
            return new CommentDto
            {
                Id = comment.Id,
                PostId = comment.PostId,
                UserId = comment.UserId,
                Content = comment.Content,
                LikesCount = comment.LikesCount,
                CreatedAt = comment.CreatedAt
            };
        }
    }
}
