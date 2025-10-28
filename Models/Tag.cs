namespace BlogTestTask.Models
{
    public class Tag : BaseEntity
    {
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// URL-friendly �������� (slug)
        /// ��������: "C# Programming" -> "csharp-programming"
        /// </summary>
        public string Slug { get; set; } = string.Empty;

        public string? Description { get; set; }

        /// <summary>
        /// ���� ��� UI (hex ���, �������� #FF5733)
        /// </summary>
        public string? Color { get; set; }

        /// <summary>
        /// ���������� ������ � ���� �����
        /// </summary>
        public int PostsCount { get; set; } = 0;

        public bool IsActive { get; set; } = true;
    }
}
