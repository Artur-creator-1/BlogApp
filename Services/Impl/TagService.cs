using BlogApp.DTOs;
using BlogApp.Repository.Interfaces;
using BlogApp.Services.Interfaces;
using BlogTestTask.Models;

namespace BlogApp.Services.Implementations
{
    public class TagService : ITagService
    {
        private readonly ITagRepository _tagRepository;
        private readonly ILogger<TagService> _logger;

        public TagService(ITagRepository tagRepository, ILogger<TagService> logger)
        {
            _tagRepository = tagRepository;
            _logger = logger;
        }

        public async Task<ApiResponse<TagDto>> GetTagByIdAsync(long id)
        {
            try
            {
                _logger.LogInformation($"[TagService] Getting tag by ID: {id}");

                if (id <= 0)
                    return ApiResponse<TagDto>.Error("Invalid tag ID");

                var tag = await _tagRepository.GetByIdAsync(id);
                if (tag == null)
                {
                    _logger.LogWarning($"[TagService] Tag not found. ID: {id}");
                    return ApiResponse<TagDto>.Error("Tag not found");
                }

                _logger.LogInformation($"[TagService] Tag retrieved. ID: {id}");
                return ApiResponse<TagDto>.Ok(MapToDto(tag));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[TagService] Error getting tag by ID: {id}");
                return ApiResponse<TagDto>.Error("An error occurred");
            }
        }

        public async Task<ApiResponse<TagDto>> GetTagBySlugAsync(string slug)
        {
            try
            {
                _logger.LogInformation($"[TagService] Getting tag by slug: {slug}");

                if (string.IsNullOrWhiteSpace(slug))
                    return ApiResponse<TagDto>.Error("Slug cannot be empty");

                var tag = await _tagRepository.GetBySlugAsync(slug);
                if (tag == null)
                {
                    _logger.LogWarning($"[TagService] Tag not found. Slug: {slug}");
                    return ApiResponse<TagDto>.Error("Tag not found");
                }

                return ApiResponse<TagDto>.Ok(MapToDto(tag));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[TagService] Error getting tag by slug: {slug}");
                return ApiResponse<TagDto>.Error("An error occurred");
            }
        }

        public async Task<ApiResponse<List<TagDto>>> GetAllTagsAsync()
        {
            try
            {
                _logger.LogInformation("[TagService] Fetching all tags");

                var tags = await _tagRepository.GetAllAsync();
                var dtos = tags.Select(MapToDto).ToList();

                _logger.LogInformation($"[TagService] Retrieved {dtos.Count} tags");
                return ApiResponse<List<TagDto>>.Ok(dtos, $"Retrieved {dtos.Count} tags");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[TagService] Error fetching all tags");
                return ApiResponse<List<TagDto>>.Error("An error occurred");
            }
        }

        public async Task<ApiResponse<TagDto>> CreateTagAsync(CreateTagDto request)
        {
            try
            {
                _logger.LogInformation($"[TagService] Creating tag: {request?.Name}");

                if (request == null || string.IsNullOrWhiteSpace(request.Name))
                    return ApiResponse<TagDto>.Error("Tag name is required");

                var existing = await _tagRepository.GetBySlugAsync(request.Slug);
                if (existing != null)
                {
                    _logger.LogWarning($"[TagService] Tag already exists. Slug: {request.Slug}");
                    return ApiResponse<TagDto>.Error("Tag with this slug already exists");
                }

                var tag = new Tag
                {
                    Name = request.Name.Trim(),
                    Slug = request.Slug.Trim().ToLower(),
                    Description = request.Description?.Trim(),
                    IsActive = true
                };

                var tagId = await _tagRepository.CreateAsync(tag);
                tag.Id = tagId;

                _logger.LogInformation($"[TagService] Tag created. ID: {tagId}");
                return ApiResponse<TagDto>.Ok(MapToDto(tag), "Tag created successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[TagService] Error creating tag");
                return ApiResponse<TagDto>.Error("An error occurred");
            }
        }

        public async Task<ApiResponse<TagDto>> UpdateTagAsync(long id, CreateTagDto request)
        {
            try
            {
                _logger.LogInformation($"[TagService] Updating tag. ID: {id}");

                if (id <= 0)
                    return ApiResponse<TagDto>.Error("Invalid tag ID");

                var tag = await _tagRepository.GetByIdAsync(id);
                if (tag == null)
                {
                    _logger.LogWarning($"[TagService] Tag not found for update. ID: {id}");
                    return ApiResponse<TagDto>.Error("Tag not found");
                }

                tag.Name = request.Name?.Trim() ?? tag.Name;
                tag.Slug = request.Slug?.Trim().ToLower() ?? tag.Slug;
                tag.Description = request.Description?.Trim() ?? tag.Description;

                await _tagRepository.UpdateAsync(tag);

                _logger.LogInformation($"[TagService] Tag updated. ID: {id}");
                return ApiResponse<TagDto>.Ok(MapToDto(tag), "Tag updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[TagService] Error updating tag. ID: {id}");
                return ApiResponse<TagDto>.Error("An error occurred");
            }
        }

        public async Task<ApiResponse<string>> DeleteTagAsync(long id)
        {
            try
            {
                _logger.LogInformation($"[TagService] Deleting tag. ID: {id}");

                if (id <= 0)
                    return ApiResponse<string>.Error("Invalid tag ID");

                var tag = await _tagRepository.GetByIdAsync(id);
                if (tag == null)
                {
                    _logger.LogWarning($"[TagService] Tag not found for deletion. ID: {id}");
                    return ApiResponse<string>.Error("Tag not found");
                }

                await _tagRepository.DeleteAsync(id);

                _logger.LogInformation($"[TagService] Tag deleted. ID: {id}");
                return ApiResponse<string>.Ok("Tag deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[TagService] Error deleting tag. ID: {id}");
                return ApiResponse<string>.Error("An error occurred");
            }
        }

        private TagDto MapToDto(Tag tag)
        {
            return new TagDto
            {
                Id = tag.Id,
                Name = tag.Name,
                Slug = tag.Slug,
                Description = tag.Description,
                PostsCount = tag.PostsCount
            };
        }
    }
}
