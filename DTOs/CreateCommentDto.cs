namespace BlogApp.DTOs
{
    public class CreateCommentDto
    {
        public string Content { get; set; }
        public long? ParentCommentId { get; set; }
    }
}
