namespace BlogTestTask.Models
{
	public class Comment : BaseEntity
	{
		public long PostId { get; set; }

		public long UserId { get; set; }

		public long? ParentCommentId { get; set; }

		public string Content { get; set; } = string.Empty;

		public int LikesCount { get; set; } = 0;

		public bool IsEdited { get; set; } = false;

		/// <summary>
		/// Удалён ли комментарий (soft delete)
		/// </summary>
		public bool IsDeleted { get; set; } = false;
	}
}
