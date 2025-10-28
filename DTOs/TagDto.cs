namespace BlogApp.DTOs
{
    public class TagDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public string Description { get; set; }
        public int PostsCount { get; set; }
    }
}
