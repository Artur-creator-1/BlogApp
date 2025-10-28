using BlogApp.Repository.Interfaces;
using BlogTestTask.Data;
using BlogTestTask.Models;
using Npgsql;

namespace BlogApp.Repositories.Implementations
{
    public class CommentRepository : ICommentRepository
    {
        private readonly DatabaseContext _context;
        private readonly ILogger<CommentRepository> _logger;

        public CommentRepository(DatabaseContext context, ILogger<CommentRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Comment?> GetByIdAsync(long id)
        {
            const string sql = @"
                SELECT id, post_id, user_id, parent_comment_id, content, likes_count,
                       is_edited, is_deleted, created_at, updated_at
                FROM comments
                WHERE id = @id AND is_deleted = FALSE
                LIMIT 1";

            using var reader = await _context.ExecuteReaderAsync(sql,
                new NpgsqlParameter("@id", id));

            if (await reader.ReadAsync())
                return MapToComment(reader);

            return null;
        }

        public async Task<List<Comment>> GetByPostIdAsync(long postId)
        {
            const string sql = @"
                SELECT id, post_id, user_id, parent_comment_id, content, likes_count,
                       is_edited, is_deleted, created_at, updated_at
                FROM comments
                WHERE post_id = @post_id AND is_deleted = FALSE
                ORDER BY created_at DESC";

            var comments = new List<Comment>();

            using var reader = await _context.ExecuteReaderAsync(sql,
                new NpgsqlParameter("@post_id", postId));

            while (await reader.ReadAsync())
                comments.Add(MapToComment(reader));

            return comments;
        }

        public async Task<long> CreateAsync(Comment comment)
        {
            const string sql = @"
                INSERT INTO comments (post_id, user_id, parent_comment_id, content, created_at)
                VALUES (@post_id, @user_id, @parent_comment_id, @content, @created_at)
                RETURNING id";

            var id = await _context.ExecuteScalarAsync(sql,
                new NpgsqlParameter("@post_id", comment.PostId),
                new NpgsqlParameter("@user_id", comment.UserId),
                new NpgsqlParameter("@parent_comment_id", comment.ParentCommentId ?? (object)DBNull.Value),
                new NpgsqlParameter("@content", comment.Content),
                new NpgsqlParameter("@created_at", DateTime.UtcNow)
            );

            return Convert.ToInt64(id);
        }

        public async Task UpdateAsync(Comment comment)
        {
            const string sql = @"
                UPDATE comments SET 
                    content = @content,
                    is_edited = @is_edited,
                    updated_at = @updated_at
                WHERE id = @id AND is_deleted = FALSE";

            await _context.ExecuteAsync(sql,
                new NpgsqlParameter("@id", comment.Id),
                new NpgsqlParameter("@content", comment.Content),
                new NpgsqlParameter("@is_edited", true),
                new NpgsqlParameter("@updated_at", DateTime.UtcNow)
            );
        }

        public async Task<bool> DeleteAsync(long id)
        {
            const string sql = @"
                UPDATE comments SET is_deleted = TRUE, updated_at = @updated_at
                WHERE id = @id";

            var affected = await _context.ExecuteAsync(sql,
                new NpgsqlParameter("@id", id),
                new NpgsqlParameter("@updated_at", DateTime.UtcNow)
            );

            return affected > 0;
        }

        public async Task<List<Comment>> GetByUserIdAsync(long userId)
        {
            const string sql = @"
                SELECT id, post_id, user_id, parent_comment_id, content, likes_count,
                       is_edited, is_deleted, created_at, updated_at
                FROM comments
                WHERE user_id = @user_id AND is_deleted = FALSE
                ORDER BY created_at DESC";

            var comments = new List<Comment>();

            using var reader = await _context.ExecuteReaderAsync(sql,
                new NpgsqlParameter("@user_id", userId));

            while (await reader.ReadAsync())
                comments.Add(MapToComment(reader));

            return comments;
        }

        private static Comment MapToComment(NpgsqlDataReader reader)
        {
            return new Comment
            {
                Id = reader.GetInt64(0),
                PostId = reader.GetInt64(1),
                UserId = reader.GetInt64(2),
                ParentCommentId = reader.IsDBNull(3) ? null : reader.GetInt64(3),
                Content = reader.GetString(4),
                LikesCount = reader.GetInt32(5),
                IsEdited = reader.GetBoolean(6),
                IsDeleted = reader.GetBoolean(7),
                CreatedAt = reader.GetDateTime(8),
                UpdatedAt = reader.IsDBNull(9) ? null : reader.GetDateTime(9)
            };
        }
    }
}
