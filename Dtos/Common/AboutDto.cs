namespace BookStore.Dtos.Common
{
    public class AboutDto
    {
        public int AboutId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string ContentHtml { get; set; } = string.Empty;

        public DateTime UpdatedAt { get; set; }
    }
}
