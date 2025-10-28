using BlogApp.Repository.Interfaces;
using BlogTestTask.Data;
using Npgsql;

namespace BlogApp.Repositories.Implementations
{
    public class PostLikeRepository : IPostLikeRepository
    {
        private readonly DatabaseContext _context;
        private readonly ILogger<PostLikeRepository> _logger;

        public PostLikeRepository(DatabaseContext context, ILogger<PostLikeRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> LikeAsync(long postId, long userId)
        {
            const string sql = @"
                INSERT INTO post_likes (post_id, user_id, created_at)
                VALUES (@post_id, @user_id, @created_at)
                ON CONFLICT DO NOTHING";

            var affected = await _context.ExecuteAsync(sql,
                new NpgsqlParameter("@post_id", postId),
                new NpgsqlParameter("@user_id", userId),
                new NpgsqlParameter("@created_at", DateTime.UtcNow)
            );

            return affected > 0;
        }

        public async Task<bool> UnlikeAsync(long postId, long userId)
        {
            const string sql = @"
                DELETE FROM post_likes
                WHERE post_id = @post_id AND user_id = @user_id";

            var affected = await _context.ExecuteAsync(sql,
                new NpgsqlParameter("@post_id", postId),
                new NpgsqlParameter("@user_id", userId)
            );

            return affected > 0;
        }

        public async Task<bool> IsLikedAsync(long postId, long userId)
        {
            const string sql = @"
                SELECT EXISTS(
                    SELECT 1 FROM post_likes
                    WHERE post_id = @post_id AND user_id = @user_id
                )";

            var result = await _context.ExecuteScalarAsync(sql,
                new NpgsqlParameter("@post_id", postId),
                new NpgsqlParameter("@user_id", userId)
            );

            return result != null && (bool)result;
        }

        public async Task<int> GetCountAsync(long postId)
        {
            const string sql = @"
                SELECT COUNT(*) FROM post_likes WHERE post_id = @post_id";

            var result = await _context.ExecuteScalarAsync(sql,
                new NpgsqlParameter("@post_id", postId)
            );

            return Convert.ToInt32(result ?? 0);
        }
    }
}
