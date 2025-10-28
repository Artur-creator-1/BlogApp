using BlogTestTask.Models;

namespace BlogApp.Repository.Interfaces
{
    public interface ITagRepository
    {
        Task<Tag?> GetByIdAsync(long id);
        Task<Tag?> GetBySlugAsync(string slug);
        Task<List<Tag>> GetAllAsync();
        Task<long> CreateAsync(Tag tag);
        Task UpdateAsync(Tag tag);
        Task<bool> DeleteAsync(long id);
        Task<List<Tag>> GetByPostIdAsync(long postId);
    }
}
