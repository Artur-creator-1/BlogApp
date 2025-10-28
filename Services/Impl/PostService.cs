using BlogApp.DTOs;
using BlogApp.Repository.Interfaces;
using BlogApp.Services.Interfaces;
using BlogTestTask.Models;

namespace BlogApp.Services.Implementations
{
    public class PostService : IPostService
    {
        private readonly IPostRepository _postRepository;
        private readonly ILogger<PostService> _logger;
        private const int MinTitleLength = 3;
        private const int MinContentLength = 10;

        public PostService(IPostRepository postRepository, ILogger<PostService> logger)
        {
            _postRepository = postRepository;
            _logger = logger;
        }

        public async Task<ApiResponse<PostDto>> GetPostByIdAsync(long id)
        {
            try
            {
                _logger.LogInformation($"[PostService] Getting post by ID: {id}");

                if (id <= 0)
                {
                    _logger.LogWarning($"[PostService] Invalid post ID: {id}");
                    return ApiResponse<PostDto>.Error("Invalid post ID");
                }

                var post = await _postRepository.GetByIdAsync(id);

                if (post == null)
                {
                    _logger.LogWarning($"[PostService] Post not found. ID: {id}");
                    return ApiResponse<PostDto>.Error("Post not found");
                }

                _logger.LogInformation($"[PostService] Post retrieved successfully. ID: {id}");
                return ApiResponse<PostDto>.Ok(MapToDto(post));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[PostService] Error getting post by ID: {id}");
                return ApiResponse<PostDto>.Error("An error occurred");
            }
        }

        public async Task<ApiResponse<List<PostDto>>> GetAllPostsAsync()
        {
            try
            {
                _logger.LogInformation("[PostService] Fetching all posts");

                var posts = await _postRepository.GetAllAsync();

                if (!posts.Any())
                {
                    _logger.LogInformation("[PostService] No posts found");
                    return ApiResponse<List<PostDto>>.Ok(new List<PostDto>(), "No posts found");
                }

                var dtos = posts.Select(MapToDto).ToList();

                _logger.LogInformation($"[PostService] Retrieved {dtos.Count} posts successfully");
                return ApiResponse<List<PostDto>>.Ok(dtos, $"Retrieved {dtos.Count} posts");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[PostService] Error fetching all posts");
                return ApiResponse<List<PostDto>>.Error("An error occurred");
            }
        }

        public async Task<ApiResponse<PostDto>> CreatePostAsync(long userId, CreatePostDto request)
        {
            try
            {
                _logger.LogInformation($"[PostService] Creating post for user ID: {userId}");

                if (userId <= 0)
                {
                    _logger.LogWarning($"[PostService] Invalid user ID: {userId}");
                    return ApiResponse<PostDto>.Error("Invalid user ID");
                }

                if (request == null)
                {
                    _logger.LogWarning("[PostService] Create post request is null");
                    return ApiResponse<PostDto>.Error("Invalid request");
                }

                var validationErrors = ValidatePost(request);
                if (validationErrors.Any())
                {
                    _logger.LogWarning($"[PostService] Post validation failed. Errors: {string.Join(", ", validationErrors)}");
                    return ApiResponse<PostDto>.Error("Validation failed", validationErrors);
                }

                var post = new Post
                {
                    UserId = userId,
                    Title = request.Title.Trim(),
                    Content = request.Content.Trim(),
                    Summary = request.Summary?.Trim(),
                    IsPublished = request.IsPublished,
                    PublishedAt = request.IsPublished ? DateTime.UtcNow : null
                };

                var postId = await _postRepository.CreateAsync(post);
                post.Id = postId;

                _logger.LogInformation($"[PostService] Post created successfully. ID: {postId}, User ID: {userId}");

                return ApiResponse<PostDto>.Ok(MapToDto(post), "Post created successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[PostService] Error creating post for user: {userId}");
                return ApiResponse<PostDto>.Error("An error occurred");
            }
        }

