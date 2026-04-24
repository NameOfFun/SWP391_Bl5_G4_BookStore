namespace BookStore.Dtos.Common
{
    public class BookTagDto
    {
        public int TagId { get; set; }
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; } = true;
    }
}
