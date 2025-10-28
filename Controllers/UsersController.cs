using BlogApp.DTOs;
using BlogApp.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserService userService, ILogger<UsersController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(long id)
        {
            _logger.LogInformation($"[UsersController] GET /api/users/{id}");

            var result = await _userService.GetUserByIdAsync(id);

            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }

        [HttpGet("by-username/{username}")]
        public async Task<IActionResult> GetUserByUsername(string username)
        {
            _logger.LogInformation($"[UsersController] GET /api/users/by-username/{username}");

            var result = await _userService.GetUserByUsernameAsync(username);

            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            _logger.LogInformation("[UsersController] GET /api/users");

            var result = await _userService.GetAllUsersAsync();

            return Ok(result);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] CreateUserDto request)
        {
            _logger.LogInformation($"[UsersController] POST /api/users/register - Username: {request?.Username}");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("[UsersController] Invalid model state for registration");
                return BadRequest(ApiResponse<UserDto>.Error("Invalid request",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
            }

            var result = await _userService.RegisterAsync(request);

            if (!result.Success)
            {
                _logger.LogWarning($"[UsersController] Registration failed - {result.Message}");
                return BadRequest(result);
            }

            _logger.LogInformation($"[UsersController] User registered successfully - ID: {result.Data.Id}");
            return CreatedAtAction(nameof(GetUserById), new { id = result.Data.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(long id, [FromBody] UpdateUserDto request)
        {
            _logger.LogInformation($"[UsersController] PUT /api/users/{id}");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning($"[UsersController] Invalid model state for user update - ID: {id}");
                return BadRequest(ApiResponse<UserDto>.Error("Invalid request",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
            }

            var result = await _userService.UpdateUserAsync(id, request);

            if (!result.Success)
            {
                _logger.LogWarning($"[UsersController] User update failed - ID: {id}, {result.Message}");
                return NotFound(result);
            }

            _logger.LogInformation($"[UsersController] User updated successfully - ID: {id}");
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(long id)
        {
            _logger.LogInformation($"[UsersController] DELETE /api/users/{id}");

            var result = await _userService.DeleteUserAsync(id);

            if (!result.Success)
            {
                _logger.LogWarning($"[UsersController] User deletion failed - ID: {id}");
                return NotFound(result);
            }

            _logger.LogInformation($"[UsersController] User deleted successfully - ID: {id}");
            return Ok(result);
        }

        [HttpPost("{id}/change-password")]
        public async Task<IActionResult> ChangePassword(long id, [FromBody] ChangePasswordRequest request)
        {
            _logger.LogInformation($"[UsersController] POST /api/users/{id}/change-password");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning($"[UsersController] Invalid model state for password change - ID: {id}");
                return BadRequest(ApiResponse<bool>.Error("Invalid request",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
            }

            var result = await _userService.ChangePasswordAsync(id, request.OldPassword, request.NewPassword);

            if (!result.Success)
            {
                _logger.LogWarning($"[UsersController] Password change failed - ID: {id}");
                return BadRequest(result);
            }

            _logger.LogInformation($"[UsersController] Password changed successfully - ID: {id}");
            return Ok(result);
        }
    }

    public class ChangePasswordRequest
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
