using BlogApp.Repository.Interfaces;
using BlogTestTask.Data;
using BlogTestTask.Models;
using Npgsql;

namespace BlogApp.Repositories.Implementations
{
    public class TagRepository : ITagRepository
    {
        private readonly DatabaseContext _context;
        private readonly ILogger<TagRepository> _logger;

        public TagRepository(DatabaseContext context, ILogger<TagRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Tag?> GetByIdAsync(long id)
        {
            const string sql = @"
                SELECT id, name, slug, description, color, posts_count, is_active, created_at, updated_at
                FROM tags
                WHERE id = @id
                LIMIT 1";

            using var reader = await _context.ExecuteReaderAsync(sql,
                new NpgsqlParameter("@id", id));

            if (await reader.ReadAsync())
                return MapToTag(reader);

            return null;
        }

        public async Task<Tag?> GetBySlugAsync(string slug)
        {
            const string sql = @"
                SELECT id, name, slug, description, color, posts_count, is_active, created_at, updated_at
                FROM tags
                WHERE slug = @slug
                LIMIT 1";

            using var reader = await _context.ExecuteReaderAsync(sql,
                new NpgsqlParameter("@slug", slug));

            if (await reader.ReadAsync())
                return MapToTag(reader);

            return null;
        }

        public async Task<List<Tag>> GetAllAsync()
        {
            const string sql = @"
                SELECT id, name, slug, description, color, posts_count, is_active, created_at, updated_at
                FROM tags
                WHERE is_active = TRUE
                ORDER BY posts_count DESC";

            var tags = new List<Tag>();

            using var reader = await _context.ExecuteReaderAsync(sql);
            while (await reader.ReadAsync())
                tags.Add(MapToTag(reader));

            return tags;
        }

        public async Task<long> CreateAsync(Tag tag)
        {
            const string sql = @"
                INSERT INTO tags (name, slug, description, color, is_active, created_at)
                VALUES (@name, @slug, @description, @color, @is_active, @created_at)
                RETURNING id";

            var id = await _context.ExecuteScalarAsync(sql,
                new NpgsqlParameter("@name", tag.Name),
                new NpgsqlParameter("@slug", tag.Slug),
                new NpgsqlParameter("@description", tag.Description ?? (object)DBNull.Value),
                new NpgsqlParameter("@color", tag.Color ?? (object)DBNull.Value),
                new NpgsqlParameter("@is_active", tag.IsActive),
                new NpgsqlParameter("@created_at", DateTime.UtcNow)
            );

            return Convert.ToInt64(id);
        }

        public async Task UpdateAsync(Tag tag)
        {
            const string sql = @"
                UPDATE tags SET 
                    name = @name,
                    slug = @slug,
                    description = @description,
                    color = @color,
                    posts_count = @posts_count,
                    is_active = @is_active,
                    updated_at = @updated_at
                WHERE id = @id";

            await _context.ExecuteAsync(sql,
                new NpgsqlParameter("@id", tag.Id),
                new NpgsqlParameter("@name", tag.Name),
                new NpgsqlParameter("@slug", tag.Slug),
                new NpgsqlParameter("@description", tag.Description ?? (object)DBNull.Value),
                new NpgsqlParameter("@color", tag.Color ?? (object)DBNull.Value),
                new NpgsqlParameter("@posts_count", tag.PostsCount),
                new NpgsqlParameter("@is_active", tag.IsActive),
                new NpgsqlParameter("@updated_at", DateTime.UtcNow)
            );
        }

        public async Task<bool> DeleteAsync(long id)
        {
            const string sql = @"
                UPDATE tags SET is_active = FALSE, updated_at = @updated_at
                WHERE id = @id";

            var affected = await _context.ExecuteAsync(sql,
                new NpgsqlParameter("@id", id),
                new NpgsqlParameter("@updated_at", DateTime.UtcNow)
            );

            return affected > 0;
        }

        public async Task<List<Tag>> GetByPostIdAsync(long postId)
        {
            const string sql = @"
                SELECT t.id, t.name, t.slug, t.description, t.color, t.posts_count, t.is_active, t.created_at, t.updated_at
                FROM tags t
                INNER JOIN post_tags pt ON t.id = pt.tag_id
                WHERE pt.post_id = @post_id";

            var tags = new List<Tag>();

            using var reader = await _context.ExecuteReaderAsync(sql,
                new NpgsqlParameter("@post_id", postId));

            while (await reader.ReadAsync())
                tags.Add(MapToTag(reader));

            return tags;
        }

        private static Tag MapToTag(NpgsqlDataReader reader)
        {
            return new Tag
            {
                Id = reader.GetInt64(0),
                Name = reader.GetString(1),
                Slug = reader.GetString(2),
                Description = reader.IsDBNull(3) ? null : reader.GetString(3),
                Color = reader.IsDBNull(4) ? null : reader.GetString(4),
                PostsCount = reader.GetInt32(5),
                IsActive = reader.GetBoolean(6),
                CreatedAt = reader.GetDateTime(7),
                UpdatedAt = reader.IsDBNull(8) ? null : reader.GetDateTime(8)
            };
        }
    }
}