        public async Task<ApiResponse<PostDto>> UpdatePostAsync(long postId, UpdatePostDto request)
        {
            try
            {
                _logger.LogInformation($"[PostService] Updating post. ID: {postId}");

                if (postId <= 0)
                {
                    _logger.LogWarning($"[PostService] Invalid post ID for update: {postId}");
                    return ApiResponse<PostDto>.Error("Invalid post ID");
                }

                if (request == null)
                {
                    _logger.LogWarning("[PostService] Update post request is null");
                    return ApiResponse<PostDto>.Error("Invalid request");
                }

                var post = await _postRepository.GetByIdAsync(postId);
                if (post == null)
                {
                    _logger.LogWarning($"[PostService] Post not found for update. ID: {postId}");
                    return ApiResponse<PostDto>.Error("Post not found");
                }

                var originalTitle = post.Title;

                if (!string.IsNullOrWhiteSpace(request.Title))
                    post.Title = request.Title.Trim();

                if (!string.IsNullOrWhiteSpace(request.Content))
                    post.Content = request.Content.Trim();

                if (!string.IsNullOrWhiteSpace(request.Summary))
                    post.Summary = request.Summary.Trim();

                post.IsPublished = request.IsPublished;
                if (request.IsPublished && post.PublishedAt == null)
                    post.PublishedAt = DateTime.UtcNow;

                await _postRepository.UpdateAsync(post);

                _logger.LogInformation($"[PostService] Post updated successfully. ID: {postId}. Changed title from '{originalTitle}' to '{post.Title}'");

                return ApiResponse<PostDto>.Ok(MapToDto(post), "Post updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[PostService] Error updating post. ID: {postId}");
                return ApiResponse<PostDto>.Error("An error occurred");
            }
        }

        public async Task<ApiResponse<string>> DeletePostAsync(long postId)
        {
            try
            {
                _logger.LogInformation($"[PostService] Deleting post. ID: {postId}");

                if (postId <= 0)
                {
                    _logger.LogWarning($"[PostService] Invalid post ID for deletion: {postId}");
                    return ApiResponse<string>.Error("Invalid post ID");
                }

                var post = await _postRepository.GetByIdAsync(postId);
                if (post == null)
                {
                    _logger.LogWarning($"[PostService] Post not found for deletion. ID: {postId}");
                    return ApiResponse<string>.Error("Post not found");
                }

                await _postRepository.DeleteAsync(postId);

                _logger.LogInformation($"[PostService] Post deleted successfully. ID: {postId}");

                return ApiResponse<string>.Ok("Post deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[PostService] Error deleting post. ID: {postId}");
                return ApiResponse<string>.Error("An error occurred");
            }
        }

        private List<string> ValidatePost(CreatePostDto request)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(request.Title))
                errors.Add("Title is required");
            else if (request.Title.Length < MinTitleLength)
                errors.Add($"Title must be at least {MinTitleLength} characters");
            else if (request.Title.Length > 200)
                errors.Add("Title must not exceed 200 characters");

            if (string.IsNullOrWhiteSpace(request.Content))
                errors.Add("Content is required");
            else if (request.Content.Length < MinContentLength)
                errors.Add($"Content must be at least {MinContentLength} characters");

            if (!string.IsNullOrWhiteSpace(request.Summary) && request.Summary.Length > 500)
                errors.Add("Summary must not exceed 500 characters");

            return errors;
        }

        private PostDto MapToDto(Post post)
        {
            return new PostDto
            {
                Id = post.Id,
                UserId = post.UserId,
                Title = post.Title,
                Content = post.Content,
                Summary = post.Summary,
                LikesCount = post.LikesCount,
                CommentsCount = post.CommentsCount,
                IsPublished = post.IsPublished,
                CreatedAt = post.CreatedAt
            };
        }
    }
}
