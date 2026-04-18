namespace BookStore.Dtos.Common
{
    public class AboutDto
    {
        public int AboutId { get; set; }
        public string Title { get; set; } = null!;
        public string ContentHtml { get; set; } = null!;
        public DateTime UpdatedAt { get; set; }
    }
}
