namespace BlogTestTask.Models
{
    public class PostLike
    {
        public long PostId { get; set; }

        public long UserId { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
