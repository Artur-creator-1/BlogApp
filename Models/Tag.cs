namespace BlogTestTask.Models
{
    public class Tag : BaseEntity
    {
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// URL-friendly название (slug)
        /// Например: "C# Programming" -> "csharp-programming"
        /// </summary>
        public string Slug { get; set; } = string.Empty;

        public string? Description { get; set; }

        /// <summary>
        /// Цвет для UI (hex код, например #FF5733)
        /// </summary>
        public string? Color { get; set; }

        /// <summary>
        /// Количество постов с этим тегом
        /// </summary>
        public int PostsCount { get; set; } = 0;

        public bool IsActive { get; set; } = true;
    }
}
