namespace BlogApp.Repository.Interfaces
{
    public interface ICommentLikeRepository
    {
        Task<bool> LikeAsync(long commentId, long userId);
        Task<bool> UnlikeAsync(long commentId, long userId);
        Task<bool> IsLikedAsync(long commentId, long userId);
        Task<int> GetCountAsync(long commentId);
    }
}
