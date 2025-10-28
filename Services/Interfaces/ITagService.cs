using BlogApp.DTOs;

namespace BlogApp.Services.Interfaces
{
    public interface ITagService
    {
        Task<ApiResponse<TagDto>> GetTagByIdAsync(long id);
        Task<ApiResponse<TagDto>> GetTagBySlugAsync(string slug);
        Task<ApiResponse<List<TagDto>>> GetAllTagsAsync();
        Task<ApiResponse<TagDto>> CreateTagAsync(CreateTagDto request);
        Task<ApiResponse<TagDto>> UpdateTagAsync(long id, CreateTagDto request);
        Task<ApiResponse<string>> DeleteTagAsync(long id);
    }
}
