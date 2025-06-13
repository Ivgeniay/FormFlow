namespace FormFlow.Domain.Models.SearchService
{
    public class TemplateSearchDocument
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string QuestionsText { get; set; } = string.Empty;
        public string CommentsText { get; set; } = string.Empty;
        public string AuthorName { get; set; } = string.Empty;
        public List<string> Tags { get; set; } = new List<string>();
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsArchived { get; set; } = false;
        public bool IsPublished { get; set; } = false;
        public bool IsDeleted { get; set; } = false;
        public int FormsCount { get; set; }
        public int LikesCount { get; set; }
        public int CommentsCount { get; set; }
    }
}
