using BlogApp.DTOs;

namespace BlogApp.Services.Interfaces
{
    public interface IPostService
    {
        Task<ApiResponse<PostDto>> GetPostByIdAsync(long id);
        Task<ApiResponse<List<PostDto>>> GetAllPostsAsync();
        Task<ApiResponse<PostDto>> CreatePostAsync(long userId, CreatePostDto request);
        Task<ApiResponse<PostDto>> UpdatePostAsync(long postId, UpdatePostDto request);
        Task<ApiResponse<string>> DeletePostAsync(long postId);
    }
}
