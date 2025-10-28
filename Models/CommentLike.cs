namespace BlogTestTask.Models
{
    public class CommentLike
    {
        public long CommentId { get; set; }

        public long UserId { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
