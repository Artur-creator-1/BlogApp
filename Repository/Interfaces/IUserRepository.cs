using BlogTestTask.Models;

namespace BlogApp.Repository.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(long id);
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetByEmailAsync(string email);
        Task<long> CreateAsync(User user);
        Task UpdateAsync(User user);
        Task<bool> DeleteAsync(long id);
        Task<List<User>> GetAllAsync();
    }
}
