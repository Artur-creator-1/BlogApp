using BlogApp.Repository.Interfaces;
using BlogTestTask.Data;
using Npgsql;

namespace BlogApp.Repositories.Implementations
{
    public class CommentLikeRepository : ICommentLikeRepository
    {
        private readonly DatabaseContext _context;
        private readonly ILogger<CommentLikeRepository> _logger;

        public CommentLikeRepository(DatabaseContext context, ILogger<CommentLikeRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> LikeAsync(long commentId, long userId)
        {
            const string sql = @"
                INSERT INTO comment_likes (comment_id, user_id, created_at)
                VALUES (@comment_id, @user_id, @created_at)
                ON CONFLICT DO NOTHING";

            var affected = await _context.ExecuteAsync(sql,
                new NpgsqlParameter("@comment_id", commentId),
                new NpgsqlParameter("@user_id", userId),
                new NpgsqlParameter("@created_at", DateTime.UtcNow)
            );

            return affected > 0;
        }

        public async Task<bool> UnlikeAsync(long commentId, long userId)
        {
            const string sql = @"
                DELETE FROM comment_likes
                WHERE comment_id = @comment_id AND user_id = @user_id";

            var affected = await _context.ExecuteAsync(sql,
                new NpgsqlParameter("@comment_id", commentId),
                new NpgsqlParameter("@user_id", userId)
            );

            return affected > 0;
        }

        public async Task<bool> IsLikedAsync(long commentId, long userId)
        {
            const string sql = @"
                SELECT EXISTS(
                    SELECT 1 FROM comment_likes
                    WHERE comment_id = @comment_id AND user_id = @user_id
                )";

            var result = await _context.ExecuteScalarAsync(sql,
                new NpgsqlParameter("@comment_id", commentId),
                new NpgsqlParameter("@user_id", userId)
            );

            return result != null && (bool)result;
        }

        public async Task<int> GetCountAsync(long commentId)
        {
            const string sql = @"
                SELECT COUNT(*) FROM comment_likes WHERE comment_id = @comment_id";

            var result = await _context.ExecuteScalarAsync(sql,
                new NpgsqlParameter("@comment_id", commentId)
            );

            return Convert.ToInt32(result ?? 0);
        }
    }
}
