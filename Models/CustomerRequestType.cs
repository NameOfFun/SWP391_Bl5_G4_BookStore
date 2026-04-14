namespace BookStore.Models;

public enum CustomerRequestType
{
    General = 0,
    ReturnBook = 1,
    Exchange = 2,
    Refund = 3,
    Other = 9
}
