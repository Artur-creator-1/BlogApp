using BlogTestTask.Models.Enums;

namespace BlogTestTask.Models
{
    public class User : BaseEntity
    {
        public string Username { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public string? DisplayName { get; set; }

        public string? Bio { get; set; }

        public UserRoleEnum Role { get; set; } = UserRoleEnum.USER;

        public bool IsActive { get; set; } = true;

        public DateTime? LastLoginAt { get; set; }
    }
}
