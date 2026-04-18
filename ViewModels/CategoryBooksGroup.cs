using BookStore.Dtos.Common;

namespace BookStore.ViewModels;

public class CategoryBooksGroup
{
    public string CategoryName { get; set; } = "";
    public List<BookDto> Books { get; set; } = new();
}
