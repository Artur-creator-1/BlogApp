namespace BlogApp.Repository.Interfaces
{
    public interface IPostLikeRepository
    {
        Task<bool> LikeAsync(long postId, long userId);
        Task<bool> UnlikeAsync(long postId, long userId);
        Task<bool> IsLikedAsync(long postId, long userId);
        Task<int> GetCountAsync(long postId);
    }
}
