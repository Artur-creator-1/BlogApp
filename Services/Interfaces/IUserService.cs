using BlogApp.DTOs;

namespace BlogApp.Services.Interfaces
{
    public interface IUserService
    {
        Task<ApiResponse<UserDto>> GetUserByIdAsync(long id);
        Task<ApiResponse<UserDto>> GetUserByUsernameAsync(string username);
        Task<ApiResponse<List<UserDto>>> GetAllUsersAsync();
        Task<ApiResponse<UserDto>> RegisterAsync(CreateUserDto request);
        Task<ApiResponse<UserDto>> UpdateUserAsync(long id, UpdateUserDto request);
        Task<ApiResponse<string>> DeleteUserAsync(long id);
        Task<ApiResponse<bool>> ChangePasswordAsync(long userId, string oldPassword, string newPassword);
    }
}
