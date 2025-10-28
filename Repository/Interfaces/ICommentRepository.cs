using BlogTestTask.Models;

namespace BlogApp.Repository.Interfaces
{
    public interface ICommentRepository
    {
        Task<Comment?> GetByIdAsync(long id);
        Task<List<Comment>> GetByPostIdAsync(long postId);
        Task<long> CreateAsync(Comment comment);
        Task UpdateAsync(Comment comment);
        Task<bool> DeleteAsync(long id);
        Task<List<Comment>> GetByUserIdAsync(long userId);
    }
}
