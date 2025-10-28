namespace BlogApp.DTOs
{
    public class CommentDto
    {
        public long Id { get; set; }
        public long PostId { get; set; }
        public long UserId { get; set; }
        public string Content { get; set; }
        public int LikesCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
