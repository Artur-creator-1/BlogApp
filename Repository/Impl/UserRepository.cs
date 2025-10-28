using BlogApp.Repository.Interfaces;
using BlogTestTask.Data;
using BlogTestTask.Models;
using BlogTestTask.Models.Enums;
using Npgsql;

namespace BlogApp.Repositories.Implementations
{

    public class UserRepository : IUserRepository
    {
        private readonly DatabaseContext _context;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(DatabaseContext context, ILogger<UserRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<User?> GetByIdAsync(long id)
        {
            const string sql = @"
                SELECT id, username, email, password_hash, display_name, bio, role, 
                       is_active, last_login_at, created_at, updated_at
                FROM users
                WHERE id = @id
                LIMIT 1";

            using var reader = await _context.ExecuteReaderAsync(sql,
                new NpgsqlParameter("@id", id));

            if (await reader.ReadAsync())
            {
                return MapToUser(reader);
            }

            return null;
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username cannot be empty");

            const string sql = @"
                SELECT id, username, email, password_hash, display_name, bio, role,
                       is_active, last_login_at, created_at, updated_at
                FROM users
                WHERE username = @username
                LIMIT 1";

            _logger.LogInformation($"Getting user by username: {username}");

            using var reader = await _context.ExecuteReaderAsync(sql,
                new NpgsqlParameter("@username", username));

            if (await reader.ReadAsync())
            {
                return MapToUser(reader);
            }

            _logger.LogWarning($"User not found: {username}");
            return null;
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be empty");

            const string sql = @"
                SELECT id, username, email, password_hash, display_name, bio, role,
                       is_active, last_login_at, created_at, updated_at
                FROM users
                WHERE email = @email
                LIMIT 1";

            using var reader = await _context.ExecuteReaderAsync(sql,
                new NpgsqlParameter("@email", email));

            if (await reader.ReadAsync())
            {
                return MapToUser(reader);
            }

            return null;
        }

        public async Task<long> CreateAsync(User user)
        {
            if (string.IsNullOrWhiteSpace(user.Username))
                throw new ArgumentException("Username is required");
            if (string.IsNullOrWhiteSpace(user.Email))
                throw new ArgumentException("Email is required");
            if (string.IsNullOrWhiteSpace(user.PasswordHash))
                throw new ArgumentException("PasswordHash is required");

            const string sql = @"
                INSERT INTO users (username, email, password_hash, display_name, bio, role, is_active, created_at)
                VALUES (@username, @email, @password_hash, @display_name, @bio, @role, @is_active, @created_at)
                RETURNING id";

            var id = await _context.ExecuteScalarAsync(sql,
                new NpgsqlParameter("@username", user.Username),
                new NpgsqlParameter("@email", user.Email),
                new NpgsqlParameter("@password_hash", user.PasswordHash),
                new NpgsqlParameter("@display_name", user.DisplayName ?? (object)DBNull.Value),
                new NpgsqlParameter("@bio", user.Bio ?? (object)DBNull.Value),
                new NpgsqlParameter("@role", (int)user.Role),
                new NpgsqlParameter("@is_active", user.IsActive),
                new NpgsqlParameter("@created_at", DateTime.UtcNow)
            );

            _logger.LogInformation($"User created: {user.Username} (ID: {id})");
            return Convert.ToInt64(id);
        }

        public async Task UpdateAsync(User user)
        {
            const string sql = @"
                UPDATE users
                SET display_name = @display_name,
                    bio = @bio,
                    role = @role,
                    is_active = @is_active,
                    last_login_at = @last_login_at,
                    updated_at = @updated_at
                WHERE id = @id";

            var affected = await _context.ExecuteAsync(sql,
                new NpgsqlParameter("@id", user.Id),
                new NpgsqlParameter("@display_name", user.DisplayName ?? (object)DBNull.Value),
                new NpgsqlParameter("@bio", user.Bio ?? (object)DBNull.Value),
                new NpgsqlParameter("@role", (int)user.Role),
                new NpgsqlParameter("@is_active", user.IsActive),
                new NpgsqlParameter("@last_login_at", user.LastLoginAt ?? (object)DBNull.Value),
                new NpgsqlParameter("@updated_at", DateTime.UtcNow)
            );

            if (affected == 0)
                _logger.LogWarning($"User not found for update: ID {user.Id}");
        }

        public async Task<bool> DeleteAsync(long id)
        {
            const string sql = @"
                UPDATE users
                SET is_active = FALSE,
                    updated_at = @updated_at
                WHERE id = @id";

            var affected = await _context.ExecuteAsync(sql,
                new NpgsqlParameter("@id", id),
                new NpgsqlParameter("@updated_at", DateTime.UtcNow)
            );

            _logger.LogInformation($"User deactivated: ID {id}");
            return affected > 0;
        }

        public async Task<List<User>> GetAllAsync()
        {
            const string sql = @"
                SELECT id, username, email, password_hash, display_name, bio, role,
                       is_active, last_login_at, created_at, updated_at
                FROM users
                WHERE is_active = TRUE
                ORDER BY created_at DESC";

            var users = new List<User>();

            using var reader = await _context.ExecuteReaderAsync(sql);

            while (await reader.ReadAsync())
            {
                users.Add(MapToUser(reader));
            }

            return users;
        }

        private static User MapToUser(NpgsqlDataReader reader)
        {
            return new User
            {
                Id = reader.GetInt64(0),
                Username = reader.GetString(1),
                Email = reader.GetString(2),
                PasswordHash = reader.GetString(3),
                DisplayName = reader.IsDBNull(4) ? null : reader.GetString(4),
                Bio = reader.IsDBNull(5) ? null : reader.GetString(5),
                Role = (UserRoleEnum)reader.GetInt32(6),
                IsActive = reader.GetBoolean(7),
                LastLoginAt = reader.IsDBNull(8) ? null : reader.GetDateTime(8),
                CreatedAt = reader.GetDateTime(9),
                UpdatedAt = reader.IsDBNull(10) ? null : reader.GetDateTime(10)
            };
        }
    }
}
