using BlogTestTask.Models;

namespace BlogApp.Repository.Interfaces
{
    public interface IPostRepository
    {
        Task<Post?> GetByIdAsync(long id);
        Task<List<Post>> GetAllAsync();
        Task<long> CreateAsync(Post post);
        Task UpdateAsync(Post post);
        Task<bool> DeleteAsync(long id);
    }
}
