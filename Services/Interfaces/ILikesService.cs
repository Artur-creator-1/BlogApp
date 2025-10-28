using BlogApp.DTOs;

namespace BlogApp.Services.Interfaces
{
    public interface ILikesService
    {
        Task<ApiResponse<bool>> LikePostAsync(long postId, long userId);
        Task<ApiResponse<bool>> UnlikePostAsync(long postId, long userId);
        Task<ApiResponse<bool>> IsPostLikedAsync(long postId, long userId);
        Task<ApiResponse<int>> GetPostLikesCountAsync(long postId);

        Task<ApiResponse<bool>> LikeCommentAsync(long commentId, long userId);
        Task<ApiResponse<bool>> UnlikeCommentAsync(long commentId, long userId);
        Task<ApiResponse<bool>> IsCommentLikedAsync(long commentId, long userId);
        Task<ApiResponse<int>> GetCommentLikesCountAsync(long commentId);
    }
}
