using BlogApp.Repository.Interfaces;
using BlogTestTask.Data;
using BlogTestTask.Models;
using Npgsql;

namespace BlogApp.Repositories.Implementations
{
    public class PostRepository : IPostRepository
    {
        private readonly DatabaseContext _context;
        private readonly ILogger<PostRepository> _logger;

        public PostRepository(DatabaseContext context, ILogger<PostRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Post?> GetByIdAsync(long id)
        {
            const string sql = @"
                SELECT id, user_id, title, content, summary, image_url, view_count,
                       likes_count, comments_count, is_published, published_at,
                       is_deleted, created_at, updated_at
                FROM posts
                WHERE id = @id AND is_deleted = FALSE
                LIMIT 1";

            using var reader = await _context.ExecuteReaderAsync(sql,
                new NpgsqlParameter("@id", id));

            if (await reader.ReadAsync())
            {
                return MapToPost(reader);
            }

            return null;
        }

        public async Task<List<Post>> GetAllAsync()
        {
            const string sql = @"
                SELECT id, user_id, title, content, summary, image_url, view_count,
                       likes_count, comments_count, is_published, published_at,
                       is_deleted, created_at, updated_at
                FROM posts
                WHERE is_deleted = FALSE
                ORDER BY created_at DESC";

            var posts = new List<Post>();

            using var reader = await _context.ExecuteReaderAsync(sql);
            while (await reader.ReadAsync())
            {
                posts.Add(MapToPost(reader));
            }

            return posts;
        }

        public async Task<long> CreateAsync(Post post)
        {
            const string sql = @"
                INSERT INTO posts (user_id, title, content, summary, image_url, 
                    is_published, published_at, created_at)
                VALUES (@user_id, @title, @content, @summary, @image_url, 
                        @is_published, @published_at, @created_at)
                RETURNING id";

            var id = await _context.ExecuteScalarAsync(sql,
                new NpgsqlParameter("@user_id", post.UserId),
                new NpgsqlParameter("@title", post.Title),
                new NpgsqlParameter("@content", post.Content),
                new NpgsqlParameter("@summary", post.Summary ?? (object)DBNull.Value),
                new NpgsqlParameter("@image_url", post.ImageUrl ?? (object)DBNull.Value),
                new NpgsqlParameter("@is_published", post.IsPublished),
                new NpgsqlParameter("@published_at", post.PublishedAt ?? (object)DBNull.Value),
                new NpgsqlParameter("@created_at", DateTime.UtcNow)
            );

            return Convert.ToInt64(id);
        }

        public async Task UpdateAsync(Post post)
        {
            const string sql = @"
                UPDATE posts SET 
                    title = @title,
                    content = @content,
                    summary = @summary,
                    image_url = @image_url,
                    is_published = @is_published,
                    published_at = @published_at,
                    updated_at = @updated_at
                WHERE id = @id AND is_deleted = FALSE";

            await _context.ExecuteAsync(sql,
                new NpgsqlParameter("@id", post.Id),
                new NpgsqlParameter("@title", post.Title),
                new NpgsqlParameter("@content", post.Content),
                new NpgsqlParameter("@summary", post.Summary ?? (object)DBNull.Value),
                new NpgsqlParameter("@image_url", post.ImageUrl ?? (object)DBNull.Value),
                new NpgsqlParameter("@is_published", post.IsPublished),
                new NpgsqlParameter("@published_at", post.PublishedAt ?? (object)DBNull.Value),
                new NpgsqlParameter("@updated_at", DateTime.UtcNow)
            );
        }

        public async Task<bool> DeleteAsync(long id)
        {
            const string sql = @"
                UPDATE posts SET is_deleted = TRUE, updated_at = @updated_at
                WHERE id = @id";

            var affected = await _context.ExecuteAsync(sql,
                new NpgsqlParameter("@id", id),
                new NpgsqlParameter("@updated_at", DateTime.UtcNow)
            );

            return affected > 0;
        }

        private static Post MapToPost(NpgsqlDataReader reader)
        {
            return new Post
            {
                Id = reader.GetInt64(0),
                UserId = reader.GetInt64(1),
                Title = reader.GetString(2),
                Content = reader.GetString(3),
                Summary = reader.IsDBNull(4) ? null : reader.GetString(4),
                ImageUrl = reader.IsDBNull(5) ? null : reader.GetString(5),
                ViewCount = reader.GetInt32(6),
                LikesCount = reader.GetInt32(7),
                CommentsCount = reader.GetInt32(8),
                IsPublished = reader.GetBoolean(9),
                PublishedAt = reader.IsDBNull(10) ? null : reader.GetDateTime(10),
                IsDeleted = reader.GetBoolean(11),
                CreatedAt = reader.GetDateTime(12),
                UpdatedAt = reader.IsDBNull(13) ? null : reader.GetDateTime(13)
            };
        }
    }
}
