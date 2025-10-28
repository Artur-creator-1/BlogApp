using BlogApp.DTOs;

namespace BlogApp.Services.Interfaces
{
    public interface ICommentService
    {
        Task<ApiResponse<CommentDto>> GetCommentByIdAsync(long id);
        Task<ApiResponse<List<CommentDto>>> GetCommentsByPostIdAsync(long postId);
        Task<ApiResponse<CommentDto>> CreateCommentAsync(long postId, long userId, CreateCommentDto request);
        Task<ApiResponse<CommentDto>> UpdateCommentAsync(long commentId, UpdateCommentDto request);
        Task<ApiResponse<string>> DeleteCommentAsync(long commentId);
    }
}
