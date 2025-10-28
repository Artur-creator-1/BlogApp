namespace BlogTestTask.Models
{
    public class Post : BaseEntity
    {
        public long UserId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        public string? Summary { get; set; }

        public string? ImageUrl { get; set; }

        public int ViewCount { get; set; } = 0;

        public int LikesCount { get; set; } = 0;

        public int CommentsCount { get; set; } = 0;

        public bool IsPublished { get; set; } = true;

        public DateTime? PublishedAt { get; set; }

        public bool IsDeleted { get; set; } = false;
    }
}
