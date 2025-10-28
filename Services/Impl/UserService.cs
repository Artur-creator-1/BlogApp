using BlogApp.DTOs;
using BlogApp.Repository.Interfaces;
using BlogApp.Services.Interfaces;
using BlogTestTask.Models;
using BlogTestTask.Models.Enums;

namespace BlogApp.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserService> _logger;
        private const int MinPasswordLength = 6;
        private const int MinUsernameLength = 3;

        public UserService(IUserRepository userRepository, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<ApiResponse<UserDto>> GetUserByIdAsync(long id)
        {
            try
            {
                _logger.LogInformation($"[UserService] Getting user by ID: {id}");

                if (id <= 0)
                {
                    _logger.LogWarning($"[UserService] Invalid user ID: {id}");
                    return ApiResponse<UserDto>.Error("Invalid user ID");
                }

                var user = await _userRepository.GetByIdAsync(id);

                if (user == null)
                {
                    _logger.LogWarning($"[UserService] User not found. ID: {id}");
                    return ApiResponse<UserDto>.Error("User not found");
                }

                _logger.LogInformation($"[UserService] User retrieved successfully. ID: {id}, Username: {user.Username}");
                return ApiResponse<UserDto>.Ok(MapToDto(user));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[UserService] Error occurred while getting user by ID: {id}");
                return ApiResponse<UserDto>.Error("An error occurred while retrieving the user");
            }
        }

        public async Task<ApiResponse<UserDto>> GetUserByUsernameAsync(string username)
        {
            try
            {
                _logger.LogInformation($"[UserService] Getting user by username: {username}");

                if (string.IsNullOrWhiteSpace(username))
                {
                    _logger.LogWarning("[UserService] Username is null or empty");
                    return ApiResponse<UserDto>.Error("Username cannot be empty");
                }

                var user = await _userRepository.GetByUsernameAsync(username.Trim());

                if (user == null)
                {
                    _logger.LogWarning($"[UserService] User not found. Username: {username}");
                    return ApiResponse<UserDto>.Error("User not found");
                }

                _logger.LogInformation($"[UserService] User retrieved successfully. ID: {user.Id}, Username: {username}");
                return ApiResponse<UserDto>.Ok(MapToDto(user));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[UserService] Error occurred while getting user by username: {username}");
                return ApiResponse<UserDto>.Error("An error occurred while retrieving the user");
            }
        }

        public async Task<ApiResponse<List<UserDto>>> GetAllUsersAsync()
        {
            try
            {
                _logger.LogInformation("[UserService] Fetching all users");

                var users = await _userRepository.GetAllAsync();

                if (!users.Any())
                {
                    _logger.LogInformation("[UserService] No users found in the database");
                    return ApiResponse<List<UserDto>>.Ok(new List<UserDto>(), "No users found");
                }

                var dtos = users.Select(MapToDto).ToList();

                _logger.LogInformation($"[UserService] Retrieved {dtos.Count} users successfully");
                return ApiResponse<List<UserDto>>.Ok(dtos, $"Retrieved {dtos.Count} users");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[UserService] Error occurred while fetching all users");
                return ApiResponse<List<UserDto>>.Error("An error occurred while retrieving users");
            }
        }

        public async Task<ApiResponse<UserDto>> RegisterAsync(CreateUserDto request)
        {
            try
            {
                _logger.LogInformation($"[UserService] Registration attempt for username: {request?.Username}");

                if (request == null)
                {
                    _logger.LogWarning("[UserService] Registration request is null");
                    return ApiResponse<UserDto>.Error("Invalid registration request");
                }

                var validationErrors = ValidateRegistration(request);
                if (validationErrors.Any())
                {
                    _logger.LogWarning($"[UserService] Registration validation failed. Errors: {string.Join(", ", validationErrors)}");
                    return ApiResponse<UserDto>.Error("Validation failed", validationErrors);
                }

                var existingUser = await _userRepository.GetByUsernameAsync(request.Username.Trim());
                if (existingUser != null)
                {
                    _logger.LogWarning($"[UserService] Registration failed - username already taken: {request.Username}");
                    return ApiResponse<UserDto>.Error("Username already taken");
                }

                var existingEmail = await _userRepository.GetByEmailAsync(request.Email.Trim());
                if (existingEmail != null)
                {
                    _logger.LogWarning($"[UserService] Registration failed - email already registered: {request.Email}");
                    return ApiResponse<UserDto>.Error("Email already registered");
                }

                var passwordHash = HashPassword(request.Password);

                var newUser = new User
                {
                    Username = request.Username.Trim(),
                    Email = request.Email.Trim(),
                    PasswordHash = passwordHash,
                    DisplayName = !string.IsNullOrWhiteSpace(request.DisplayName)
                        ? request.DisplayName.Trim()
                        : request.Username.Trim(),
                    IsActive = true,
                    Role = UserRoleEnum.USER
                };

                var userId = await _userRepository.CreateAsync(newUser);
                newUser.Id = userId;

                _logger.LogInformation($"[UserService] User registered successfully. New User ID: {userId}, Username: {request.Username}");

                return ApiResponse<UserDto>.Ok(MapToDto(newUser), "User registered successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[UserService] Error occurred during registration for username: {request?.Username}");
                return ApiResponse<UserDto>.Error("An error occurred during registration");
            }
        }

        public async Task<ApiResponse<UserDto>> UpdateUserAsync(long id, UpdateUserDto request)
        {
            try
            {
                _logger.LogInformation($"[UserService] Updating user. ID: {id}");

                if (id <= 0)
                {
                    _logger.LogWarning($"[UserService] Invalid user ID for update: {id}");
                    return ApiResponse<UserDto>.Error("Invalid user ID");
                }

                if (request == null)
                {
                    _logger.LogWarning("[UserService] Update request is null");
                    return ApiResponse<UserDto>.Error("Invalid update request");
                }

                var user = await _userRepository.GetByIdAsync(id);
                if (user == null)
                {
                    _logger.LogWarning($"[UserService] User not found for update. ID: {id}");
                    return ApiResponse<UserDto>.Error("User not found");
                }

                var originalValues = $"DisplayName: '{user.DisplayName}', Bio: '{user.Bio}'";

                if (!string.IsNullOrWhiteSpace(request.DisplayName))
                    user.DisplayName = request.DisplayName.Trim();

                if (!string.IsNullOrWhiteSpace(request.Bio))
                    user.Bio = request.Bio.Trim();

                await _userRepository.UpdateAsync(user);

                _logger.LogInformation($"[UserService] User updated successfully. ID: {id}. Changed from: {originalValues} to DisplayName: '{user.DisplayName}', Bio: '{user.Bio}'");

                return ApiResponse<UserDto>.Ok(MapToDto(user), "User updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[UserService] Error occurred while updating user. ID: {id}");
                return ApiResponse<UserDto>.Error("An error occurred during update");
            }
        }

        public async Task<ApiResponse<string>> DeleteUserAsync(long id)
        {
            try
            {
                _logger.LogInformation($"[UserService] Deleting user. ID: {id}");

                if (id <= 0)
                {
                    _logger.LogWarning($"[UserService] Invalid user ID for deletion: {id}");
                    return ApiResponse<string>.Error("Invalid user ID");
                }

                var user = await _userRepository.GetByIdAsync(id);
                if (user == null)
                {
                    _logger.LogWarning($"[UserService] User not found for deletion. ID: {id}");
                    return ApiResponse<string>.Error("User not found");
                }

                await _userRepository.DeleteAsync(id);

                _logger.LogInformation($"[UserService] User deleted successfully. ID: {id}, Username: {user.Username}");

                return ApiResponse<string>.Ok("User deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[UserService] Error occurred while deleting user. ID: {id}");
                return ApiResponse<string>.Error("An error occurred during deletion");
            }
        }

        public async Task<ApiResponse<bool>> ChangePasswordAsync(long userId, string oldPassword, string newPassword)
        {
            try
            {
                _logger.LogInformation($"[UserService] Password change attempt for user ID: {userId}");

                if (userId <= 0)
                {
                    _logger.LogWarning($"[UserService] Invalid user ID for password change: {userId}");
                    return ApiResponse<bool>.Error("Invalid user ID");
                }

                if (string.IsNullOrWhiteSpace(oldPassword) || string.IsNullOrWhiteSpace(newPassword))
                {
                    _logger.LogWarning($"[UserService] Password change failed - empty password. User ID: {userId}");
                    return ApiResponse<bool>.Error("Old and new passwords cannot be empty");
                }

                if (newPassword.Length < MinPasswordLength)
                {
                    _logger.LogWarning($"[UserService] New password too short. User ID: {userId}");
                    return ApiResponse<bool>.Error($"New password must be at least {MinPasswordLength} characters");
                }

                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning($"[UserService] User not found for password change. ID: {userId}");
                    return ApiResponse<bool>.Error("User not found");
                }

                if (!VerifyPassword(oldPassword, user.PasswordHash))
                {
                    _logger.LogWarning($"[UserService] Password change failed - incorrect old password. User ID: {userId}");
                    return ApiResponse<bool>.Error("Incorrect old password");
                }

                user.PasswordHash = HashPassword(newPassword);
                await _userRepository.UpdateAsync(user);

                _logger.LogInformation($"[UserService] Password changed successfully for user ID: {userId}");

                return ApiResponse<bool>.Ok(true, "Password changed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[UserService] Error occurred while changing password for user ID: {userId}");
                return ApiResponse<bool>.Error("An error occurred while changing password");
            }
        }

        private List<string> ValidateRegistration(CreateUserDto request)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(request.Username))
                errors.Add("Username is required");
            else if (request.Username.Length < MinUsernameLength)
                errors.Add($"Username must be at least {MinUsernameLength} characters");
            else if (request.Username.Length > 64)
                errors.Add("Username must not exceed 64 characters");
            else if (!IsValidUsername(request.Username))
                errors.Add("Username can only contain letters, numbers, and underscores");

            if (string.IsNullOrWhiteSpace(request.Email))
                errors.Add("Email is required");
            else if (!IsValidEmail(request.Email))
                errors.Add("Invalid email format");
            else if (request.Email.Length > 255)
                errors.Add("Email must not exceed 255 characters");

            if (string.IsNullOrWhiteSpace(request.Password))
                errors.Add("Password is required");
            else if (request.Password.Length < MinPasswordLength)
                errors.Add($"Password must be at least {MinPasswordLength} characters");
            else if (!IsStrongPassword(request.Password))
                errors.Add("Password must contain uppercase, lowercase, and number");

            if (!string.IsNullOrWhiteSpace(request.DisplayName) && request.DisplayName.Length > 128)
                errors.Add("Display name must not exceed 128 characters");

            return errors;
        }

        private bool IsValidUsername(string username)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(username, @"^[a-zA-Z0-9_]+$");
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private bool IsStrongPassword(string password)
        {
            return password.Any(char.IsUpper) &&
                   password.Any(char.IsLower) &&
                   password.Any(char.IsDigit);
        }

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        private bool VerifyPassword(string password, string hash)
        {
            try
            {
                return BCrypt.Net.BCrypt.Verify(password, hash);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[UserService] Error occurred while verifying password");
                return false;
            }
        }

        private UserDto MapToDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                DisplayName = user.DisplayName ?? user.Username,
                Role = (int)user.Role,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt
            };
        }
    }
}
