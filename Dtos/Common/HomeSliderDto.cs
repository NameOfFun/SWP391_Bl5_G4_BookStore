namespace BookStore.Dtos.Common
{
    public class HomeSliderDto
    {
        public int HomeSliderId { get; set; }
        public string ImageUrl { get; set; } = null!;
        public string? Caption { get; set; }
        public string? LinkUrl { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
