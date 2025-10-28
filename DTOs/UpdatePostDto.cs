namespace BlogApp.DTOs
{
    public class UpdatePostDto
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string Summary { get; set; }
        public bool IsPublished { get; set; }
    }
}
